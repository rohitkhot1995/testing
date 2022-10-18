using System;
using System.IO;

namespace Gaming.Predictor.Library.FileSystem
{
    public partial class Broker
    {
        public bool Remove(String filePath)
        {
            bool success = false;

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    success = true;
                }
            }
            catch { }

            return success;
        }
    }
}
