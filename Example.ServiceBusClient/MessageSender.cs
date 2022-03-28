namespace Example.ServiceBusClient;

public class MessageSender : IMessageSender
{
    private readonly Azure.Messaging.ServiceBus.ServiceBusClient _serviceBusClient;

    public MessageSender(Azure.Messaging.ServiceBus.ServiceBusClient serviceBusClient)
    {
        _serviceBusClient = serviceBusClient;
    }

    public async Task SendMessageAsync(string queueName, int tenantId, object data, string targetProcessorId)
    {
        var sender = _serviceBusClient.CreateSender(queueName);

        await sender.SendMessageAsync(new Azure.Messaging.ServiceBus.ServiceBusMessage
        {
            To = tenantId.ToString(),
            Subject = targetProcessorId,
            Body = new BinaryData(data)
        });
    }
}
