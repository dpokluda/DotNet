using CacheProvider;
using Microsoft.Extensions.Logging;
using RedisCacheProvider;

namespace DistributedLock.Redis;

public class RedisDistributedLock : IDistributedLock
{
    private readonly ICache _cache;
    private readonly ILogger<RedisCache> _logger;

    public string Name { get; }
    private readonly string _value = Guid.NewGuid().ToString("N");

    public RedisDistributedLock(string name, ICache cache, ILogger<RedisCache> logger)
    {
        Name = name;
        _cache = cache;
        _logger = logger;
    }

    public ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(DateTimeOffset timeout = default, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IDistributedSynchronizationHandle> AcquireAsync(DateTimeOffset? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}