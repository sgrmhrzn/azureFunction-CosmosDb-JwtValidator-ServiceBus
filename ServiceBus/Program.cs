using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceBus;

namespace ServiceBus
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://mynewsbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ymSmx8fo6S0qqc+XrKnoKWbrjbP1ymqMmeLHn9TQRC0=";
        const string QueueName = "myqueue";
        const string TopicName = "mytopic";
        static IQueueClient queueClient;
        static ITopicClient topicClient;


        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            //await QueueConcept();
            await TopicConcept();
        }
        static async Task SendMessagesAsync(int numberOfMessagesToSend, string client)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the queue.
                    string messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending message: {messageBody}");

                    switch (client)
                    {
                        case "QueueClient":
                            {
                                // Send the message to the queue.
                                await queueClient.SendAsync(message);
                                break;
                            }
                        case "TopicClient":
                            {
                                // Send the message to the queue.
                                await topicClient.SendAsync(message);
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("No Client");
                                break;
                            }
                    }

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
        static async Task TopicConcept()
        {
            try
            {
                const int numberOfMessages = 10;

                topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

                Console.WriteLine("======================================================");
                Console.WriteLine("Press ENTER key to exit after sending all the messages.");
                Console.WriteLine("======================================================");
                //
                var nsm = NamespaceManager.CreateFromConnectionString(ServiceBusConnectionString);
                if (!nsm.SubscriptionExists(TopicName, "AllMessages"))
                {
                    nsm.CreateSubscription(TopicName, "AllMessages");
                }
                // Send messages.
                await SendMessagesAsync(numberOfMessages, "TopicClient");

                Console.ReadKey();

                await topicClient.CloseAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

        static async Task QueueConcept()
        {
            const int numberOfMessages = 10;
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);



            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after sending all the messages.");
            Console.WriteLine("======================================================");

            // Send messages.
            await SendMessagesAsync(numberOfMessages, "QueueClient");

            Console.ReadKey();

            await queueClient.CloseAsync();
        }
    }
}
