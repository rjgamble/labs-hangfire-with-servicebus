using Example.ServiceBusClient;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configBuilder =>
    {
        var configuration = configBuilder.AddJsonFile("appsettings.json", false)
                   .AddJsonFile($"appsettings.development.json", true)
                   .AddCommandLine(args)
                   .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithThreadId()
            .CreateLogger();
    })
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {
        services.AddHangfire((services, globalConfig) =>
        {
            var configuration = services.GetService(typeof(IConfiguration)) as IConfiguration;

            globalConfig.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    PrepareSchemaIfNecessary = true
                })
                .UseThrottling()
                .UseConsole(new ConsoleOptions { FollowJobRetentionPolicy = true })
                .UseColouredConsoleLogProvider();
        }).AddHangfireConsoleExtensions();

        services.AddHangfireServer();

        services.AddServiceBusDependencies();
    });

var host = builder.Build();

host.Run();