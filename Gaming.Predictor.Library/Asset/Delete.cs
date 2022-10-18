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
    public class Delete : Write
    {
        public Delete(IAWS aws, IRedis redis, IOptions<Application> appSettings, ICookies cookies, IHttpContextAccessor httpContextAccessor)
            : base(aws, redis, appSettings, cookies, httpContextAccessor)
        {
        }

        public async Task<bool> REMOVE(String key)
        {
            bool success = false;

            if (_UseRedis)
                success = _Redis.Delete(key);
            else
                success = await _AWS.Remove(key);

            return success;
        }
    }
}
