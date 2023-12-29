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
    
    public async Task<IDistributedSemaphoreHandle> AcquireAsync(string name, int maxCount, CancellationToken cancellationToken = default)
    {
        var _cache = _cacheProvider.GetCache();
        var lockName = SemaphorePrefix + name;
        var lockValue = Guid.NewGuid().ToString("N");
        if (await _cache.SetValueAsync(lockName, lockValue, onlyIfNew: true,
                cancellationToken: cancellationToken))
        {
            return new RedisDistributedSemaphoreHandle(lockName, _cache, _logger);
        }
        
        throw new DistributedResourceException(UnableToAcquireExceptionMessage);
    }
}