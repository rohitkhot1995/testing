using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Leaderboard;
using Gaming.Predictor.Contracts.Notification;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Library.Utility;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Gaming.Predictor.DataAccess.Notification
{
    public class Publish : Common.BaseDataAccess
    {
        public Publish(IPostgre postgre) : base(postgre)
        {
        }

        public Messages FetchEvent(Int64 optType, Int64 tourId)
        {
            Messages message = new Messages();
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_user_nt_generic_notification_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_notification" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_notification", NpgsqlDbType.Refcursor)).Value = cursors[0];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        message = new DataInitializer.Notification.Publish().FetchEvent(mNpgsqlCmd, cursors);

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw new Exception("DataAccess.Notification.Publish.FetchEvent: " + ex.Message);
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return message;
        }
    }
}
