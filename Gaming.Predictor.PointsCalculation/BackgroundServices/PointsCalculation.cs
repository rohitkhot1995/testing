using Gaming.Predictor.Interfaces.AWS;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Contracts.BackgroundServices;
using System;
using System.Threading;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.Session;
using System.Collections.Generic;
using Gaming.Predictor.Contracts.Feeds;
using System.Linq;
using Gaming.Predictor.Library.Utility;
using Gaming.Predictor.Contracts.Admin;
using Gaming.Predictor.Contracts.Automate;
using System.Data;
using System.Text;
using Gaming.Predictor.Contracts.Enums;

namespace Gaming.Predictor.PointsCalculation.BackgroundServices
{
    public class PointsCalculation : BaseService<PointsCalculation>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private Blanket.BackgroundServices.PointsCalculation _Calculation;
        private readonly Blanket.Feeds.Gameplay _Feeds;
        private Blanket.Feeds.Ingestion _Ingestion;
        private Blanket.Notification.Topics _NotificationTopics;
        private Blanket.Notification.Publish _NotificationPublish;
        private Int32 _Interval;
        private Int32 _NotificationsDelaySeconds;
        private String _LeaderBoardType;

        public PointsCalculation(ILogger<PointsCalculation> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings,
           IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset) : base(logger, appSettings, serviceSettings, aws, postgre, redis, asset)
        {
            _Calculation = new Blanket.BackgroundServices.PointsCalculation(appSettings, serviceSettings, aws, postgre, redis, cookies, asset);
            _Ingestion = new Blanket.Feeds.Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _Feeds = new Blanket.Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _NotificationTopics = new Blanket.Notification.Topics(appSettings, aws, postgre, redis, cookies, asset);
            _NotificationPublish = new Blanket.Notification.Publish(appSettings, aws, postgre, redis, cookies, asset);
            _Interval = serviceSettings.Value.PointsCalculation.IntervalMinutes;
            _NotificationsDelaySeconds = serviceSettings.Value.NotificationDelaySeconds;
            _LeaderBoardType = serviceSettings.Value.PointsCalculation.LeaderBoardType;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Catcher($"{_Service} started.");

            //Timer runs immediately. Periodic intervals is disabled.
            _Timer = new Timer(Process, null, 0, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private void Process(object state)
        {
            Run(state);

            //Timer runs after the interval period. Periodic intervals is disabled.
            _Timer?.Change(Convert.ToInt32(TimeSpan.FromMinutes(_Interval).TotalMilliseconds), Timeout.Infinite);
        }

        private async void Run(object state)
        {

            MOLPointTrigger MOLTrigger = new MOLPointTrigger();
            Matchdays PredictorMatchdays = new Matchdays();
            Int32 MOLRetVal = -40;
            Int32 PredictorRetVal = -40;
            Int32 CombineLBRetVal = -40;

            Int32 DaemonServiceStatus = 0;
            string _daemonStatus = "";

            try
            {
                _daemonStatus = await _Asset.GET(_Asset.DaemonServiceCombineLB());

                DaemonServiceStatus = string.IsNullOrEmpty(_daemonStatus) ? 0 : Convert.ToInt32(_daemonStatus);
            }
            catch(Exception ex)
            {
                Catcher("Combine Leaderboard : Fetch Daemon Status : Data : " + _daemonStatus, LogLevel.Error, ex);
            }

            if (DaemonServiceStatus > 0)
            {

                #region " MOL Point Calculation "

                try
                {

                    Catcher("MOL : Checking for Points to calculate");
                    MOLTrigger = _Calculation.GetMOLPointTrigger();
                    if (MOLTrigger.RetVal == 1 && MOLTrigger.GameDayId != 0 && MOLTrigger.WeekId != 0)
                    {

                        Catcher($"MOL : Data found: WeekId:{MOLTrigger.WeekId}, tourId:{MOLTrigger.TourId}");

                        Catcher($"MOL : Point Process for Tour & Week Initiated [Gameday :{MOLTrigger.GameDayId}, Week:{MOLTrigger.WeekId}]...");
                        MOLRetVal = _Calculation.GetMOLPointProcess(MOLTrigger.GameDayId, MOLTrigger.WeekId);
                        Catcher($"Point Process for Tour & Week Finished... [Retval: {MOLRetVal}]");
                        if (MOLRetVal == 1)
                        {
                        }
                    }
                    else
                    {
                        Catcher($"No data found.");
                    }

                }
                catch (Exception ex)
                { Catcher("MOL : Run MOL Point Calculation", LogLevel.Error, ex); }

                #endregion

                #region  " Predictor Point Calculation "

                try
                {
                    Boolean success = false;

                    PredictorMatchdays = _Calculation.Matchdays();
                    //if (matchdays == null || matchdays.GamedayId == 0 || matchdays.Matchday == 0)
                    if (PredictorMatchdays != null && PredictorMatchdays.GamedayId > 0)
                    {

                        Catcher("Predictor : MatchdayId found: " + GenericFunctions.Serialize(PredictorMatchdays));


                        Catcher("Predictor : Points calculation process started for gameDayId " + PredictorMatchdays.GamedayId + " matchdayId " + PredictorMatchdays.Matchday);

                        PredictorRetVal = _Calculation.UserPointsProcess(PredictorMatchdays.GamedayId, PredictorMatchdays.Matchday);

                        Catcher("Predictor : Points calculation process completed. Retval: " + PredictorRetVal);

                        Catcher("Predictor : Reports process started.");
                        DataSet ds = _Calculation.UserPointsProcessReports(PredictorRetVal, PredictorMatchdays.GamedayId, PredictorMatchdays.Matchday);

                        StringBuilder rp = _Calculation.ParseReports(ds);

                        Catcher("Predictor : Reports process completed.");

                        if (PredictorRetVal == 1)
                        {

                            Catcher("Predictor : Match Questions ingestion started");

                            List<Fixtures> fixtures = new List<Fixtures>();
                            Contracts.Common.HTTPResponse httpResponse = _Feeds.GetFixtures("en").Result;
                            if (httpResponse.Meta.RetVal == 1)
                            {
                                if (httpResponse.Data != null)
                                    fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((Contracts.Common.ResponseObject)httpResponse.Data).Value));
                            }
                            List<Int32> _MatchIds = new List<Int32>();
                            _MatchIds = fixtures.Where(c => c.GamedayId == PredictorMatchdays.Matchday).Select(x => x.MatchId).ToList();
                            foreach (Int32 mMatchId in _MatchIds)
                            {
                                Int32 retval = await _Ingestion.Questions(mMatchId);
                                Catcher("Predictor : Match Questions ingestion completed for matchId : " + mMatchId + " retVal " + retval);
                            }

                            Catcher("Predictor : Match Questions ingestion completed.");

                            //await _Ingestion.LeaderBoard(matchdays.GamedayId, matchdays.PhaseId);

                            Catcher("Predictor : Teams recent recent results ingestion started.");

                            await _Ingestion.GetRecentResults();

                            Catcher("Teams recent recent results ingestion completed. Fixtures ingestion started again.");


                            await _Ingestion.Fixtures();

                            Catcher("Predictor : Fixtures ingestion completed again. CurrentGamedayMatches ingestion started.");


                            await _Ingestion.CurrentGamedayMatches();

                            Catcher("Predictor : CurrentGamedayMatches ingestion completed.");

                            //await _Ingestion.UpdateMixApiLeaderboard(2, matchdays.GamedayId);
                            foreach (Int32 mMatchId in _MatchIds)
                            {
                                await _Ingestion.UpdateMixApiLeaderboard(1, mMatchId);
                            }

                        }

                        Catcher("Predictor : Points calculation process fully completed.");

                        CalculationNotify(PredictorMatchdays.GamedayId, PredictorRetVal, rp);
                    }

                }
                catch (Exception ex)
                { Catcher("Predictor : Run", LogLevel.Error, ex); }

                #endregion

                #region " Combine Leaderboard Point Calculation "

                try
                {
                    Int32 combineLBGamedayId = 0;
                    Int32 combineLBWeekid = 0;

                    if (PredictorMatchdays != null && PredictorMatchdays.GamedayId > 0)
                        combineLBGamedayId = PredictorMatchdays.GamedayId;
                    else if (MOLTrigger != null && MOLTrigger.GameDayId > 0)
                        combineLBGamedayId = MOLTrigger.GameDayId;

                    if (MOLTrigger != null && MOLTrigger.WeekId > 0)
                        combineLBWeekid = MOLTrigger.WeekId;


                    if (combineLBGamedayId > 0 && combineLBWeekid > 0)
                    {
                        Catcher("Combine Leaderboard : Points calculation process started for gameDayId " + combineLBGamedayId + " weekId " + combineLBWeekid);

                        CombineLBRetVal = _Calculation.GetCombinePointProcess(MOLTrigger.GameDayId, MOLTrigger.WeekId);

                        Catcher("Combine Leaderboard : Points calculation process completed retVal " + CombineLBRetVal);

                        if (CombineLBRetVal == 1)
                        {
                            Catcher("Combine Leaderboard : Leaderboard Ingestion Started");
                            await _Ingestion.LeaderBoardCombine(combineLBGamedayId, combineLBWeekid);
                            Catcher("Combine Leaderboard : Leaderboard Ingestion Completed");

                            Catcher($"Week Mapping Initiated...");
                            await _Ingestion.WeekMappingCombine();
                            Catcher($"Week Mapping Finished...");
                        }
                        else
                        {
                            Catcher($"Combine Leaderboard Process : RetVal : " + CombineLBRetVal);
                        }
                    }
                    else
                    {
                        Catcher($"Combine Leaderboard : No data found.");
                    }

                    Catcher("Combine Leaderboard : Points calculation process fully completed.");

                }
                catch (Exception ex)
                { Catcher("Combine Leaderboard : Run", LogLevel.Error, ex); }

                #endregion
            }
            else
            {
                Catcher("Combine Leaderboard Service is disabled from admin.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _Timer?.Change(Timeout.Infinite, 0);

            Catcher($"{_Service} stopped.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _Timer?.Dispose();
        }

        private async Task<bool> SendPushNotification(Int32 EventType, Int32? MatchId)
        {
            Contracts.Notification.NotificationText notificationText = new Contracts.Notification.NotificationText();
            notificationText = await _NotificationTopics.GetNotificationText();
            String message = "";
            bool success = false;
            switch (EventType)
            {
                // Notification Before Match Lock
                case 1:
                    message = notificationText.preMatch;
                    List<Contracts.Notification.NotificationMessages> n = new List<Contracts.Notification.NotificationMessages>();
                    n.Add(new Contracts.Notification.NotificationMessages
                    {
                        EventId = (Int32)Contracts.Notification.NotificationEvents.Generic,
                        Language = "en",
                        Message = message,
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, MatchId);
                    break;
                // Notification 1st Inning Unlock
                case 2:
                    message = notificationText.openInningOne;
                    n = new List<Contracts.Notification.NotificationMessages>();
                    n.Add(new Contracts.Notification.NotificationMessages
                    {
                        EventId = (Int32)Contracts.Notification.NotificationEvents.Generic,
                        Language = "en",
                        Message = message,
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, MatchId);
                    break;
                // Notification 2nd Inning Unlock
                case 3:
                    message = notificationText.openInningTwo;
                    n = new List<Contracts.Notification.NotificationMessages>();
                    n.Add(new Contracts.Notification.NotificationMessages
                    {
                        EventId = (Int32)Contracts.Notification.NotificationEvents.Generic,
                        Language = "en",
                        Message = message,
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, MatchId);
                    break;
                // Point Calculation
                case 4:
                    message = notificationText.postMatch;
                    n = new List<Contracts.Notification.NotificationMessages>();
                    n.Add(new Contracts.Notification.NotificationMessages
                    {
                        EventId = (Int32)Contracts.Notification.NotificationEvents.Generic,
                        Language = "en",
                        Message = message,
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, leaderboard: _LeaderBoardType);
                    break;
            }
            return success;
        }

        #region " Helper Functions  "

        #region " Notification body "

        private void CalculationNotify(Int32 matchdayId, Int64 result, StringBuilder reports)
        {
            try
            {
                string caption = $"{_Service} [MatchdayId: {matchdayId}]";
                string remark = ((result == 1) ? "SUCCESS" : "FAILED");

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += $"Predictor - - [ {_Service} RetVal: {result} ]<br/><br/><br/>";
                content += reports.ToString();

                String body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        #endregion



        public async Task<int> IngestThis(Int32 matchId)
        {
            Int32 retval = await _Ingestion.Fixtures();
            retval = await _Ingestion.MatchInningStatus(matchId);
            retval = await _Ingestion.CurrentGamedayMatches();
            return retval;
        }

        #endregion

        #region " Notification body "

        private void LockNotify(LockList list, Int32 lockResult, Int32 loadResult)
        {
            try
            {
                string caption = "Game Locking [MatchdayIds: " + string.Join(",", list.MatchdayIdList) + " MatchIds: " + string.Join(",", list.MatchIdList) + "]";
                string remark = ((lockResult == 1) ? "SUCCESS" : "FAILED");

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += "Livepools Fantasy - [ Lock RetVal: " + lockResult + " -- Load RetVal: " + loadResult + " ]<br/>";

                String body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        private void LiveNotify(LockList list, Int32 lockResult)
        {
            try
            {
                string caption = "Game Live [MatchdayIds: " + string.Join(",", list.MatchdayIdList) + " MatchIds: " + string.Join(",", list.MatchIdList) + "]";
                string remark = ((lockResult == 1) ? "SUCCESS" : "FAILED");

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += "Livepools Fantasy - [ Live RetVal: " + lockResult + " ]<br/>";

                String body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        #endregion

    }
}
