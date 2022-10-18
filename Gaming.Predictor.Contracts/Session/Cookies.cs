using Newtonsoft.Json;
using System;

namespace Gaming.Predictor.Contracts.Session
{
    public class UserCookie
    {
        public UserCookie()
        {
            this.UserId = 0;
        }

        public Int32 UserId { get; set; }
        public Int32 TeamId { get; set; }
        public string FullName { get; set; }
        public string SocialId { get; set; }
    }

    public class GameCookie
    {
        public String GUID { get; set; }
        public String WAF_GUID { get; set; }
        public String SocialId { get; set; }
        public String ClientId { get; set; }
        public Int32 CoinTotal { get; set; }
    }

    public class UserDetails
    {
        public UserCookie User { get; set; }
        public GameCookie Game { get; set; }
    }

    public class UserLoginDBResp
    {
            [JsonProperty("retval")]
            public int Retval { get; set; }
            [JsonProperty("usrid")]
            public int? Usrid { get; set; }
            [JsonProperty("teamid")]
            public int? Teamid { get; set; }
            [JsonProperty("usrguid")]
            public string Usrguid { get; set; }
            [JsonProperty("usrsocid")]
            public string Usrsocid { get; set; }
            [JsonProperty("usrclnid")]
            public int Usrclnid { get; set; }
            [JsonProperty("temname")]
            public string Temname { get; set; }
            [JsonProperty("usrname")]
            public string Usrname { get; set; }
            [JsonProperty("favteam")]
            public int Favteam { get; set; }
            [JsonProperty("profpic")]
            public string Profpic { get; set; }
            [JsonProperty("cointotal")]
            public Int32? coinTotal { get; set; }
    }

    public class WAFCookie
    {
        public string user_guid { get; set; }
        public string name { get; set; }
        public string email_id { get; set; }
        public string is_first_login { get; set; }
        public string favourite_club { get; set; }
        public string edition { get; set; }
        public string status { get; set; }
        public string is_app { get; set; }
        public string is_custom_image { get; set; }
        public string social_user_image { get; set; }
    }

    #region " User Profile Response "

    public class WAFResultDetails
    {
        public ResultData data { get; set; }
    }

    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class User
    {
        public string name { get; set; }
        public string edition { get; set; }
        public string favourite_club { get; set; }
        public string social_user_image { get; set; }
        public string mobile_no { get; set; }
        public object profile_completion_percentage { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public object profile_page_enabled { get; set; }
        public object campaign_id { get; set; }
        public CampaignJson campaign_json { get; set; }
        public string membership_code { get; set; }
        public string social_user_name { get; set; }
        public bool subscribe_for_email { get; set; }
        public string dob { get; set; }
        public object sports_vertical { get; set; }
        public object sporting_event_inspired_you { get; set; }
        public object mail { get; set; }
        public string referral_code { get; set; }
        public string referral_code_change { get; set; }
    }

    public class ResultData
    {
        public string user_guid { get; set; }
        public string user_id { get; set; }
        public string status { get; set; }
        public string email_id { get; set; }
        public object failed_attempts { get; set; }
        public object unlock_date { get; set; }
        public string message { get; set; }
        public Error error { get; set; }
        public User user { get; set; }
        public string is_first_login { get; set; }
        public string is_custom_image { get; set; }
        public string old_profile_img_url { get; set; }
        public object gift_id { get; set; }
        public object gift_name { get; set; }
        public string client_id { get; set; }
        public object campaign_id { get; set; }
        public object id { get; set; }
        public string waf_user_guid { get; set; }
        public string created_date { get; set; }
    }

    public class CampaignJson
    {
    }

    #endregion

    public class LoginResponse
    {
        public String GUID { get; set; }
        public String WAF_GUID { get; set; }
        public Int32 CoinTotal { get; set; }
    }


    #region " Master Demo Login "

    public class MasterLoginCookie
    {
        public String SocialId { get; set; }
        public String UserName { get; set; }
        public String EmailId { get; set; }
    }

    #endregion
}