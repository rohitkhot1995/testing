using System;
using System.Collections.Generic;

namespace Gaming.Predictor.Contracts.Configuration
{
    public class Application
    {
        public Properties Properties { get; set; }
        public Connection Connection { get; set; }
        public SMTP SMTP { get; set; }
        public Cookies Cookies { get; set; }

        public API API { get; set; }
        public Admin Admin { get; set; }

        public Redirect Redirect { get; set; }
        public CustomSwaggerConfig CustomSwaggerConfig { get; set; }
    }

    #region "Children "

    public class CustomSwaggerConfig
    {
        public String BasePath { get; set; }
    }

    public class Redirect
    {
        public String PreLogin { get; set; }
        public String PostLogin { get; set; }
        public String ProfileIncomplete { get; set; }
    }

    public class Properties
    {
        public Int32 TourId { get; set; }

        //public List<TourId> TourId { get; set; }

        public String ClientName { get; set; }
        public Int32 ClientId { get; set; }
        public List<String> Languages { get; set; }
        public Int32 OptionCount { get; set; }
        public String WAFProfileUrl { get; set; }
        public String WAFAuthKey { get; set; }
        public String WAFUserAgent { get; set; }
        public Int32 TeamId { get; set; }
        public String StaticAssetBasePath { get; set; }
        public Int32 ShowPointProcessScreen { get; set; }
        public Int32 ShowMatchAbandonScreen { get; set; }
        public Int32 QuestionMultiplierEnabled { get; set; }
        public Int32 IsLivePredictor { get; set; }
        public Int32 TermsCondition { get; set; }
        public Int32 PrivacyPolicy { get; set; }
        public Int32 CookiePolicy { get; set; }

    }
    public class TourId
    {
        public Int32 Sport_Id { get; set; }
        public Int32 Tour_Id { get; set; }
    }

    public class Connection
    {
        public String Environment { get; set; }
        public AWSConfig AWS { get; set; }
        public Redis Redis { get; set; }
        public Postgre Postgre { get; set; }
    }

    public class AWSConfig
    {
        public SQS SQS { get; set; }
        public String S3Bucket { get; set; }
        public String S3FolderPath { get; set; }
        public bool Apply { get; set; }
        public bool UseCredentials { get; set; }
        public String IOSARN { get; set; }
        public String AndroidARN { get; set; }
        public String ARN { get; set; }
        public string ClinetS3Bucket { get; set; }
    }
    public class SQS
    {
        public String NotificationQueueUrl { get; set; }
        public String EventsQueueUrl { get; set; }
        public String TrackingQueueUrl { get; set; }
        public String ServiceUrl { get; set; }
    }
    public class Redis
    {
        public String Server { get; set; }
        public Int32 Port { get; set; }
        public bool Apply { get; set; }
    }

    public class Postgre
    {
        public String Host { get; set; }
        public String HostMOL { get; set; }
        public String Schema { get; set; }
        public bool Pooling { get; set; }
        public Int32 MinPoolSize { get; set; }
        public Int32 MaxPoolSize { get; set; }
    }

    public class SMTP
    {
        public String Host { get; set; }
        public Int32 Port { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
    }

    public class Cookies
    {
        public String WAFURCCookie { get; set; }
        public String WAFUSCCookie { get; set; }
        public Int32 ExpiryDays { get; set; }
        public String Domain { get; set; }
    }

    #endregion "Children "
}