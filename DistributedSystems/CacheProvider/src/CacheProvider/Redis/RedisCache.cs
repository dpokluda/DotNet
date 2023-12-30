﻿using System.Net.Sockets;
using CacheProvider;
using CacheProvider.Configuration;
using CacheProvider.Lua;
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
        private readonly ITimestampProvider _timestampProvider;
        private readonly AsyncRetryPolicy _policy;
        private readonly ILogger<RedisCache> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache"/> class.
        /// </summary>
        /// <param name="config">The Redis configuration.</param>
        /// <param name="timestampProvider">Factory class to generate current timestamp.</param>
        /// <param name="logger">The logger.</param>
        public RedisCache( IOptions<RedisConfiguration> config, ITimestampProvider timestampProvider, ILogger<RedisCache> logger)
        {
            RedisLazyReconnect.InitializeConnectionString(config.Value.ConnectionString);
            _logger = logger;
            _timestampProvider = timestampProvider;
            _redis = RedisLazyReconnect.Connection;

            _policy = Policy
                      .Handle<RedisConnectionException>()
                      .Or<SocketException>()
                      .Or<ObjectDisposedException>()
                      .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt-1)),
                          (exception, timeSpan, retryCount, context) =>
                          {
                              RedisLazyReconnect.ForceReconnect();
                              _redis = RedisLazyReconnect.Connection;
                          });
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
        /// <param name="onlyIfNew">(Optional) Boolean flag indicating whether we should only set the key if it does not already exist.</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        /// <seealso cref="ICache.SetValueAsync{T}(string,T,CancellationToken)"/>
        public async Task<bool> SetValueAsync<T>(string key, T value, bool onlyIfNew = false, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                           {
                               var serializedValue = JsonConvert.SerializeObject(value);
                               return await GetDatabase().StringSetAsync(key, serializedValue, null, onlyIfNew ? When.NotExists : When.Always);
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
        /// <param name="onlyIfNew">(Optional) Boolean flag indicating whether we should only set the key if it does not already exist.</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        /// <seealso cref="ICache.SetValueAsync{T}(string,T,TimeSpan,CancellationToken)"/>
        public async Task<bool> SetValueAsync<T>(string key, T value, TimeSpan relativeExpiration, bool onlyIfNew = false, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                           {
                               var serializedValue = JsonConvert.SerializeObject(value);
                               return await GetDatabase().StringSetAsync(key, serializedValue, relativeExpiration, onlyIfNew ? When.NotExists : When.Always);
                           },
                           cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("RedisCache threw exception", e);
                throw;
            }
        }

        public async Task<bool> DeleteValueAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                           {
                               return await GetDatabase().KeyDeleteAsync(key);
                           },
                           cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("RedisCache threw exception", e);
                throw;
            }
        }

        public async Task<bool> DeleteValueAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : IEquatable<T>
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                           {

                               if (typeof(T) == typeof(string))
                               {
                                   var result = await GetDatabase().ScriptEvaluateAsync(
                                       LuaResource.DeleteValue, 
                                       new
                                       {
                                           key = (RedisKey)key, 
                                           value = $"\"{value.ToString()}\""
                                       });
                                   return (int)result == 1;
                               }
                               else
                               {
                                   var result = await GetDatabase().ScriptEvaluateAsync(
                                       LuaResource.DeleteValue, 
                                       new
                                       {
                                           key = (RedisKey)key, 
                                           value
                                       });
                                   return (int)result == 1;
                               }
                           },
                           cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("RedisCache threw exception", e);
                throw;
            }
        }

        public async Task<int> GetCounterAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                    {
                        var result = await GetDatabase().ScriptEvaluateAsync(
                            LuaResource.GetCounter, 
                            new
                            {
                                key = (RedisKey)key, 
                                currentTime = _timestampProvider.Get()
                            });
                        return (int)result;
                    },
                    cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("RedisCache threw exception", e);
                throw;
            }
        }

        public Task<int> IncrementCounterAsync(string key, CancellationToken cancellationToken = default)
        {
            return IncrementCounterAsync(key, Int32.MaxValue, cancellationToken);
        }

        public async Task<int> IncrementCounterAsync(string key, int maxValue, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                    {
                        var result = await GetDatabase().ScriptEvaluateAsync(
                            LuaResource.IncrementCounter, 
                            new
                            {
                                key = (RedisKey)key, 
                                maxValue
                            });
                        return (int)result;
                    },
                    cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("RedisCache threw exception", e);
                throw;
            }
        }

        public async Task<int> DecrementCounterAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                    {
                        var result = await GetDatabase().ScriptEvaluateAsync(
                            LuaResource.DecrementCounter, 
                            new
                            {
                                key = (RedisKey)key
                            });
                        return (int)result;
                    },
                    cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("RedisCache threw exception", e);
                throw;
            }
        }
        
        public Task<int> IncrementCounterAsync(string key, string id, TimeSpan expiration, CancellationToken cancellationToken = default)
        {
            return IncrementCounterAsync(key, id, expiration, Int32.MaxValue, cancellationToken);
        }

        public async Task<int> IncrementCounterAsync(string key, string id, TimeSpan expiration, int maxValue, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                    {
                        var result = await GetDatabase().ScriptEvaluateAsync(
                            LuaResource.IncrementCounterWithExpiration, 
                            new
                            {
                                key = (RedisKey)key,
                                value = id,
                                currentTime = _timestampProvider.Get(),
                                expirationTime = expiration.TotalMilliseconds,
                                maxValue = Int32.MaxValue
                            });
                        return (int)result;
                    },
                    cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("RedisCache threw exception", e);
                throw;
            }
        }

        public async Task<int> DecrementCounterAsync(string key, string id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _policy.ExecuteAsync(async (ct) =>
                    {
                        var result = await GetDatabase().ScriptEvaluateAsync(
                            LuaResource.DecrementCounterWithExpiration, 
                            new
                            {
                                key = (RedisKey)key,
                                value = id,
                                currentTime = _timestampProvider.Get(),
                            });
                        return (int)result;
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