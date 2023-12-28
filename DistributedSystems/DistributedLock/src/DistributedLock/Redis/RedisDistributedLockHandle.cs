using CacheProvider;
using Microsoft.Extensions.Logging;
using RedisCacheProvider;

namespace DistributedLock.Redis;

public class RedisDistributedLockHandle : IDistributedLockHandle
{
    private readonly ICache _cache;
    private readonly string _lockName;
    private readonly string _lockValue;
    private readonly ILogger<RedisCache> _logger;

    public RedisDistributedLockHandle(ICache cache, string lockName, string lockValue)
    {
        _cache = cache;
        _lockName = lockName;
        _lockValue = lockValue;
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken = default)
    {
        await _cache.DeleteAsync(_lockName, _lockValue, CancellationToken.None);
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