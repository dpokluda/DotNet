namespace CacheProvider;

/// <summary>
/// Acts as a factory for <see cref="ICache"/> instances of a certain type. This interface may be
/// easier to use than <see cref="ICache"/> in dependency injection scenarios.
/// </summary>
public interface ICacheProvider
{
    /// <summary>
    /// Constructs an <see cref="ICache"/> instance to access distributed cache.
    /// </summary>
    ICache GetCache();
}