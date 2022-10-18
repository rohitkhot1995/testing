using System;
using Gaming.Predictor.Contracts.Configuration;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Common;
using System.IO;
using Gaming.Predictor.Interfaces.Connection;

namespace Gaming.Predictor.Library.Asset
{
    public class Write : Read
    {
        private readonly FileSystem.Broker _FileSystem;

        public Write(IAWS aws, IRedis redis, IOptions<Application> appSettings, ICookies cookies, IHttpContextAccessor httpContextAccessor)
            : base(aws, redis, appSettings, cookies, httpContextAccessor)
        {
            _FileSystem = new FileSystem.Broker();
        }

        public async Task<bool> SET(String key, Object content, bool serialize = true)
        {
            bool success = false;

            if (_UseRedis)
                success = _Redis.SetData(key, content, serialize);
            else
                success = await _AWS.Set(key, content, serialize);

            return success;
        }

        

        
        public async Task<bool> SETToS3(String key, Object content, bool serialize = true)
        {
            bool success = false;

            success = await _AWS.Set(key, content, serialize);

            return success;
        }

        public async Task<bool> RedisSET(String key, Object content, bool serialize = true)
        {
            _Redis.SetData(key, content, serialize);

            return true;
        }

        public async Task<bool> SET(String bucket, String key, Object content, bool serialize = true)
        {
            return await _AWS.Set(bucket, key, content, serialize);
        }

        public async Task<bool> SETimage(String key, Stream content, bool downloadable = false)
        {
            bool success = false;

            if (_UseRedis)
                success = _Redis.SetData(key, content, false);
            else
                success = await _AWS.SetImage(key, content, downloadable);

            return success;
        }

        public async Task DEBUG(String message)
        {
            await _AWS.Debug(message);
        }
    }
}