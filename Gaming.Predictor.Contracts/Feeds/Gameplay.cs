﻿using System;
using System.Collections.Generic;

namespace Gaming.Predictor.Contracts.Feeds
{
    #region " Fixtures "

    public class Fixtures
    {
        public Int32 TourId { get; set; }
        public Int32 MatchId { get; set; }
        public Int32 TotalPredQstn { get; set; }
        public String TotalPredQstnNotation { get; set; }

        public Int32 PhaseId { get; set; }
        public Int32 SeriesId { get; set; }
        public Int32 GamedayId { get; set; }

        public String MatchDate { get; set; }
        public DateTime MatchDateTime { get; set; }
        public String Matchfile { get; set; }
        public Int32 MatchStatus { get; set; }

        public Int32 TeamA { get; set; }
        public String TeamAName { get; set; }
        public String TeamAShortName { get; set; }
        public Int32 TeamB { get; set; }
        public String TeamBName { get; set; }
        public String TeamBShortName { get; set; }

        public String Venue { get; set; }

        //public Int32 UserPlayedCount { get; set; }

        public Int32 TeamGamedayId { get; set; }
        public Int32 TourGamedayId { get; set; }
        
        public String Date { get; set; }
        public String Deadlinedate { get; set; }

        

        //public Int32 TeamAId { get; set; }
        //public String TeamAName { get; set; }
        //public String TeamAShortName { get; set; }
        //public String TeamACountryCode { get; set; }
        //public decimal TeamAScore { get; set; }

        //public Int32 TeamBId { get; set; }
        //public String TeamBName { get; set; }
        //public String TeamBShortName { get; set; }
        //public String TeamBCountryCode { get; set; }
        //public decimal TeamBScore { get; set; }

        public String MatchdayName { get; set; }
        
        public Int32 Is_Lineup_Process { get; set; }
        public Int32 Is_Toss_Process { get; set; }
        public Int32 Inning_1_BWL_Teamid { get; set; }
        public Int32 Inning_1_BAT_Teamid { get; set; }
        public Int32 Inning_2_BAT_Teamid { get; set; }
        public Int32 Inning_2_BWL_Teamid { get; set; }
        public Int32 Match_Inning_Status { get; set; }

        
        public Int32 IsQuestionAnswerProcess { get; set; }
        public Int32 IsTBC { get; set; }
    }

    public class Fixtures_Stats
    {
        public Int32 Matchid { get; set; }
        public Int32 TotalPrdQstn { get; set; }
    }

    #endregion " Fixtures "

    #region " Skills "

    public class Skills
    {
        public Int32 SkillId { get; set; }
        public String SkillName { get; set; }
    }

    #endregion

    #region " Match Questions Players "

    public class MatchQuestionPlayers
    {
        public String MatchId { get; set; }
        public String PlayerId { get; set; }
        public String SkillId { get; set; }
        public String TeamId { get; set; }
    }

    public class MatchQuestionPlayerArray
    {
        public List<List<double>> MyArray { get; set; }
    }


    #endregion

    #region " Match Questions "

    public class MatchQuestions
    {
        public Int32 MatchId { get; set; }
        public Int32 QuestionId { get; set; }
        public Int32 QuestionNo { get; set; }
        
        public Int32 InningNo { get; set; }
        public Int32 CoinMult { get; set; }
        public Int32 LastQstn { get; set; }


        public String QuestionDesc { get; set; }
        public String QuestionType { get; set; }
        public Int32 Status { get; set; }
        public String QuestionCode { get; set; }
        public String QuestionOccurrence { get; set; }
        public String OptionJson { get; set; }
        public List<OptionList> OptionLists { get; set; }
        public List<Options> Options { get; set; }
        //public String LockedDate { get; set; }
        public String PublishedDate { get; set; }
        public String QuestionTime { get; set; }
        public String QuestionPoints { get; set; }

        public List<CoinMultiplier> CoinMultiplier { get; set; }
        public Int32 QuestionNumber { get; set; }

        public List<ListControl> LstQstnList { get; set; }

        public Int32 IsLiveQuestion { get; set; }
    }

