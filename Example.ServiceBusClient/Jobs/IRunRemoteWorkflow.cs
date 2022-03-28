namespace Example.ServiceBusClient.Jobs;

public interface IRunRemoteWorkflow
{
    Task Run(string queueName, int tenantId, object triggerData, string processorId);
}
