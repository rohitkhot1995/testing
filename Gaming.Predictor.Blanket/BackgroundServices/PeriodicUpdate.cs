using System;
using System.Linq;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Common;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Contracts.Feeds;
using System.Collections.Generic;
using Gaming.Predictor.Interfaces.Session;
using Gaming.Predictor.Library.Utility;
using Gaming.Predictor.Contracts.BackgroundServices;
using Gaming.Predictor.Contracts.Admin;
using System.Net;
using System.Xml.Linq;

namespace Gaming.Predictor.Blanket.BackgroundServices
{
    public class PeriodicUpdate : Common.BaseServiceBlanket
    {

        private readonly Feeds.Gameplay _Feeds;
        private readonly DataAccess.BackgroundServices.PeriodicUpdate _PeriodicUpdateContext;
        private readonly Int32 _TourId;

        public PeriodicUpdate(IOptions<Application> appSettings, IOptions<Daemon> serviceSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
         : base(appSettings, serviceSettings, aws, postgre, redis, cookies, asset)
        {
            _Feeds = new Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _PeriodicUpdateContext = new DataAccess.BackgroundServices.PeriodicUpdate(postgre);
            _TourId = appSettings.Value.Properties.TourId;            
        }

        public Int32 PartitionUpdate(Int32 optType, Int32 matchId, Int32 gamedayId)
        {
            return _PeriodicUpdateContext.PartitionUpdate(optType, _TourId, gamedayId);
        }
    }
}
