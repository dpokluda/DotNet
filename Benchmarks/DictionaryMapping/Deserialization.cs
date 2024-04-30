using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace DictionaryMapping;

public class Deserialization
{
    private const string SerializedMappings = @"{""Name"":""David Pokluda"",""YearOfBirth"":1973,""City"":""Redmond""}";

    [Benchmark]
    public void Synchronous()
    {
        var d = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(SerializedMappings);
    }
    
    [Benchmark]
    public async Task Asynchronous()
    {
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(SerializedMappings));
        var d = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(stream);
    }
}