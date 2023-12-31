using Newtonsoft.Json;

namespace CacheProvider
{
    /// <summary>
    /// Simple in-memory cache implementation that stores all data in a dictionary.
    /// </summary>
    public class MemoryCache : ICache
    {
        // cache is keyed by counter name (key)
        // - each cache value contain the actual counter values
        //        - counter values is sorted list sorted by expiration date
        //        - sorted list values are actual ids (it is possible that counter is incremented multiple times at the same time
        private readonly Dictionary<string, SortedList<long, List<string>>> _cache = new();
        // separate cache for simple values
        private readonly Dictionary<string, string> _simpleCache = new();
        
        private readonly ITimestampProvider _timestampProvider;

        public MemoryCache(ITimestampProvider timestampProvider)
        {
            _timestampProvider = timestampProvider;
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
        public Task<bool> SetSimpleValueAsync<T>(string key, T value, bool onlyIfNew = false, CancellationToken cancellationToken = default)
        {
            if (onlyIfNew && _simpleCache.ContainsKey(key))
            {
                return Task.FromResult(false);
            }
            _simpleCache[key] = JsonConvert.SerializeObject(value);
            return Task.FromResult(true);        
        }

        /// <summary>
        /// Gets cache value asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache value.
        /// </returns>
        public Task<T> GetSimpleValueAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (_simpleCache.TryGetValue(key, out string? value))
            {

                try
                {
                    return Task.FromResult(JsonConvert.DeserializeObject<T>(value));
                }
                catch (Exception e)
                {
                    throw new InvalidCastException(e.Message, e);
                }
            }
            throw new KeyNotFoundException("Cache entry with the specified key is not found.");        
        }

        /// <summary>
        /// Deletes cache entry asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public Task<bool> DeleteSimpleValueAsync(string key, CancellationToken cancellationToken = default)
        {
            _simpleCache.Remove(key);

            return Task.FromResult(true);        
        }

        /// <summary>
        /// Deletes cache entry asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cache value (if the cached value is different, then don't delete).</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public async Task<bool> DeleteSimpleValueAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : IEquatable<T>
        {
            if (value.Equals(await GetSimpleValueAsync<T>(key, cancellationToken)))
            {
                await DeleteSimpleValueAsync(key, cancellationToken);
                return true;
            }

            return false;        
        }

        /// <summary>
        /// Sets cache value asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cache value.</param>
        /// <param name="expiration">The relative time when the cache entry expires.</param>
        /// <param name="onlyIfNew">(Optional) Boolean flag indicating whether we should only set the key if it does not already exist.</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public Task<bool> SetValueAsync<T>(string key, T value, TimeSpan expiration, bool onlyIfNew = false, CancellationToken cancellationToken = default)
        {
            if (onlyIfNew && _cache.ContainsKey(key))
            {
                return Task.FromResult(false);
            }

            _cache[key] = new SortedList<long, List<string>>(new Dictionary<long, List<string>>()
            {
                {
                    _timestampProvider.Get() + (long)expiration.TotalMilliseconds,
                    new List<string>()
                    {
                        JsonConvert.SerializeObject(value)
                    }
                }
            });
            
            return Task.FromResult(true);
        }
        
