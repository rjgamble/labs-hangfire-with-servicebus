# Skills Lab: Remote Messaging With Hangfire & Azure Service Bus

In this skills lab I am exploring remote [Hangfire](https://docs.hangfire.io/en/latest/) job execution using a common pub/sub pattern with [Azure Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview).

## Prerequisites

In order to clone and run this project you must obtain the following:
* [Hangfire Ace](https://www.hangfire.io/ace/) licence with access to the private NuGet feed for throttling
* Access to a SQL Server instance with two databases on it, `hf_server_a` and `hf_server_b`
* Azure subscription with a [Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview) resource 
    * Make sure you copy the connection string
    * Setup the following Queues in the Service Bus Namespace: `servera` and `serverb`   
    ![Queues](./documentation-assets/service-bus-queues.png)

## 🚧👷‍♂️ Build & Run

1. Roll through each of the `ServerX` and `ClientX` projects and create an `appsettings.Development.json` file
    1. For the `ClientX` project settings, specify the following:   
    ```json
    {
        "ConnectionStrings": {
            "HangfireConnection": "Server={YOUR SQL SERVER};Database={hf_server_a|hf_server_b};Trusted_Connection=True;",
            "ServiceBusConnection": "{YOUR SERVICE BUS CONNECTION STRING}"
	    }
    }
    ```
    1. For the `ServerX` project settings, specify the following:   
    ```json
    {
        "ConnectionStrings": {
            "HangfireConnection": "Server={YOUR SQL SERVER};Database={hf_server_a|hf_server_b};Trusted_Connection=True;",
            "ServiceBusConnection": "{YOUR SERVICE BUS CONNECTION STRING}"
        },
        "ServiceBusListeningQueueName": "{servera|serverb}"
    }
    ```