using System;
using System.Threading.Tasks;

namespace Gaming.Predictor.Interfaces.Asset
{
    public interface IAsset
    {
        Task<String> GET(String key);

        Task<bool> SET(String key, Object content, bool serialize = true);
        

        String Languages();
        
        String Fixtures(String lang);

        String Skills(String lang);

        String MatchQuestions(Int32? MatchQuestions);

        String RecentResult();

        String MatchInningStatus(Int32 MatchId);

        String LeaderBoard(Int32 vOptType, Int32 gamedayId, Int32 phaseId);
        String LeaderBoardCombine(Int32 vOptType, Int32 gamedayId, Int32 weekId);

        String Debug(String FileName);

        String ShareImage(String FileName);

        String CurrentGamedayMatches();

        String UniqueEvents();

        String NotificationTopics(); 

        String NotificationStatus();

        String NotificationText();

        String UserDetailsReport();

        String GetMixAPI(Int32 tourId);

        String GetConstraints(Int32 tourId);

        String EOTFlag(Int32 tourId);


        #region " Template "

        String PreHeaderTemplate();

        String PostFooterTemplate();

        String PageTemplate(String lang, Int32 IsWebView);

        String PageTemplateMobile(String lang, Int32 IsWebView);

        String PageUnavailableTemplate(String lang);

        #endregion

        #region " SEO Template "

        String SEOHome();
        String SEORules();
        String SEOFAQ();

        #endregion

        #region " Combine Leaderbaord "

        string WeekMappingMOLPred(string lang);

        #endregion

        String DaemonServiceCombineLB();
        String DaemonService();
        String DaemonServiceMatchAnswer();
    }
}