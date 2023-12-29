﻿namespace CacheProvider
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

        public Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            Cache.Remove(key);
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : IEquatable<T>
        {
            if (value.Equals(await GetValueAsync<T>(key, cancellationToken)))
            {
                Cache.Remove(key);
                return true;
            }

            return false;
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

        public Task<int> DecrementValueAsync(string key, CancellationToken cancellationToken = default)
        {
            if (!Cache.ContainsKey(key))
            {
                return Task.FromResult(0);
            }

            var value = Math.Max(0, Convert.ToInt32(Cache[key].Value) - 1);
            Cache[key] = new SimpleCacheEntry { Value = value };
            return Task.FromResult(value);
        }

        public Task<int> IncrementValueAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
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

            Cache[key] = new SimpleCacheEntry { Value = value, Expiration = DateTimeOffset.UtcNow + expiration
            return Task.FromResult(value);        
        }
        
        public Task<int> IncrementValueAsync(string key, int maxValue, TimeSpan expiration, CancellationToken cancellationToken = default)
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

            Cache[key] = new SimpleCacheEntry { Value = value, Expiration = DateTimeOffset.UtcNow + expiration };
            return Task.FromResult(value);        
        }
    }
}
