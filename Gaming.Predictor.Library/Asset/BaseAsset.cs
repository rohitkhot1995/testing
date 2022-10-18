using System;
using Gaming.Predictor.Contracts.Configuration;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Http;
using Gaming.Predictor.Interfaces.Connection;

namespace Gaming.Predictor.Library.Asset
{
    public class BaseAsset
    {
        protected readonly IAWS _AWS;
        protected readonly IRedis _Redis;
        protected readonly bool _UseRedis;
        protected readonly Int32 _TourId;
        protected readonly String _Environment;
        protected readonly ICookies _Cookies;
        protected readonly IHttpContextAccessor _HttpContextAccessor;
        protected readonly Application _AppSettings;

        public BaseAsset(IAWS aws, IRedis redis, IOptions<Application> appSettings, ICookies cookies, IHttpContextAccessor httpContextAccessor)
        {
            _AWS = aws;
            _Redis = redis;
            _UseRedis = appSettings.Value.Connection.Redis.Apply;
            _TourId = appSettings.Value.Properties.TourId;
            _Environment = appSettings.Value.Connection.Environment;
            _Cookies = cookies;
            _HttpContextAccessor = httpContextAccessor;
            _AppSettings = appSettings.Value;
        }
    }
}
