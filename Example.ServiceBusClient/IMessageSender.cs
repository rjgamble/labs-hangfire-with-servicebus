
namespace Example.ServiceBusClient
{
    public interface IMessageSender
    {
        Task SendMessageAsync(string queueName, int tenantId, object data, string targetProcessorId);
    }
}