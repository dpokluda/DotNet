namespace CacheProvider
{
    /// <summary>
    /// Interface for accessing and manipulating cache objects.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Gets cache value asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache value.
        /// </returns>
        Task<T> GetValueAsync<T>(string key, CancellationToken cancellationToken = default);

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
        Task<bool> SetValueAsync<T>(string key, T value, bool onlyIfNew = false, CancellationToken cancellationToken = default);

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
        Task<bool> SetValueAsync<T>(string key, T value, TimeSpan relativeExpiration, bool onlyIfNew = false, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Deletes cache entry asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        Task<bool> DeleteValueAsync(string key, CancellationToken cancellationToken = default);

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
        Task<bool> DeleteValueAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : IEquatable<T>;

        /// <summary>
        /// Retrieves current cache counter value asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache counter value.
        /// </returns>
        Task<int> GetCounterAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increments cache counter by one asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache counter value.
        /// </returns>
        Task<int> IncrementCounterAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increments cache counter by one asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="maxValue">The maximum counter value.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache counter value.
        /// </returns>
        Task<int> IncrementCounterAsync(string key, int maxValue, CancellationToken cancellationToken = default);

        /// <summary>
        /// Decrements cache counter by one asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache counter value.
        /// </returns>
        Task<int> DecrementCounterAsync(string key, CancellationToken cancellationToken = default);

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
        Task<int> IncrementCounterAsync(string key, string id, TimeSpan expiration, CancellationToken cancellationToken = default);

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
        Task<int> IncrementCounterAsync(string key, string id, TimeSpan expiration, int maxValue, CancellationToken cancellationToken = default);

        /// <summary>
        /// Decrements cache counter by one asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="id">Unique identifier used to decrement the counter before the expiration.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The cache counter value.
        /// </returns>
        Task<int> DecrementCounterAsync(string key, string id, CancellationToken cancellationToken = default);
    }
}