using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace EventHubReceiverFunction
{
    public static class EventHub
    {
        private static EventHubClient eventHubClient;
        private const string EhConnectionString = "Endpoint=sb://bpcci2eventhub-dev.servicebus.windows.net/;SharedAccessKeyName=EhConnectionString;SharedAccessKey=gafRDSkuDjFHlXpnLA+5NkAbPOKAblDLRKCaOFc5OQA=;EntityPath=bpcci2eventhub";
        private const string EhEntityPath = "bpcci2eventhub";

        [FunctionName("EventHub")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EhConnectionString)
            {
                EntityPath = EhEntityPath
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            log.Info("C# HTTP trigger function processed a request.");

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        //Creates an event hub client 
        //public static async Task SendMessagesToEventHub(string message)
        //{

        //    try
        //    {
        //        await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine(exception.Message);
               
        //    }

        //    await Task.Delay(10);

        //    // await eventHubClient.CloseAsync();

        //    Console.WriteLine($"{message} messages sent.");
        //}


    }
}
