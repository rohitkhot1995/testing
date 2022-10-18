using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Feeds;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Gaming.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Admin;
using System.Xml.Linq;
using Gaming.Predictor.DataAccess.Feeds;

namespace Gaming.Predictor.Blanket.Scoring
{
    public class Answers : Common.BaseBlanket
    {
        private readonly Blanket.BackgroundServices.GameLocking _GameLocking;
        private readonly PlayerStatistics _PlayerStatistics;
        private readonly Gameplay _DBContext;
        private readonly Int32 _TourId;
        private readonly Int32 _SlkTeamId;

        public Answers(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
           : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _GameLocking = new Blanket.BackgroundServices.GameLocking(appSettings, null, aws, postgre, redis, cookies, asset);
            _DBContext = new Gameplay(postgre);
            _PlayerStatistics = new PlayerStatistics(appSettings, aws, postgre, redis, cookies, asset);
            _TourId = appSettings.Value.Properties.TourId;
            _SlkTeamId = appSettings.Value.Properties.TeamId;
        }

        public List<Questions> GetQuestionsWithAnswers(String vMatchFile, Int32 vMatchId)
        {
            MatchFeed mMatchFeed = new MatchFeed();
            List<Lineups> mMatchLineups = new List<Lineups>();
            //XDocument mMatchAnalyticsDoc = new XDocument();
            MatchPlayerStats mMatchPlayerStats = new MatchPlayerStats();
            List<Questions> mQuestionsList = new List<Questions>();
            List<Match> mMatches = new List<Match>();
            Match mMatch = new Match();
            ResponseObject responseObject = new ResponseObject();
            HTTPMeta hTTPMeta = new HTTPMeta();
            Int32 OptType = 1;



            mMatchFeed = _GameLocking.GetMatchScoresFeed(vMatchFile);
            mMatchLineups = _GameLocking.GetLineupsFromMatchFeed(mMatchFeed);

            //mMatchAnalyticsDoc = _GameLocking.GetMatchAnalyticsFeed(vMatchFile);
            mMatchPlayerStats = _PlayerStatistics.GetPlayerStats(mMatchFeed, mMatchLineups);


            responseObject = _DBContext.GetQuestions(OptType, _TourId, vMatchId, ref hTTPMeta);
            mQuestionsList = GenericFunctions.Deserialize<List<Questions>>(GenericFunctions.Serialize(responseObject.Value))
                .Where(x => x.QuestionOccurrence.ToLower() == "prm"|| x.QuestionOccurrence.ToLower() == "prm1" ||
                            x.QuestionOccurrence.ToLower() == "prm2"|| x.QuestionOccurrence.ToLower() == "ing1").ToList();

            mMatches = _GameLocking.GetFixturesFeed();
            mMatch = mMatches.Where(c => c.match_Id == vMatchId.ToString()).FirstOrDefault();

            PreMatchQuestions mPreMatchQuestions = new PreMatchQuestions(mMatchFeed, mMatchPlayerStats, mMatch);
            InningQuestions mFirstInnningQuestions = new InningQuestions(mMatchFeed, mMatchPlayerStats, "First");
            InningQuestions mSecondInnningQuestions = new InningQuestions(mMatchFeed, mMatchPlayerStats, "Second");
            PostMatchQuestions mPostMatchQuestions = new PostMatchQuestions(mMatchFeed, mMatchPlayerStats);

            foreach (Questions mQuestions in mQuestionsList)
            {
                //if (mQuestions.QuestionOccurrence == "PRM")
                //{
                //    GetCorrectOption(mQuestions, mPreMatchQuestions);
                //}
                //else
                //{
                //if (mQuestions.InningNo == 1)
                //{
                //    GetCorrectOption(mQuestions, mFirstInnningQuestions);
                //}
                //else if (mQuestions.InningNo == 2)
                //{
                //    GetCorrectOption(mQuestions, mSecondInnningQuestions);
                //}

                if(mQuestions.QuestionCode.ToLower() == "win_team")
                {
                    GetCorrectOption(mQuestions, mPreMatchQuestions);
                }
                else if(mQuestions.QuestionCode.ToLower() == "four_team")
                    {
                        if (mMatchFeed != null && mMatchFeed.Innings.Count > 0)
                        {

                            //String mSlkInning = mMatchFeed.Innings.Where(y => y.Battingteam == _SlkTeamId.ToString()).Select(x => x.Number).FirstOrDefault();

                            //if (mSlkInning.ToLower() == "first")
                                GetCorrectOption(mQuestions, mPreMatchQuestions);
                            //else
                            //    GetCorrectOption(mQuestions, mSecondInnningQuestions, _SlkTeamId);
                        }
                    }
                else if(mQuestions.QuestionCode.ToLower() == "six_team")
                    {
                        if (mMatchFeed != null && mMatchFeed.Innings.Count > 0)
                        {

                            //String mSlkInning = mMatchFeed.Innings.Where(y => y.Battingteam == _SlkTeamId.ToString()).Select(x => x.Number).FirstOrDefault();

                            //if (mSlkInning.ToLower() == "first")
                                GetCorrectOption(mQuestions, mPreMatchQuestions);
                            //else
                            //    GetCorrectOption(mQuestions, mSecondInnningQuestions, _SlkTeamId);
                        }
                    }
                else if (mQuestions.QuestionCode.ToLower() == "wckt_tkn_plyr")
                    {
                        if (mMatchFeed != null && mMatchFeed.Innings.Count > 0)
                        {

                            //String mSlkInning = mMatchFeed.Innings.Where(y => y.Bowlingteam == _SlkTeamId.ToString()).Select(x => x.Number).FirstOrDefault();

                            //if (mSlkInning.ToLower() == "first")
                                GetCorrectOption(mQuestions, mPostMatchQuestions);
                            //else
                            //    GetCorrectOption(mQuestions, mSecondInnningQuestions, _SlkTeamId);
                        }
                    }
                else if (mQuestions.QuestionCode.ToLower() == "hig_scor_plyr")
                   {
                        if (mMatchFeed != null && mMatchFeed.Innings.Count > 0)
                        {

                            //String mSlkInning = mMatchFeed.Innings.Where(y => y.Battingteam == _SlkTeamId.ToString()).Select(x => x.Number).FirstOrDefault();

                            //if (mSlkInning.ToLower() == "first")
                                GetCorrectOption(mQuestions, mPostMatchQuestions);
                            //else
                            //    GetCorrectOption(mQuestions, mSecondInnningQuestions, _SlkTeamId);

                        }
                   }
                //}
            }

            return mQuestionsList;
        }


