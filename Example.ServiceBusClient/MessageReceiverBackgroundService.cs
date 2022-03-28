using Azure.Messaging.ServiceBus;
using Example.ServiceBusClient.Jobs;
using Hangfire;
using Microsoft.Extensions.Hosting;

namespace Example.ServiceBusClient;

public class MessageReceiverBackgroundService : BackgroundService
{
    private readonly Azure.Messaging.ServiceBus.ServiceBusClient _serviceBusClient;
    private readonly string _listeningQueueName;
    private ServiceBusProcessor? _serviceBusProcessor;

    public MessageReceiverBackgroundService(Azure.Messaging.ServiceBus.ServiceBusClient serviceBusClient, 
        string listeningQueueName)
    {
        _serviceBusClient = serviceBusClient;
        _listeningQueueName = listeningQueueName;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _serviceBusProcessor = _serviceBusClient.CreateProcessor(_listeningQueueName);
        _serviceBusProcessor.ProcessMessageAsync += ProcessMessageReceivedAsync;
        _serviceBusProcessor.ProcessErrorAsync += ProcessMessageErrorAsync;

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_serviceBusProcessor?.IsProcessing ?? false)
        {
            await _serviceBusProcessor.StopProcessingAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _serviceBusProcessor!.StartProcessingAsync(stoppingToken);
    }

    public static async Task ProcessMessageReceivedAsync(ProcessMessageEventArgs args)
    {
        var tenantId = int.Parse(args.Message.To);
        var processorId = args.Message.Subject;
        var triggerData = args.Message.Body.ToObjectFromJson<TriggerWorkflowMessage>();

        BackgroundJob.Enqueue<IRunWorkflow>(job => job.Run(tenantId, triggerData, processorId));

        await args.CompleteMessageAsync(args.Message);
    }

    public static Task ProcessMessageErrorAsync(ProcessErrorEventArgs args)
    {
        return Task.CompletedTask;
    }
}
