using AzureServiceBus.Queue;

Console.WriteLine("Hello Service Bus!");

await QueueSender.SendMessages();
//await QueueSender.SendMessagesOneByOne();
await QueueReceiver.ReceiveMessages();