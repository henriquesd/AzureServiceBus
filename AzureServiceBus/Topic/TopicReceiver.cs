using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.Topic
{
    public static class TopicReceiver
    {
        private static string _namespaceConnectionString = "<inform-the-namespace-connection-string>";
        private static string _topic = "<inform-the-topic-name>";
        private static string _subscriptionName = "<inform-the-subscription-name>";

        public static async Task ReceiveMessages()
        {
            // The Service Bus client types are safe to cache and use as a singleton for the lifetime
            // of the application, which is best practice when messages are being published or read regularly;
            var serviceBusClient = new ServiceBusClient(_namespaceConnectionString);

            var serviceBusProcessor = serviceBusClient.CreateProcessor(_topic, _subscriptionName, new ServiceBusProcessorOptions());

            try
            {
                // Add handler to process messages
                serviceBusProcessor.ProcessMessageAsync += MessageHandler;

                // Add handler to process any errors
                serviceBusProcessor.ProcessErrorAsync += ErrorHandler;

                await serviceBusProcessor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

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

        // Handle received messages;
        private static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body} from subscription: {_subscriptionName}");

            // Complete the message, and messages are deleted from the subscription;
            await args.CompleteMessageAsync(args.Message);
        }

        // Handle any errors when receiving messages;
        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}