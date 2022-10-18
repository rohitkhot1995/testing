using Amazon;
using Amazon.EC2;
using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Contracts.Configuration;
using System.Threading.Tasks;

namespace Gaming.Predictor.Library.AWS.EC2
{
    public class Instance : SQS.Queue
    {
        //private readonly Cache _Cache;
        private IAmazonEC2 _Client;

        public Instance(IOptions<Application> appSettings) : base(appSettings)
        {
            //_Cache = new Cache(cache);
        }

        public async Task<String> Environment()
        {
            String environment = String.Empty;

            try
            {
                //AmazonEC2Config config = new AmazonEC2Config();
                //config.RegionEndpoint = _RegionEndpoint;

                //AmazonEC2Client client = new AmazonEC2Client(config);

                using (_Client = EC2Client())
                {
                    var request = new Amazon.EC2.Model.DescribeInstancesRequest();
                    var response = await _Client.DescribeInstancesAsync(request);

                    foreach (var ec2 in response.Reservations)
                    {
                        foreach (var instance in ec2.Instances)
                        {
                            if (instance.InstanceId == Amazon.Util.EC2InstanceMetadata.InstanceId)
                            {
                                foreach (Amazon.EC2.Model.Tag instanceTag in instance.Tags)
                                {
                                    if (instanceTag.Key.ToLower().Trim() == "environment")
                                        environment = instanceTag.Value.ToLower().Trim();
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            //if (!String.IsNullOrEmpty(environment))
            //    _Cache.CacheTrySet(environment);

            return environment;
        }
    }
}