    public class CoinMultiplier
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public Int32 IsSelected { get; set; }
    }
    public class ListControl
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public Int32 IsSelected { get; set; }
    }

    public class OptionList
    {
        public Int32 cf_questionid { get; set; }
        public Int32 cf_optionid { get; set; }
        public String option_dec { get; set; }
        public Int32 cf_assetid { get; set; }
        public String asset_type { get; set; }
        public Int32? is_correct { get; set; }
        public Int32? min_val { get; set; }
        public Int32? max_val { get; set; }
    }

    public class Options
    {
        public Int32 QuestionId { get; set; }
        public Int32 OptionId { get; set; }
        public String OptionDesc { get; set; }
        public Int32 AssetId { get; set; }
        public String AssetType { get; set; }
        public Int32 IsCorrect { get; set; }
        public Int32? MinVal { get; set; }
        public Int32? MaxVal { get; set; }
        public bool IsCorrectBool { get; set; }
        public Double SelectedPercent { get; set; }

        public String TeamId { get; set; }
        public String SkillId { get; set; }
    }

    public class UserPredictionSubmit
    {
        public Int32 MatchId { get; set; }
        public Int32 TourGamedayId { get; set; }
        public Int32 QuestionId { get; set; }
        public Int32 OptionId { get; set; }
    }

    public class UserPredictionResult
    {
        //public Int32 QuestionId { get; set; }
        //public Int32 Question_No { get; set; }
        //public Int32 OptionId { get; set; }
        //public Int64 Points { get; set; }
        //public Int64 Rank { get; set; }
        public List<QuestionDetails> QuestionDetails { get; set; }
        public List<PointsRank> PointRank { get; set; }
    }

    public class QuestionDetails
    {
        public Int32 QuestionId { get; set; }
        public Int32 Question_No { get; set; }
        public Int32 OptionId { get; set; }
    }

    public class PointsRank
    {
        public Int32 Points { get; set; }
        public String Rank { get; set; }
    }

    public class GameDays
    {
        public Int32 TourGamedayId { get; set; }
    }

    #endregion " Match Questions "

    #region " Recent Results "
    public class RecentResults
    {
        public Int32 TeamID { get; set; }
        public Int64 TotalPlayed { get; set; }
        public Int64 TotalWin { get; set; }
        public Int64 TotalLoss { get; set; }
    }
    #endregion " Recent Results "

    #region " Inning Status "
    public class InningStatus
    {
        public Int32 MatchId { get; set; }
        public Int32 MatchStatus { get; set; }
        public Int32 MatchInningStatus { get; set; }
    }
    #endregion " Inning Status "

    #region " Played Gamedays "
    public class PlayedGamedays
    {
        public Int32 GamedayId { get; set; }
        //public String GamedayName { get; set; }
        public String GamedayName { get; set; }
        public String Matchdate { get; set; }
    }
    public class PlayedPhase
    {
        public Int32 PhaseId { get; set; }
        public String PhaseName { get; set; }
    }
    public class UserPlayedLB
    {
        // public String OverAll { get; set; }
        public List<PlayedGamedays> PlayedGamedays { get; set; }
        public List<PlayedPhase> PlayedPhase { get; set; }
    }
    #endregion

    #region " User Profile "
    public class UserDataPointsRank
    {
        public String EmailId { get; set; }
        public String PhoneNumber { get; set; }
        public String UserName { get; set; }
        public String ProfilePicture { get; set; }
        //public Int32 Points { get; set; }
        //public Int32 Rank { get; set; }
        public Int32 Points { get; set; }
        public String Rank { get; set; }
    }
    public class UserMatchData
    {
        public Int32 MatchId { get; set; }
        public String Date { get; set; }
        public Int32 Points { get; set; }
        public String Rank { get; set; }
        public String HomeTeamId { get; set; }
        public String HomeTeamName { get; set; }
        public String HomeTeamShortName { get; set; }
        public String AwayTeamId { get; set; }
        public String AwayTeamName { get; set; }
        public String AwayTeamShortName { get; set; }
    }

    public class UserProfile
    {
        public List<UserDataPointsRank> UserDataPointsRankList { get; set; }
        public List<UserMatchData> UserMatchDataList { get; set; }
    }

    #endregion " User Profile "

    #region " Currentgameday Matches "
    public class CurrentGamedayMatches
    {
        public Int32 MatchId { get; set; }
        public Int32 TeamGamedayId { get; set; }
        public Int32 TourGamedayId { get; set; }
        public String Date { get; set; }
        public Int32 MatchStatus { get; set; }
        public Int32 Live { get; set; }
        public Int32 Match_Inning_Status { get; set; }
    }
    #endregion

    #region "MIXAPI"

    public class Mixapi
    {
        public string ov { get; set; }
        public Int32 isPointProcess { get; set; }
        public Int32 isGDAbandoned { get; set; }
        public Int32 isMaintenance { get; set; }


        public List<GameAPI> api = new List<GameAPI>();
        public List<LeaderboardVersion> lb = new List<LeaderboardVersion>();
    }

    public class GameAPI
    {
        public string nm { get; set; }
        public string vn { get; set; }
    }
    public class LeaderboardVersion
    {
        public Int32 matchId { get; set; }
        public String vn { get; set; }
    }

    #endregion "MIXAPI"

    #region " DB Response "

    public class UserPredictionPostDBResponse
    {
        public int retval { get; set; }
        public object cointt { get; set; }
    }

    public class UserCoinGetDBResponse
    {
        public int retval { get; set; }
        public Int32 coinTotal { get; set; }
    }

    #endregion

    #region " Payload "

    public class UserPredictionPayload
    {
        public Int32 MatchId { get; set; }
        public Int32 TourGamedayId { get; set; }
        public Int32 QuestionId { get; set; }
        public Int32 OptionId { get; set; }
        public Int32 BetCoin { get; set; }
        public Int32 PlatformId { get; set; }
    }

    #endregion

    #region " Joker Booster "
    public class AppkyJokerPayload
    {
        public Int32 OptType { get; set; }
        public Int32 MatchId { get; set; }
        public Int32 TourGamedayId { get; set; }
        public Int32 BoosterId { get; set; }
        public Int32 PlatformId { get; set; }
        public Int32 QuestionId { get; set; }
    }
    #endregion
}