using CacheProvider;
using CacheProvider.Configuration;
using CacheProvider.Timestamp;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RedisCacheProvider;

namespace CacheProviderTests;

[TestClass]
public class RedisCacheTest
{
    private const string LocalRedisConfiguration = "127.0.0.1:6379";
    private ManualTimestampProvider timestampProvider = new ManualTimestampProvider();
    
    [TestMethod]
    public void Construct()
    {
        ICache cache = GetRedisCache();
        Assert.IsNotNull(cache);
    }

    [TestMethod]
    public async Task SetAndGetString()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", false, CancellationToken.None);
        Assert.AreEqual("value", await cache.GetValueAsync<string>(key));
    }

    [TestMethod]
    public async Task SetAndGetInt()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, 123, false, CancellationToken.None);
        Assert.AreEqual(123, await cache.GetValueAsync<int>(key));
    }

    [TestMethod]
    public async Task SetIfNew()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.IsTrue(await cache.SetValueAsync(key, "value", true, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetValueAsync<string>(key));
    }
    
    [TestMethod]
    public async Task SetIfNewFail()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.IsTrue(await cache.SetValueAsync(key, "value", true, CancellationToken.None));
        Assert.IsFalse(await cache.SetValueAsync(key, "value2", true, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetValueAsync<string>(key));
    }
    
    [TestMethod]
    public async Task SetWithRelativeExpirationAndGet()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", TimeSpan.FromMinutes(1), false, CancellationToken.None);
        Assert.AreEqual("value", await cache.GetValueAsync<string>(key));
    }

    [TestMethod]
    public async Task GetNotFound()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.IsNull(await cache.GetValueAsync<string>(key));
    }


    [TestMethod]
    public async Task SetWithRelativeExpirationAndGetExpired()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", TimeSpan.FromMilliseconds(100), false, CancellationToken.None);
        await Task.Delay(500);
        Assert.IsNull(await cache.GetValueAsync<string>(key));
    }

    [TestMethod]
    public async Task SetAndGetWithInvalidType()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", false, CancellationToken.None);
        await Assert.ThrowsExceptionAsync<JsonReaderException>(async () => await cache.GetValueAsync<int>(key));
    }
    
    [TestMethod]
    public async Task Delete()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.IsTrue(await cache.SetValueAsync(key, "value", false, CancellationToken.None));
        Assert.IsTrue(await cache.DeleteValueAsync(key));
    }
    
    [TestMethod]
    public async Task DeleteWithValue()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.IsTrue(await cache.SetValueAsync(key, "value", false, CancellationToken.None));
        Assert.IsTrue(await cache.DeleteValueAsync(key, "value"));
    }
    
    [TestMethod]
    public async Task DeleteWithValueFail()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.IsTrue(await cache.SetValueAsync(key, "value", false, CancellationToken.None));
        Assert.IsFalse(await cache.DeleteValueAsync(key, "value2"));
    }
    
    [TestMethod]
    public async Task IncrementAndDecrementCounter_Simple()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key));
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
        Assert.AreEqual(1, await cache.IncrementCounterAsync(key));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key));
        Assert.AreEqual(2, await cache.GetCounterAsync(key));
        Assert.AreEqual(1, await cache.DecrementCounterAsync(key));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key));
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
    }
    
    [TestMethod]
    public async Task IncrementWithMaxValueAndDecrementCounter_MaxValue()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key));
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
        Assert.AreEqual(1, await cache.IncrementCounterAsync(key, 2));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key, 2));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key, 2));
        Assert.AreEqual(2, await cache.GetCounterAsync(key));
        Assert.AreEqual(1, await cache.DecrementCounterAsync(key));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key));
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
    }

    [TestMethod]
    public async Task IncrementAndDecrementCounterWithExpiration_Simple()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key, "1"));
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
        Assert.AreEqual(1, await cache.IncrementCounterAsync(key, "1", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key, "2", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.GetCounterAsync(key));
        Assert.AreEqual(1, await cache.DecrementCounterAsync(key, "2"));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key, "1"));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key, "1"));
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
    }
    
    [TestMethod]
    public async Task IncrementAndDecrementCounterWithExpiration_MaxValue()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key, "1"));
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
        Assert.AreEqual(1, await cache.IncrementCounterAsync(key, "1", TimeSpan.FromMilliseconds(10), 2));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key, "2", TimeSpan.FromMilliseconds(10), 2));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key, "2", TimeSpan.FromMilliseconds(10), 2));
        Assert.AreEqual(2, await cache.GetCounterAsync(key));
        Assert.AreEqual(1, await cache.DecrementCounterAsync(key, "2"));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key, "1"));
        Assert.AreEqual(0, await cache.DecrementCounterAsync(key, "1"));
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
    }

    [TestMethod]
    public async Task IncrementAndDecrementCounterWithExpiration_Expiration()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.AreEqual(1, await cache.IncrementCounterAsync(key, "1", TimeSpan.FromMilliseconds(10)));
        timestampProvider.Value = 5;
        Assert.AreEqual(1, await cache.GetCounterAsync(key));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key, "2", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key, "2", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.GetCounterAsync(key));
        timestampProvider.Value = 10;
        Assert.AreEqual(1, await cache.GetCounterAsync(key));
        Assert.AreEqual(1, await cache.GetCounterAsync(key));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key, "1", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.GetCounterAsync(key));
        timestampProvider.Value = 15;
        Assert.AreEqual(1, await cache.GetCounterAsync(key));
        timestampProvider.Value = 20;
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
    }
    
    private RedisCache GetRedisCache()
    {
        timestampProvider.Value = 0;
        
        return new RedisCache(
            Options.Create(new RedisConfiguration
            {
                ConnectionString = LocalRedisConfiguration
            }), 
            timestampProvider,
            new NullLogger<RedisCache>());
    }

    private string GenerateCacheKey()
    {
        return $"key_{Guid.NewGuid():N}";
    }
}