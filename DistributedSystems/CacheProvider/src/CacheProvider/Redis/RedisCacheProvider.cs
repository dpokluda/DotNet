using CacheProvider;
using CacheProvider.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RedisCacheProvider;

/// <summary>
/// The redis cache provider.
/// </summary>
/// <seealso cref="CacheProvider.ICacheProvider"/>
public class RedisCacheProvider : ICacheProvider
{
    private IOptions<RedisConfiguration> _config;
    private readonly ITimestampProvider _timestampProvider;
    private ILogger<RedisCache> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCacheProvider"/> class.
    /// </summary>
    /// <param name="config">The Redis configuration.</param>
    /// <param name="timestampProvider">Factory class to generate current timestamp.</param>
    /// <param name="logger">The logger.</param>
    public RedisCacheProvider(IOptions<RedisConfiguration> config, ITimestampProvider timestampProvider, ILogger<RedisCache> logger)
    {
        _config = config;
        _timestampProvider = timestampProvider;
        _logger = logger;
    }

    /// <summary>
    /// Constructs an <see cref="ICache"/> instance to access distributed cache.
    /// </summary>
    /// <returns>
    /// The cache.
    /// </returns>
    /// <seealso cref="ICacheProvider.GetCache()"/>
    public ICache GetCache()
    {
        return new RedisCache(_config, _timestampProvider, _logger);
    }
}