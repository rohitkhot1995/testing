using Gaming.Predictor.Contracts.Automate;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Gaming.Predictor.Blanket.BackgroundServices
{
    public class Abandon : Common.BaseServiceBlanket
    {
        private readonly DataAccess.BackgroundServices.Abandon _DBContext;
        private readonly Int32 _TourId;

        public Abandon(IOptions<Application> appSettings, IOptions<Daemon> serviceSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
                : base(appSettings, serviceSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.BackgroundServices.Abandon(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public AbdPointPrcGet AbandonPrcGet()
        {
            Int32 optType = 1;

            return _DBContext.AbandonPrcGet(optType, _TourId);
        }
        public Int32 AbandonPointPrc(Int32 TourGamedayId,Int32 Matchday)
        {
            Int32 optType = 1;

            return _DBContext.AbandonPointPrc(optType, _TourId, TourGamedayId, Matchday);
        }

        public DataSet AbandonPointProcessReports(Int32 processRetVal, Int32 gamedayId, Int32 matchday)
        {
            DataSet ds = new DataSet();

            try
            {
                Int32 optType = 1;

                ds = _DBContext.AbandonPointProcessReports(optType, processRetVal, _TourId, gamedayId, matchday);
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.BackgroundServices.PointsCalculation.AbandonPointProcessReports: " + ex.Message);
            }

            return ds;
        }

        public StringBuilder ParseReports(DataSet ds)
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
                throw new Exception("Blanket.BackgroundServices.AbandonPointProcess.ParseReports: " + ex.Message);
            }

            return sb;
        }
    }
}
