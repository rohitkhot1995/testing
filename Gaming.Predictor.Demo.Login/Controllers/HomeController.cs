using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Blanket.Template;
using Gaming.Predictor.Contracts.Session;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Contracts.Common;

namespace DCF.Fantasy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly Template _TemplateContext;
        public readonly String _StaticAssetsBasePath;
        private readonly IHttpContextAccessor _HttpContext;
        private readonly String _Lang;

        protected readonly ICookies _Cookies;
        private readonly Gaming.Predictor.Blanket.Session.User _SessionContext;
        private readonly String _BasePath;

        public HomeController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis,
                                                ICookies cookies, IAsset asset, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext)
           : base()
        {
            _TemplateContext = new Template(appSettings, aws, postgre, redis, cookies, asset);
            _StaticAssetsBasePath = appSettings.Value.Properties.StaticAssetBasePath;
            _HttpContext = httpContext;
            _Lang = appSettings.Value.Properties.Languages[0].ToString();

            _Cookies = cookies;
            _SessionContext = new Gaming.Predictor.Blanket.Session.User(appSettings, aws, postgre, redis, cookies, asset);
            _BasePath = appSettings.Value.CustomSwaggerConfig.BasePath;
        }

        public async Task<IActionResult> Login(string redirect)
        {
            //if (_Cookies._HasUserCookies || _Cookies._HasGameCookies)
            //{
            //    _Cookies.DeleteCookies();
            //    //Response.Redirect("matches/upcoming");
            //}
            //else
            //{
                string url = "" + Uri.EscapeUriString(HttpContext.Request.QueryString.Value);

                ViewBag.Redirect = url.Replace("?redirect=", "").Replace("%252F", "/").Replace("%253A", ":");
            //}

            ViewBag.BasePath = _BasePath;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Gaming.Predictor.Web.Models.DummyLogin loginDetails, string redirect)
        {
            String data = String.Empty;
            HTTPResponse response;

            string url = "" + HttpContext.Request.QueryString;
            if (!string.IsNullOrEmpty(loginDetails.SocialId))
            {


                MasterLoginCookie credentials = new MasterLoginCookie();
                credentials.SocialId = loginDetails.SocialId;
                credentials.UserName = loginDetails.FullName;
                credentials.EmailId = loginDetails.FullName.Replace(" ", "");

                bool result = _Cookies.SetGamingDemoCookie(credentials);

                if (result)
                {
                    ViewBag.HasUserCookie = "1";
                    if (!string.IsNullOrEmpty(redirect))
                    {

                        Response.Redirect(redirect);
                    }
                    else
                    {
                        Response.Redirect("home");
                    }

                }
            }

            ViewBag.BasePath = _BasePath;

            return View();
        }


    }
}