        public List<Questions> GetFirstInningQuestionsWithAnswers(String vMatchFile, Int32 vMatchId)
        {
            MatchFeed mMatchFeed = new MatchFeed();
            List<Lineups> mMatchLineups = new List<Lineups>();
            //XDocument mMatchAnalyticsDoc = new XDocument();
            MatchPlayerStats mMatchPlayerStats = new MatchPlayerStats();
            List<Questions> mQuestionsList = new List<Questions>();
            List<Match> mMatches = new List<Match>();
            Match mMatch = new Match();
            ResponseObject responseObject = new ResponseObject();
            HTTPMeta hTTPMeta = new HTTPMeta();
            Int32 OptType = 1;



            mMatchFeed = _GameLocking.GetMatchScoresFeed(vMatchFile);
            mMatchLineups = _GameLocking.GetLineupsFromMatchFeed(mMatchFeed);

            //mMatchAnalyticsDoc = _GameLocking.GetMatchAnalyticsFeed(vMatchFile);
            mMatchPlayerStats = _PlayerStatistics.GetPlayerStats(mMatchFeed, mMatchLineups);


            responseObject = _DBContext.GetQuestions(OptType, _TourId, vMatchId, ref hTTPMeta);
            mQuestionsList = GenericFunctions.Deserialize<List<Questions>>(GenericFunctions.Serialize(responseObject.Value)).Where(x => x.QuestionOccurrence.ToLower() == "ing1").Select(y => y).ToList();

            mMatches = _GameLocking.GetFixturesFeed();
            mMatch = mMatches.Where(c => c.match_Id == vMatchId.ToString()).FirstOrDefault();

            //PreMatchQuestions mPreMatchQuestions = new PreMatchQuestions(mMatchFeed, mMatchPlayerStats, mMatchAnalyticsDoc, mMatch);
            InningQuestions mFirstInnningQuestions = new InningQuestions(mMatchFeed, mMatchPlayerStats, "First");
            //InningQuestions mSecondInnningQuestions = new InningQuestions(mMatchFeed, mMatchPlayerStats, mMatchAnalyticsDoc, "Second");

            foreach (Questions mQuestions in mQuestionsList)
            {
                //if (mQuestions.QuestionOccurrence == "PRM")
                //{
                //    GetCorrectOption(mQuestions, mPreMatchQuestions);
                //}
                //else
                //{
                if (mQuestions.InningNo == 1)
                {
                    GetCorrectOption(mQuestions, mFirstInnningQuestions);
                }
                //    else if (mQuestions.InningNo == 2)
                //    {
                //        GetCorrectOption(mQuestions, mSecondInnningQuestions);
                //    }
                //}

            }

            return mQuestionsList;
        }
        public List<MatchQuestions> GetAdminQUestions(Int32 vMatchId)
        {
            //List<Questions> mQuestionsList = new List<Questions>();
            List<MatchQuestions> mQuestions = new List<MatchQuestions>();
            ResponseObject responseObject = new ResponseObject();
            HTTPMeta hTTPMeta = new HTTPMeta();
            Int32 OptType = 1;

            responseObject = _DBContext.GetQuestionsV1(OptType, _TourId, vMatchId, ref hTTPMeta);
            mQuestions = GenericFunctions.Deserialize<List<MatchQuestions>>(GenericFunctions.Serialize(responseObject.Value));

            return mQuestions;
        }


