﻿using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.Queue
{
    public static class QueueReceiver
    {
        private static string _namespaceConnectionString = "<inform-the-namespace-connection-string>";
        private static string _queueName = "<inform-the-queue-name>";

        public static async Task ReceiveMessages()
        {
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            var serviceBusClient = new ServiceBusClient(_namespaceConnectionString, clientOptions);

            var serviceBusProcessor = serviceBusClient.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

            try
            {
                // Add handler to process messages;
                serviceBusProcessor.ProcessMessageAsync += MessageHandler;

                // Add handler to process any errors;
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
                // resources and other unmanaged objects are properly cleaned up;
                await serviceBusProcessor.DisposeAsync();
                await serviceBusClient.DisposeAsync();
            }
        }

        // Handle received messages;
        private static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            // Complete the message, and message is deleted from the queue;
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