using CacheProvider.Configuration;
using CacheProvider.Timestamp;
using DistributedLock.Exceptions;
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
    
    private readonly ManualTimestampProvider _timestampProvider = new ManualTimestampProvider();

    [TestMethod]
    public async Task SimpleAcquireAndDispose()
    {
        var name = Guid.NewGuid().ToString("N");
        var provider = new RedisDistributedSemaphoreProvider(GetCacheProvider(), new NullLogger<RedisDistributedSemaphoreProvider>());
        var lockHandle = await provider.AcquireAsync(name, "1", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle);
        
        await lockHandle.ReleaseAsync();
    }
    
    [TestMethod]
    public async Task MultipleAcquireActions()
    {
        var name = Guid.NewGuid().ToString("N");
        var provider = new RedisDistributedSemaphoreProvider(GetCacheProvider(), new NullLogger<RedisDistributedSemaphoreProvider>());
        
        Assert.AreEqual(0, await provider.GetCountAsync(name, CancellationToken.None));
        
        // increment semaphore counter
        var lockHandle1 = await provider.AcquireAsync(name, "1", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle1);
        Assert.AreEqual(1, await provider.GetCountAsync(name, CancellationToken.None));

        var lockHandle2 = await provider.AcquireAsync(name, "2", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle1);
        Assert.AreEqual(2, await provider.GetCountAsync(name, CancellationToken.None));

        // acquire an already acquired lock - should fail
        Assert.ThrowsExceptionAsync<DistributedResourceException>(async () =>
        {
            await provider.AcquireAsync(name, "3", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        });
        Assert.AreEqual(2, await provider.GetCountAsync(name, CancellationToken.None));
        
        // release the acquired lock
        await lockHandle2.ReleaseAsync();
        Assert.AreEqual(1, await provider.GetCountAsync(name, CancellationToken.None));
        
        // now we should be able to re-acquire the same lock
        var lockHandle3 = await provider.AcquireAsync(name, "3", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle3);
        Assert.AreEqual(2, await provider.GetCountAsync(name, CancellationToken.None));

        // final release
        await lockHandle3.ReleaseAsync();
        await lockHandle1.ReleaseAsync();
        Assert.AreEqual(0, await provider.GetCountAsync(name, CancellationToken.None));
    }

    [TestMethod]
    public async Task Expirations()
    {
        var name = Guid.NewGuid().ToString("N");
        var provider = new RedisDistributedSemaphoreProvider(GetCacheProvider(), new NullLogger<RedisDistributedSemaphoreProvider>());
        
        Assert.AreEqual(0, await provider.GetCountAsync(name, CancellationToken.None));
        
        // increment semaphore counter
        _timestampProvider.Value = 0;
        var lockHandle1 = await provider.AcquireAsync(name, "1", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle1);
        Assert.AreEqual(1, await provider.GetCountAsync(name, CancellationToken.None));

        _timestampProvider.Value = 5;
        var lockHandle2 = await provider.AcquireAsync(name, "2", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle1);
        Assert.AreEqual(2, await provider.GetCountAsync(name, CancellationToken.None));

        // acquire an already acquired lock - should fail
        Assert.ThrowsExceptionAsync<DistributedResourceException>(async () =>
        {
            await provider.AcquireAsync(name, "3", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        });
        Assert.AreEqual(2, await provider.GetCountAsync(name, CancellationToken.None));
        
        // expirations
        _timestampProvider.Value = 10;
        Assert.AreEqual(1, await provider.GetCountAsync(name, CancellationToken.None));
        // should be no-op
        await lockHandle1.ReleaseAsync();
        
        // now we should be able to re-acquire the same lock
        var lockHandle3 = await provider.AcquireAsync(name, "3", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle3);
        Assert.AreEqual(2, await provider.GetCountAsync(name, CancellationToken.None));

        // expirations
        _timestampProvider.Value = 15;
        Assert.AreEqual(1, await provider.GetCountAsync(name, CancellationToken.None));

        _timestampProvider.Value = 20;
        Assert.AreEqual(0, await provider.GetCountAsync(name, CancellationToken.None));
    }
    
    [TestMethod]
    public async Task GetCounter()
    {
        var name = Guid.NewGuid().ToString("N");
        var cacheProvider = GetCacheProvider();
        var cache = cacheProvider.GetCache();
        var provider = new RedisDistributedSemaphoreProvider(cacheProvider, new NullLogger<RedisDistributedSemaphoreProvider>());
        var lockHandle = await provider.AcquireAsync(name, "1", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle);
        Assert.AreEqual(1, await cache.GetCounterAsync(SemaphorePrefix + name));
        Assert.AreEqual(1, await provider.GetCountAsync(name));
        
        await lockHandle.ReleaseAsync();
        Assert.AreEqual(0, await cache.GetCounterAsync(SemaphorePrefix + name));
        Assert.AreEqual(0, await provider.GetCountAsync(name));
    }
    
    [TestMethod]
    public async Task Release()
    {
        var name = Guid.NewGuid().ToString("N");
        var cacheProvider = GetCacheProvider();
        var cache = cacheProvider.GetCache();
        var provider = new RedisDistributedSemaphoreProvider(cacheProvider, new NullLogger<RedisDistributedSemaphoreProvider>());
        var lockHandle = await provider.AcquireAsync(name, "1", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle);
        Assert.AreEqual(1, await cache.GetCounterAsync(SemaphorePrefix + name));
        
        await lockHandle.ReleaseAsync();
        Assert.AreEqual(0, await cache.GetCounterAsync(SemaphorePrefix + name));
    }
    
    [TestMethod]
    public async Task DisposeAsync()
    {
        var name = Guid.NewGuid().ToString("N");
        var cacheProvider = GetCacheProvider();
        var cache = cacheProvider.GetCache();
        var provider = new RedisDistributedSemaphoreProvider(cacheProvider, new NullLogger<RedisDistributedSemaphoreProvider>());
        var lockHandle = await provider.AcquireAsync(name, "1", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle);
        Assert.AreEqual(1, await cache.GetCounterAsync(SemaphorePrefix + name));
        
        await lockHandle.DisposeAsync();
        Assert.AreEqual(0, await cache.GetCounterAsync(SemaphorePrefix + name));
    }
    
    [TestMethod]
    public async Task Dispose()
    {
        var name = Guid.NewGuid().ToString("N");
        var cacheProvider = GetCacheProvider();
        var cache = cacheProvider.GetCache();
        var provider = new RedisDistributedSemaphoreProvider(cacheProvider, new NullLogger<RedisDistributedSemaphoreProvider>());
        var lockHandle = await provider.AcquireAsync(name, "1", TimeSpan.FromMilliseconds(10), 2, CancellationToken.None);
        Assert.IsNotNull(lockHandle);
        Assert.AreEqual(1, await cache.GetCounterAsync(SemaphorePrefix + name));
        
        lockHandle.Dispose();
        Assert.AreEqual(0, await cache.GetCounterAsync(SemaphorePrefix + name));
    }
    
    private RedisCacheProvider.RedisCacheProvider GetCacheProvider()
    {
        _timestampProvider.Value = 0;
        
        return new RedisCacheProvider.RedisCacheProvider(
            Options.Create(new RedisConfiguration
            {
                ConnectionString = LocalRedisConfiguration
            }), 
            _timestampProvider,
            new NullLogger<RedisCache>());
    }
}