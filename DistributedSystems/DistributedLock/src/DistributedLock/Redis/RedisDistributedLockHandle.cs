using CacheProvider;
using Microsoft.Extensions.Logging;

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
        await _cache.DeleteValueAsync(_lockName, _lockValue, cancellationToken);
    }

    public async Task<bool> IsStillValidAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return (await _cache.GetValueAsync<string>(_lockName, cancellationToken) == _lockValue);
        }
        catch (Exception)
        {
            return false;
        }
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