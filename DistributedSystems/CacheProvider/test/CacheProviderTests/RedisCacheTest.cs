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
    private readonly ManualTimestampProvider _timestampProvider = new ManualTimestampProvider();
    
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
        await cache.SetValueAsync(key, "value", TimeSpan.FromMilliseconds(1), false, CancellationToken.None);
        Assert.AreEqual("value", await cache.GetValueAsync<string>(key));
    }

    [TestMethod]
    public async Task SetAndGetInt()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, 123, TimeSpan.FromMilliseconds(1), false, CancellationToken.None);
        Assert.AreEqual(123, await cache.GetValueAsync<int>(key));
    }

    [TestMethod]
    public async Task SetIfNew()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.IsTrue(await cache.SetValueAsync(key, "value", TimeSpan.FromMilliseconds(1), true, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetValueAsync<string>(key));
        
        Assert.IsFalse(await cache.SetValueAsync(key, "value2", TimeSpan.FromMilliseconds(1), true, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetValueAsync<string>(key));
    }
    
    [TestMethod]
    public async Task GetNotFound()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>(key));
    }


    [TestMethod]
    public async Task SetWithInvalidExpiration()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", TimeSpan.FromMilliseconds(-1), false, CancellationToken.None);
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>(key));
    }

    [TestMethod]
    public async Task SetWithExpiration()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", TimeSpan.FromMilliseconds(1), false, CancellationToken.None);
        Assert.AreEqual("value", await cache.GetValueAsync<string>(key));

        _timestampProvider.Value = 5;
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>(key));
    }

    [TestMethod]
    public async Task SetAndGetWithInvalidType()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", TimeSpan.FromMilliseconds(1), false, CancellationToken.None);
        await Assert.ThrowsExceptionAsync<InvalidCastException>(async () => await cache.GetValueAsync<int>(key));
    }
    
    [TestMethod]
    public async Task Delete()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.IsTrue(await cache.SetValueAsync(key, "value", TimeSpan.FromMilliseconds(1), false, CancellationToken.None));
        Assert.IsTrue(await cache.DeleteValueAsync(key, "value"));
    }
    
    [TestMethod]
    public async Task DeleteWithValueFail()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.IsTrue(await cache.SetValueAsync(key, "value", TimeSpan.FromMilliseconds(1), false, CancellationToken.None));
        Assert.IsFalse(await cache.DeleteValueAsync(key, "value2"));
    }

    [TestMethod]
    public async Task IncrementAndDecrementCounter_Simple()
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
    public async Task IncrementAndDecrementCounter_MaxValue()
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
    public async Task IncrementAndDecrementCounter_Expiration()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        Assert.AreEqual(1, await cache.IncrementCounterAsync(key, "1", TimeSpan.FromMilliseconds(10)));
        _timestampProvider.Value = 5;
        Assert.AreEqual(1, await cache.GetCounterAsync(key));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key, "2", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key, "2", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.GetCounterAsync(key));
        _timestampProvider.Value = 10;
        Assert.AreEqual(1, await cache.GetCounterAsync(key));
        Assert.AreEqual(1, await cache.GetCounterAsync(key));
        Assert.AreEqual(2, await cache.IncrementCounterAsync(key, "1", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.GetCounterAsync(key));
        _timestampProvider.Value = 15;
        Assert.AreEqual(1, await cache.GetCounterAsync(key));
        _timestampProvider.Value = 20;
        Assert.AreEqual(0, await cache.GetCounterAsync(key));
    }
    
    private RedisCache GetRedisCache()
    {
        _timestampProvider.Value = 0;
        
        return new RedisCache(
            Options.Create(new RedisConfiguration
            {
                ConnectionString = LocalRedisConfiguration
            }), 
            _timestampProvider,
            new NullLogger<RedisCache>());
    }

    private string GenerateCacheKey()
    {
        return $"key_{Guid.NewGuid():N}";
    }
}