namespace BloomFilter;

/// <summary>
/// Bloom filter interface.
/// </summary>
public interface IBloomFilter
{
    /// <summary>
    /// Add value to the filter.
    /// </summary>
    /// <param name="element">The value.</param>
    /// <returns>
    /// Boolean value <c>true</c> if it succeeds, <c>false</c> if it fails.
    /// </returns>
    bool Add(string element);

    /// <summary>
    /// Tests whether the value is present in the filter.
    /// </summary>
    /// <param name="element">The value.</param>
    /// <returns>
    /// Boolean value <c>true</c> if the value is present in the filter, <c>false</c> if not.
    /// </returns>
    bool Contains(string element);

    /// <summary>
    /// Removes all values from the filter.
    /// </summary>
    void Clear();
}