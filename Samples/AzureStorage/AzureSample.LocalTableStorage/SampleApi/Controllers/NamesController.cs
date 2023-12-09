using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
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
            var queueClient = await _azureData.GetQueueClientAsync(token);

            var message = new QueueMessage
            {
                Name = input.Name,
                TimeStamp = DateTimeOffset.UtcNow
            };
            await queueClient.SendMessageAsync(JsonConvert.SerializeObject(message), token);

            return AcceptedAtRoute("nameof(Get)", new { name = input.Name });
        }

        [HttpGet("{input}", Name="GetName")]
        public async Task<IActionResult> Get([FromRoute] string name, CancellationToken token)
        {
            var tableClient = await _azureData.GetTableClientAsync(token);

            var entity = await tableClient.GetEntityAsync<TableEntity>(partitionKey: "names", rowKey: name, null, token);
            if (entity != null)
            {
                var jsonData = entity.Value.GetString("Data");
                if (jsonData != null)
                {
                    TableNameData? data = JsonConvert.DeserializeObject<TableNameData>(jsonData);

                    return Ok(data);
                }
            }

            return BadRequest();
        }

        [HttpGet("", Name="GetNames")]
        public async Task<IActionResult> GetAll(CancellationToken token)
        {
            var tableClient = await _azureData.GetTableClientAsync(token);

            var names = new List<string>();
            var rows = tableClient.QueryAsync<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq 'names'"), null, null, token);
            await foreach (var entity in rows)
            {
                names.Add(entity.RowKey);
            }

            return Ok(names);
        }
    }
}
