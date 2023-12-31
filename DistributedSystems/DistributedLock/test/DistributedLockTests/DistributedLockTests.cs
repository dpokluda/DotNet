using CacheProvider.Configuration;
using CacheProvider.Timestamp;
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

    private readonly ManualTimestampProvider _timestampProvider = new ManualTimestampProvider();

    
    [TestMethod]
    public async Task SimpleAcquireAndDispose()
    {
        var provider = new RedisDistributedLockProvider(GetCacheProvider(), new NullLogger<RedisDistributedLockProvider>());
        var name = Guid.NewGuid().ToString("N");
        var lockHandle = await provider.AcquireAsync(name, TimeSpan.FromMilliseconds(10), CancellationToken.None);
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
        var lockHandle = await lockProvider.AcquireAsync(name, TimeSpan.FromMilliseconds(10), CancellationToken.None);
        Assert.IsNotNull(lockHandle);

        // acquire an already acquired lock - should fail
        Assert.ThrowsExceptionAsync<DistributedResourceException>(async () =>
        {
            await lockProvider.AcquireAsync(name, TimeSpan.FromMilliseconds(10), CancellationToken.None);
        });
        
        // release the acquired lock
        await lockHandle.ReleaseAsync();
        
        // now we should be able to re-acquire the same lock
        lockHandle = await lockProvider.AcquireAsync(name, TimeSpan.FromMilliseconds(10), CancellationToken.None);
        Assert.IsNotNull(lockHandle);
        
        // final release
        await lockHandle.ReleaseAsync();
    }
    
    [TestMethod]
    public async Task Expirations()
    {
        var name = Guid.NewGuid().ToString("N");
        var cacheProvider = GetCacheProvider();
        var lockProvider = new RedisDistributedLockProvider(cacheProvider, new NullLogger<RedisDistributedLockProvider>());
        
        // acquire new lock - should succeed
        var lockHandle = await lockProvider.AcquireAsync(name, TimeSpan.FromMilliseconds(10), CancellationToken.None);
        Assert.IsNotNull(lockHandle);

        // acquire an already acquired lock - should fail
        _timestampProvider.Value = 0;
        Assert.ThrowsExceptionAsync<DistributedResourceException>(async () =>
        {
            await lockProvider.AcquireAsync(name, TimeSpan.FromMilliseconds(10), CancellationToken.None);
        });
        
        // release the acquired lock
        await lockHandle.ReleaseAsync();
        
        // now we should be able to re-acquire the same lock
        lockHandle = await lockProvider.AcquireAsync(name, TimeSpan.FromMilliseconds(10), CancellationToken.None);
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
        var lockHandle = await lockProvider.AcquireAsync(name, TimeSpan.FromMilliseconds(10), CancellationToken.None);
        Assert.IsNotNull(lockHandle);

        // test existence of lock cache entry
        Assert.IsNotNull(await cache.GetValueAsync<string>(LockPrefix + name));
        
        //release lock
        await lockHandle.ReleaseAsync();
        
        // test non-existence of lock cache entry
        Assert.IsNull(await cache.GetValueAsync<string>(LockPrefix + name));
    }
    
    [TestMethod]
    public async Task DisposeAsync()
    {
        var name = Guid.NewGuid().ToString("N");
        var cacheProvider = GetCacheProvider();
        var cache = cacheProvider.GetCache();
        var lockProvider = new RedisDistributedLockProvider(cacheProvider, new NullLogger<RedisDistributedLockProvider>());

        // acquire lock
        var lockHandle = await lockProvider.AcquireAsync(name, TimeSpan.FromMilliseconds(10), CancellationToken.None);
        Assert.IsNotNull(lockHandle);

        // test existence of lock cache entry
        Assert.IsNotNull(await cache.GetValueAsync<string>(LockPrefix + name));
        
        //release lock
        await lockHandle.DisposeAsync();
        
        // test non-existence of lock cache entry
        Assert.IsNull(await cache.GetValueAsync<string>(LockPrefix + name));
    }
    
    [TestMethod]
    public async Task Dispose()
    {
        var name = Guid.NewGuid().ToString("N");
        var cacheProvider = GetCacheProvider();
        var cache = cacheProvider.GetCache();
        var lockProvider = new RedisDistributedLockProvider(cacheProvider, new NullLogger<RedisDistributedLockProvider>());

        // acquire lock
        var lockHandle = await lockProvider.AcquireAsync(name, TimeSpan.FromMilliseconds(10), CancellationToken.None);
        Assert.IsNotNull(lockHandle);

        // test existence of lock cache entry
        Assert.IsNotNull(await cache.GetValueAsync<string>(LockPrefix + name));
        
        //release lock
        lockHandle.Dispose();
        
        // test non-existence of lock cache entry
        Assert.IsNull(await cache.GetValueAsync<string>(LockPrefix + name));
    }
    
    private RedisCacheProvider.RedisCacheProvider GetCacheProvider()
    {
        _timestampProvider.Value = 0;
        
        return new RedisCacheProvider.RedisCacheProvider(
            Options.Create(new RedisConfiguration
            {
                ConnectionString = LocalRedisConfiguration
            }), 
            new ManualTimestampProvider(),
            new NullLogger<RedisCache>());
    }
}