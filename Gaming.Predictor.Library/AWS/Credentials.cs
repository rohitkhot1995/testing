using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace Gaming.Predictor.Library.AWS
{
    public class Credentials
    {
        //public static Amazon.Runtime.AWSCredentials _AWSCredentials
        //{
        //    get { return new Amazon.Runtime.BasicAWSCredentials(); }
        //}
        public static AWSCredentials _AWSCredentials
        {
            get
            {
                var chain = new CredentialProfileStoreChain();
                AWSCredentials awsCredentials;
                //read the below profile name from appsettings
                
                chain.TryGetAWSCredentials("asim", out awsCredentials);

                return awsCredentials;
            }
        }
    }
}