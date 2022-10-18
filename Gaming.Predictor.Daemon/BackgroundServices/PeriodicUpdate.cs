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
    class PeriodicUpdate : BaseService<PeriodicUpdate>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private Blanket.BackgroundServices.PeriodicUpdate _PeriodicUpdate;
        private Blanket.Feeds.Ingestion _Ingestion;
        private Int32 _Interval;

        public PeriodicUpdate(ILogger<PeriodicUpdate> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings,
           IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset) : base(logger, appSettings, serviceSettings, aws, postgre, redis, asset)
        {
            _PeriodicUpdate = new Blanket.BackgroundServices.PeriodicUpdate(appSettings, serviceSettings, aws, postgre, redis, cookies, asset);
            _Ingestion = new Blanket.Feeds.Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _Interval = serviceSettings.Value.PeriodicUpdate.IntervalMinutes;
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
            _Timer?.Change(Convert.ToInt32(TimeSpan.FromMinutes(_Interval).TotalMilliseconds), Timeout.Infinite);
        }

        private async void Run(object state)
        {
            try
            {
                Int32 retVal = 0;
                Int32 partitionRetVal = _PeriodicUpdate.PartitionUpdate(1, _TourId, 0);
                Catcher($"Iteration completed. Partition RetVal: {partitionRetVal}");
                if (partitionRetVal == 1)
                    retVal = await _Ingestion.Fixtures();                
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
