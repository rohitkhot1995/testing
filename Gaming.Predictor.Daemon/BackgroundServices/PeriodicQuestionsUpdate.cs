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
    class PeriodicQuestionsUpdate : BaseService<PeriodicQuestionsUpdate>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private Blanket.Feeds.Ingestion _Ingestion;
        private Blanket.BackgroundServices.GameLocking _Locking;
        private Int32 _Interval;

        public PeriodicQuestionsUpdate(ILogger<PeriodicQuestionsUpdate> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings,
           IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset) : base(logger, appSettings, serviceSettings, aws, postgre, redis, asset)
        {
            _Ingestion = new Blanket.Feeds.Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _Interval = serviceSettings.Value.PeriodicQuestionsUpdate.IntervalMinutes;
            _Locking = new Blanket.BackgroundServices.GameLocking(appSettings, serviceSettings, aws, postgre, redis, cookies, asset);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Catcher("Started.");

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
            try
            {
                List<Fixtures> liveMatchFixtures = _Locking.LiveMatchList();
                Int32 RetVal = -60;
                foreach (Fixtures fixtures in liveMatchFixtures)
                {
                    Catcher($"Periodic Question started for matchID:{fixtures.MatchId}.");
                    RetVal = await _Ingestion.Questions(fixtures.MatchId);
                    Catcher($"Periodic Question updated for matchID:{fixtures.MatchId}. RetVal: {RetVal}");
                }
            }
            catch (Exception ex)
            {
                Catcher("Run", LogLevel.Error, ex);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _Timer?.Change(Timeout.Infinite, 0);

            Catcher("Stopped.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _Timer?.Dispose();
        }
    }
}
