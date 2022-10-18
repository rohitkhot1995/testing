using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Data;

namespace Gaming.Predictor.Blanket.Management
{
    public class Tour : Common.BaseBlanket
    {
        private readonly DataAccess.Management.Tour _DBContext;
        private readonly Int32 _TourId;

        public Tour(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
          : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Management.Tour(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public DataTable GetTournaments()
        {
            Int32 optType = 1;
            return _DBContext.GetTournaments(optType, _TourId);
        }
    }
}