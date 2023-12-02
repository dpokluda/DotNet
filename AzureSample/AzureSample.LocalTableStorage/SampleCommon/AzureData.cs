using Azure.Data.Tables;
using Azure.Storage.Queues;

namespace SampleCommon;

public class AzureData
{
    public async Task<QueueClient> GetQueueClientAsync(CancellationToken token)
    {
        var queueClient = new QueueClient("UseDevelopmentStorage=true", "names");
        if (await queueClient.ExistsAsync(token) == false)
        {
            await queueClient.CreateIfNotExistsAsync();
        }

        return queueClient;
    }

    public async Task<TableClient> GetTableClientAsync(CancellationToken token)
    {
        var tableClient = new TableClient("UseDevelopmentStorage=true", "names");
        await tableClient.CreateIfNotExistsAsync(token);

        return tableClient;
    }
}