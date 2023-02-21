using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.Queue
{
    public static class QueueReceiver
    {
        private static string _namespaceConnectionString = "<inform-the-namespace-connection-string>";
        private static string _queue = "<inform-the-queue-name>";

        public static async Task ReceiveMessages()
        {
            // handle received messages
            async Task MessageHandler(ProcessMessageEventArgs args)
            {
                string body = args.Message.Body.ToString();
                Console.WriteLine($"Received: {body}");

                // Complete the message, and message is deleted from the queue;
                await args.CompleteMessageAsync(args.Message);
            }

            // handle any errors when receiving messages
            Task ErrorHandler(ProcessErrorEventArgs args)
            {
                Console.WriteLine(args.Exception.ToString());
                return Task.CompletedTask;
            }

            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            var serviceBusClient = new ServiceBusClient(_namespaceConnectionString, clientOptions);
            var serviceBusProcessor = serviceBusClient.CreateProcessor(_queue, new ServiceBusProcessorOptions());

            try
            {
                // add handler to process messages
                serviceBusProcessor.ProcessMessageAsync += MessageHandler;

                // add handler to process any errors
                serviceBusProcessor.ProcessErrorAsync += ErrorHandler;

                await serviceBusProcessor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                // stop processing 
                Console.WriteLine("\nStopping the receiver...");
                await serviceBusProcessor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await serviceBusProcessor.DisposeAsync();
                await serviceBusClient.DisposeAsync();
            }
        }
    }
}
