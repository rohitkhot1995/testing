using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Gaming.Predictor.Contracts.Configuration;
using System.Threading.Tasks;

namespace Gaming.Predictor.Library.AWS.S3
{
    public partial class Broker : BaseAWS
    {
        private IAmazonS3 _Client;

        public Broker(IOptions<Application> appSettings) : base(appSettings)
        {
        }

        public async Task<String> Get(String fileName)
        {
            String content = "";
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
                        using (StreamReader sr = new StreamReader(amazonStream))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return content;
        }

        public async Task<byte[]> GetImage(String fileName)
        {
            byte[] content = null;
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
                        var buffer = new byte[16 * 1024];
                        int bytesRead = -1;

                        using (MemoryStream ms = new MemoryStream())
                        {
                            while ((bytesRead = amazonStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, bytesRead);
                                content = ms.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return content;
        }
    }
}
