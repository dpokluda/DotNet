using System.Net.Sockets;
using CacheProvider;
using CacheProvider.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using StackExchange.Redis;

namespace RedisCacheProvider
{
    /// <summary>
    /// The redis cache.
    /// </summary>
    /// <seealso cref="ICache"/>
    public class RedisCache : ICache
    {
        private ConnectionMultiplexer _redis;
        private readonly AsyncRetryPolicy _policy;
        private readonly ILogger<RedisCache> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache"/> class.
        /// </summary>
        /// <param name="config">The Redis configuration.</param>
        /// <param name="logger">The logger.</param>
        public RedisCache(IOptions<RedisConfiguration> config, ILogger<RedisCache> logger)
        {
            RedisLazyReconnect.InitializeConnectionString(config.Value.ConnectionString);
            _logger = logger;
            _redis = RedisLazyReconnect.Connection;

            _policy = Policy
                      .Handle<RedisConnectionException>()
                      .Or<SocketException>()
                      .Or<ObjectDisposedException>()
                      .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        /// <summary>
        /// Gets cache value asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache value.
        /// </returns>
        /// <seealso cref="ICache.GetValueAsync{T}(string,CancellationToken)"/>
        public async Task<T> GetValueAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                           {
                               var value = await GetDatabase().StringGetAsync(key);
                               if (!value.HasValue)
                               {
                                   return default;
                               }

                               return JsonConvert.DeserializeObject<T>(value);
                           },
                           cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("RedisCache threw exception", e);
                throw;
            }
        }

        /// <summary>
        /// Sets cache value asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cache value.</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        /// <seealso cref="ICache.SetValueAsync{T}(string,T,CancellationToken)"/>
        public async Task<bool> SetValueAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                           {
                               var serializedValue = JsonConvert.SerializeObject(value);
                               return await GetDatabase().StringSetAsync(key, serializedValue);
                           },
                           cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("RedisCache threw exception", e);
                throw;
            }
        }

        /// <summary>
        /// Sets cache value asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cache value.</param>
        /// <param name="absoluteExpiration">The absolute time when the cache entry expires.</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        /// <seealso cref="ICache.SetValueAsync{T}(string,T,DateTimeOffset,CancellationToken)"/>
        public async Task<bool> SetValueAsync<T>(string key, T value, DateTimeOffset absoluteExpiration, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                           {
                               var serializedValue = JsonConvert.SerializeObject(value);
                               TimeSpan? expiration = absoluteExpiration - DateTimeOffset.Now;
                               return await GetDatabase().StringSetAsync(key, serializedValue, expiration);
                           },
                           cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("RedisCache threw exception", e);
                throw;
            }
        }

        /// <summary>
        /// Sets cache value asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cache value.</param>
        /// <param name="relativeExpiration">The relative time when the cache entry expires.</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        /// <seealso cref="ICache.SetValueAsync{T}(string,T,TimeSpan,CancellationToken)"/>
        public async Task<bool> SetValueAsync<T>(string key, T value, TimeSpan relativeExpiration, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                           {
                               var serializedValue = JsonConvert.SerializeObject(value);
                               return await GetDatabase().StringSetAsync(key, serializedValue, relativeExpiration);
                           },
                           cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("RedisCache threw exception", e);
                throw;
            }
        }

        private IDatabase GetDatabase()
        {
            return _redis.GetDatabase();
        }
    }
}