using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.Queue
{
    public static class QueueSender
    {
        private static string _namespaceConnectionString = "<inform-the-namespace-connection-string>";
        private static string _queue = "<inform-the-queue-name>";

        private static int _numberOfMessages = 10;

        public static async Task SendMessages()
        {
            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            var serviceBusClient = new ServiceBusClient(_namespaceConnectionString, clientOptions);
            var serviceBusSender = serviceBusClient.CreateSender(_queue);

            using ServiceBusMessageBatch messageBatch = await serviceBusSender.CreateMessageBatchAsync();

            for (int i = 1; i <= _numberOfMessages; i++)
            {
                // This Try is used to check if the size of the batch with the new message is too large, if so, it will throw an exception;
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }

            try
            {
                await serviceBusSender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {_numberOfMessages} messages has been published to the queue.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await serviceBusSender.DisposeAsync();
                await serviceBusClient.DisposeAsync();
            }
        }

        public static async Task SendMessagesOneByOne()
        {
            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            var serviceBusClient = new ServiceBusClient(_namespaceConnectionString, clientOptions);
            var serviceBusSender = serviceBusClient.CreateSender(_queue);

            try
            {
                for (int i = 1; i <= _numberOfMessages; i++)
                {
                    await serviceBusSender.SendMessageAsync(new ServiceBusMessage($"Message {i}"));
                    Console.WriteLine($"Sent: Message {i}");
                }
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await serviceBusSender.DisposeAsync();
                await serviceBusClient.DisposeAsync();
            }
        }
    }
}