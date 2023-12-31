using CacheProvider;
using DistributedLock.Exceptions;
using Microsoft.Extensions.Logging;

namespace DistributedLock.Redis;

public class RedisDistributedSemaphoreProvider : IDistributedSemaphoreProvider
{
    private const string UnableToAcquireExceptionMessage = "Unable to acquire distributed semaphore";
    private const string SemaphorePrefix = "semaphore:";
    
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<RedisDistributedSemaphoreProvider> _logger;

    public RedisDistributedSemaphoreProvider(ICacheProvider cacheProvider, ILogger<RedisDistributedSemaphoreProvider> logger)
    {
        _cacheProvider = cacheProvider;
        _logger = logger;
    }
    
    public async Task<IDistributedSemaphoreHandle> AcquireAsync(string name, string id, TimeSpan expiration, int maxValue, CancellationToken cancellationToken = default)
    {
        var _cache = _cacheProvider.GetCache();
        var lockName = SemaphorePrefix + name;
        var lockValue = id;
        if (await _cache.IncrementCounterAsync(lockName, lockValue, expiration, maxValue, cancellationToken))
        {
            return new RedisDistributedSemaphoreHandle(lockName, id, _cache, _logger);
        }
        
        throw new DistributedResourceException(UnableToAcquireExceptionMessage);
    }

    public async Task<int> GetCountAsync(string name, CancellationToken cancellationToken = default)
    {
        var _cache = _cacheProvider.GetCache();
        var lockName = SemaphorePrefix + name;

        return await _cache.GetCounterAsync(lockName, cancellationToken);
    }
}