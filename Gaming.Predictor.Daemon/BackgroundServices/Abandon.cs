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

namespace Gaming.Predictor.Daemon.BackgroundServices
{
    public class Abandon : BaseService<PointsCalculation>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private String _Service = nameof(Abandon);
        private readonly Blanket.Feeds.Gameplay _Feeds;
        private readonly Blanket.BackgroundServices.Abandon _Abandon;

        private Blanket.Feeds.Ingestion _Ingestion;
        private Blanket.Notification.Topics _NotificationTopics;
        private Blanket.Notification.Publish _NotificationPublish;
        private Int32 _Interval;
        private Int32 _NotificationsDelaySeconds;

        public Abandon(ILogger<PointsCalculation> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings,
           IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset) : base(logger, appSettings, serviceSettings, aws, postgre, redis, asset)
        {
            _Ingestion = new Blanket.Feeds.Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _Feeds = new Blanket.Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _Abandon = new Blanket.BackgroundServices.Abandon(appSettings,serviceSettings, aws, postgre, redis, cookies, asset);
            _NotificationTopics = new Blanket.Notification.Topics(appSettings, aws, postgre, redis, cookies, asset);
            _NotificationPublish = new Blanket.Notification.Publish(appSettings, aws, postgre, redis, cookies, asset);
            _Interval = serviceSettings.Value.PointsCalculation.IntervalMinutes;
            _NotificationsDelaySeconds = serviceSettings.Value.NotificationDelaySeconds;
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
            Int32 retVal = -40;
            StringBuilder rp = new StringBuilder();
            try
            {
                Boolean success = false;

                AbdPointPrcGet abdPointPrcGet = _Abandon.AbandonPrcGet();

                if (abdPointPrcGet != null && abdPointPrcGet.TourGamedayId != 0)
                {

                    Catcher("MatchdayId found: " + GenericFunctions.Serialize(abdPointPrcGet));

                    Catcher("Abandon Point calculation process started for gameDayId " + abdPointPrcGet.TourGamedayId + " matchdayId " + abdPointPrcGet.MatchDay);

                    retVal = _Abandon.AbandonPointPrc(abdPointPrcGet.TourGamedayId, abdPointPrcGet.MatchDay);

                    Catcher("Abandon Point calculation process completed. Retval: " + retVal);

                    if (retVal == 1)
                    {

                        Catcher("Reports process started.");

                        DataSet ds = _Abandon.AbandonPointProcessReports(retVal, abdPointPrcGet.TourGamedayId, abdPointPrcGet.MatchDay);

                        rp = _Abandon.ParseReports(ds);

                        Catcher("Reports process completed.");

                        Catcher("Match Questions ingestion started");

                        List<Fixtures> fixtures = new List<Fixtures>();
                        Contracts.Common.HTTPResponse httpResponse = _Feeds.GetFixtures("en").Result;
                        if (httpResponse.Meta.RetVal == 1)
                        {
                            if (httpResponse.Data != null)
                                fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((Contracts.Common.ResponseObject)httpResponse.Data).Value));
                        }
                        List<Int32> _MatchIds = new List<Int32>();
                        _MatchIds = fixtures.Where(c => c.GamedayId == abdPointPrcGet.TourGamedayId).Select(x => x.MatchId).ToList();
                        foreach (Int32 mMatchId in _MatchIds)
                        {
                            Int32 retval = await _Ingestion.Questions(mMatchId);
                            Catcher("Match Questions ingestion completed for matchId : " + mMatchId + " retVal " + retval);
                        }

                        Catcher("Match Questions ingestion completed. leaderboard ingestion started.");

                        await _Ingestion.LeaderBoard(abdPointPrcGet.TourGamedayId, abdPointPrcGet.PhaseID);

                        Catcher("General leaderboard ingestion completed. Teams recent recent results ingestion started.");

                        await _Ingestion.GetRecentResults();

                        Catcher("Teams recent recent results ingestion completed. Fixtures ingestion started again.");


                        await _Ingestion.Fixtures();

                        Catcher("Fixtures ingestion completed again. CurrentGamedayMatches ingestion started.");


                        await _Ingestion.CurrentGamedayMatches();

                        Catcher("CurrentGamedayMatches ingestion completed.");

                        //await _Ingestion.UpdateMixApiLeaderboard(2, abdPointPrcGet.TourGamedayId);
                        foreach (Int32 mMatchId in _MatchIds)
                        {
                            await _Ingestion.UpdateMixApiLeaderboard(2, mMatchId);
                        }

                        CalculationNotify(abdPointPrcGet.TourGamedayId, retVal, rp);
                    }

                    Catcher("Abandon Points calculation process fully completed.");
                }
                else
                    Catcher("No Match to Abandon.");


            }
            catch (Exception ex)
            { Catcher("Run", LogLevel.Error, ex); }
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

        #region " Helper Functions  "

        #region " Notification body "

        private void CalculationNotify(Int32 matchdayId, Int64 result, StringBuilder reports)
        {
            try
            {
                string caption = $"{_Service} [MatchdayId: {matchdayId}]";
                string remark = ((result == 1) ? "SUCCESS" : "FAILED");

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += $"DCF Fantasy - [ {_Service} RetVal: {result} ]<br/><br/><br/>";
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
