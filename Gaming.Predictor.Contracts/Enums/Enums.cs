using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming.Predictor.Contracts.Enums
{
    class Enums
    {
    }

    public enum LeaderboardType
    {
        Match = 1,
        Overall = 2
    }

    public class DBRankType
    {
        public static String Match { get; } = "OVERALL_MATCH";
        public static String Tour { get; } = "OVERALL_TOUR";
        public static String Tour_League { get; } = "LEAGUE_TOUR";
    }

    public enum QuestionStatus
    {
        All = -2,
        Unpublished = 0,
        Published = 1,
        Locked = 2,
        Resolved = 3,
        Delete = -1,
        Notification = -3,
        Points_Calculation = -4
    }

    public enum Apis
    {
        afix, acon, aqus, lbov, lbtour
    }

    public enum MimeType
    {
        Application, Text, Image, json
    }
}
