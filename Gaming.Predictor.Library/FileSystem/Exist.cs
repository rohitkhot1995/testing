using System;
using System.IO;

namespace Gaming.Predictor.Library.FileSystem
{
    public partial class Broker
    {
        public bool Has(String filePath)
        {
            bool success = false;

            try
            {
                success = File.Exists(filePath);
            }
            catch { }

            return success;
        }
    }
}
