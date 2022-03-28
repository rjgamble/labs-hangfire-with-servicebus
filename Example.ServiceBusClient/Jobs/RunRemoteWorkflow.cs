using Microsoft.Extensions.Logging;

namespace Example.ServiceBusClient.Jobs;

public class RunRemoteWorkflow : IRunRemoteWorkflow
{
    private readonly IMessageSender _messageSender;
    private readonly ILogger<RunRemoteWorkflow> _logger;

    public RunRemoteWorkflow(IMessageSender messageSender, 
        ILogger<RunRemoteWorkflow> logger)
    {
        _messageSender = messageSender;
        _logger = logger;
    }

    public async Task Run(string queueName, int tenantId, object message, string processorId)
    {
        _logger.LogInformation("Triggering remote workflow execution for tenant {@tenantId} and processor {@processorId}", tenantId, processorId);
        await _messageSender.SendMessageAsync(queueName, tenantId, message, processorId);
    }
}
