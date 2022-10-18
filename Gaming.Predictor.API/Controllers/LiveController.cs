using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Configuration;
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
    public class LiveController : BaseController
    {
        private readonly Blanket.Feeds.Gameplay _FeedContext;
        private readonly IHostingEnvironment _Env;

        public LiveController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IHostingEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _FeedContext = new Blanket.Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _Env = env;
        }

        /// <summary>
        /// Returns Current Gameday Matches  
        /// </summary>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [HttpGet("currentgamedaymatches")]
        public async Task<ActionResult<HTTPResponse>> CurrentGamedayMatches(String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _FeedContext.GetCurrentGamedayMatches();

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// MIX API 
        /// </summary>
        /// <param name="backdoor">Request Authentication Key</param>
        /// <returns></returns>
        [HttpGet("mixApi")]
        public async Task<ActionResult<HTTPResponse>> GetMixAPI(string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _FeedContext.GetMixAPI();

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Tour Status 
        /// </summary>
        /// <param name="backdoor">Request Authentication Key</param>
        /// <returns></returns>
        [HttpGet("tourstatus")]
        public async Task<ActionResult<HTTPResponse>> GetTourStatus(string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _FeedContext.GetTourStatus();

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