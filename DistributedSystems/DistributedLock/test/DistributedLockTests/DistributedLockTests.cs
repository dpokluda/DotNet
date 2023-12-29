using CacheProvider.Configuration;
using DistributedLock.Exceptions;
using DistributedLock.Redis;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RedisCacheProvider;

namespace DistributedLockTests;

[TestClass]
public class DistributedLockTests
{
    private const string LocalRedisConfiguration = "127.0.0.1:6379";
    private const string LockPrefix = "lock:";
    
    [TestMethod]
    public async Task SimpleAcquireAndDispose()
    {
        var provider = new RedisDistributedLockProvider(GetCacheProvider(), new NullLogger<RedisDistributedLockProvider>());
        var name = Guid.NewGuid().ToString("N");
        var lockHandle = await provider.AcquireAsync(name, CancellationToken.None);
        Assert.IsNotNull(lockHandle);
        await lockHandle.ReleaseAsync();
    }

    [TestMethod]
    public async Task MultipleAcquireActions()
    {
        var name = Guid.NewGuid().ToString("N");
        var cacheProvider = GetCacheProvider();
        var lockProvider = new RedisDistributedLockProvider(cacheProvider, new NullLogger<RedisDistributedLockProvider>());
        
        // acquire new lock - should succeed
        var lockHandle = await lockProvider.AcquireAsync(name, CancellationToken.None);
        Assert.IsNotNull(lockHandle);

        // acquire an already acquired lock - should fail
        Assert.ThrowsExceptionAsync<DistributedResourceException>(async () =>
        {
            await lockProvider.AcquireAsync(name, CancellationToken.None);
        });
        
        // release the acquired lock
        await lockHandle.ReleaseAsync();
        
        // now we should be able to re-acquire the same lock
        lockHandle = await lockProvider.AcquireAsync(name, CancellationToken.None);
        Assert.IsNotNull(lockHandle);
        
        // final release
        await lockHandle.ReleaseAsync();
    }
    
    [TestMethod]
    public async Task Release()
    {
        var name = Guid.NewGuid().ToString("N");
        var cacheProvider = GetCacheProvider();
        var cache = cacheProvider.GetCache();
        var lockProvider = new RedisDistributedLockProvider(cacheProvider, new NullLogger<RedisDistributedLockProvider>());

        // acquire lock
        var lockHandle = await lockProvider.AcquireAsync(name, CancellationToken.None);
        Assert.IsNotNull(lockHandle);

        // test existence of lock cache entry
        Assert.IsNotNull(cache.GetValueAsync<string>(LockPrefix + name).Result);
        
        //release lock
        await lockHandle.ReleaseAsync();
        
        // test non-existence of lock cache entry
        Assert.IsNull(cache.GetValueAsync<string>(LockPrefix + name).Result);
    }
    
    [TestMethod]
    public async Task DisposeAsync()
    {
        var name = Guid.NewGuid().ToString("N");
        var cacheProvider = GetCacheProvider();
        var cache = cacheProvider.GetCache();
        var lockProvider = new RedisDistributedLockProvider(cacheProvider, new NullLogger<RedisDistributedLockProvider>());

        // acquire lock
        var lockHandle = await lockProvider.AcquireAsync(name, CancellationToken.None);
        Assert.IsNotNull(lockHandle);

        // test existence of lock cache entry
        Assert.IsNotNull(cache.GetValueAsync<string>(LockPrefix + name).Result);
        
        //release lock
        await lockHandle.DisposeAsync();
        
        // test non-existence of lock cache entry
        Assert.IsNull(cache.GetValueAsync<string>(LockPrefix + name).Result);
    }
    
    [TestMethod]
    public async Task Dispose()
    {
        var name = Guid.NewGuid().ToString("N");
        var cacheProvider = GetCacheProvider();
        var cache = cacheProvider.GetCache();
        var lockProvider = new RedisDistributedLockProvider(cacheProvider, new NullLogger<RedisDistributedLockProvider>());

        // acquire lock
        var lockHandle = await lockProvider.AcquireAsync(name, CancellationToken.None);
        Assert.IsNotNull(lockHandle);

        // test existence of lock cache entry
        Assert.IsNotNull(cache.GetValueAsync<string>(LockPrefix + name).Result);
        
        //release lock
        lockHandle.Dispose();
        
        // test non-existence of lock cache entry
        Assert.IsNull(cache.GetValueAsync<string>(LockPrefix + name).Result);
    }
    
    private static RedisCacheProvider.RedisCacheProvider GetCacheProvider()
    {
        return new RedisCacheProvider.RedisCacheProvider(
            Options.Create(new RedisConfiguration
            {
                ConnectionString = LocalRedisConfiguration
            }), 
            new NullLogger<RedisCache>());
    }
}