using CacheProvider;
using Microsoft.Extensions.Logging;

namespace DistributedLock.Redis;

public class RedisDistributedLockProvider : IDistributedLockProvider
{
    private const string TimeoutExceptionMessage = "Timeout waiting for distributed lock";
    private const string LockPrefix = "lock:";
    
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<RedisDistributedLockProvider> _logger;

    public RedisDistributedLockProvider(ICacheProvider cacheProvider, ILogger<RedisDistributedLockProvider> logger)
    {
        _cacheProvider = cacheProvider;
        _logger = logger;
    }

    public async Task<IDistributedLockHandle> AcquireAsync(string name, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var _cache = _cacheProvider.GetCache();
        var lockName = LockPrefix + name;
        var lockValue = Guid.NewGuid().ToString("N");
        if (await _cache.SetValueAsync(lockName, lockValue, onlyIfNew: true,
                        cancellationToken: cancellationToken))
        {
            return new RedisDistributedLockHandle(_cache, lockName, lockValue);
        }
        
        throw new TimeoutException(TimeoutExceptionMessage);
    }
}