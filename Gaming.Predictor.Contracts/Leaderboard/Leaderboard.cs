using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming.Predictor.Contracts.Leaderboard
{
    #region " Leaderboard "

    public class Users
    {
        public String UserId { get; set; }
        public String UserTeamId { get; set; }
        public String GUID { get; set; }
        public String TeamName { get; set; }
        public String FullName { get; set; }
        public String RankNo { set; get; }
        public String Rank { set; get; }
        public String Trend { set; get; }
        public Int64 TotalMember { get; set; }
        public string Notation { set; get; }
        public String CurrentGamedayPoints { set; get; }
        public String Points { set; get; }


        //public Int64 GamedayNo { get; set; }
        //public Int64 GamedayId { get; set; }


        //public String SocialId { get; set; }
        //public Int64 ClientId { get; set; }

        //public String PhasePoints { set; get; }
        //public String OverallPoints { set; get; }

        //public bool IsCeleb { get; set; }

        //public FeedTime FeedTime { get; set; }
    }
    public class Top
    {
        public List<Users> Users { get; set; }
        public Int32 TotalMembers { get; set; }
    }

    public class LeaderboardUser
    {
        public String UserId { get; set; }
        public String FullName { get; set; }
        public String SocialId { get; set; }
        public Int32 SocialType { get; set; }
        public Int32? Rank { get; set; }
        public double? Points { get; set; }
        public Int32 PageNo { get; set; }
        public Int32 RowNo { get; set; }
    }

    public class LeaderboardRank
    {
        [JsonProperty("retVal")]
        public int RetVal { get; set; }
        [JsonProperty("totalUserCount")]
        public int TotalUserCount { get; set; }
        [JsonProperty("userTopLeague")]
        public List<UserTopLeague> UserTopLeague { get; set; }
    }
    public class UserTopLeague
    {
        [JsonProperty("userTeamId")]
        public int UserTeamId { get; set; }
        [JsonProperty("userGuid")]
        public string UserGuid { get; set; }
        [JsonProperty("userId")]
        public int UserId { get; set; }
        [JsonProperty("teamName")]
        public string TeamName { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("rank")]
        public int Rank { get; set; }
        [JsonProperty("trend")]
        public int Trend { get; set; }
        [JsonProperty("points")]
        public int Points { get; set; }
        [JsonProperty("currentGdPoint")]
        public object CurrentGdPoint { get; set; }
        [JsonProperty("curRno")]
        public int CurRno { get; set; }
    }


    #endregion

    #region " Combine Leaderboard "

    public class WeekMapping
    {
        public int weekId { get; set; }
        public DateTime FromDateIST { get; set; }
        public DateTime ToDateIST { get; set; }
        public DateTime FromDateUTC { get; set; }
        public DateTime ToDateUTC { get; set; }
        public int IsPastDate { get; set; }
    }
    public class WeekMapping1
    {
        public int weekId { get; set; }
        public int weekno { get; set; }
        public int gamedayId { get; set; }
        public int gamedayno { get; set; }
        public string gamedayDt { get; set; }
    }

    public class TopCombineLeaderboard
    {
        public List<UsersCombineLeaderboard> Users { get; set; }
        public Int32 TotalMembers { get; set; }
    }
    public class UsersCombineLeaderboard
    {
        public String SocialId { get; set; }
        public String FullName { get; set; }
        public String RankNo { set; get; }
        public String Rank { set; get; }
        public String Trend { set; get; }
        public Int64 TotalMember { get; set; }
        public string Notation { set; get; }
        public String Points { set; get; }


        //public Int64 GamedayNo { get; set; }
        //public Int64 GamedayId { get; set; }


        //public String SocialId { get; set; }
        //public Int64 ClientId { get; set; }

        //public String PhasePoints { set; get; }
        //public String OverallPoints { set; get; }

        //public bool IsCeleb { get; set; }

        //public FeedTime FeedTime { get; set; }
    }

    public class CombineLeaderboardDBResponse
    {
        public string UserName { get; set; }
        public string SocialId { get; set; }
        public int? TourId { get; set; }
        public int? CurRank { get; set; }
        public int? TotalScore { get; set; }
        public int? CurRankNo { get; set; }
        public int? Page { get; set; }
        public int? Trend { get; set; }
    }

    #endregion


}
