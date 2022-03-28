namespace Example.ServiceBusClient.Jobs;

public interface IRunWorkflow
{
    Task Run(int tenantId, object triggerData, string processorId);
}