        private static void GetCorrectOption(Questions vQuestion, PreMatchQuestions vPreMatchQuestions)
        {
            switch (vQuestion.QuestionCode)
            {
                case "SIX_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToHitMostSixes);
                    break;
                case "FOUR_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToHitMostFours);
                    break;
                case "WCKT_GVN_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToConcedeMostWickets);
                    break;
                case "WCKT_TKN_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToTakeMostWickets);
                    break;
                case "EXTRA_GVN_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToConcedeMostExtras);
                    break;
                //case "MAX_PP_TEAM":
                //    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToScoreMaxRunsInPP);
                //    break;

                case "WIN_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToWinMatch);
                    break;
                case "SIX_MATCH":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.NoOfSixesInMatch);
                    break;
                case "FOUR_MATCH":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.NoOfFoursInMatch);
                    break;
                case "WCKT_MATCH":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.NoOfWicketsInMatch);
                    break;
                case "RUN_MATCH":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TotalRunsInMatch);
                    break;
                case "HIG_SCOR_MATCH":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.HighestScoreInMatch);
                    break;
            }
        }

        private static void GetCorrectOption(Questions vQuestion, InningQuestions vInningQuestions)
        {

            switch (vQuestion.QuestionCode)
            {
                case "SIX_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToHitMostSixes);
                    break;
                case "FOUR_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToHitMostFours);
                    break;
                case "WCKT_TKN_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToTakeMostWickets);
                    break;
                case "CATCH_TKN_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToTakeMostCatchs);
                    break;
                case "EXTRA_GVN_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToConcedeMostExtras);
                    break;
                case "RUN_GVN_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToConcedeMostRuns);
                    break;
                case "HIG_SCOR_MATCH":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToHitMostRuns);
                    break;

                //case "WCKT_TKN_ING":
                case "WCKT_MATCH":
                    ResolveQuestionOption(vQuestion, vInningQuestions.WicketsInInning);
                    break;
                case "WCKT_TKN_PP":
                    ResolveQuestionOption(vQuestion, vInningQuestions.WicketsInPP);
                    break;
                case "EXTRA_TKN_ING":
                    ResolveQuestionOption(vQuestion, vInningQuestions.ExtrasInInning);
                    break;
                case "RUN_PP":
                    ResolveQuestionOption(vQuestion, vInningQuestions.RunsInPP);
                    break;
                case "RUN_ING":
                    ResolveQuestionOption(vQuestion, vInningQuestions.RunsInInning);
                    break;
                case "RUN_L5":
                    ResolveQuestionOption(vQuestion, vInningQuestions.RunsInLst5Overs);
                    break;
                case "SIX_MATCH":
                    ResolveQuestionOption(vQuestion, vInningQuestions.NoOfSixesInInning);
                    break;
            }
        }

        private static void GetCorrectOption(Questions vQuestion, PostMatchQuestions vPostMatchQuestions)
        {

            switch (vQuestion.QuestionCode)
            {
                case "SIX_PLYR":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.PlayerToHitMostSixes);
                    break;
                case "FOUR_PLYR":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.PlayerToHitMostFours);
                    break;
                case "WCKT_TKN_PLYR":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.PlayerToTakeMostWickets);
                    break;
                case "CATCH_TKN_PLYR":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.PlayerToTakeMostCatchs);
                    break;
                case "EXTRA_GVN_PLYR":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.PlayerToConcedeMostExtras);
                    break;
                case "RUN_GVN_PLYR":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.PlayerToConcedeMostRuns);
                    break;
                case "HIG_SCOR_PLYR":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.PlayerToHitMostRuns);
                    break;

                //case "WCKT_TKN_ING":
                case "WCKT_MATCH":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.WicketsInMatch);
                    break;
                case "WCKT_TKN_PP":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.WicketsInMatch);
                    break;
                case "EXTRA_TKN_ING":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.ExtrasInMatch);
                    break;
                //case "RUN_PP":
                //    ResolveQuestionOption(vQuestion, vPostMatchQuestions.RunsInPP);
                //    break;
                case "RUN_ING":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.RunsInInning);
                    break;
                case "RUN_L5":
                    ResolveQuestionOption(vQuestion, vPostMatchQuestions.RunsInLst5Overs);
                    break;
                //case "SIX_MATCH":
                //    ResolveQuestionOption(vQuestion, vPostMatchQuestions.NoOfSixesInInning);
                //    break;
            }
        }
        private static void ResolveQuestionOption(Questions vQuestions, List<String> vAnswers)
        {
            foreach (Option mOption in vQuestions.Options)
            {
                if (vAnswers.Count == 0 || vAnswers == null || vAnswers.Where(x => vQuestions.Options.Any(c => c.AssetId.ToString() == x)).Count() <= 0)
                {
                    if (mOption.AssetType.ToLower() == "none")
                    {
                        mOption.IsCorrect = 1;
                    }
                }
                else if (vAnswers.IndexOf(mOption.AssetId.ToString()) > -1)
                {
                    mOption.IsCorrect = 1;
                }
            }
        }

        private static void ResolveQuestionOption(Questions vQuestions, Int32 vAnswer)
        {
            foreach (Option mOption in vQuestions.Options)
            {
                if (vQuestions.QuestionType.ToLower() == "tem")
                {
                    if (vQuestions.QuestionCode.ToLower() == "win_team")
                    {

                        //todo : need to confirm if win teamId = 0 then will both teams will get points
                        if (vAnswer == 0 && (mOption.AssetType.ToLower() == "draw" || mOption.AssetType.ToLower() == "none"))
                        {
                            mOption.IsCorrect = 1;
                        }
                        else if (mOption.AssetId == vAnswer)
                        {
                            mOption.IsCorrect = 1;
                        }

                        //if(vAnswer == mOption.AssetId && mOption.OptionId == 1)
                        //{
                        //    mOption.IsCorrect = 1;
                        //}
                        //else if(vAnswer != SLKTeamId && mOption.OptionId == 2)
                        //{
                        //    mOption.IsCorrect = 1;
                        //}
                    }
                    else
                    {
                        if (vAnswer == 0 && mOption.AssetType.ToLower() == "draw" || mOption.AssetType.ToLower() == "none")
                        {
                            mOption.IsCorrect = 1;
                        }
                        else if (mOption.AssetId == vAnswer)
                        {
                            mOption.IsCorrect = 1;
                        }
                    }
                }
                else if (mOption.MinVal <= vAnswer && (mOption.MaxVal >= vAnswer || mOption.MaxVal == null))
                {
                    mOption.IsCorrect = 1;
                }


                // if (vAnswer == 0 && (mOption.AssetType.ToLower() == "draw" || mOption.AssetType.ToLower() == "none"))
                //    {
                //        mOption.IsCorrect = 1;
                //        break;
                //    }
                //    else if (mOption.AssetId == vAnswer)
                //    {
                //        mOption.IsCorrect = 1;
                //    }           
                //else if (vAnswer != 0 && mOption.MinVal <= vAnswer && (mOption.MaxVal >= vAnswer || mOption.MaxVal == null))
                //{
                //    mOption.IsCorrect = 1;
                //}

            }
        }

    }
}
