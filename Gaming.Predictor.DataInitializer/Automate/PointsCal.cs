using Gaming.Predictor.Contracts.Automate;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Feeds;
using Gaming.Predictor.Library.Utility;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Gaming.Predictor.DataInitializer.Automate
{
   public class PointsCal
    {
        public static Matchdays Matchdays(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            Matchdays matchdays = new Matchdays();
            DataSet ds = null;

            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            matchdays = (from a in ds.Tables[0].AsEnumerable()
                                         select new Matchdays
                                         {
                                             GamedayId = Convert.IsDBNull(a["cf_tour_gamedayid"]) ? 0 : Convert.ToInt32(a["cf_tour_gamedayid"]),
                                             PhaseId = Convert.IsDBNull(a["cf_phaseid"]) ? 0 : Convert.ToInt32(a["cf_phaseid"]),
                                             Matchday = Convert.IsDBNull(a["cf_match_day"]) ? 0 : Convert.ToInt32(a["cf_match_day"])
                                         }).FirstOrDefault();
                        }

                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            matchdays.TeamIds = ds.Tables[1].AsEnumerable().Select(o =>
                            (Convert.IsDBNull(o["cf_soccer_teamid"]) ? 0 : Convert.ToInt32(o["cf_soccer_teamid"].ToString()))).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataInitializer.Automate.PointsCal.Matchdays: " + ex.Message);
            }

            return matchdays;
        }

        public static MOLPointTrigger InitializeMOLPointTrigger(NpgsqlCommand mNpgsqlCmd, List<string> cursors, ref HTTPMeta httpMeta)
        {
            MOLPointTrigger trigger = new MOLPointTrigger();
            DataSet ds = null;
            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            trigger = (from a in ds.Tables[0].AsEnumerable()
                                       select new MOLPointTrigger
                                       {
                                           GameDayId = Convert.IsDBNull(a["gameday_id"]) ? 0 : Convert.ToInt32(a["gameday_id"]),
                                           TourId = Convert.IsDBNull(a["cf_tourid"]) ? 0 : Convert.ToInt32(a["cf_tourid"]),
                                           WeekId = Convert.IsDBNull(a["week_id"]) ? 0 : Convert.ToInt32(a["week_id"]),
                                           RetVal = Convert.IsDBNull(a["ret_type"]) ? 0 : Convert.ToInt32(a["ret_type"])
                                       }).FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataInitializer.Automate.PointsCal.InitializeMOLPointTrigger: " + ex.Message);
            }

            return trigger;
        }
    }
}
