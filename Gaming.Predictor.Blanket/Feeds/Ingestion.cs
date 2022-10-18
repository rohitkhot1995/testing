using Gaming.Predictor.Blanket.Leaderboard;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Enums;
using Gaming.Predictor.Contracts.Feeds;
using Gaming.Predictor.Contracts.Notification;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Gaming.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gaming.Predictor.Blanket.Feeds
{
    public class Ingestion : Common.BaseBlanket
    {
        private readonly Gameplay _FeedContext;
        private readonly Leaderbaord _LeaderbaordContext;
        private readonly DataAccess.Leaderboard.Leaderbaord _DBContext;
        private readonly DataAccess.Notification.Subscription _NotificationContext;

        private readonly Int32 _TourId;
        private readonly List<String> _Lang;

        public Ingestion(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _FeedContext = new Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _LeaderbaordContext = new Leaderbaord(appSettings, aws, postgre, redis, cookies, asset);
            _DBContext = new DataAccess.Leaderboard.Leaderbaord(postgre);
            _NotificationContext = new DataAccess.Notification.Subscription(postgre);
            _TourId = appSettings.Value.Properties.TourId;
            _Lang = appSettings.Value.Properties.Languages;
        }

        public async Task<Int32> Languages()
        {
            Int32 retVal = -50;

            try
            {
                ResponseObject response = new ResponseObject();
                response.Value = _Lang;
                response.FeedTime = GenericFunctions.GetFeedTime();

                bool success = await _Asset.SET(_Asset.Languages(), response);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public async Task<Int32> Fixtures()
        {
            Int32 retVal = -50;
            bool success = false;

            try
            {
                foreach (String lang in await GetLanguages())
                {
                    HTTPResponse response = await _FeedContext.GetFixtures(lang, offloadDb: false);

                    if (response.Meta.RetVal == 1)
                        success = await _Asset.SET(_Asset.Fixtures(lang), response.Data);
                }

                retVal = Convert.ToInt32(success);

                if (success)
                {
                    success = await UpdateMixApi(_TourId, Apis.afix.ToString());
                }

            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public async Task<Int32> Skills()
        {
            Int32 retVal = -50;
            bool success = false;

            try
            {
                foreach (String lang in await GetLanguages())
                {
                    HTTPResponse response = await _FeedContext.GetSkills(lang, offloadDb: false);

                    if (response.Meta.RetVal == 1)
                        success = await _Asset.SET(_Asset.Skills(lang), response.Data);
                }

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public async Task<Int32> Questions(Int32 QuestionsMatchID)
        {
            Int32 retVal = -50;
            bool success = false;

            try
            {
                HTTPResponse response = await _FeedContext.GetQuestions(QuestionsMatchID, offloadDb: false);

                if (response.Meta.RetVal == 1)
                    success = await _Asset.SET(_Asset.MatchQuestions(QuestionsMatchID), GenericFunctions.Serialize(response.Data, true), false);

                retVal = Convert.ToInt32(success);

                if (success)
                {
                    success = await UpdateMixApi(_TourId, Apis.aqus.ToString());
                }
            }
            catch (Exception ex)
            {

            }
            return retVal;

        }

        public async Task<Int32> AllMatchQuestions()
        {
            Int32 retVal = -50;
            bool success = false;
            String lang = "en";
            List<Fixtures> fixtures = new List<Fixtures>();
            HTTPResponse hTTPResponse = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();
            try
            {
                hTTPResponse = await _FeedContext.GetFixtures(lang);
                responseObject = GenericFunctions.Deserialize<ResponseObject>(GenericFunctions.Serialize(hTTPResponse.Data));
                fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(responseObject.Value));

                foreach (Fixtures mFixture in fixtures) { 
                HTTPResponse response = await _FeedContext.GetQuestions(mFixture.MatchId, offloadDb: false);

                if (response.Meta.RetVal == 1)
                    success = await _Asset.SET(_Asset.MatchQuestions(mFixture.MatchId), GenericFunctions.Serialize(response.Data, true), false);

                    retVal = Convert.ToInt32(success);
                }
            }
            catch (Exception ex)
            {

            }
            return retVal;

        }

        public async Task<Int32> GetRecentResults()
        {
            Int32 retVal = -50;
            bool success = false;

            try
            {
                HTTPResponse response = await _FeedContext.GetRecentResults(offloadDb: false);

                if (response.Meta.RetVal == 1)
                    success = await _Asset.SET(_Asset.RecentResult(), response.Data);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {

            }
            return retVal;

        }

        public async Task<Int32> MatchInningStatus(Int32 MatchID)
        {
            Int32 retVal = -50;
            bool success = false;

            try
            {
                HTTPResponse response = await _FeedContext.MatchInningStatus(MatchID, offloadDb: false);

                if (response.Meta.RetVal == 1)
                    success = await _Asset.SET(_Asset.MatchInningStatus(MatchID), response.Data);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {

            }
            return retVal;

        }

        public async Task<Int32> LeaderBoard(Int32 GamedayId, Int32 PhaseId)
        {
            Int32 retVal = -50;
            bool success = false;
            HTTPMeta hTTPMeta = new HTTPMeta();
            string error = string.Empty;

            try
            {
                Int32[] mOptTypes = new Int32[3] { 1, 2, 3 };//1: Overall; 2: Gameday; 3: Weekly
                Int32 pageNo = 1, top = 2000, fromRowNo = 1, toRowNo = 2000;


                foreach(Int32 mOptType in mOptTypes)
                {
                    ResponseObject response = _DBContext.Top(mOptType, PhaseId, GamedayId, pageNo, top, _TourId, fromRowNo, toRowNo, ref hTTPMeta);

                    if (hTTPMeta.RetVal == 1)
                    {
                        if(mOptType == 1)
                        success = await _Asset.SET(_Asset.LeaderBoard(mOptType, 0, 0), response);
                        else if (mOptType == 2)
                        success = await _Asset.SET(_Asset.LeaderBoard(mOptType, GamedayId, PhaseId), response);
                        else if (mOptType == 3)
                        success = await _Asset.SET(_Asset.LeaderBoard(mOptType, GamedayId, PhaseId), response);
                    }
                }

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                //await _Asset.SET(_Asset.Debug("LeaderBoard"), ex.Message);
            }
            return retVal /*new Tuple<int, string>(retVal,error)*/;

        }

        public async Task<Int32> LeaderBoardCombine(Int32 GamedayId, Int32 WeekId)
        {
            Int32 retVal = -50;
            bool success = false;
            HTTPMeta hTTPMeta = new HTTPMeta();
            string error = string.Empty;

            try
            {
                Int32[] mOptTypes = new Int32[2] { 1, 2 };//1: Overall; 2: Week;
                Int32 pageNo = 1, top = 2000, fromRowNo = 1, toRowNo = 2000;


                foreach (Int32 mOptType in mOptTypes)
                {
                    ResponseObject response = _DBContext.TopCombineLB(mOptType, WeekId, GamedayId, _TourId, ref hTTPMeta);

                    if (hTTPMeta.RetVal == 1)
                    {
                        if (mOptType == 1)
                            success = await _Asset.SET(_Asset.LeaderBoardCombine(mOptType, 0, 0), response);
                        else if (mOptType == 2)
                            success = await _Asset.SET(_Asset.LeaderBoardCombine(mOptType, 0, WeekId), response);
                        
                    }
                }

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                //await _Asset.SET(_Asset.Debug("LeaderBoard"), ex.Message);
            }
            return retVal /*new Tuple<int, string>(retVal,error)*/;

        }

        public async Task<Int32> LeaderBoardCombineWeek(Int32 WeekId)
        {
            Int32 retVal = -50;
            bool success = false;
            HTTPMeta hTTPMeta = new HTTPMeta();
            string error = string.Empty;

            try
            {
                Int32 mOptType = 2;

                ResponseObject response = _DBContext.TopCombineLB(mOptType, WeekId, 0, _TourId, ref hTTPMeta);

                success = await _Asset.SET(_Asset.LeaderBoardCombine(mOptType, 0, WeekId), response);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                //await _Asset.SET(_Asset.Debug("LeaderBoard"), ex.Message);
            }
            return retVal /*new Tuple<int, string>(retVal,error)*/;

        }
        public async Task<Int32> LeaderBoardCombineOverall()
        {
            Int32 retVal = -50;
            bool success = false;
            HTTPMeta hTTPMeta = new HTTPMeta();
            string error = string.Empty;

            try
            {
                Int32 mOptType = 1;
                ResponseObject response = _DBContext.TopCombineLB(mOptType, 0, 0, _TourId, ref hTTPMeta);

                success = await _Asset.SET(_Asset.LeaderBoardCombine(mOptType, 0, 0), response);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                //await _Asset.SET(_Asset.Debug("LeaderBoard"), ex.Message);
            }
            return retVal /*new Tuple<int, string>(retVal,error)*/;

        }

        public async Task<Int32> WeekMappingCombine()
        {
            Int32 retVal = -50;
            bool success = false;
            HTTPResponse httpResponse = new HTTPResponse();
            string error = string.Empty;

            try
            {
                httpResponse = await _LeaderbaordContext.GetWeekMappingCombineLB(isDbFetch: true);

                success = await _Asset.SET(_Asset.WeekMappingMOLPred("en"), httpResponse.Data);

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                //await _Asset.SET(_Asset.Debug("LeaderBoard"), ex.Message);
            }
            return retVal /*new Tuple<int, string>(retVal,error)*/;

        }

        public async Task<Int32> CurrentGamedayMatches()
        {
            Int32 retVal = -50;
            bool success = false;

            ResponseObject responseObject = new ResponseObject();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 optType = 1;
            String lang = "en";
            List<Fixtures> mFixtures = new List<Fixtures>();
            List<CurrentGamedayMatches> mCurrentGamedayMatches = new List<CurrentGamedayMatches>();
            try
            {

                String data = await _Asset.GET(_Asset.Fixtures(lang));

                mFixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(GenericFunctions.Deserialize<ResponseObject>(data).Value));

                mFixtures = mFixtures.Where(i => i.MatchStatus != 3).OrderBy(x => GenericFunctions.ToUSCulture(x.Deadlinedate)).Select(o => o).ToList();
                if (mFixtures.Any())
                {
                    Int32 CurrentGamedayId = mFixtures[0].TourGamedayId;
                    mFixtures = mFixtures.Where(c => c.TourGamedayId == CurrentGamedayId).ToList();
                    foreach (Fixtures fixture in mFixtures)
                    {
                        mCurrentGamedayMatches.Add(new CurrentGamedayMatches
                        {
                            MatchId = fixture.MatchId,
                            TeamGamedayId = fixture.TeamGamedayId,
                            TourGamedayId = fixture.TourGamedayId,
                            Date = fixture.Date,
                            MatchStatus = fixture.MatchStatus,
                            Live = (fixture.MatchStatus == 2 && fixture.Match_Inning_Status != 6) ? 1 : 0,
                            Match_Inning_Status = fixture.Match_Inning_Status
                        });
                    }
                }

                responseObject.Value = mCurrentGamedayMatches;
                responseObject.FeedTime = GenericFunctions.GetFeedTime();

                success = await _Asset.SET(_Asset.CurrentGamedayMatches(), responseObject);
                retVal = Convert.ToInt32(success);

               
                
            }
            catch (Exception ex)
            { }

            return retVal;
        }

        public async Task<bool> UpdateMixApi(int tourId, params string[] apiName)
        {
            bool success = false;
            HTTPResponse response = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();
            Mixapi mixapis = new Mixapi();
            String data = "";

            try
            {
                data = await _Asset.GET(_Asset.GetMixAPI(tourId));
                if (data != "")
                {
                    mixapis = GenericFunctions.Deserialize<Mixapi>(GenericFunctions.Serialize(GenericFunctions.Deserialize<ResponseObject>(data).Value));
                }
                var version = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                foreach (string _apiName in apiName)
                {

                    if (mixapis.api.Any(o => o.nm.ToLower().Equals(_apiName.ToLower())))
                    {
                        mixapis.ov = version;
                        mixapis.api.Where(o => o.nm.ToLower().Equals(_apiName.ToLower())).Select(o => { o.vn = version; return o; }).ToList();
                    }
                    else
                    {
                        mixapis.ov = version;
                        mixapis.api.Add(new GameAPI
                        {
                            nm = _apiName,
                            vn = version
                        });
                    }
                }

                responseObject.Value = mixapis;
                responseObject.FeedTime = GenericFunctions.GetFeedTime();

                success = await _Asset.SET(_Asset.GetMixAPI(tourId), responseObject);
            }
            catch (Exception ex)
            {
            }
            return success;
        }

        public async Task<Int32> UpdateMixApiAdmin(int tourId, Int32 isPoint, Int32 isAbandon, Int32 isMaintenance)
        {
            bool success = false;
            HTTPResponse response = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();
            Mixapi mixapis = new Mixapi();
            String data = "";

            try
            {
                data = await _Asset.GET(_Asset.GetMixAPI(tourId));
                if (data != "")
                {
                    mixapis = GenericFunctions.Deserialize<Mixapi>(GenericFunctions.Serialize(GenericFunctions.Deserialize<ResponseObject>(data).Value));
                }

                mixapis.isPointProcess = isPoint;
                mixapis.isGDAbandoned = isAbandon;
                mixapis.isMaintenance = isMaintenance;

                responseObject.Value = mixapis;
                responseObject.FeedTime = GenericFunctions.GetFeedTime();

                success = await _Asset.SET(_Asset.GetMixAPI(tourId), responseObject);
            }
            catch (Exception ex)
            {
            }
            return Convert.ToInt32(success);
        }


        public async Task<bool> UpdateMixApiAbandon(Int32 tourId)
        {
            bool success = false;
            HTTPResponse response = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();
            Mixapi mixapis = new Mixapi();
            String data = "";

            try
            {
                data = await _Asset.GET(_Asset.GetMixAPI(tourId));
                if (data != "")
                {
                    mixapis = GenericFunctions.Deserialize<Mixapi>(GenericFunctions.Serialize(GenericFunctions.Deserialize<ResponseObject>(data).Value));
                }

                mixapis.isGDAbandoned = 1;

                responseObject.Value = mixapis;
                responseObject.FeedTime = GenericFunctions.GetFeedTime();

                success = await _Asset.SET(_Asset.GetMixAPI(tourId), responseObject);
            }
            catch (Exception ex)
            {
            }
            return success;
        }

        public async Task<bool> UpdateMixApiPointCalOn(int tourId)
        {
            bool success = false;
            HTTPResponse response = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();
            Mixapi mixapis = new Mixapi();
            String data = "";

            try
            {
                data = await _Asset.GET(_Asset.GetMixAPI(tourId));
                if (data != "")
                {
                    mixapis = GenericFunctions.Deserialize<Mixapi>(GenericFunctions.Serialize(GenericFunctions.Deserialize<ResponseObject>(data).Value));
                }

                mixapis.isPointProcess = 1;

                responseObject.Value = mixapis;
                responseObject.FeedTime = GenericFunctions.GetFeedTime();

                success = await _Asset.SET(_Asset.GetMixAPI(tourId), responseObject);
            }
            catch (Exception ex)
            {
            }
            return success;
        }


        public async Task<bool> UpdateMixApiLeaderboard(int optType, Int32 matchId, List<int> abandonedMatches = null)
        {
            bool success = false;
            HTTPResponse response = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();
            Mixapi mixapis = new Mixapi();
            String data = "";

            try
            {
                data = await _Asset.GET(_Asset.GetMixAPI(_TourId));
                if (data != "")
                {
                    mixapis = GenericFunctions.Deserialize<Mixapi>(GenericFunctions.Serialize(GenericFunctions.Deserialize<ResponseObject>(data).Value));
                }
                var version = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                if (optType == 1)
                {
                    if (abandonedMatches != null && abandonedMatches.Count > 0)
                    {
                        try
                        {
                            //List<Int32> _matchIds = abandonedMatches.Select(x => x.IntValue()).ToList();
                            if (mixapis.lb.Any(x => abandonedMatches.Contains(x.matchId)))
                            {
                                mixapis.lb = mixapis.lb.Where(x => !abandonedMatches.Contains(x.matchId)).ToList();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (mixapis.lb != null && mixapis.lb.Count > 0)
                    {
                        if (mixapis.lb.Any(x => x.matchId == matchId))
                        {
                            mixapis.lb = mixapis.lb.Where(x => x.matchId != matchId).ToList();
                        }
                        mixapis.lb.Add(new LeaderboardVersion { matchId = matchId, vn = version });
                    }
                    else
                    {
                        mixapis.lb = new List<LeaderboardVersion>();
                        mixapis.lb.Add(new LeaderboardVersion { matchId = matchId, vn = version });
                    }

                    mixapis.api = mixapis.api.Where(o => o.nm.ToLower() != Apis.lbov.ToString()).ToList();
                    mixapis.api.Add(new GameAPI { nm = Apis.lbov.ToString(), vn = version });

                }
                else if (optType == 2)
                {
                    if (mixapis.lb != null && mixapis.lb.Count > 0 && mixapis.lb.Any(x => x.matchId == matchId))
                    {
                        mixapis.lb = mixapis.lb.Where(x => x.matchId != matchId).ToList();
                    }

                    if (mixapis.api.Any(o => o.nm.ToLower().Equals(Apis.lbtour.ToString())))
                    {
                        mixapis.api.Where(o => o.nm.ToLower().Equals(Apis.lbtour.ToString())).Select(o => { o.vn = version; return o; }).ToList();
                    }
                    else
                    {
                        mixapis.ov = version;
                        mixapis.api.Add(new GameAPI
                        {
                            nm = Apis.lbtour.ToString(),
                            vn = version
                        });
                    }

                    //if(liveMatchIds != null && liveMatchIds.Count > 0)
                    // mixapis.lb = mixapis.lb.Where(x => liveMatchIds.Contains(x.matchId)).ToList();
                    //else
                    // mixapis.lb = new List<LeaderboardVersion>();
                }

                mixapis.isPointProcess = 0;
                mixapis.isGDAbandoned = 0;

                responseObject.Value = mixapis;
                responseObject.FeedTime = GenericFunctions.GetFeedTime();

                success = await _Asset.SET(_Asset.GetMixAPI(_TourId), responseObject);
            }
            catch (Exception ex)
            {
            }
            return success;
        }

        public async Task<Int32> UpdateEOTFlag(Int32 EOTStatus)
        {
            bool result = false;

            result = await _Asset.SET(_Asset.EOTFlag(_TourId), EOTStatus, serialize: false);

            return result ? 1 : -40;
        }

        #region " Notification "
        public async Task<Int32> InsertTopics()
        {
            Int32 retVal = -50;
            Int32 optType = 1;
            HTTPMeta hTTPMeta = new HTTPMeta();
            bool success = false;

            try
            {
               
                    ResponseObject response = _NotificationContext.TopicsGet(optType, _TourId, ref hTTPMeta);

                    if (hTTPMeta.RetVal == 1)
                        success = await _Asset.SET(_Asset.NotificationTopics(), response);
              

                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }

            return retVal;
        }

        public async Task<Int32> IngestNotificationMessages(String notificationMessages)
        {
            Int32 retVal = -50;
            bool success = false;
            try
            {
                NotificationText mNotificationText = new NotificationText();
                mNotificationText = GenericFunctions.Deserialize<NotificationText>(notificationMessages);
                success = await _Asset.SET(_Asset.NotificationText(), mNotificationText);
                retVal = Convert.ToInt32(success);
            }
            catch (Exception ex)
            {
            }
            return retVal;
        }

        public async Task<Int32> IngestNotificationStatus()
        {
            Int32 retVal = -50;
            bool success = false;
            String lang = "en";
            List<Fixtures> fixtures = new List<Fixtures>();
            HTTPResponse hTTPResponse = new HTTPResponse();
            List<NotificationStatus> notificationStatuses = new List<NotificationStatus>();
            ResponseObject responseObject = new ResponseObject();
            try
            {
                hTTPResponse = await _FeedContext.GetFixtures(lang);
                responseObject = GenericFunctions.Deserialize<ResponseObject>(GenericFunctions.Serialize(hTTPResponse.Data));
                fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(responseObject.Value));

                foreach (Fixtures mFixture in fixtures)
                {
                    notificationStatuses.Add(new NotificationStatus
                    {
                        MatchId = mFixture.MatchId,
                        PreMatchNotification = false
                    });
                    success = await _Asset.SET(_Asset.NotificationStatus(), notificationStatuses);

                    retVal = Convert.ToInt32(success);
                }
            }
            catch (Exception ex)
            {
               await _Asset.SET(_Asset.Debug("IngestNotificationStatus"), ex.Message);
            }
            return retVal;
        }
        #endregion

    }
}