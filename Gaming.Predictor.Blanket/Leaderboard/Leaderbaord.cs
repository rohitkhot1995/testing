using Gaming.Predictor.Contracts.Admin;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Enums;
using Gaming.Predictor.Contracts.Feeds;
using Gaming.Predictor.Contracts.Leaderboard;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Gaming.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Gaming.Predictor.Blanket.Leaderboard
{
    public class Leaderbaord : Common.BaseBlanket
    {
        private readonly DataAccess.Leaderboard.Leaderbaord _DBContext;
        private readonly Utility _Utility;
        protected readonly Int32 _TourId;
        private readonly DataAccess.Feeds.Gameplay _DBFeedContext;

        public Leaderbaord(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Leaderboard.Leaderbaord(postgre);
            _Utility = new Utility(appSettings, aws, postgre, redis, cookies, asset);
            _TourId = appSettings.Value.Properties.TourId;
            _DBFeedContext = new DataAccess.Feeds.Gameplay(postgre);
        }

        #region "Leaderboard"

        public async Task<HTTPResponse> GetLeaderboard(int optType, int GamedayId, Int32 phaseId, bool offline = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 retVal = -40;

            //if (Enum.IsDefined(typeof(LeaderboardType), optType))
            //{
            //    LeaderboardType leaderboardType = (LeaderboardType)optType;
            try
            {
                String data = await _Asset.GET(_Asset.LeaderBoard(optType, GamedayId, phaseId));
                httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);
                retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

                //switch (leaderboardType)
                //{
                //    case LeaderboardType.Match:
                //httpResponse = await GetMatchLeaderboard(optType,GamedayId,phaseId,offline);
                //    break;
                //case LeaderboardType.Overall:
                //httpResponse = await GetTourLeaderboard(clientId, sportId, pageNo, isLive, offline);
                //        break;
                //}
            }
            catch (Exception ex)
            {
                retVal = -40;
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Leaderboard.GetLeaderboard", ex.Message);
                _AWS.Log(httpLog);
            }
            //}
            //else
            //    return CatchResponse("Invalid OptType");

            GenericFunctions.AssetMeta(retVal, ref httpMeta, "");
            httpResponse.Meta = httpMeta;

            return httpResponse;
        }

        public async Task<HTTPResponse> GetLeaderboardCombine(int optType, int GamedayId, Int32 weekId, bool offline = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 retVal = -40;

            //if (Enum.IsDefined(typeof(LeaderboardType), optType))
            //{
            //    LeaderboardType leaderboardType = (LeaderboardType)optType;
            try
            {
                String data = await _Asset.GET(_Asset.LeaderBoardCombine(optType, GamedayId, weekId));
                httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);
                retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

                //switch (leaderboardType)
                //{
                //    case LeaderboardType.Match:
                //httpResponse = await GetMatchLeaderboard(optType,GamedayId,phaseId,offline);
                //    break;
                //case LeaderboardType.Overall:
                //httpResponse = await GetTourLeaderboard(clientId, sportId, pageNo, isLive, offline);
                //        break;
                //}
            }
            catch (Exception ex)
            {
                retVal = -40;
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Leaderboard.GetLeaderboardCombine", ex.Message);
                _AWS.Log(httpLog);
            }
            //}
            //else
            //    return CatchResponse("Invalid OptType");

            GenericFunctions.AssetMeta(retVal, ref httpMeta, "");
            httpResponse.Meta = httpMeta;

            return httpResponse;
        }

        #endregion

        public async Task<HTTPResponse> GetUserRank(Int32 OptType, Int32 GameDayID, Int32 PhaseId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();

            if (_Cookies._HasGameCookies && _Cookies._HasUserCookies)
            {
                Int32 TeamId = _Cookies._GetUserCookies.TeamId;

                Int32 UserId = _Cookies._GetUserCookies.UserId;

                try
                {
                    ResponseObject responseObject = _DBContext.UserRank(OptType, _TourId, UserId, TeamId, GameDayID, PhaseId, ref httpMeta);
                    httpResponse.Data =  GenericFunctions.Deserialize<ResponseObject>(GenericFunctions.Serialize(responseObject));
                } 
                catch (Exception ex)
                {
                    GenericFunctions.AssetMeta(-40, ref httpMeta, ex.Message);

                    HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Leaderboard.GetUserRank", ex.Message);
                    _AWS.Log(httpLog);
                }
            }
            else
            {
                GenericFunctions.AssetMeta(-40, ref httpMeta, "Not Authorized");
            }


            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> GetUserRankCombine(Int32 OptType, Int32 GameDayID, Int32 weekId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();

            if (_Cookies._HasGameCookies && _Cookies._HasUserCookies)
            {
                String SocialId = _Cookies._GetUserCookies.SocialId;

                //Int32 UserId = _Cookies._GetUserCookies.UserId;

                try
                {
                    ResponseObject responseObject = _DBContext.UserRankCombineLB(OptType, _TourId, SocialId, weekId, GameDayID, ref httpMeta);
                    httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(GenericFunctions.Serialize(responseObject));
                }
                catch (Exception ex)
                {
                    GenericFunctions.AssetMeta(-40, ref httpMeta, ex.Message);

                    HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Leaderboard.GetUserRankCombine", ex.Message);
                    _AWS.Log(httpLog);
                }
            }
            else
            {
                GenericFunctions.AssetMeta(-40, ref httpMeta, "Not Authorized");
            }


            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> PlayedGamedays()
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            ResponseObject responseObject = new ResponseObject();
            Int32 optType = 1;
            String lang = "en";
            List<Fixtures> fixtures = new List<Fixtures>();
            try
            {
                String data = await _Asset.GET(_Asset.Fixtures(lang));
                responseObject = GenericFunctions.Deserialize<ResponseObject>(data);
                fixtures = GenericFunctions.Deserialize<List<Fixtures>>(responseObject.Value.ToString());
                if (fixtures != null && fixtures.Any())
                {
                    fixtures = fixtures.Where(i => i.MatchStatus == 3).OrderByDescending(x => GenericFunctions.ToUSCulture(x.Date)).Select(o => o).ToList();

                    UserPlayedLB mUserPlayedLB = new UserPlayedLB();
                    mUserPlayedLB.PlayedGamedays = (from a in fixtures.AsEnumerable()
                                                    select new PlayedGamedays
                                                    {
                                                        GamedayId = a.TourGamedayId,
                                                        GamedayName = GenericFunctions.ToUSCulture(a.Date).ToString("MMMM d"),
                                                        Matchdate = a.Date

                                                    }).ToList().GroupBy(x => x.GamedayId).Select(x => x.First()).ToList();
                    mUserPlayedLB.PlayedPhase = (from a in fixtures.AsEnumerable()
                                                 select new PlayedPhase
                                                 {
                                                     PhaseId = a.PhaseId,
                                                     PhaseName = "Week " + a.PhaseId,
                                                 }).ToList().GroupBy(x => x.PhaseId).Select(x => x.First()).ToList();


                    //mUserPlayedLB.OverAll = "OverAll";
                    responseObject.Value = mUserPlayedLB;
                    responseObject.FeedTime = GenericFunctions.GetFeedTime();
                    GenericFunctions.AssetMeta(1, ref httpMeta, "Success");
                }


            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Leaderboard.Leaderboard.PlayedGamedays", ex.Message);
                _AWS.Log(httpLog);
            }
            httpResponse.Meta = httpMeta;
            httpResponse.Data = responseObject;
            return httpResponse;
        }

        public List<Fixtures> getFixtures()
        {
            ResponseObject responseObject = new ResponseObject();
            HTTPMeta httpMeta = new HTTPMeta();
            List<Fixtures> mFixtures = new List<Fixtures>();

            try
            {

                responseObject = _DBFeedContext.GetFixtures(1, _TourId, "en", ref httpMeta);
                mFixtures = (List<Fixtures>)responseObject.Value;

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return mFixtures;
        }
        public Int32 AdminLeaderBoard(Int32 TopUsers, Int32 LeaderBoardTypeId, Int32 GamedayId, Int32 PhaseId, out Reports leaderboard)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 PageNo = 1;
            Int32 FromRowNo = 1;
            Int32 ToRowNo = TopUsers; //100;
            Int32 retVal;

            leaderboard = new Reports();

            try
            {
                if (LeaderBoardTypeId == 1)
                {
                    leaderboard = _DBContext.AdminLeaderBoard(LeaderBoardTypeId, PageNo, TopUsers, _TourId, 0, 0, FromRowNo, ToRowNo, ref httpMeta);
                }
                else if (LeaderBoardTypeId == 2)
                {
                    leaderboard = _DBContext.AdminLeaderBoard(LeaderBoardTypeId, PageNo, TopUsers, _TourId, 0, GamedayId, FromRowNo, ToRowNo, ref httpMeta);
                }
                else if (LeaderBoardTypeId == 3)
                {
                    leaderboard = _DBContext.AdminLeaderBoard(LeaderBoardTypeId, PageNo, TopUsers, _TourId, PhaseId, 0, FromRowNo, ToRowNo, ref httpMeta);
                }
            }
            catch (Exception ex)
            {
                httpMeta.Message = ex.Message;
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Leaderboard.AdminLeaderBoard", ex.Message);
                _AWS.Log(httpLog);
            }

            //httpResponse.Meta = httpMeta;
            return Convert.ToInt32(httpMeta.RetVal);
        }

        public async Task<HTTPResponse> GetWeekMappingCombineLB(bool isDbFetch = false)
        {
            int optType = 1;
            HTTPResponse hResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            ResponseObject res = new ResponseObject();
            try
            {
                if (isDbFetch)
                {
                    //Int64 gameDayId = GetGameDayId();
                    List<WeekMapping> mapping = _DBContext.GetWeekMappingCombineLB(optType, _TourId, ref httpMeta);

                    String timeZone = String.Empty;

                    //timeZone = (_IsServer) ? "Asia/Kolkata" : "India Standard Time";
                    timeZone =  "Asia/Kolkata";
                    //timeZone = "India Standard Time";

                    DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZone));

                    mapping.Where(a => a.ToDateIST < today || a.FromDateIST < today).ToList().ForEach(a => a.IsPastDate = 1);


                    res.Value = mapping;
                    res.FeedTime = GenericFunctions.GetFeedTime();
                    bool success = await _Asset.SET(_Asset.WeekMappingMOLPred("en"), res);
                }
                else
                {
                    String data = await _Asset.GET(_Asset.WeekMappingMOLPred("en"));
                    res = GenericFunctions.Deserialize<ResponseObject>(data);
                    List<WeekMapping> mapping = GenericFunctions.Deserialize<List<WeekMapping>>(GenericFunctions.Serialize(res.Value));

                    String timeZone = String.Empty;

                    //timeZone = (_IsServer) ? "Asia/Kolkata" : "India Standard Time";
                    //timeZone = "Asia/Kolkata";
                    timeZone = "India Standard Time";

                    DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZone));

                    mapping.Where(a => a.ToDateIST < today || a.FromDateIST < today).ToList().ForEach(a => a.IsPastDate = 1);

                    res.Value = mapping;

                    Int32 retVal = res != null && res.ToString() != "" ? 1 : -40;
                    GenericFunctions.AssetMeta(retVal, ref httpMeta, "Success");
                }
            }
            catch (Exception ex)
            {
                httpMeta.Message = ex.Message;
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Leaderboard.Leaderboard.GetWeekMapping", ex.Message);
                _AWS.Log(httpLog);
            }
            hResponse.Data = res;
            hResponse.Meta = httpMeta;
            return hResponse;
        }
    }
}
