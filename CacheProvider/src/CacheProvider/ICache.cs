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
        Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes cache entry asynchronously.
        /// </summary>
        /// <typeparam name="T">The cache value type.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="valueAsString">The cache value as string.</param>
        /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        Task<bool> DeleteAsync(string key, string valueAsString, CancellationToken cancellationToken = default);
    }
}