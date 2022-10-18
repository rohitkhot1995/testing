using System;
using Gaming.Predictor.Contracts.Configuration;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Gaming.Predictor.Interfaces.Connection;

namespace Gaming.Predictor.Library.Asset
{
    public class Exist : Delete
    {
        public Exist(IAWS aws, IRedis redis, IOptions<Application> appSettings, ICookies cookies, IHttpContextAccessor httpContextAccessor)
            : base(aws, redis, appSettings, cookies, httpContextAccessor)
        {
        }

        public async Task<bool> HAS(String key)
        {
            bool has = false;

            if (_UseRedis)
                has = _Redis.Has(key);
            else
                has = await _AWS.Has(key);

            return has;
        }
    }
}
