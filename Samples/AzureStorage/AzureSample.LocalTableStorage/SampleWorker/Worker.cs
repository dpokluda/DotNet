using Azure;
using Azure.Data.Tables;
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
                var queueClient = await _azureData.GetQueueClientAsync(token);
                var queueMessage = await queueClient.ReceiveMessageAsync(TimeSpan.FromSeconds(30), token);
                if (queueMessage?.Value != null)
                {
                    _logger.LogInformation($"New message received: {queueMessage.Value.MessageId}");
                    var message = JsonConvert.DeserializeObject<QueueMessage>(queueMessage.Value.Body.ToString());
                    if (message != null)
                    {
                        var tableClient = await _azureData.GetTableClientAsync(token);

                        TableNameData? data = null;
                        // read existing data (if exists)
                        try
                        {
                            var entity = await tableClient.GetEntityAsync<TableEntity>(partitionKey: "names", rowKey: message.Name, null, token);
                            if (entity != null)
                            {
                                var jsonData = entity.Value.GetString("Data");
                                if (jsonData != null)
                                {
                                    data = JsonConvert.DeserializeObject<TableNameData>(jsonData);
                                }
                            }
                        }
                        catch (RequestFailedException )
                        {
                            // doesn't exist
                            data = null;
                        }

                        // create new data if needed
                        if (data == null)
                        {
                            data = new TableNameData
                            {
                                Name = message.Name
                            };
                        }

                        // update data
                        data.TimeStamps.Add(message.TimeStamp);

                        // save data
                        var tableEntity = new TableEntity("names", message.Name)
                        {
                            {"Data", JsonConvert.SerializeObject(data)}
                        };
                        await tableClient.UpsertEntityAsync(tableEntity, TableUpdateMode.Replace, token);
                    }

                    await queueClient.DeleteMessageAsync(queueMessage.Value.MessageId, queueMessage.Value.PopReceipt, token);
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
