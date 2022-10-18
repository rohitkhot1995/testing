using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Interfaces.Admin;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gaming.Predictor.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IOptions<Application> _AppSettings;
        protected readonly ISession _Session;
        protected readonly IAWS _AWS;
        protected readonly IPostgre _Postgre;
        protected readonly ICookies _Cookies;
        protected readonly IRedis _Redis;
        protected readonly IAsset _Asset;
        protected readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _HttpContext;
        protected readonly Library.Dependency.Authentication _Authentication;
        protected readonly Int32 _ClientId;


        public BaseController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext)
        {
            _AppSettings = appSettings;
            _AWS = aws;
            _Postgre = postgre;
            _Cookies = cookies;
            _Redis = redis;
            _Asset = asset;
            _HttpContext = httpContext;
            _Authentication = new Library.Dependency.Authentication(appSettings, httpContext);
            _ClientId = appSettings.Value.Properties.ClientId;

        }
    }
}
