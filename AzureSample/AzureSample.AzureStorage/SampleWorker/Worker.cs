using System.Net;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using SampleCommon;
using SampleCommon.Data;

namespace SampleWorker
{
    public class Worker : BackgroundService
    {
        private readonly AzureData _azureData;
        private readonly ILogger<Worker> _logger;

        public Worker(AzureData azureData, ILogger<Worker> logger)
        {
            _azureData = azureData;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            while (!token.IsCancellationRequested)
            {
                var receiver = await _azureData.GetServiceBusReceiverAsync(token);
                var queueMessage = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(30), token);
                if (queueMessage?.Body != null)
                {
                    _logger.LogInformation($"New message received: {queueMessage.MessageId}");
                    var message = JsonConvert.DeserializeObject<QueueMessage>(queueMessage.Body.ToString());
                    if (message != null)
                    {
                        var dataContainer = await _azureData.GetDataContainer(token);

                        NameData? data = null;
                        // read existing data (if exists)
                        try
                        {
                            var entity = await dataContainer.ReadItemAsync<NameData>(message.Name, new PartitionKey("names"), null, token);
                            data = entity?.Resource;
                        }
                        catch (CosmosException e)
                        {
                            if (e.StatusCode != HttpStatusCode.NotFound)
                            {
                                throw;
                            }
                            else
                            {
                                data = null;
                            }
                        }

                        // create new data if needed
                        if (data == null)
                        {
                            data = new NameData
                            {
                                id = message.Name,
                                Name = message.Name
                            };
                        }

                        // update data
                        data.TimeStamps.Add(message.TimeStamp);

                        // save data
                        await dataContainer.UpsertItemAsync(data, new PartitionKey("names"), null, token);
                    }

                    await receiver.CompleteMessageAsync(queueMessage, token);
                }
                else
                {
                    _logger.LogInformation("No messages in queue");
                }

                await Task.Delay(1000, token);
            }
        }
    }
}