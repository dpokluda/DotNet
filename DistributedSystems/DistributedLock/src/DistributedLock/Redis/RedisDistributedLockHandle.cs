using CacheProvider;
using Microsoft.Extensions.Logging;
using RedisCacheProvider;

namespace DistributedLock.Redis;

public class RedisDistributedLockHandle : IDistributedLockHandle
{
    private readonly string _lockName;
    private readonly string _lockValue;
    private readonly ICache _cache;
    private readonly ILogger<RedisDistributedLockProvider> _logger;

    public RedisDistributedLockHandle(string lockName, string lockValue, ICache cache, ILogger<RedisDistributedLockProvider> logger)
    {
        _lockName = lockName;
        _lockValue = lockValue;
        _cache = cache;
        _logger = logger;
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken = default)
    {
        await _cache.DeleteValueAsync(_lockName, _lockValue, CancellationToken.None);
    }
    
    public async ValueTask DisposeAsync()
    {
        await ReleaseAsync(CancellationToken.None);
    }

    public void Dispose()
    {
        Task.Factory.StartNew(async () => await ReleaseAsync(CancellationToken.None)).Wait();
    }
}