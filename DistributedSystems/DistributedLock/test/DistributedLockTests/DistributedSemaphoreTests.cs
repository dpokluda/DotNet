using CacheProvider.Configuration;
using CacheProvider.Timestamp;
using DistributedLock.Redis;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RedisCacheProvider;

namespace DistributedLockTests;

[TestClass]
public class DistributedSemaphoreTests
{
    private const string LocalRedisConfiguration = "127.0.0.1:6379";
    private const string SemaphorePrefix = "semaphore:";
    
    [TestMethod]
    public async Task SimpleAcquireAndDispose()
    {
        var provider = new RedisDistributedSemaphoreProvider(GetCacheProvider(), new NullLogger<RedisDistributedSemaphoreProvider>());
        var name = Guid.NewGuid().ToString("N");
        var lockHandle = await provider.AcquireAsync(name, 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle);
        await lockHandle.ReleaseAsync();
    }
    
    private static RedisCacheProvider.RedisCacheProvider GetCacheProvider()
    {
        return new RedisCacheProvider.RedisCacheProvider(
            Options.Create(new RedisConfiguration
            {
                ConnectionString = LocalRedisConfiguration
            }), 
            new ManualTimestampProvider(),
            new NullLogger<RedisCache>());
    }
}