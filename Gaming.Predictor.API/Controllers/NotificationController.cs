using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Feeds;
using Gaming.Predictor.Contracts.Notification;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace Gaming.Predictor.API.Controllers
{
    [Route("services/api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly Blanket.Notification.Subscription _SubscriptionContext;
        private readonly IHostingEnvironment _Env;

        public NotificationController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IHostingEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _SubscriptionContext = new Blanket.Notification.Subscription(appSettings, aws, postgre, redis, cookies, asset);
            _Env = env;
        }

        /// <summary>
        /// Performs subscription related operations
        /// </summary>
        /// <param name="subscription">Post Data</param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [Route("{guid}/subscriptions")]
        [ActionName("subscriptions")]
        [HttpPost]
        public async Task<ActionResult<HTTPResponse>> Subscription([FromBody]Subscription subscription, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    String language = "en";
                    HTTPResponse response = await _SubscriptionContext.Subscriptions(subscription, language);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Updates the user's subscription details on device update
        /// </summary>
        /// <param name="subscription">Post Data</param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [Route("{guid}/deviceupdate")]
        [ActionName("deviceupdate")]
        [HttpPost]
        public async Task<ActionResult<HTTPResponse>> DeviceUpdate([FromBody]Subscription subscription, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    String language = "en";
                    HTTPResponse response = await _SubscriptionContext.DeviceUpdate(subscription, language);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Get Events subscribed by a user
        /// </summary>
        /// <param name="deviceId"> Id of the user's Device </param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [Route("{guid}/usersubscriptions")]
        [ActionName("usersubscriptions")]
        [HttpGet]
        public async Task<ActionResult<HTTPResponse>> GetUserSubscription(String deviceId, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = _SubscriptionContext.EventsGet(deviceId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

    }
}