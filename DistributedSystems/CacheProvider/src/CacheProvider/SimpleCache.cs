using Newtonsoft.Json;

namespace CacheProvider
{
    public class SimpleCache : ICache
    {
        // cache is keyed by counter name (key)
        // - each cache value contain the actual counter values
        //        - counter values is sorted list sorted by expiration date
        //        - sorted list values are actual ids (it is possible that counter is incremented multiple times at the same time
        private readonly Dictionary<string, SortedList<long, List<string>>> _cache = new();
        private readonly ITimestampProvider _timestampProvider;

        public SimpleCache(ITimestampProvider timestampProvider)
        {
            _timestampProvider = timestampProvider;
        }

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
                    throw new InvalidDataException();
                }
            }
            
            throw new KeyNotFoundException("Cache entry with the specified key is not found.");
        }

        public async Task<bool> DeleteValueAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : IEquatable<T>
        {
            if (value.Equals(await GetValueAsync<T>(key, cancellationToken)))
            {
                _cache.Remove(key);
                return true;
            }

            return false;
        }

        public Task<int> IncrementCounterAsync(string key, string id, TimeSpan expiration, CancellationToken cancellationToken = default)
        {
            return IncrementCounterAsync(key, id, expiration, Int32.MaxValue, cancellationToken);
        }
        
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