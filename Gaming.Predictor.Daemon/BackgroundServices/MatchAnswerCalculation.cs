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

namespace Gaming.Predictor.Daemon.BackgroundServices
{
    public class MatchAnswerCalculation : BaseService<MatchAnswerCalculation>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private readonly Blanket.Scoring.Process _ScoringContext;
        private readonly Blanket.Feeds.Gameplay _Feeds;
        private Blanket.Feeds.Ingestion _Ingestion;
        private readonly Blanket.BackgroundServices.MatchAnswerCalculation _MatchAnswerCalculationContext;
        private Int32 _Interval;

        public MatchAnswerCalculation(ILogger<MatchAnswerCalculation> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings,
           IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset) : base(logger, appSettings, serviceSettings, aws, postgre, redis, asset)
        {
            _ScoringContext = new Blanket.Scoring.Process(appSettings, aws, postgre, redis, cookies, asset);
            _Ingestion = new Blanket.Feeds.Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _Feeds = new Blanket.Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _MatchAnswerCalculationContext = new Blanket.BackgroundServices.MatchAnswerCalculation(appSettings, aws, postgre, redis, cookies, asset);
            _Interval = serviceSettings.Value.MatchAnswerCalculation.IntervalMinutes;
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
            _Timer?.Change(Convert.ToInt32(TimeSpan.FromSeconds(_Interval).TotalMilliseconds), Timeout.Infinite);
        }

        private async void Run(object state)
        {
            Int32 DaemonServiceStatus = 0;
            string _daemonStatus = "";

            try
            {
                _daemonStatus = await _Asset.GET(_Asset.DaemonServiceMatchAnswer());

                DaemonServiceStatus = string.IsNullOrEmpty(_daemonStatus) ? 0 : Convert.ToInt32(_daemonStatus);
            }
            catch (Exception ex)
            {
                Catcher("Game Locking : Fetch Daemon Status : Data : " + _daemonStatus, LogLevel.Error, ex);
            }

            if (DaemonServiceStatus > 0)
            {

                try
            {
                Boolean success = false;

                List<Fixtures> mFixtures = new List<Fixtures>();
                mFixtures = _MatchAnswerCalculationContext.GetFinishedMatches();
                if (mFixtures != null && mFixtures.Any())
                {

                    foreach (Fixtures fixture in mFixtures)
                    {
                        MatchFeed mMatchFeed = new MatchFeed();
                        mMatchFeed = _ScoringContext.GetMatchFeed(fixture.Matchfile);

                        if (mMatchFeed != null && mMatchFeed.Matchdetail.Status.ToLower() == "match ended" 
                            && mMatchFeed.Matchdetail.Verification_Completed == true)
                        {

                            Catcher("Answers Submission Started For Match Id : " + fixture.MatchId);

                            success = _ScoringContext.CalculateAnswers(fixture);

                            Catcher("Answers Submission Completed For Match Id : " + fixture.MatchId + " Result :" + success);

                            //todo: commented db call for testing
                            if (success)
                                success = _ScoringContext.QuestionAnswerProcessUpdate(fixture.MatchId);
                            if (success)
                                success = _ScoringContext.SubmitMatchWinTeam(fixture);
                            else
                                break;
                        }
                    }


                    Catcher("Fixtures ingestion started.");

                    await _Ingestion.Fixtures();

                    Catcher("Fixtures ingestion completed. CurrentGamedayMatches ingestion started.");

                    await _Ingestion.CurrentGamedayMatches();

                    Catcher("CurrentGamedayMatches ingestion completed. Teams Recent Results igestion started.");

                    await _Ingestion.GetRecentResults();

                    Catcher("Teams Recent Results ingestion completed.");
                }
            }
            catch (Exception ex)
            { 
                Catcher("Run", LogLevel.Error, ex);
            }

            }
            else
            {
                Catcher("Match Answer Calculation Service is disabled from admin.");
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
