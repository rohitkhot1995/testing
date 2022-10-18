using Amazon.SQS;
using System;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Library.Utility;
using System.Threading.Tasks;
using Amazon.SQS.Model;

namespace Gaming.Predictor.Library.AWS.SQS
{
    public class Queue : S3.Broker
    {
        private readonly AmazonSQSClient _Client;
        private readonly AmazonSQSConfig _Config;

        public Queue(IOptions<Application> appSettings) : base(appSettings)
        {
            _Config = new AmazonSQSConfig();
            _Config.ServiceURL = _SQSProperties.ServiceUrl;
            _Client = SQSClient(_Config);
        }




        public async Task<String> ReadMessage(String queueURL)
        {
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();

            receiveMessageRequest.QueueUrl = queueURL;

            ReceiveMessageResponse receiveMessageResponse = await _Client.ReceiveMessageAsync(receiveMessageRequest);

            return GenericFunctions.Serialize(receiveMessageResponse);
        }

        public async Task<String> DeleteMessage(String receiptHandle, String queueURL)
        {
            DeleteMessageRequest deleteMessageRequest = new DeleteMessageRequest();

            deleteMessageRequest.QueueUrl = queueURL;
            deleteMessageRequest.ReceiptHandle = receiptHandle;

            DeleteMessageResponse response = await _Client.DeleteMessageAsync(deleteMessageRequest);

            return GenericFunctions.Serialize(response);
        }
    }
}
