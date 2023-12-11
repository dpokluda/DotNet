using CacheProvider;
using Microsoft.Extensions.Logging;
using RedisCacheProvider;

namespace DistributedLock.Redis;

public class RedisDistributedLockProvider : IDistributedLockProvider
{
    private readonly ICache _cache;
    private readonly ILogger<RedisCache> _logger;

    public RedisDistributedLockProvider(ICache cache, ILogger<RedisCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public IDistributedLock CreateLock(string name)
    {
        return new RedisDistributedLock(name, _cache, _logger);
    }
}