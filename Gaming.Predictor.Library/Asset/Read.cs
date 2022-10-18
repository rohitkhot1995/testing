using System;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Gaming.Predictor.Interfaces.Connection;

namespace Gaming.Predictor.Library.Asset
{
    public class Read : BaseAsset
    {
        public Read(IAWS aws, IRedis redis, IOptions<Application> appSettings, ICookies cookies, IHttpContextAccessor httpContextAccessor)
            : base(aws, redis, appSettings, cookies, httpContextAccessor)
        {
        }

        public async Task<String> GET(String key)
        {
            String content = "";

            if (_UseRedis)
                content =  _Redis.GetData(key);
            else
            {
                try
                {
                    content = await _AWS.Get(key);
                }
                catch { }
            }

            return content;
        }

        

        

        public async Task<String> GETFromS3(String key)
        {
            String content = "";

            content = await _AWS.Get(key);

            return content;
        }
    }
}