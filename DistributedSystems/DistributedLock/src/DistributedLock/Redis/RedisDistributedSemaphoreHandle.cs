using CacheProvider;
using Microsoft.Extensions.Logging;

namespace DistributedLock.Redis;

public class RedisDistributedSemaphoreHandle : IDistributedSemaphoreHandle
{
    private readonly ICache _cache;
    private readonly string _semaphoreName;
    private readonly ILogger<RedisDistributedSemaphoreProvider> _logger;

    public RedisDistributedSemaphoreHandle(string semaphoreName, ICache cache, ILogger<RedisDistributedSemaphoreProvider> logger)
    {
        _semaphoreName = semaphoreName;
        _cache = cache;
        _logger = logger;
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task ReleaseAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}