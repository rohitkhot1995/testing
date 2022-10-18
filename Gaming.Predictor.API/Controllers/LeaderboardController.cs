using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Feeds;
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
    public class LeaderboardController : BaseController
    {
        private readonly Blanket.Leaderboard.Leaderbaord _LeaderbaordContext;
        private readonly IHostingEnvironment _Env;


        public LeaderboardController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IHostingEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _LeaderbaordContext = new Blanket.Leaderboard.Leaderbaord(appSettings, aws, postgre, redis, cookies, asset);
            _Env = env;
        }

        /// <summary>
        /// GET LEADERBOARD
        /// </summary>
        /// <param name="optType">1 for Overall  | 2 for Match </param>
        /// <param name="gamedayID"> Required Gameday ID</param>
        /// <param name="phaseId"> PhaseId </param>
        /// <param name="backdoor"> Request Authentication Key </param>
        /// <returns></returns>
        [HttpGet("leaderboard")]
        public async Task<ActionResult<HTTPResponse>> GetLeaderboard(Int32 optType, Int32 gamedayID, Int32 phaseId ,String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _LeaderbaordContext.GetLeaderboard(optType,gamedayID, phaseId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Return User Rank And points
        /// </summary>
        /// <param name="optType">1 for Overall  | 2 for Match </param>
        /// <param name="gamedayId"> Required Gameday ID</param>
        /// <param name="phaseId"> Phase ID</param>
        /// <param name="backdoor">Request Authentication Key</param>
        /// <returns></returns>
        [HttpGet("user/{userguid}/userrank")]
        public async Task<ActionResult<HTTPResponse>> GetUserRank(Int32 optType, Int32 gamedayId, Int32 phaseId, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _LeaderbaordContext.GetUserRank(optType, gamedayId, phaseId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Returns played gamedays.
        /// </summary>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [HttpGet("{userguid}/playedgamedays")]
        public async Task<ActionResult<HTTPResponse>> GetPlayedGameDays(String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _LeaderbaordContext.PlayedGamedays();
                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// GET COMBINE LEADERBOARD
        /// </summary>
        /// <param name="optType">1 for Overall  | 2 for Week </param>
        /// <param name="gamedayID"> Gameday ID</param>
        /// <param name="weekId"> Week Id </param>
        /// <param name="backdoor"> Request Authentication Key </param>
        /// <returns></returns>
        [HttpGet("leaderboardcombine")]
        public async Task<ActionResult<HTTPResponse>> GetLeaderboardCombine(Int32 optType, Int32 gamedayID, Int32 weekId, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _LeaderbaordContext.GetLeaderboardCombine(optType, gamedayID, weekId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Return User Combine Rank And points
        /// </summary>
        /// <param name="optType">1 for Overall  | 2 for Week </param>
        /// <param name="gamedayId"> Gameday ID</param>
        /// <param name="weekId"> Week ID</param>
        /// <param name="backdoor">Request Authentication Key</param>
        /// <returns></returns>
        [HttpGet("user/{userguid}/userrankCombine")]
        public async Task<ActionResult<HTTPResponse>> GetUserRankCombine(Int32 optType, Int32 gamedayId, Int32 weekId, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _LeaderbaordContext.GetUserRankCombine(optType, gamedayId, weekId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        [HttpGet("weekmapping")]
        public async Task<ActionResult<HTTPResponse>> GetWeekMapping(bool fetch = false, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    ResponseObject rObject = new ResponseObject();
                    HTTPResponse httpResponse = new HTTPResponse();
                    HTTPMeta meta = new HTTPMeta();
                    httpResponse = await _LeaderbaordContext.GetWeekMappingCombineLB(fetch);
                    return Ok(httpResponse);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }
    }
}