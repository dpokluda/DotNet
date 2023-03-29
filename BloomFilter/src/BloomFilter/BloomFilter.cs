using System.Collections;
using Newtonsoft.Json;

namespace BloomFilter;

/// <summary>
/// Bloom filter implementations using Murmur3KirschMitzenmacher hash algorithm.
/// </summary>
/// <seealso cref="BloomFilter.BaseFilter"/>
public class BloomFilter : BaseFilter
{
    private readonly object _sync = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BloomFilter"/> class.
    /// </summary>
    [JsonConstructor]
    internal BloomFilter()
        : base()
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="BloomFilter"/> class.
    /// </summary>
    /// <param name="expectedElements">Number of expected elements stored in the bloom filter.</param>
    /// <param name="errorRate">Acceptable error rate (false positives).</param>
    public BloomFilter(int expectedElements, double errorRate)
        : base(expectedElements, errorRate)
    {
        _hashBits = new BitArray(Capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BloomFilter"/> class.
    /// </summary>
    /// <param name="capacity">Capacity of the bloom filter.</param>
    /// <param name="hashes">Number of hash functions to use when calculating positions.</param>
    public BloomFilter(int capacity, int hashes)
        : base(capacity, hashes)
    {
        _hashBits = new BitArray(Capacity);
    }

    /// <summary>
    /// Bloom filter value (the actual hash bits).
    /// </summary>
    [JsonIgnore]
    public BitArray HashBits => _hashBits;

    [JsonProperty(PropertyName = "HashBits")]
    private readonly BitArray _hashBits;

    /// <summary>
    /// Add value to the filter.
    /// </summary>
    /// <param name="element">The value.</param>
    /// <returns>
    /// Boolean value <c>true</c> if it succeeds, <c>false</c> if it fails.
    /// </returns>
    /// <seealso cref="BloomFilter.BaseFilter.Add(string)"/>
    /// <seealso cref="IBloomFilter.Add(string)"/>
    public override bool Add(string element)
    {
        bool added = false;
        var positions = ComputeHash(ToBytes(element));
        lock (_sync)
        {
            foreach (int position in positions)
            {
                if (!HashBits.Get(position))
                {
                    added = true;
                    HashBits.Set(position, true);
                }
            }
        }
        return added;
    }

    /// <summary>
    /// Tests whether the value is present in the filter.
    /// </summary>
    /// <param name="element">The value.</param>
    /// <returns>
    /// Boolean value <c>true</c> if the value is present in the filter, <c>false</c> if not.
    /// </returns>
    /// <seealso cref="BloomFilter.BaseFilter.Contains(string)"/>
    /// <seealso cref="IBloomFilter.Contains(string)"/>
    public override bool Contains(string element)
    {
        var positions = ComputeHash(ToBytes(element));
        lock (_sync)
        {
            foreach (int position in positions)
            {
                if (!HashBits.Get(position))
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Removes all values from the filter.
    /// </summary>
    /// <seealso cref="BloomFilter.BaseFilter.Clear()"/>
    /// <seealso cref="IBloomFilter.Clear()"/>
    public override void Clear()
    {
        lock (_sync)
        {
            HashBits.SetAll(false);
        }
    }
}