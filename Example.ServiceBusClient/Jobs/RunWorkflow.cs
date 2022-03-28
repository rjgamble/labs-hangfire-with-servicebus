using Microsoft.Extensions.Logging;

namespace Example.ServiceBusClient.Jobs;

public class RunWorkflow : IRunWorkflow
{
    private readonly ILogger<RunWorkflow> _logger;

    public RunWorkflow(ILogger<RunWorkflow> logger)
    {
        _logger = logger;
    }

    public Task Run(int tenantId, object triggerData, string processorId)
    {
        _logger.LogInformation("Running workflow with tenant {@tenantId} and processor {@processorId}", tenantId, processorId);
        _logger.LogDebug("Trigger data {@triggerData}", triggerData);
        return Task.CompletedTask;
    }
}
