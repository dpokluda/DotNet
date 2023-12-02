using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using SampleCommon;
using SampleCommon.Data;

namespace SampleApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NamesController : ControllerBase
    {
        private readonly AzureData _azureData;
        public NamesController(AzureData azureData)
        {
            _azureData = azureData;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApiNameInput input, CancellationToken token)
        {
            var sender = await _azureData.GetServiceBusSenderAsync(token);
            var message = new QueueMessage { Name = input.Name, TimeStamp = DateTimeOffset.UtcNow };
            await sender.SendMessageAsync(new ServiceBusMessage(JsonConvert.SerializeObject(message)), token);
            await sender.CloseAsync(token);
            return AcceptedAtRoute("GetName", new { name = input.Name });
        }

        [HttpGet("{name}", Name = "GetName")]
        public async Task<IActionResult> Get(string name, CancellationToken token)
        {
            var dataContainer = await _azureData.GetDataContainer(token);
            var entity = await dataContainer.ReadItemAsync<NameData>(name, new PartitionKey("names"), null, token);
            var data = entity?.Resource;
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpGet(Name = "GetNames")]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var dataContainer = await _azureData.GetDataContainer(token);
            var query = new QueryDefinition("SELECT * FROM c WHERE c.PartitionId = @partitionKey")
                .WithParameter("@partitionKey", "names");

            var iterator = dataContainer.GetItemQueryIterator<NameData>(query, requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey("names")
            });

            List<string> results = new List<string>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(token);
                foreach (var item in response)
                {
                    results.Add(item.Name);
                }
            }


            return Ok(results);
        }
    }
}
