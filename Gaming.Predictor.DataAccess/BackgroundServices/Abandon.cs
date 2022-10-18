using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Gaming.Predictor.Contracts.Admin;
using Gaming.Predictor.Contracts.Automate;
using Gaming.Predictor.Interfaces.Connection;
using Npgsql;
using NpgsqlTypes;

namespace Gaming.Predictor.DataAccess.BackgroundServices
{
    public class Abandon : Common.BaseDataAccess
    {
        public Abandon(IPostgre postgre) : base(postgre)
        {
        }

        public AbdPointPrcGet AbandonPrcGet(Int32 optType, Int32 tourId)
        {
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            AbdPointPrcGet abdPointPrcGet = new AbdPointPrcGet();

            spName = "cf_fant_point_process_abandon_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_match_day", "p_cur_match_day_teamid " };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_match_day", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_match_day_teamid", NpgsqlDbType.Refcursor)).Value = cursors[1];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        abdPointPrcGet = DataInitializer.BackgroundServices.Abandon.InitializeAbdPrcGet(mNpgsqlCmd, cursors);
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return abdPointPrcGet;
        }

        public Int32 AbandonPointPrc(Int32 optType, Int32 tourId,Int32 TourGamedayId,Int32 Matchday)
        {
            String spName = String.Empty;
            Int32 retVal = -50;

            AbdPointPrcGet abdPointPrcGet = new AbdPointPrcGet();

            spName = "dcfrank.cf_abandon_user_point_process";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = TourGamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_match_day", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = Matchday;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.CommandTimeout = 36000;

                        Object value = mNpgsqlCmd.ExecuteScalar();

                        retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return retVal;
        }

        public DataSet AbandonPointProcessReports(Int32 optType, Int32 processRetVal, Int32 tourId, Int32 gamedayId, Int32 matchday)
        {
            DataSet ds = new DataSet();
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            spName = "dcfrank.cf_abandon_user_point_process_report_get_v1";

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

                    throw new Exception("DataAccess.Automate.PointsCal.AbandonProcessReports: " + ex.Message);
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

    }
}
