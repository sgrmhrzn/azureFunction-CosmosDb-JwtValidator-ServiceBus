using Microsoft.AspNet.SignalR.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace FunctionApp4
{
    public class Function1
    {
        private static string _signalRNotificationHubUrl = Environment.GetEnvironmentVariable("SignalRNotificationHubUrl");
        private static HubConnection _hub = new HubConnection(_signalRNotificationHubUrl);
        private static IHubProxy _proxy;

        [FunctionName("Function1")]
        public async Task Run([EventHubTrigger("MyHub", Connection = "EhConnectionString")]string myEventHubMessage, TraceWriter log)
        {
            log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");

            log.Info(myEventHubMessage);

            // drop message in signal R (deserializedMessage.ConversationId, deserializedMessage.Message, agentConnectionId);
            log.Info(Convert.ToString(_hub.State));
            if (_hub.State != ConnectionState.Connected)
            {
                _proxy = _hub.CreateHubProxy("ConversationHub");
                await _hub.Start();
            }
            await _proxy.Invoke("Send", "1");
        }


        public class EventHubMessage
        {
            public string ConversationId { get; set; }
            public string Message { get; set; }
            public string ChannelId { get; set; }
        }


        public class ConversationDetails
        {
            public string ConversationId { get; set; }
            public string AgentId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
    }
}
