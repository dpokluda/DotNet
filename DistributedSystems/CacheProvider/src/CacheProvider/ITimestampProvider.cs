namespace CacheProvider;

/// <summary>
/// Factory to generate current timestamp..
/// </summary>
public interface ITimestampProvider
{
    /// <summary>
    /// Get current timestamp.
    /// </summary>
    /// <returns>Current timestamp value.</returns>
    long Get();
}