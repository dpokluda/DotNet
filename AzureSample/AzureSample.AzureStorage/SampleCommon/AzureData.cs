using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;

namespace SampleCommon;

public class AzureData
{
    public Task<ServiceBusSender> GetServiceBusSenderAsync(CancellationToken token)
    {
        var client = new ServiceBusClient("Endpoint=sb://dpokluda-test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ErWsmKIsr2A3Ma/PDZKLBepjebibtkSIw+ASbI42hfk=");
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
        var cosmosClient = new CosmosClient("AccountEndpoint=https://dpokluda-test.documents.azure.com:443/;AccountKey=GXdkrb0IFzYLNtTTagSMKuzsUAdl6x3nubfoJ5dY4o7ELyM8NUXFlAtgA2dNpaB4LKgDgpu5P0fPACDb9cmaYQ==;");
        var database = cosmosClient.GetDatabase("AzureSample");
        var container = database.GetContainer("Names");

        return Task.FromResult<Container>(container);
    }

    private ServiceBusClient GetServiceBusClient()
    {
        var client = new ServiceBusClient("Endpoint=sb://dpokluda-test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ErWsmKIsr2A3Ma/PDZKLBepjebibtkSIw+ASbI42hfk=");
        return client;
    }
}