        /// <summary>
        /// Gets cache value asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache value.
        /// </returns>
        public Task<T> GetValueAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(key, out SortedList<long, List<string>>? entry))
            {
                var now = _timestampProvider.Get();
                RemoveExpired(entry, now);
                if (entry.Count == 1)
                {
                    try
                    {
                        return Task.FromResult(JsonConvert.DeserializeObject<T>(entry[entry.Keys[0]][0]));
                    }
                    catch (Exception e)
                    {
                        throw new InvalidCastException(e.Message, e);
                    }
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            
            throw new KeyNotFoundException("Cache entry with the specified key is not found.");
        }

        /// <summary>
        /// Deletes cache entry asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cache value (if the cached value is different, then don't delete).</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public async Task<bool> DeleteValueAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : IEquatable<T>
        {
            if (value.Equals(await GetValueAsync<T>(key, cancellationToken)))
            {
                _cache.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Increments cache counter by one asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="id">Unique identifier used to decrement the counter before the expiration.</param>
        /// <param name="expiration">The time period after which the semaphore will be automatically released.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache counter value.
        /// </returns>
        public Task<int> IncrementCounterAsync(string key, string id, TimeSpan expiration, CancellationToken cancellationToken = default)
        {
            return IncrementCounterAsync(key, id, expiration, Int32.MaxValue, cancellationToken);
        }
        
        /// <summary>
        /// Increments cache counter by one asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="id">Unique identifier used to decrement the counter before the expiration.</param>
        /// <param name="expiration">The time period after which the semaphore will be automatically released.</param>
        /// <param name="maxValue">The maximum counter value.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache counter value.
        /// </returns>
        public Task<int> IncrementCounterAsync(string key, string id, TimeSpan expiration, int maxValue, CancellationToken cancellationToken = default)
        {
            var now = _timestampProvider.Get();
            if (!_cache.TryGetValue(key, out SortedList<long, List<string>>? entry))
            {
                // create new
                entry = new SortedList<long, List<string>>();
                _cache[key] = entry;
            }
            else
            {
                // clean expired items
                RemoveExpired(entry, now);
                
                if (entry.Values
                    .SelectMany(values => values)
                    .Any(value => value == id))
                {
                    // already exists
                    return Task.FromResult(GetCounterCount(entry));
                }
            }
            
            // increment the counter
            if (entry.Count < maxValue)
            {
                if (entry.ContainsKey(now + (long)expiration.TotalMilliseconds))
                {
                    entry[now + (long)expiration.TotalMilliseconds].Add(id);
                }
                else
                {
                    entry.Add(now + (long)expiration.TotalMilliseconds, new List<string>() { id });
                }
            }
            
            return Task.FromResult(GetCounterCount(entry));
        }
        
        /// <summary>
        /// Decrements cache counter by one asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="id">Unique identifier used to decrement the counter before the expiration.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache counter value.
        /// </returns>
        public Task<int> DecrementCounterAsync(string key, string id, CancellationToken cancellationToken = default)
        {
            var now = _timestampProvider.Get();
            if (!_cache.TryGetValue(key, out SortedList<long, List<string>>? entry))
            {
                // counter doesn't exist
                return Task.FromResult(0);
            }
            
            // clean expired items
            RemoveExpired(entry, now);
            
            // remove item with value `id`
            var keysToRemove = new List<long>();
            foreach (var entryIds in entry)
            {
                if (entryIds.Value.Count == 1 && entryIds.Value[0] == id)
                {
                    keysToRemove.Add(entryIds.Key);
                }
                else
                {
                    entryIds.Value.Remove(id);
                }
            }

            foreach (var keyToRemove in keysToRemove.ToArray())
            {
                entry.Remove(keyToRemove);
            }

            return Task.FromResult(GetCounterCount(entry));
        }

        /// <summary>
        /// Retrieves current cache counter value asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache counter value.
        /// </returns>
        public Task<int> GetCounterAsync(string key, CancellationToken cancellationToken = default)
        {
            var now = _timestampProvider.Get();
            if (!_cache.TryGetValue(key, out SortedList<long, List<string>>? entry))
            {
                // counter doesn't exist
                return Task.FromResult(0);
            }
            
            // clean expired items
            RemoveExpired(entry, now);
            
            return Task.FromResult(GetCounterCount(entry));
        }

        private static int GetCounterCount(SortedList<long, List<string>> entry)
        {
            return entry.Values.Sum(ids => ids.Count);
        }

        private static void RemoveExpired(SortedList<long, List<string>> ids, long now)
        {
            var expiredKeys = 
                from expiration in ids.Keys
                where expiration <= now
                select expiration;
            
            foreach (var key in expiredKeys.ToArray())
            {
                ids.Remove(key);
            }
        }
    }
}