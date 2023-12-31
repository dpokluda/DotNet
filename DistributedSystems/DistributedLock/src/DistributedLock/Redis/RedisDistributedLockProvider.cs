using CacheProvider;
using DistributedLock.Exceptions;
using Microsoft.Extensions.Logging;

namespace DistributedLock.Redis;

public class RedisDistributedLockProvider : IDistributedLockProvider
{
    private const string UnableToAcquireExceptionMessage = "Unable to acquire distributed lock";
    private const string LockPrefix = "lock:";
    
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<RedisDistributedLockProvider> _logger;

    public RedisDistributedLockProvider(ICacheProvider cacheProvider, ILogger<RedisDistributedLockProvider> logger)
    {
        _cacheProvider = cacheProvider;
        _logger = logger;
    }

    public async Task<IDistributedLockHandle> AcquireAsync(string name, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        var cache = _cacheProvider.GetCache();
        var lockName = LockPrefix + name;
        var lockValue = Guid.NewGuid().ToString("N");
        if (await cache.SetValueAsync(lockName, lockValue, expiration, onlyIfNew: true,
                        cancellationToken: cancellationToken))
        {
            return new RedisDistributedLockHandle(lockName, lockValue, cache, _logger);
        }
        
        throw new DistributedResourceException(UnableToAcquireExceptionMessage);
    }

    public async Task<bool> IsAcquiredAsync(string name, CancellationToken cancellationToken = default)
    {
        var cache = _cacheProvider.GetCache();
        var lockName = LockPrefix + name;

        return (await cache.GetValueAsync<string>(lockName, cancellationToken: cancellationToken) != null);
    }
}