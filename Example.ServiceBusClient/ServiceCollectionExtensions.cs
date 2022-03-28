using Example.ServiceBusClient.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Example.ServiceBusClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceBusDependencies(this IServiceCollection services)
    {
        services.AddTransient<IRunRemoteWorkflow, RunRemoteWorkflow>();
        services.AddTransient<IRunWorkflow, RunWorkflow>();

        services.AddSingleton((serviceProvider) =>
        {
            var configuration = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
            var serviceBusConnectionString = configuration.GetConnectionString("ServiceBusConnection");

            return new Azure.Messaging.ServiceBus.ServiceBusClient(serviceBusConnectionString);
        });

        services.AddTransient<IMessageSender, MessageSender>();

        services.AddHostedService((serviceProvider) =>
        {
            var configuration = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
            var serviceBusClient = serviceProvider.GetService(typeof(Azure.Messaging.ServiceBus.ServiceBusClient)) as Azure.Messaging.ServiceBus.ServiceBusClient;
            var listeningQueueName = configuration!["ServiceBusListeningQueueName"];

            return new MessageReceiverBackgroundService(serviceBusClient!, listeningQueueName);
        });

        return services;
    }
}
