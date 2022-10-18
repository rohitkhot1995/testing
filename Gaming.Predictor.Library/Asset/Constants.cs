using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace Gaming.Predictor.Library.Asset
{
    public class Constants : Exist, Interfaces.Asset.IAsset
    {
        private readonly String _RedisBaseKey;

        public Constants(IAWS aws, IRedis redis, IOptions<Application> appSettings, ICookies cookies, IHttpContextAccessor httpContextAccessor)
           : base(aws, redis, appSettings, cookies, httpContextAccessor)
        {
            _RedisBaseKey = $"predictor:{_TourId}:";
        }

        public String Languages()
        {
            String key = $"/assets/languages/languages_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-languages";

            return key;
        }

        public String GetLeaderboard(Int32 optType, Int32 gamedayID, Int32 tourId, Int32 PhaseId)
        {
            String key = $"/assets/feeds/leaderboard/{tourId}_{optType}_{PhaseId}_{gamedayID}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-leaderboard_{tourId}_{optType}_{PhaseId}_{gamedayID}";

            return key;
        }

        public String GetTourLeaderboard(Int32 clientId, Int32 sportId, Int32 tourId, Int32 pageNo)
        {
            String key = $"/{clientId}/{sportId}/assets/feeds/leaderboard/{tourId}_{pageNo}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-{clientId}-{sportId}-leaderboard_{tourId}_{pageNo}";

            return key;
        }

        public String Fixtures(String lang)
        {
            String key = $"/assets/fixtures/fixtures_{_TourId}_{lang}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-fixtures-{lang}";

            return key;
        }

        public String Skills(String lang)
        {
            String key = $"/assets/skill/skill_{_TourId}_{lang}.html";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-skill-{lang}";

            return key;
        }

        public String MatchQuestions(Int32? QuestionsMatchID)
        {
            String key = $"/assets/matchquestions/questions_{_TourId}_{QuestionsMatchID}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-questions_-{QuestionsMatchID}";

            return key;
        }

        public String RecentResult()
        {
            String key = $"/assets/recentmatchresults/recentresults_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-recentresults_-{_TourId}";

            return key;
        }

        public String MatchInningStatus(Int32 MatchId)
        {
            String key = $"/assets/matchinningstatus/matchstatus_{_TourId}_{MatchId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-matchinningstatus_-{MatchId}";

            return key;
        }

        public String LeaderBoard(Int32 vOptType, Int32 gamedayId, Int32 phaseId)
        {
            String key = $"/assets/leaderboard/leaderboard_{_TourId}_{vOptType}_{gamedayId}_{phaseId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-leaderboard_-{vOptType}_{gamedayId}_{phaseId}";

            return key;
        }

        public String Debug(String FileName)
        {
            String key = $"/assets/debug/{FileName}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-debug-{FileName}";

            return key;
        }

        public String ShareImage(String FileName)
        {
            String key = $"/assets/debug/{FileName}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-debug-{FileName}";

            return key;
        }

        public String CurrentGamedayMatches()
        {
            String key = $"/assets/currentgameday/currentgamedaymatches_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-currentgamedaymatches-{_TourId}";

            return key;
        }

        public String UserDetailsReport()
        {
            String key = $"/assets/userdetailsreport/userdetailsreport{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-userdetailsreport-{_TourId}";

            return key;
        }

        public String GetMixAPI(Int32 tourId)
        {
            String key = $"/assets/mixapi/{tourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-mixapi_{tourId}";

            return key;
        }

        public String GetConstraints(Int32 tourId)
        {
            String key = $"/assets/constraints/{tourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-constraints_{tourId}";

            return key;
        }
        public String EOTFlag(Int32 tourId)
        {
            String key = $"/assets/constraints/EOTFlag_{tourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-constraints_eotflag_{tourId}";

            return key;
        }

        #region " Template "


        public String PreHeaderTemplate()
        {
            String key = $"/assets/template/preheader_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-template-preheader-{_TourId}";

            return key;
        }

        public String PostFooterTemplate()
        {
            String key = $"/assets/template/postfooter{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-template-postfooter-{_TourId}";

            return key;
        }

        public String PageTemplate(String lang, Int32 IsWebView)
        {
            String key = $"/assets/template/pagetemplate_{_TourId}_{IsWebView}_{lang}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-pagetemplate_-{IsWebView}-{lang}";

            return key;
        }

        public String PageTemplateMobile(String lang, Int32 IsWebView)
        {
            String key = $"/assets/template/pagetemplate_mobile_{_TourId}_{IsWebView}_{lang}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-pagetemplate_mobile_-{IsWebView}-{lang}";

            return key;
        }

        public String PageUnavailableTemplate(String lang)
        {
            String key = $"/assets/template/unavailable-pagetemplate_{_TourId}_{lang}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-pagetemplate_-{lang}";

            return key;
        }

        #endregion

        #region " SEO Templates "

        public String SEOHome()
        {
            String key = $"/assets/template/meta/home_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-template-meta-home-{_TourId}";

            return key;
        }

        public String SEORules()
        {
            String key = $"/assets/template/meta/team_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-template-meta-team-{_TourId}";

            return key;
        }

        public String SEOFAQ()
        {
            String key = $"/assets/template/meta/rules_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-template-meta-rules-{_TourId}";

            return key;
        }

        #endregion

        #region " Notifications "

        public String NotificationTopics()
        {
            String key = $"/assets/notification/topics_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-topics_{_TourId}";

            return key;
        }        

        public String UniqueEvents()
        {
            String key = $"/assets/notification/events_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-events_{_TourId}";

            return key;
        }

        public String NotificationText()
        {
            String key = $"/assets/notification/text_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-text_{_TourId}";

            return key;
        }

        public String NotificationStatus()
        {
            String key = $"/assets/notification/notification_status_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-notification-statuss_{_TourId}";

            return key;
        }

        #endregion

        #region " Comman Leaderboard "
        public string WeekMappingMOLPred(String lang)
        {
            string key = $"/assets/feeds/combineleaderboard/week_mapping_{lang}.json";
            return key;
        }

        public String LeaderBoardCombine(Int32 vOptType, Int32 gamedayId, Int32 weekId)
        {
            String key = $"/assets/feeds/combineleaderboard/leaderboard_{_TourId}_{vOptType}_{gamedayId}_{weekId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-leaderboard_-{vOptType}_{gamedayId}_{weekId}";

            return key;
        }

        #endregion

        public String DaemonServiceCombineLB()
        {
            String key = $"/assets/config/daemon/combine_lb_status_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-config-daemon-combine_lb-status-{_TourId}";

            return key;
        }
        public String DaemonService()
        {
            String key = $"/assets/config/daemon/status_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-config-daemon-status-{_TourId}";

            return key;
        }
        public String DaemonServiceMatchAnswer()
        {
            String key = $"/assets/config/daemon/match_answer_status_{_TourId}.json";

            if (_UseRedis)
                key = $"{_RedisBaseKey}-config-daemon-match-answer-status-{_TourId}";

            return key;
        }
    }
}