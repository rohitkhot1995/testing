using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Gaming.Predictor.Library.AWS.S3
{
    public partial class Broker
    {
        public async Task<bool> Has(String fileName)
        {
            bool has = false;
            String key = _AWSS3FolderPath + fileName;

            try
            {
                using (_Client = S3Client())
                {
                    GetObjectRequest request = new GetObjectRequest()
                    {
                        BucketName = _AWSS3Bucket,
                        Key = key
                    };

                    var response = await _Client.GetObjectAsync(request);

                    using (Stream amazonStream = response.ResponseStream)
                    {
                        if (amazonStream != null && amazonStream.Length != 0)
                            has = true;
                    }
                }
            }
            catch { }

            return has;
        }
    }
}
