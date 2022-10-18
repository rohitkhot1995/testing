using Gaming.Predictor.Contracts.Automate;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Library.Utility;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Gaming.Predictor.DataAccess.BackgroundServices
{
    public class PointsCalculation : Common.BaseDataAccess
    {
        public PointsCalculation(IPostgre postgre) : base(postgre)
        {
        }

        public Matchdays Matchdays(Int32 optType, Int32 tourId)
        {
            Matchdays matchdays = new Matchdays();
            NpgsqlTransaction transaction = null;
            String spName = String.Empty;

            spName = "cf_fant_point_process_match_day_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {

                    List<String> cursors = new List<String>() { "p_cur_match_day", "p_cur_match_day_teamid" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_match_day", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_match_day_teamid", NpgsqlDbType.Refcursor)).Value = cursors[1];

                        mNpgsqlCmd.CommandTimeout = 0;
                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        matchdays = DataInitializer.Automate.PointsCal.Matchdays(mNpgsqlCmd, cursors);

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

            return matchdays;
        }

        public MOLPointTrigger GetMOLPointTrigger(int optType, int tourId, ref HTTPMeta meta)
        {
            MOLPointTrigger trigger = new MOLPointTrigger();
            Int32 retVal = -50;
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            spName = "molrank.ml_point_process_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionStringMOL))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_details" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_details", NpgsqlDbType.Refcursor)).Value = cursors[0];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        trigger = DataInitializer.Automate.PointsCal.InitializeMOLPointTrigger(mNpgsqlCmd, cursors, ref meta);

                        transaction.Commit();

                        retVal = trigger.RetVal;

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
                    GenericFunctions.AssetMeta(retVal, ref meta, "molrank.ml_point_process_get");
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

            return trigger;
        }

        public int GetMOLPointProcess(int optType, int tourId, int gamedatId, int weekId)
        {
            Int32 retVal = -50;
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            spName = "molrank.ml_user_point_process";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionStringMOL))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = gamedatId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_weekid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = weekId;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_ret_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Output });

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        Object mVal = mNpgsqlCmd.Parameters["p_ret_type"].Value;
                        retVal = mVal == null ? retVal : Convert.ToInt32(mVal);
                        transaction.Commit();
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
            return retVal;
        }

        public Int32 UserPointsProcess(Int32 optType, Int32 tourId, Int32 gamedayId, Int32 matchday)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "dcfrank.cf_user_point_process";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_match_day", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = matchday;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        //because the query takes time.
                        mNpgsqlCmd.CommandTimeout = 36000;

                        Object value = mNpgsqlCmd.ExecuteScalar();

                        retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DataAccess.Automate.PointsCal.UserPointsProcess: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return retVal;
        }


        //Point Calculation process for MOL + Predictor
        public Int32 UserPointsProcessCombine(Int32 optType, Int32 taskId, Int32 tourId, Int32 gamedayId, Int32 weekId )
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "rsmlrank.mlpr_user_point_process_rank_overall_ins";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_ml_taskid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = taskId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_weekid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = weekId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = gamedayId;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        //because the query takes time.
                        mNpgsqlCmd.CommandTimeout = 36000;

                        Object value = mNpgsqlCmd.ExecuteScalar();

                        retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DataAccess.Automate.PointsCal.UserPointsProcessCombine: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return retVal;
        }

        public DataSet UserPointsProcessReports(Int32 optType, Int32 processRetVal, Int32 tourId, Int32 gamedayId, Int32 matchday)
        {
            DataSet ds = new DataSet();
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            spName = "dcfrank.cf_user_point_process_report_get_v1";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_report" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_point_process_ret_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = processRetVal;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_match_day", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = matchday;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_report", NpgsqlDbType.Refcursor)).Value = cursors[0];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        ds = DataInitializer.Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw new Exception("DataAccess.Automate.PointsCal.UserPointsProcessReports: " + ex.Message);
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return ds;
        }

        public Int32 UserPointsProcessMatchdayUpdated(Int32 optType, Int32 tourId, Int32 matchid)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_fant_point_process_match_day_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = matchid;
                        NpgsqlParameter value = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(value);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        retVal = value.Value != null && value.Value.ToString().Trim() != "" ? Int32.Parse(value.Value.ToString()) : retVal;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DataAccess.Automate.PointsCal.UserPointsProcessMatchdayUpdated: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return retVal;
        }
    }
}
