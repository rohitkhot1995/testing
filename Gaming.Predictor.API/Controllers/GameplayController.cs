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
    public class GameplayController : BaseController
    {
        private readonly Blanket.Feeds.Gameplay _GamePlayContext;
        private readonly IHostingEnvironment _Env;

        public GameplayController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext, IHostingEnvironment env)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _GamePlayContext = new Blanket.Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _Env = env;
        }

        #region " GET "   

        /// <summary>
        /// Return recent results of teams.
        /// </summary>
        /// <param name="backdoor">backdoor</param>
        /// <returns></returns>
        [HttpGet("recentresults")]
        public async Task<ActionResult<HTTPResponse>> RecentResults(String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {

                    HTTPResponse response = await _GamePlayContext.GetRecentResults();

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Returns match status and match inning status of match.
        /// </summary>
        /// <param name="MatchId">MatchId</param>
        /// <param name="backdoor">backdoor</param>
        /// <returns></returns>
        [HttpGet("matchinningstatus")]
        public async Task<ActionResult<HTTPResponse>> MatchInningStatus(Int32 MatchId, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _GamePlayContext.MatchInningStatus(MatchId);
                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        [HttpGet("{userguid}/getprofile")]
        public async Task<ActionResult<HTTPResponse>> GetProfile(Int32 PlatformId, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _GamePlayContext.GetUserProfile(PlatformId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }


        [HttpGet("{userguid}/getgameplays")]
        public async Task<ActionResult<HTTPResponse>> GetGamePlays(String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {

                    HTTPResponse response = await _GamePlayContext.GetGamePlays();

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        #endregion " GET "

        #region " Prediction API "

        ///<summary>
        ///Returns prediction submitted by users.
        ///</summary>
        ///<param name="MatchId">MatchId</param>
        ///<param name="GameDayId">GameDayId</param>
        /// <param name="userguid">The GUID of the user</param>
        ///<param name="backdoor">backdoor</param>
        ///<returns></returns>
        [HttpGet("{userguid}/getpredictions")]
        public async Task<ActionResult<HTTPResponse>> GetPredictions(Int32 MatchId, Int32 GameDayId, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {

                    HTTPResponse response = await _GamePlayContext.GetUserPredictions(MatchId, GameDayId);

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Returns Match Detail of User Predict
        /// </summary>
        /// <param name="SportId">Sport ID</param>
        /// <param name="backdoor">Request Authentication Key</param>
        /// <returns></returns>
        [HttpGet("user/{userguid}/predictedmatches")]
        public async Task<ActionResult<HTTPResponse>> GetUserMatchPredicts(String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _GamePlayContext.GetUserPredictedMatches();

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Return User Rank & points of Match
        /// </summary>
        /// <param name="matchIds">List of Coma(,) Seperated Match Id</param>
        /// <param name="backdoor">Request Authentication Key</param>
        /// <returns></returns>
        [HttpGet("user/{userguid}/usermatchrank")]
        public async Task<ActionResult<HTTPResponse>> GetUserMatchRank(String matchIds, String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    if (matchIds.Length > 0)
                    {

                        var Ids = matchIds.Split(",");
                        HTTPResponse response = await _GamePlayContext.GetUserMatchRank(Ids);

                        return Ok(response);
                    }
                    else
                        return BadRequest();
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Submit the User's predictions.
        /// </summary>
        /// <param name="MatchId">MatchId</param>
        /// <param name="TourGamedayId">TourGamedayId</param>
        /// <param name="QuestionId">QuestionId</param>
        /// <param name="OptionId">OptionId</param>
        /// <param name="BetCoin">BetCoin</param>
        /// <param name="PlatformId">PlatformId - 3 Web</param>
        /// <param name="userguid">The GUID of the user</param>
        /// <param name="backdoor">backdoor</param>
        /// <returns></returns>
        [HttpPost("{userguid}/userprediction")]
        public ActionResult<HTTPResponse> UserPrediction(UserPredictionPayload prediction, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = _GamePlayContext.UserPrediction(prediction.MatchId, prediction.TourGamedayId, prediction.QuestionId, prediction.OptionId, prediction.BetCoin, prediction.PlatformId);
                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Submit the User's predictions.
        /// </summary>
        /// <param name="MatchId">MatchId</param>
        /// <param name="TourGamedayId">TourGamedayId</param>
        /// <param name="QuestionId">QuestionId</param>
        /// <param name="OptionId">OptionId</param>
        /// <param name="PlatformId">PlatformId - 3 Web</param>
        /// <param name="BetCoin">PlatformId - 1 Android | 2 IOS</param>
        /// <param name="userguid">The GUID of the user</param>
        /// <param name="backdoor">backdoor</param>
        /// <returns></returns>
        [HttpPost("{userguid}/userpredictionbet")]
        public ActionResult<HTTPResponse> UserPredictionBet(Int32 MatchId, Int32 TourGamedayId, Int32 QuestionId, Int32 OptionId ,Int32 BetCoin, Int32 PlatformId = 3, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = _GamePlayContext.UserPredictionBet(MatchId, TourGamedayId, QuestionId, OptionId, BetCoin, PlatformId);
                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }


        /// <summary>
        /// Returns Balance Coins & update cookie
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/{userguid}/getuserbalance")]
        public async Task<ActionResult<HTTPResponse>> GetUserBalance(String backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = await _GamePlayContext.GetUserBalance();

                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optType"></param>
        /// <param name="matchId"></param>
        /// <param name="gamedayId"></param>
        /// <param name="boosterId"></param>
        /// <param name="platformId"></param>
        /// <param name="questionId"></param>
        /// <param name="backdoor"></param>
        /// <returns></returns>
        [HttpPost("user/{userguid}/applyjoker")]
        public ActionResult<HTTPResponse> ApplyJoker(AppkyJokerPayload payload, string backdoor = null)
        {
            if (ModelState.IsValid)
            {
                if (_Authentication.Validate(backdoor))
                {
                    HTTPResponse response = _GamePlayContext.ApplyJokerCard(payload.OptType, payload.MatchId, payload.TourGamedayId, payload.PlatformId,
                                                                            payload.BoosterId, payload.QuestionId);
                    return Ok(response);
                }
                else
                    return Unauthorized();
            }
            else
                return BadRequest();
        }

        #endregion " POST "


        /////<summary>
        /////Returns prediction submitted by users.
        /////</summary>
        /////<param name="MatchId">MatchId</param>
        /////<param name="GameDayId">GameDayId</param>
        /////<param name="UserId">UserId</param>
        /////<param name="UserTeamId">GameDayId</param>
        ///// <param name="userguid">The GUID of the user</param>
        /////<param name="backdoor">backdoor</param>
        /////<returns></returns>
        //[HttpGet("{userguid}/getotheruserpredictions")]
        //public async Task<ActionResult<HTTPResponse>> GetOtherUserPredictions(Int32 MatchId, Int32 GameDayId, String UserId, String UserTeamId, String backdoor = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (_Authentication.Validate(backdoor))
        //        {

        //            HTTPResponse response = await _GamePlayContext.GetOtherUserPredictions(MatchId, GameDayId, UserId, UserTeamId);

        //            return Ok(response);
        //        }
        //        else
        //            return Unauthorized();
        //    }
        //    else
        //        return BadRequest();
        //}

    }
}