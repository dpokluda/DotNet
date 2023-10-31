using CacheProvider;
using CacheProvider.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RedisCacheProvider;

namespace CacheProviderTests;

[TestClass]
public class RedisCacheTest
{
    private const string LocalRedisConfiguration = "127.0.0.1:6379";
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
        await cache.SetValueAsync(key, "value", CancellationToken.None);
        Assert.AreEqual("value", await cache.GetValueAsync<string>(key));
    }

    [TestMethod]
    public async Task SetAndGetInt()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, 123, CancellationToken.None);
        Assert.AreEqual(123, await cache.GetValueAsync<int>(key));
    }

    [TestMethod]
    public async Task SetWithAbsoluteExpirationAndGet()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", DateTimeOffset.UtcNow + TimeSpan.FromMinutes(1), CancellationToken.None);
        Assert.AreEqual("value", await cache.GetValueAsync<string>(key));
    }

    [TestMethod]
    public async Task SetWithRelativeExpirationAndGet()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", TimeSpan.FromMinutes(1), CancellationToken.None);
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
    public async Task SetWithAbsoluteExpirationAndGetExpired()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", DateTimeOffset.UtcNow + TimeSpan.FromMilliseconds(100), CancellationToken.None);
        await Task.Delay(500);
        Assert.IsNull(await cache.GetValueAsync<string>(key));
    }

    [TestMethod]
    public async Task SetWithRelativeExpirationAndGetExpired()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", TimeSpan.FromMilliseconds(100), CancellationToken.None);
        await Task.Delay(500);
        Assert.IsNull(await cache.GetValueAsync<string>(key));
    }

    [TestMethod]
    public async Task SetAndGetWithInvalidType()
    {
        ICache cache = GetRedisCache();
        var key = GenerateCacheKey();
        await cache.SetValueAsync(key, "value", CancellationToken.None);
        await Assert.ThrowsExceptionAsync<JsonReaderException>(async () => await cache.GetValueAsync<int>(key));
    }

    private static RedisCache GetRedisCache()
    {
        return new RedisCache(
            Options.Create(new RedisConfiguration
            {
                ConnectionString = LocalRedisConfiguration
            }), 
            new NullLogger<RedisCache>());
    }

    private string GenerateCacheKey()
    {
        return $"key_{Guid.NewGuid():N}";
    }
}