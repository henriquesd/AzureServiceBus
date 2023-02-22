using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.Topic
{
    public static class TopicSender
    {
        private static string _namespaceConnectionString = "<inform-the-namespace-connection-string>";
        private static string _topicName = "<inform-the-topic-name>";

        private static int _numberOfMessages = 10;

        public static async Task SendMessages()
        {
            // The Service Bus client types are safe to cache and use as a singleton for the lifetime
            // of the application, which is best practice when messages are being published or read regularly;
            var serviceBusClient = new ServiceBusClient(_namespaceConnectionString);

            var serviceBusSender = serviceBusClient.CreateSender(_topicName);

            using ServiceBusMessageBatch messageBatch = await serviceBusSender.CreateMessageBatchAsync();

            for (int i = 1; i <= _numberOfMessages; i++)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    // throw an Exception in case it is too large for the batch;
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }

            try
            {
                await serviceBusSender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {_numberOfMessages} messages has been published to the topic.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up;
                await serviceBusSender.DisposeAsync();
                await serviceBusClient.DisposeAsync();
            }

            Console.WriteLine("Press any key to end the application");
            Console.ReadKey();
        }
    }
}