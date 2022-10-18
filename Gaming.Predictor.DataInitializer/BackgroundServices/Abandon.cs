using Gaming.Predictor.Contracts.Automate;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Gaming.Predictor.DataInitializer.BackgroundServices
{
    public class Abandon
    {
        public static AbdPointPrcGet InitializeAbdPrcGet(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            DataSet ds = null;
            AbdPointPrcGet abdPointPrcGet = new AbdPointPrcGet();
            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            abdPointPrcGet = (from a in ds.Tables[0].AsEnumerable()
                                         select new AbdPointPrcGet
                                         {
                                             TourGamedayId = Convert.IsDBNull(a["cf_tour_gamedayid"]) ? 0 : Convert.ToInt32(a["cf_tour_gamedayid"]),
                                             MatchDay = Convert.IsDBNull(a["cf_match_day"]) ? 0 : Convert.ToInt32(a["cf_match_day"]),
                                             PhaseID = Convert.IsDBNull(a["cf_phaseid"]) ? 0 : Convert.ToInt32(a["cf_phaseid"]),
                                             Tourid = Convert.IsDBNull(a["cf_tourid"]) ? 0 : Convert.ToInt32(a["cf_tourid"])
                                         }).FirstOrDefault();
                        }

                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            abdPointPrcGet.TeamIds = ds.Tables[1].AsEnumerable().Select(o =>
                            (Convert.IsDBNull(o["cf_soccer_teamid"]) ? 0 : Convert.ToInt32(o["cf_soccer_teamid"].ToString()))).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataInitializer.Automate.PointsCal.InitializeAbdPointPrcGet: " + ex.Message);
            }

            return abdPointPrcGet;
        }
    }
}
