using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Library.Utility;

namespace Gaming.Predictor.Library.FileSystem
{
    public partial class Broker
    {
        public async Task<bool> Set(String filePath, Object content, bool serialize)
        {
            bool success = false;

            CreateDirectory(filePath);

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    String val = serialize ? GenericFunctions.Serialize(content) : content.ToString();
                    await streamWriter.WriteLineAsync(val);
                }

                success = true;
            }
            catch { }

            return success;
        }

        public async Task Log(HTTPLog logMessage, String logPath)
        {
            try
            {
                if (Has(logPath))
                {
                    List<HTTPLog> logs = new List<HTTPLog>();
                    String existingLog = await Get(logPath);

                    if (!String.IsNullOrEmpty(existingLog))
                        logs = ParseLogs(logMessage, GenericFunctions.Deserialize<List<HTTPLog>>(existingLog));

                    if (logs != null)
                        await Set(logPath, logs, true);
                }
                else
                    await Set(logPath, ParseLogs(logMessage, null), true);
            }
            catch { }
        }

        #region " Helper "

        private List<HTTPLog> ParseLogs(HTTPLog newLog, List<HTTPLog> existingLog)
        {
            List<HTTPLog> newRange = new List<HTTPLog>();

            if (existingLog != null)
                newRange.AddRange(existingLog);

            newRange.Add(newLog);

            return newRange;
        }

        private void CreateDirectory(String fullAbsolutePath)
        {
            /* To create a directory if it doesn't exist */
            String folderPath = Path.GetDirectoryName(fullAbsolutePath);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            /* To create a directory if it doesn't exist */
        }

        #endregion
    }
}
