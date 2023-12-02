using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;

namespace SampleCommon;

public class AzureData
{
    private const string serviceBusConnectionString = "";
    private const string cosmosDbConnectionString = ""
    
    public Task<ServiceBusSender> GetServiceBusSenderAsync(CancellationToken token)
    {
        ServiceBusClient client = GetServiceBusClient();
        ServiceBusSender sender = client.CreateSender("names");

        return Task.FromResult(sender);
    }

    public Task<ServiceBusReceiver> GetServiceBusReceiverAsync(CancellationToken token)
    {
        ServiceBusClient client = GetServiceBusClient();
        ServiceBusReceiver receiver = client.CreateReceiver("names");

        return Task.FromResult(receiver);
    }

    public Task<Container> GetDataContainer(CancellationToken token)
    {
        var cosmosClient = new CosmosClient(cosmosDbConnectionString);
        var database = cosmosClient.GetDatabase("AzureSample");
        var container = database.GetContainer("Names");

        return Task.FromResult<Container>(container);
    }

    private ServiceBusClient GetServiceBusClient()
    {
        var client = new ServiceBusClient(serviceBusConnectionString);
        return client;
    }
}