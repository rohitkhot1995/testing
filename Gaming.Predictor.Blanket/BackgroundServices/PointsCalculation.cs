using Gaming.Predictor.Contracts.Admin;
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

namespace Gaming.Predictor.Blanket.BackgroundServices
{
    public class PointsCalculation : Common.BaseServiceBlanket
    {
        private readonly DataAccess.BackgroundServices.PointsCalculation _DBContext;
        private readonly Int32 _TourId;

        public PointsCalculation(IOptions<Application> appSettings, IOptions<Daemon> serviceSettings, IAWS aws, IPostgre postgre, IRedis redis,
            ICookies cookies, IAsset asset) : base(appSettings, serviceSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.BackgroundServices.PointsCalculation(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public Matchdays Matchdays()
        {
            Int32 optType = 1;

            Matchdays matchdays = new Matchdays();

            return _DBContext.Matchdays(optType, _TourId);
        }

        public MOLPointTrigger GetMOLPointTrigger()
        {
            HTTPMeta meta = new HTTPMeta();
            int optType = 1;
            MOLPointTrigger trigger = new MOLPointTrigger();
            try
            {
                trigger = _DBContext.GetMOLPointTrigger(optType, _TourId, ref meta);
            }
            catch (Exception ex)
            {
                throw new Exception("Blanket.BackgroundServces.PointsCalculation.GetMOLPointTrigger: " + ex.Message);
            }
            return trigger;
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
                throw new Exception("Engine.BackgroundServices.PointsCalculation.UserPointsProcess: " + ex.Message);
            }

            return retVal;
        }

        public Int32 GetMOLPointProcess(int gamedayId, int weekId)
        {
            HTTPMeta meta = new HTTPMeta();
            int optType = 1;
            int retVal = -40;
            try
            {
                retVal = _DBContext.GetMOLPointProcess(optType, _TourId, gamedayId, weekId);
            }
            catch (Exception ex)
            {
                throw new Exception("Blanket.BackgroundServces.PointsCalculation.GetMOLPointProcess: " + ex.Message);
            }
            return retVal;
        }

        public Int32 GetCombinePointProcess(int gamedayId, int weekId)
        {
            HTTPMeta meta = new HTTPMeta();
            int optType = 1;
            int retVal = -40;
            try
            {
                retVal = _DBContext.UserPointsProcessCombine(optType, _TourId, _TourId, gamedayId, weekId);
            }
            catch (Exception ex)
            {
                throw new Exception("Blanket.BackgroundServces.PointsCalculation.GetCombinePointProcess: " + ex.Message);
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
                throw new Exception("Engine.BackgroundServices.PointsCalculation.UserPointsProcessReports: " + ex.Message);
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
                throw new Exception("Blanket.BackgroundServices.PointsCalculation.ParseReports: " + ex.Message);
            }

            return sb;
        }

        public bool UserPointsProcessMatchdayUpdated(Int32 matchid)
        {
            Int32 retVal = new Int32();

            try
            {
                Int32 optType = 1;

                retVal = _DBContext.UserPointsProcessMatchdayUpdated(optType, _TourId, matchid);
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.BackgroundServices.PointsCalculation.UserPointsProcessMatchdayUpdated: " + ex.Message);
            }

            return retVal == 1 ? true : false;
        }

    }
}
