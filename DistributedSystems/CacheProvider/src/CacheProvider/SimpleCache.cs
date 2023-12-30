namespace CacheProvider
{
    public class SimpleCache : ICache
    {
        protected class SimpleCacheEntry
        {
            public object? Value { get; init; }
            public DateTimeOffset? Expiration { get; init; }
        }

        protected readonly Dictionary<string, SimpleCacheEntry> Cache = new();

        public Task<T> GetValueAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (Cache.TryGetValue(key, out SimpleCacheEntry? entry))
            {
                if (entry.Expiration.HasValue && entry.Expiration.Value < DateTimeOffset.UtcNow)
                {
                    throw new KeyNotFoundException("Cache entry with the specified key has already expired.");
                }

                if (entry.Value is T typedValue)
                {
                    return Task.FromResult(typedValue);
                }
                throw new InvalidCastException("Cache value is not of the specified type.");
                
                
            }
            throw new KeyNotFoundException("Cache entry with the specified key is not found.");
        }

        public Task<bool> SetValueAsync<T>(string key, T value, bool onlyIfNew = false, CancellationToken cancellationToken = default)
        {
            if (onlyIfNew && Cache.ContainsKey(key))
            {
                return Task.FromResult(false);
            }
            Cache[key] = new SimpleCacheEntry { Value = value };
            return Task.FromResult(true);
        }

        public Task<bool> SetValueAsync<T>(string key, T value, TimeSpan relativeExpiration, bool onlyIfNew = false, CancellationToken cancellationToken = default)
        {
            if (onlyIfNew && Cache.ContainsKey(key))
            {
                return Task.FromResult(false);
            }
            Cache[key] = new SimpleCacheEntry { Value = value, Expiration = DateTimeOffset.UtcNow + relativeExpiration };
            return Task.FromResult(true);
        }
        
        public Task<bool> DeleteValueAsync(string key, CancellationToken cancellationToken = default)
        {
            Cache.Remove(key);
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteValueAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : IEquatable<T>
        {
            if (value.Equals(await GetValueAsync<T>(key, cancellationToken)))
            {
                Cache.Remove(key);
                return true;
            }

            return false;
        }

        public Task<int> IncrementCounterAsync(string key, CancellationToken cancellationToken = default)
        {
            int value = 0;
            if (!Cache.ContainsKey(key))
            {
                value = 1;
            }
            else
            {
                value = Convert.ToInt32(Cache[key].Value) + 1;
            }

            Cache[key] = new SimpleCacheEntry { Value = value };
            return Task.FromResult(value);        
        }
        
        public Task<int> IncrementCounterAsync(string key, int maxValue, CancellationToken cancellationToken = default)
        {
            int value = 0;
            if (!Cache.ContainsKey(key))
            {
                value = 1;
            }
            else
            {
                value = Math.Min(maxValue, Convert.ToInt32(Cache[key].Value) + 1);
            }

            Cache[key] = new SimpleCacheEntry { Value = value };
            return Task.FromResult(value);        
        }
        
        public Task<int> DecrementCounterAsync(string key, CancellationToken cancellationToken = default)
        {
            if (!Cache.ContainsKey(key))
            {
                return Task.FromResult(0);
            }

            var value = Math.Max(0, Convert.ToInt32(Cache[key].Value) - 1);
            Cache[key] = new SimpleCacheEntry { Value = value };
            return Task.FromResult(value);
        }
        
        public Task<int> IncrementCounterAsync(string key, string id, TimeSpan expiration, CancellationToken cancellationToken = default)
        {
            // get counter ids
            SortedList<DateTimeOffset, string> ids;
            if (!Cache.ContainsKey(key))
            {
                ids = new SortedList<DateTimeOffset, string>();
            }
            else
            {
                ids = Cache[key].Value as SortedList<DateTimeOffset, string>;
                if (ids is null)
                {
                    throw new ArgumentException("Key doesn't represent counter with expirations");
                }
            }

            // remove expired
            var now = DateTimeOffset.UtcNow;
            RemoveExpired(ids, now);

            // increment the counter
            if (ids.ContainsValue(id))
            {
                throw new ArgumentException("The specified id is not unique within the key counter");
            }
            ids.Add(now + expiration, id);
            
            // update the counter
            Cache[key] = new SimpleCacheEntry { Value = ids };
            
            return Task.FromResult(ids.Count);
        }
        
        public Task<int> IncrementCounterAsync(string key, string id, TimeSpan expiration, int maxValue, CancellationToken cancellationToken = default)
        {
            // get counter ids
            SortedList<DateTimeOffset, string> ids;
            if (!Cache.ContainsKey(key))
            {
                ids = new SortedList<DateTimeOffset, string>();
            }
            else
            {
                ids = Cache[key].Value as SortedList<DateTimeOffset, string>;
                if (ids is null)
                {
                    throw new ArgumentException("Key doesn't represent counter with expirations");
                }
            }

            // remove expired
            var now = DateTimeOffset.UtcNow;
            RemoveExpired(ids, now);

            // check id uniqueness
            if (ids.ContainsValue(id))
            {
                throw new ArgumentException("The specified id is not unique within the key counter");
            }

            // increment the counter
            if (ids.Count < maxValue)
            {
                ids.Add(now + expiration, id);
            }
            // update the counter
            Cache[key] = new SimpleCacheEntry { Value = ids };
            
            return Task.FromResult(ids.Count);
        }
        
        public Task<int> DecrementCounterAsync(string key, string id, CancellationToken cancellationToken = default)
        {
            // get counter ids
            SortedList<DateTimeOffset, string> ids;
            if (!Cache.ContainsKey(key))
            {
                ids = new SortedList<DateTimeOffset, string>();
            }
            else
            {
                ids = Cache[key].Value as SortedList<DateTimeOffset, string>;
                if (ids is null)
                {
                    throw new NotSupportedException();
                }
            }

            // remove expired
            var now = DateTimeOffset.UtcNow;
            RemoveExpired(ids, now);

            // update the counter
            Cache[key] = new SimpleCacheEntry { Value = ids };
            
            return Task.FromResult(ids.Count);
        }

        public Task<int> GetCounterAsync(string key, CancellationToken cancellationToken = default)
        {
            // get counter ids
            SortedList<DateTimeOffset, string> ids;
            if (!Cache.ContainsKey(key))
            {
                return Task.FromResult(0);
            }

            // if simple counter, then simply return value
            if (Cache[key].Value is int)
            {
                return Task.FromResult((int) Cache[key].Value);
            }
            
            // if counter with expirations; we need to remove expired ids first
            ids = Cache[key].Value as SortedList<DateTimeOffset, string>;
            if (ids is null)
            {
                throw new NotSupportedException();
            }

            // remove expired
            var now = DateTimeOffset.UtcNow;
            RemoveExpired(ids, now);
            
            return Task.FromResult(ids.Count);
        }
        
        private static void RemoveExpired(SortedList<DateTimeOffset, string>? ids, DateTimeOffset now)
        {
            var toRemove = 
                from k in ids.Keys
                where k < now
                select k;
            foreach (var keyToRemove in toRemove)
            {
                ids.Remove(keyToRemove);
            }
        }
    }
}