using Example.ServiceBusClient;
using Example.ServiceBusClient.Jobs;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.SqlServer;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithThreadId()
    .CreateLogger();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddTransient((serviceProvider) =>
{
    var configuration = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
    var serviceBusConnectionString = configuration.GetConnectionString("ServiceBusConnection");

    return new Azure.Messaging.ServiceBus.ServiceBusClient(serviceBusConnectionString);
});
builder.Services.AddTransient<IMessageSender, MessageSender>();

builder.Services.AddHangfire((services, options) =>
{
    var config = services.GetRequiredService<IConfiguration>();

    options.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(config.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true,
            PrepareSchemaIfNecessary = false,
        })
        .UseThrottling()
        .UseConsole(new ConsoleOptions
        {
            FollowJobRetentionPolicy = true,
        })
        .UseColouredConsoleLogProvider();
}).AddHangfireConsoleExtensions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard();

app.MapGet("/enqueue", (HttpContext context) =>
{
    var messageBody = new TriggerWorkflowMessage
    {
        TriggerType = "TriggerType enum value",
        TriggerData = "Trigger remote workflow from Client A"
    };

    var jobId = BackgroundJob.Enqueue<IRunRemoteWorkflow>(job => job.Run(QueueNames.ServerA, 11, messageBody, "MyKnownServerBV1Processor"));

    return new
    {
        JobId = jobId,
    };
})
.WithName("Enqueue");

app.Run();