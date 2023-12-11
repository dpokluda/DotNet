using System.Diagnostics;
using BloomFilter.Serialization;
using Newtonsoft.Json;

namespace BloomFilter.Tests;

[TestClass]
public class SerializationTests
{
    [TestMethod]
    public void Serialize()
    {   
        var filter = new BloomFilter(100, 0.01);
        filter.Add("one");
        filter.Add("two");

        var jsonSettings = new JsonSerializerSettings();
        jsonSettings.Converters.Add(new BitArrayConverter());
        var serialized = JsonConvert.SerializeObject(filter, jsonSettings);
        
        Trace.WriteLine(serialized);
    }
    
    [TestMethod]
    public void Deserialize()
    {   
        var filter = new BloomFilter(100, 0.01);
        filter.Add("one");
        filter.Add("two");

        var jsonSettings = new JsonSerializerSettings();
        jsonSettings.Converters.Add(new BitArrayConverter());
        var serialized = JsonConvert.SerializeObject(filter, jsonSettings);
        var deserialized = JsonConvert.DeserializeObject<BloomFilter>(serialized, jsonSettings);
        
        Assert.IsNotNull(deserialized);
        Trace.WriteLine(JsonConvert.SerializeObject(deserialized, jsonSettings));
        
        Assert.IsTrue(filter.Contains("one"));
        Assert.IsTrue(filter.Contains("two"));
        Assert.IsFalse(filter.Contains("three"));
    }
    
    [TestMethod]
    public void SerializeToStringBits()
    {   
        var filter = new BloomFilter(100, 0.01);
        filter.Add("one");
        filter.Add("two");

        var jsonSettings = new JsonSerializerSettings();
        jsonSettings.Converters.Add(new BitArrayToStringConverter());
        var serialized = JsonConvert.SerializeObject(filter, jsonSettings);
        var deserialized = JsonConvert.DeserializeObject<BloomFilter>(serialized, jsonSettings);

        Assert.IsNotNull(deserialized);
        Trace.WriteLine(JsonConvert.SerializeObject(deserialized, jsonSettings));
        
        Assert.IsTrue(filter.Contains("one"));
        Assert.IsTrue(filter.Contains("two"));
        Assert.IsFalse(filter.Contains("three"));
    }
}