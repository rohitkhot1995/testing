using Gaming.Predictor.Contracts.Configuration;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Session;

namespace Gaming.Predictor.Blanket.Common
{
    public class BaseServiceBlanket : BaseBlanket
    {
        protected readonly IOptions<Daemon> _ServiceSettings;

        public BaseServiceBlanket(IOptions<Application> appSettings, IOptions<Daemon> serviceSettings, IAWS aws, IPostgre postgre, IRedis redis,
            ICookies cookies, IAsset asset) : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _ServiceSettings = serviceSettings;
        }
    }
}
