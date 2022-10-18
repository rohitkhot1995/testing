using Gaming.Predictor.Contracts.Common;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Gaming.Predictor.Library.Utility
{
    public class GenericFunctions
    {
        public static String Serialize(object data, bool isCamelCase = false)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            if (isCamelCase)
                return Newtonsoft.Json.JsonConvert.SerializeObject(data, new Newtonsoft.Json.JsonSerializerSettings()
                {
                    MaxDepth = Int32.MaxValue,
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                });
            else
                return Newtonsoft.Json.JsonConvert.SerializeObject(data, new Newtonsoft.Json.JsonSerializerSettings() { MaxDepth = Int32.MaxValue });
        }

        public static T Deserialize<T>(String data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data, new Newtonsoft.Json.JsonSerializerSettings() { MaxDepth = Int32.MaxValue });
        }

        public static String GetWebData(String url)
        {
            string strRetVal = string.Empty;

            try
            {
                System.Net.HttpWebRequest mHttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

                mHttpWebRequest.Method = "GET";
                mHttpWebRequest.Timeout = 30000; // 30 Seconds
                mHttpWebRequest.KeepAlive = false;
                mHttpWebRequest.ProtocolVersion = System.Net.HttpVersion.Version10;
                mHttpWebRequest.Accept = "application/json";
                mHttpWebRequest.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;


                using (System.Net.WebResponse response = mHttpWebRequest.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        strRetVal = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex) { }

            return strRetVal;
        }

        public static String GetWebData(String url, Dictionary<String, String> headers = null)
        {
            string strRetVal = string.Empty;

            try
            {
                System.Net.HttpWebRequest mHttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

                mHttpWebRequest.Method = "GET";
                mHttpWebRequest.Timeout = 30000; // 30 Seconds
                mHttpWebRequest.KeepAlive = false;
                mHttpWebRequest.ProtocolVersion = System.Net.HttpVersion.Version10;
                mHttpWebRequest.Accept = "application/json";
                mHttpWebRequest.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;

                if (headers != null)
                {
                    foreach (String mHeader in headers.Keys)
                        mHttpWebRequest.Headers.Add(mHeader, headers[mHeader].ToString());
                }

                using (System.Net.WebResponse response = mHttpWebRequest.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        strRetVal = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex) { }

            return strRetVal;
        }

        public static String PostWebData(String url, String param)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";

            byte[] bytes = Encoding.UTF8.GetBytes(param);
            req.ContentLength = bytes.Length;
            Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);

            os.Close();

            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null) return "";
            StreamReader sr = new StreamReader(resp.GetResponseStream());

            string responsecontent = sr.ReadToEnd().Trim();
            return responsecontent;
        }

        public static DateTime ToUSCulture(String dateTime)
        {
            return Convert.ToDateTime(dateTime, new System.Globalization.CultureInfo("en-US"));
        }

        public static FeedTime GetFeedTime()
        {
            return new FeedTime()
            {
                UTCtime = TimeZone.CurrentUTCtime(),
                //CESTtime = TimeZone.CurrentCESTtime(),
                ISTtime = TimeZone.CurrentISTtime()
            };
        }

        public static void AssetMeta(Int64 retVal, ref HTTPMeta httpMeta, String message = "")
        {
            httpMeta.Success = (retVal == 1);
            httpMeta.RetVal = retVal;
            httpMeta.Message = !String.IsNullOrEmpty(message) ? message : (retVal == 1 ? "Success" : "Failed");
            httpMeta.Timestamp = GetFeedTime();
        }

        public static String TimeDifference(DateTime startTime, DateTime endTime)
        {
            TimeSpan span = endTime.Subtract(startTime);
            return String.Format("{0} minute, {1} seconds", span.Minutes, span.Seconds);
        }

        public static String DecryptedValue(String encryptedValue)
        {
            string val = "";

            try
            {
                if (!String.IsNullOrEmpty(encryptedValue))
                    val = Encryption.BaseDecrypt(encryptedValue);
            }
            catch
            {
                try
                {
                    val = BareEncryption.BaseDecrypt(encryptedValue);
                }
                catch { }
            }

            return val;
        }

        public static String DebugTable(System.Data.DataTable table)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--- cursor {" + table.TableName + "} ---");
            sb.Append(Environment.NewLine);
            int zeilen = table.Rows.Count;
            int spalten = table.Columns.Count;

            // Header
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string s = table.Columns[i].ToString();
                sb.Append(String.Format("{0,-20} | ", s));
            }
            sb.Append(Environment.NewLine);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append("---------------------|-");
            }
            sb.Append(Environment.NewLine);

            // Data
            for (int i = 0; i < zeilen; i++)
            {
                System.Data.DataRow row = table.Rows[i];

                for (int j = 0; j < spalten; j++)
                {
                    string s = row[j].ToString();
                    if (s.Length > 20) s = s.Substring(0, 17) + "...";
                    sb.Append(String.Format("{0,-20} | ", s));
                }
                sb.Append(Environment.NewLine);
            }
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append("---------------------|-");
            }
            sb.Append(Environment.NewLine);

            Debug.WriteLine(sb);//writes to Output window
            return sb.ToString();
        }

        public static String EmailBody(String service, String contents)
        {
            String body = String.Empty;

            body = "This is a system generated mail from " + service + " daemon service running on " + Environment.MachineName.ToUpper() + ".<br/><br/>";
            body += "The service invoked the " + service + " process.<br/><br/>";
            body += "" + contents + "<br/><br/>";
            body += "Thanks.";

            return body;
        }

        public static Int32[] GetPagePoints(Int32 pageOneChunk, Int32 pageChunk, Int32 pageNo)
        {
            Int32[] address = new Int32[2];

            Int32 mPageOneSize = pageOneChunk;
            Int32 mCurrPageSize = pageChunk;
            Int32 mPageNo = pageNo;

            Int32 mFrom = 0;
            Int32 mTo = 0;

            mTo = mPageOneSize + ((mPageNo - 1) * mCurrPageSize);
            if (mPageNo == 1)
                mFrom = mTo - mPageOneSize;
            else
                mFrom = mTo - mCurrPageSize;

            mFrom = mFrom + 1;

            address[0] = mFrom;
            address[1] = mTo;

            return address;
        }

        public static String MemberNotation(Int64 count)
        {
            String notation = count.ToString();

            //if (count > 9 && count < 100)
            //    notation = "9+";
            //else if (count > 99 && count < 1000)
            //    notation = "99+";
            //else if (count > 999 && count < 10000)
            if (count > 999 && count < 1999)
                notation = "1k+";
            if (count > 1999 && count < 2999)
                notation = "2k+";
            if (count > 2999 && count < 3999)
                notation = "3k+";
            if (count > 3999 && count < 4999)
                notation = "4k+";
            if (count > 4999 && count < 5999)
                notation = "5k+";
            if (count > 5999 && count < 6999)
                notation = "6k+";
            if (count > 6999 && count < 7999)
                notation = "7k+";
            if (count > 7999 && count < 8999)
                notation = "8k+";
            if (count > 8999 && count < 9999)
                notation = "9k+";
            else if (count > 9999 && count <= 10000)
                notation = "10k+";
            else if (count > 10000 && count <= 11000)
                notation = "11k+";
            else if (count > 11000 && count <= 12000)
                notation = "12k+";
            else if (count > 12000 && count <= 13000)
                notation = "13k+";
            else if (count > 13000 && count <= 14000)
                notation = "14k+";
            else if (count > 14000 && count <= 15000)
                notation = "15k+";
            else if (count > 15000 && count <= 16000)
                notation = "16k+";
            else if (count > 16000 && count <= 17000)
                notation = "17k+";
            else if (count > 17000 && count <= 18000)
                notation = "18k+";
            else if (count > 18000 && count <= 19000)
                notation = "19k+";
            else if (count > 19000 && count <= 20000)
                notation = "20k+";
            else if (count > 20000 && count <= 22000)
                notation = "21k+";
            else if (count > 21000 && count <= 22000)
                notation = "22k+";
            else if (count > 22000 && count <= 23000)
                notation = "23k+";
            else if (count > 23000 && count <= 24000)
                notation = "24k+";
            else if (count > 24000 && count <= 25000)
                notation = "25k+";
            else if (count > 25000 && count <= 26000)
                notation = "26k+";
            else if (count > 26000 && count <= 27000)
                notation = "27k+";
            else if (count > 27000 && count <= 28000)
                notation = "28k+";
            else if (count > 28000 && count <= 29000)
                notation = "29k+";
            else if (count > 29000 && count <= 30000)
                notation = "30k+";
            else if (count > 30000 && count <= 31000)
                notation = "31k+";
            else if (count > 31000 && count <= 32000)
                notation = "32k+";
            else if (count > 31000 && count <= 32000)
                notation = "32k+";
            else if (count > 32000 && count <= 33000)
                notation = "33k+";
            else if (count > 33000 && count <= 34000)
                notation = "34k+";
            else if (count > 34000 && count <= 35000)
                notation = "35k+";
            else if (count > 35000 && count <= 36000)
                notation = "36k+";
            else if (count > 36000 && count <= 37000)
                notation = "37k+";
            else if (count > 37000 && count <= 39000)
                notation = "39k+";
            else if (count > 39000 && count <= 40000)
                notation = "40k+";

            return notation;
        }
    }
}