using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Threading.Tasks;

namespace Gaming.Predictor.Library.AWS.S3
{
    public partial class Broker
    {
        public async Task<bool> Remove(String fileName)
        {
            bool success = false;
            String key = _AWSS3FolderPath + fileName;

            try
            {
                using (_Client = S3Client())
                {
                    try
                    {
                        DeleteObjectRequest request = new DeleteObjectRequest
                        {
                            BucketName = _AWSS3Bucket,
                            Key = key
                        };

                        var response = await _Client.DeleteObjectAsync(request);

                        if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                            success = true;
                    }
                    catch (AmazonS3Exception ex)
                    {
                        if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                            success = true;
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode.Equals("NoSuchKey"))
                    success = true;
            }

            return success;
        }
    }
}
