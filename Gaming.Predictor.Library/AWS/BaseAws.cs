using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Options;
using Amazon.SimpleEmail;
using Amazon.EC2;
using Amazon.SQS;
using Amazon.SimpleNotificationService;

namespace Gaming.Predictor.Library.AWS
{
    public class BaseAWS
    {
        protected string _AWSS3Bucket;
        protected string _ClinetAWSS3Bucket;

        protected string _AWSS3FolderPath;
        protected Contracts.Configuration.SQS _SQSProperties;
        protected RegionEndpoint _AWSS3Region;
        protected RegionEndpoint _AWSSESRegion;
        protected RegionEndpoint _AWSEC2Region;
        protected RegionEndpoint _AWSSNSRegion;
        protected string _AWSSNSArs;


        private bool _UseCredentials;

        public BaseAWS(IOptions<Contracts.Configuration.Application> appSettings)
        {
            _AWSS3Bucket = appSettings.Value.Connection.AWS.S3Bucket;
            _AWSS3FolderPath = appSettings.Value.Connection.AWS.S3FolderPath;
            _SQSProperties = appSettings.Value.Connection.AWS.SQS;
            _AWSS3Region = RegionEndpoint.USEast1;
            _AWSSESRegion = RegionEndpoint.USEast1;
            _AWSEC2Region = RegionEndpoint.EUCentral1;
            _AWSSNSRegion = RegionEndpoint.USEast1;
            _UseCredentials = appSettings.Value.Connection.AWS.UseCredentials;
            _AWSSNSArs = appSettings.Value.Connection.AWS.ARN;
            _ClinetAWSS3Bucket = appSettings.Value.Connection.AWS.ClinetS3Bucket;
        }

        public IAmazonS3 S3Client()
        {
            if (_UseCredentials)
                return new AmazonS3Client(Credentials._AWSCredentials, _AWSS3Region);
            else
                return new AmazonS3Client(_AWSS3Region);
        }

        public AmazonSimpleEmailServiceClient SESClient()
        {
            if (_UseCredentials)
                return new AmazonSimpleEmailServiceClient(Credentials._AWSCredentials, _AWSSESRegion);
            else
                return new AmazonSimpleEmailServiceClient(_AWSSESRegion);
        }

        public AmazonSimpleNotificationServiceClient SNSClient()
        {
            if (_UseCredentials)
                return new AmazonSimpleNotificationServiceClient(Credentials._AWSCredentials, _AWSSNSRegion);
            else
                return new AmazonSimpleNotificationServiceClient(_AWSSNSRegion);
        }

        public AmazonEC2Client EC2Client()
        {
            if (_UseCredentials)
                return new AmazonEC2Client(Credentials._AWSCredentials, _AWSEC2Region);
            else
                return new AmazonEC2Client(_AWSEC2Region);
        }

        public AmazonSQSClient SQSClient(AmazonSQSConfig sqsConfig)
        {
            if (_UseCredentials)
                return new AmazonSQSClient(Credentials._AWSCredentials, sqsConfig);
            else
                return new AmazonSQSClient(sqsConfig);
        }
    }
}
