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

        public Task<bool> SetValueAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            Cache[key] = new SimpleCacheEntry { Value = value };
            return Task.FromResult(true);
        }

        public Task<bool> SetValueAsync<T>(string key, T value, DateTimeOffset absoluteExpiration, CancellationToken cancellationToken = default)
        {
            Cache[key] = new SimpleCacheEntry { Value = value, Expiration = absoluteExpiration };
            return Task.FromResult(true);
        }

        public Task<bool> SetValueAsync<T>(string key, T value, TimeSpan relativeExpiration, CancellationToken cancellationToken = default)
        {
            Cache[key] = new SimpleCacheEntry { Value = value, Expiration = DateTimeOffset.UtcNow + relativeExpiration };
            return Task.FromResult(true);
        }
    }
}
