using CacheProvider;
using Microsoft.Extensions.Logging;

namespace DistributedLock.Redis;

public class RedisDistributedSemaphoreHandle : IDistributedSemaphoreHandle
{
    private readonly ICache _cache;
    private readonly string _semaphoreName;
    private readonly string _id;
    private readonly ILogger<RedisDistributedSemaphoreProvider> _logger;

    public RedisDistributedSemaphoreHandle(string semaphoreName, string id, ICache cache, ILogger<RedisDistributedSemaphoreProvider> logger)
    {
        _semaphoreName = semaphoreName;
        _id = id;
        _cache = cache;
        _logger = logger;
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken = default)
    {
        await _cache.DecrementCounterAsync(_semaphoreName, _id, cancellationToken);
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