﻿using Gaming.Predictor.Contracts.Admin;
using Gaming.Predictor.Contracts.Automate;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Feeds;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Gaming.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaming.Predictor.Blanket.Automate
{
    class PointsCal : Common.BaseBlanket
    {
        private readonly DataAccess.Automate.PointsCal _DBContext;
        private readonly Int32 _TourId;

        public PointsCal(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Automate.PointsCal(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public Matchdays Matchdays()
        {
            Int32 optType = 1;

            Matchdays matchdays = new Matchdays();
          
            return _DBContext.Matchdays(optType, _TourId);
        }

        public Int32 UserPointsProcess(Int32 gamedayId, Int32 matchday)
        {
            Int32 retVal = new Int32();

            try
            {
                Int32 optType = 1;               

                retVal = _DBContext.UserPointsProcess(optType, _TourId, gamedayId, matchday);
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.Automate.PointsCal.UserPointsProcess: " + ex.Message);
            }

            return retVal;
        }

        public DataSet UserPointsProcessReports(Int32 processRetVal, Int32 gamedayId, Int32 matchday)
        {
            DataSet ds = new DataSet();

            try
            {
                Int32 optType = 1;

                ds = _DBContext.UserPointsProcessReports(optType, processRetVal, _TourId, gamedayId, matchday);
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.Automate.PointsCal.UserPointsProcessReports: " + ex.Message);
            }

            return ds;
        }

        public static StringBuilder ParseReports(DataSet ds)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                if (ds != null && ds.Tables != null)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];

                        //Open tag
                        sb.Append("<table border='1px' cellpadding='2' cellspacing='1' bgcolor='#e6eeff' style='font-family:Garamond; font-size:smaller;border-collapse: collapse'>");

                        //Column Header row
                        sb.Append("<tr >");
                        foreach (DataColumn myColumn in dt.Columns)
                        {
                            sb.Append("<td>");
                            sb.Append(myColumn.ColumnName);
                            sb.Append("</td>");
                        }
                        sb.Append("</tr>");


                        foreach (DataRow myRow in dt.Rows)
                        {
                            //Value rows
                            sb.Append("<tr>");
                            foreach (DataColumn myColumn in dt.Columns)
                            {
                                sb.Append("<td>");
                                sb.Append(myRow[myColumn.ColumnName].ToString());
                                sb.Append("</td>");
                            }
                            sb.Append("</tr>");
                        }

                        //Close tag
                        sb.Append("</table>");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.Automate.PointsCal.ParseReports: " + ex.Message);
            }

            return sb;
        }

    }
}
