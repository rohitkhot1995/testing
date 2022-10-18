using Gaming.Predictor.Contracts.Admin;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Leaderboard;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Library.Utility;
using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Gaming.Predictor.DataAccess.Leaderboard
{
    public class Leaderbaord : Common.BaseDataAccess
    {
        public Leaderbaord(IPostgre postgre) : base(postgre)
        {
        }


        public ResponseObject UserRank(Int32 optType, Int32 tourId, Int32 userId, Int32 teamId, Int32 gamedayId, Int32 phaseId, ref HTTPMeta httpMeta)
        {
            ResponseObject ranks = new ResponseObject();
            Int32 retVal = -50;
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            Top userrank = new Top();
            userrank.Users = new List<Users>();

            spName = "cf_user_rank_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.CommandTimeout = 0;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_user_tour_teamid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = teamId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_userid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = userId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_phaseid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = phaseId;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();

                        dynamic retjson =  mNpgsqlCmd.ExecuteScalar();

                        JObject obj = JObject.Parse(retjson);
                        retVal = (Int32)obj["retVal"];

                        if(obj["userLeague"].HasValues)
                        userrank.Users.Add(new Users
                        {
                            UserTeamId = (obj["userLeague"]["userTeamId"] != null && !String.IsNullOrEmpty(obj["userLeague"]["userTeamId"].ToString())) ? BareEncryption.BaseEncrypt(obj["userLeague"]["userTeamId"].ToString()) : "",
                            GUID = (obj["userLeague"]["userGuid"] != null && !String.IsNullOrEmpty(obj["userLeague"]["userGuid"].ToString())) ? obj["userLeague"]["userGuid"].ToString() : "",
                            UserId = (obj["userLeague"]["userId"] != null && !String.IsNullOrEmpty(obj["userLeague"]["userId"].ToString())) ? BareEncryption.BaseEncrypt(obj["userLeague"]["userId"].ToString()) : "",
                            TeamName = (obj["userLeague"]["teamName"] != null && !String.IsNullOrEmpty(obj["userLeague"]["teamName"].ToString())) ? obj["userLeague"]["teamName"].ToString() : "",
                            FullName = (obj["userLeague"]["fullName"] != null && !String.IsNullOrEmpty(obj["userLeague"]["fullName"].ToString())) ? BareEncryption.BaseDecrypt(obj["userLeague"]["fullName"].ToString()) : "",
                            RankNo = (obj["userLeague"]["rowNo"] != null && !String.IsNullOrEmpty(obj["userLeague"]["rowNo"].ToString())) ? obj["userLeague"]["rowNo"].ToString() : "",
                            Rank = (obj["userLeague"]["rank"] != null && !String.IsNullOrEmpty(obj["userLeague"]["rank"].ToString())) ? obj["userLeague"]["rank"].ToString() : "",
                            Trend = (obj["userLeague"]["trend"] != null && !String.IsNullOrEmpty(obj["userLeague"]["trend"].ToString())) ? obj["userLeague"]["trend"].ToString() : "",
                            Points = (obj["userLeague"]["points"] != null && !String.IsNullOrEmpty(obj["userLeague"]["points"].ToString())) ? obj["userLeague"]["points"].ToString() : "",
                            CurrentGamedayPoints = (obj["userLeague"]["currentGdPoint"] != null && !String.IsNullOrEmpty(obj["userLeague"]["currentGdPoint"].ToString())) ? obj["userLeague"]["currentGdPoint"].ToString() : "",
                        });

                        if (retVal == 1)
                        {
                            ranks.Value = userrank;
                            ranks.FeedTime = GenericFunctions.GetFeedTime();
                        }

                        transaction.Commit();

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);

                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw ex;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return ranks;
        }

        public ResponseObject Top(Int32 optType, Int32 phaseId, Int32 gamedayId, Int32 pageNo, Int32 top, Int32 tourId, Int32 fromRowNo, Int32 toRowNo, ref HTTPMeta httpMeta)
        {
            ResponseObject vLeaderboard = new ResponseObject();
            Int64 retVal = -50;
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_user_rank_top_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_top_rank", "p_cur_detail " };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_page_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = pageNo;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_top_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = top;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_phaseid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = phaseId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_from_row_number", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = fromRowNo;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_to_row_number", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = toRowNo;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_top_rank", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_detail", NpgsqlDbType.Refcursor)).Value = cursors[1];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        vLeaderboard = DataInitializer.Leaderboard.Leaderboard.InitializeTop(mNpgsqlCmd, cursors, out retVal);

                        transaction.Commit();

                        //retVal = 1;

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw ex;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return vLeaderboard;
        }

        public ResponseObject TopJson(Int32 optType, Int32 phaseId, Int32 gamedayId, Int32 pageNo, Int32 top, Int32 tourId, Int32 fromRowNo, Int32 toRowNo, ref HTTPMeta httpMeta)
        {
            ResponseObject vLeaderboard = new ResponseObject();
            Int64 retVal = -50;
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_user_rank_top_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_page_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = pageNo;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_top_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = top;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_phaseid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = phaseId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_from_row_number", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = fromRowNo;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_to_row_number", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = toRowNo;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();

                        dynamic retJson =  mNpgsqlCmd.ExecuteScalar();

                        JObject obj = JObject.Parse(retJson);
                        retVal = (Int32)obj["retVal"];

                        if(retVal==1)
                        {
                            vLeaderboard.Value = GenericFunctions.Deserialize<dynamic>(retJson);
                            vLeaderboard.FeedTime = GenericFunctions.GetFeedTime();
                        }

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw ex;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return vLeaderboard;
        }

        public Reports AdminLeaderBoard(Int32 optType, Int32 pageno, Int32 topno, Int32 tourId, Int32 phaseId, Int32 gamedayId, Int32 fromrowno, Int32 torowno, ref HTTPMeta httpMeta)
        {
            Reports leaderboard = new Reports();
            Int32 retVal = -50;
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_admin_user_rank_top_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_top_rank", "p_cur_detail " };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_page_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = pageno;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_top_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = topno;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_phaseid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = phaseId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_from_row_number", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = fromrowno;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_to_row_number", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = torowno;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_top_rank", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_detail", NpgsqlDbType.Refcursor)).Value = cursors[1];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        leaderboard = DataInitializer.Leaderboard.Leaderboard.InitializeAdminLeaderboard(mNpgsqlCmd, cursors);

                        transaction.Commit();

                        retVal = 1;

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw ex;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return leaderboard;
        }

        public List<WeekMapping> GetWeekMappingCombineLB(int optType, int tourId, ref HTTPMeta meta)
        {
            int retVal = -50;

            List<WeekMapping> mappings = new List<WeekMapping>();
            List<WeekMapping1> mappings1 = new List<WeekMapping1>();
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            spName = "mol.ml_tour_week_info_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionStringMOL))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        //mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_weekid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = weekId;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();

                        Object retJson = mNpgsqlCmd.ExecuteScalar();

                        String str = (Convert.IsDBNull(retJson) || retJson == null || String.IsNullOrEmpty(retJson.ToString())) ? "" : retJson.ToString();

                        mappings1 = GenericFunctions.Deserialize<List<WeekMapping1>>(str);

                        if (mappings1.Count > 0)
                        {
                            //mappings1 = mappings1.OrderBy(a => a.gamedayno).ToList();
                            mappings1 = mappings1.OrderByDescending(a => a.gamedayno).ToList();

                            foreach (int weekId in mappings1.Select(a => a.weekId).Distinct().ToList())
                            {
                                WeekMapping m = new WeekMapping();
                                string f = mappings1.Where(a => a.weekId == weekId).Select(a => a.gamedayDt.ToString()).FirstOrDefault();
                                string l = mappings1.Where(a => a.weekId == weekId).Select(a => a.gamedayDt.ToString()).LastOrDefault();

                                m.FromDateUTC = DateTime.ParseExact(f, "yyyy-M-dTH:m:s", null);
                                m.ToDateUTC = DateTime.ParseExact(l, "yyyy-M-dTH:m:s", null);
                                m.FromDateIST = m.FromDateUTC;
                                m.ToDateIST = m.ToDateUTC;
                                m.weekId = weekId;
                                mappings.Add(m);
                            }
                        }

                        transaction.Commit();
                        retVal = 1;

                        GenericFunctions.AssetMeta(retVal, ref meta, spName);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (transaction != null)
                            transaction.Rollback();
                    }
                    catch { }
                    GenericFunctions.AssetMeta(retVal, ref meta, "ml_tour_week_info_get");
                    throw ex;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }

            }
            return mappings;
        }

        public ResponseObject UserRankCombineLB(Int32 optType, Int32 tourId, String socialId, Int32 weekId, Int32 gamedayId, ref HTTPMeta httpMeta)
        {
            ResponseObject ranks = new ResponseObject();
            Int32 retVal = -50;
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            TopCombineLeaderboard userrank = new TopCombineLeaderboard();
            userrank.Users = new List<UsersCombineLeaderboard>();

            spName = "rsmlrank.mlpr_user_rank_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.CommandTimeout = 0;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_social_id", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = socialId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_weekid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = weekId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = gamedayId;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();

                        dynamic retjson = mNpgsqlCmd.ExecuteScalar();

                        List<CombineLeaderboardDBResponse> dbResponse = new List<CombineLeaderboardDBResponse>();

                        if (String.IsNullOrEmpty(retjson) == false)
                            dbResponse = GenericFunctions.Deserialize<List<CombineLeaderboardDBResponse>>(retjson);

                        //JObject obj = JArray.Parse(retjson)[0];
                        retVal = 1;

                        if (dbResponse != null && dbResponse.Count > 0)
                        {
                            foreach (CombineLeaderboardDBResponse user in dbResponse)
                            {
                                userrank.Users.Add(new UsersCombineLeaderboard
                                {
                                    FullName = String.IsNullOrEmpty(user.UserName)
                                               ? "" : BareEncryption.BaseDecrypt(user.UserName),
                                    SocialId = String.IsNullOrEmpty(user.SocialId)
                                               ? "" : BareEncryption.BaseEncrypt(user.SocialId),
                                    RankNo = user.CurRankNo == null ? "0" : user.CurRankNo.Value.ToString(),
                                    Rank = user.CurRank == null ? "0" : user.CurRank.Value.ToString(),
                                    Trend = user.Trend == null ? "0" : user.Trend.Value.ToString(),
                                    Points = user.TotalScore == null ? "0" : user.TotalScore.Value.ToString(),
                                });
                            }
                        }

                        if (retVal == 1)
                        {
                            ranks.Value = userrank;
                            ranks.FeedTime = GenericFunctions.GetFeedTime();
                        }

                        transaction.Commit();

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);

                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw ex;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return ranks;
        }

        public ResponseObject TopCombineLB(Int32 optType, Int32 weekId, Int32 gamedayId, Int32 tourId, ref HTTPMeta httpMeta)
        {
            ResponseObject vLeaderboard = new ResponseObject();
            Int64 retVal = -50;
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            TopCombineLeaderboard userrank = new TopCombineLeaderboard();
            userrank.Users = new List<UsersCombineLeaderboard>();

            spName = "rsmlrank.mlpr_user_rank_top_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_weekid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = weekId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = gamedayId;


                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        dynamic retjson = mNpgsqlCmd.ExecuteScalar();


                        List<CombineLeaderboardDBResponse> dbResponse = new List<CombineLeaderboardDBResponse>();
                            
                        if(String.IsNullOrEmpty(retjson) == false)
                            dbResponse = GenericFunctions.Deserialize<List<CombineLeaderboardDBResponse>>(retjson);

                        //JObject obj = JArray.Parse(retjson)[0];
                        retVal = 1;

                        if (dbResponse != null && dbResponse.Count > 0)
                        {
                            foreach (CombineLeaderboardDBResponse user in dbResponse)
                            {
                                userrank.Users.Add(new UsersCombineLeaderboard
                                {
                                    FullName = String.IsNullOrEmpty(user.UserName)
                                               ? "" : BareEncryption.BaseDecrypt(user.UserName),
                                    SocialId = String.IsNullOrEmpty(user.SocialId)
                                               ? "" : BareEncryption.BaseEncrypt(user.SocialId),
                                    RankNo = user.CurRankNo == null ? "0" : user.CurRankNo.Value.ToString(),
                                    Rank = user.CurRank == null ? "0" : user.CurRank.Value.ToString(),
                                    Trend = user.Trend == null ? "0" : user.Trend.Value.ToString(),
                                    Points = user.TotalScore == null ? "0" : user.TotalScore.Value.ToString(),
                                });
                            }
                        }
                        if (retVal == 1)
                        {
                            vLeaderboard.Value = userrank;
                            vLeaderboard.FeedTime = GenericFunctions.GetFeedTime();
                        }

                            transaction.Commit();

                            GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw ex;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return vLeaderboard;
        }
    }
}
