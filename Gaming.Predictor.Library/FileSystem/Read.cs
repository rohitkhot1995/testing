using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Gaming.Predictor.Library.FileSystem
{
    public partial class Broker
    {
        public async Task<String> Get(String filePath)
        {
            String content = "";

            try
            {
                if (File.Exists(filePath))
                {
                    using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
                    {
                        content = await sr.ReadToEndAsync();
                    }
                }
            }
            catch { }

            return content;
        }
    }
}
