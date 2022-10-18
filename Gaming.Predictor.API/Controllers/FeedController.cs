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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gaming.Predictor.API.Controllers
{
    [Route("services/api/[controller]")]
    [ApiController]
    public class FeedController : BaseController
    {
        private readonly Blanket.Feeds.Gameplay _FeedContext;
        private readonly Blanket.Feeds.Gameplay _GamePlayContext;
        private readonly IHostingEnvironment _Env;

        public FeedController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IHostingEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _FeedContext = new Blanket.Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _GamePlayContext = new Blanket.Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _Env = env;
        }

        /// <summary>
        /// API Status check
        /// </summary>
        /// <returns></returns>
        [HttpGet("ping")]
        [ProducesResponseType(200)]
        public ActionResult Ping()
        {
            if (_Env.IsDevelopment())
            {
                string jsonFile = "Gaming.Predictor.API.json";
                string jsonPath = System.IO.Path.Combine(@"D:\publish\RoyalStag\Predictor\", jsonFile);
                string swaggerUrl = "http://localhost:56801/swagger/v1/swagger.json";
                System.IO.File.WriteAllText(jsonPath, Library.Utility.GenericFunctions.GetWebData(swaggerUrl));
            }

            return Ok("Gaming.Predictor.API");
        }

        /// <summary>
        /// Returns all the available languages
        /// </summary>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [HttpGet("languages")]
        public async Task<ActionResult<HTTPResponse>> Languages(String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _FeedContext.GetLanguages();

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Returns language-wise fixtures for the tournament
        /// </summary>
        /// <param name="lang">Language | Default = en</param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [HttpGet("tourfixtures")]
        public async Task<ActionResult<HTTPResponse>> TourFixtures(string lang, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    lang = await _FeedContext.DefaultLang(lang);

                    HTTPResponse response = await _FeedContext.GetFixtures(lang);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Returns Question for the match.
        /// </summary>
        /// <param name="MatchId">MatchId</param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [HttpGet("match/questions")]
        public async Task<ActionResult<HTTPResponse>> MatchQuestions(Int32 MatchId, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {

                    HTTPResponse response = await _GamePlayContext.GetQuestions(MatchId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Return Sport Constraints
        /// </summary>
        /// <param name="backdoor">Request Authentication Key</param>
        /// <returns></returns>
        [HttpGet("tourconstraints")]
        public async Task<ActionResult<HTTPResponse>> GetConstraints(string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _FeedContext.GetConstraints();

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
