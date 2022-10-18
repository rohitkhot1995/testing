using System;


namespace Gaming.Predictor.Library.Utility
{
    public class TimeFunction
    {
        public static DateTime UTCtoIST(DateTime UtcTime)
        {
            //TimeZoneInfo otherTimezone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            //return Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(UtcTime, otherTimezone));
            return UtcTime.AddHours(5).AddMinutes(30);
        }

        //public static DateTime UTCtoGMT(DateTime UtcTime)
        //{
        //    //TimeZoneInfo otherTimezone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    //return Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(UtcTime, otherTimezone));
        //    return UtcTime.AddHours(5).AddMinutes(30);
        //}

        public static DateTime UTCtoCEST(DateTime UtcTime)
        {
            TimeZoneInfo easternTimezone;
            DateTime dt;

            try
            {
                //Time zone as listed in 'tz' database.
                //Works on Linux platform
                easternTimezone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
                dt = Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(UtcTime, easternTimezone));
            }
            catch
            {
                try
                {
                    //Works on Windows platform
                    easternTimezone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
                    dt = Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(UtcTime, easternTimezone));
                }
                catch
                {
                    try
                    {
                        //Works on Windows platform
                        easternTimezone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                        dt = Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(UtcTime, easternTimezone));
                    }
                    catch
                    {
                        //Worst case fix
                        dt = UtcTime.AddHours(2).AddMinutes(0);//When DST is Active
                    }
                }
            }

            return dt;
        }

        public static String CurrentUTCtime()
        {
            return DateTime.UtcNow.ToString(new System.Globalization.CultureInfo("en-US"));
        }

        public static String CurrentISTtime()
        {
            return UTCtoIST(DateTime.UtcNow).ToString(new System.Globalization.CultureInfo("en-US"));
        }

        public static String CurrentCESTtime()
        {
            return UTCtoCEST(DateTime.UtcNow).ToString(new System.Globalization.CultureInfo("en-US"));
        }

        #region " Service Timer "

        public static double RunMilliseconds(Int32 time)
        {
            DateTime cestTime = UTCtoCEST(DateTime.UtcNow);
            Int32 days = 0;

            if (cestTime.Hour > time || (cestTime.Hour == time && (cestTime.Minute > 0 || cestTime.Second > 0)))
                days = 1;

            DateTime runTime = new DateTime(cestTime.Year, cestTime.Month, cestTime.Day, time, 0, 0).AddDays(days);

            double runms = runTime.Subtract(cestTime).TotalMilliseconds;

            if (runms < 0)
                runms = 5000;//Setting run in 5 seconds

            return runms;
        }

        #endregion

        #region " Conversion Functions "

        public static String StringToDateString(String value, String format = "")
        {
            DateTime mDateTime;

            if (format != "")
                mDateTime = DateTime.ParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture);
            else
                mDateTime = DateTime.Parse(value);

            return mDateTime.ToString("dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture); ;
        }

        public static String StringToTimeString(String value, String format = "")
        {
            DateTime mDateTime;

            if (format != "")
                mDateTime = DateTime.ParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture);
            else
                mDateTime = DateTime.Parse(value);

            return mDateTime.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture); ;
        }

        public static String StringToDateTimeString(String value, String format = "", String vCulture = "")
        {
            DateTime mDateTime;

            if (vCulture != "")
            {
                mDateTime = Convert.ToDateTime(value, new System.Globalization.CultureInfo("en-GB"));
            }
            else
            {
                if (format != "")
                    mDateTime = DateTime.ParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture);
                else
                    mDateTime = DateTime.Parse(value);
            }

            return mDateTime.ToString("dd-MMM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static String ConvertToUniversalFormat(String vDate)
        {
            String mDateStr = String.Empty;
            DateTime mDate = DateTime.Parse(vDate, System.Globalization.CultureInfo.InvariantCulture);

            mDateStr = mDate.ToString("u").Replace(" ", "T");

            return mDateStr;
        }

        public static DateTime StringToDateTime(string date, string original_format)
        {
            try
            {
                return Convert.ToDateTime(DateTime.ParseExact(date, original_format, System.Globalization.CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy"));

            }
            catch
            {
                return Convert.ToDateTime(DateTime.ParseExact(date, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy"));

            }
        }
        public static DateTime StringToDate(string date, string original_format)
        {
            return Convert.ToDateTime(DateTime.ParseExact(date, original_format, System.Globalization.CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy"));
        }
        public static DateTime StringToDate(string date)
        {
            return Convert.ToDateTime(date);
        }



        #endregion
    }
}
