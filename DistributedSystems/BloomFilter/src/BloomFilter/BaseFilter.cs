using System.Text;
using BloomFilter.HashAlgorithms;
using Newtonsoft.Json;

namespace BloomFilter;

/// <summary>
/// An abstract base class for bloom filter implementations using Murmur3KirschMitzenmacher hash algorithm.
/// </summary>
/// <seealso cref="BloomFilter.IBloomFilter"/>
public abstract class BaseFilter : IBloomFilter
{
    /// <summary>
    /// Hash function used when calculating value positions.
    /// </summary>
    protected HashFunction Hash { get; } = new Murmur3KirschMitzenmacher();

    /// <summary>
    /// Capacity of the bloom filter.
    /// </summary>
    [JsonIgnore]
    public int Capacity => _capacity;

    [JsonProperty(PropertyName = "Capacity")]
    private int _capacity;

    /// <summary>
    /// Number of hash functions to use when calculating positions.
    /// </summary>
    [JsonIgnore]
    public int Hashes => _hashes;

    [JsonProperty(PropertyName = "Hashes")]
    private int _hashes;

    /// <summary>
    /// Number of expected elements stored in the bloom filter.
    /// </summary>
    [JsonIgnore]
    public int ExpectedElements => _expectedElements;

    [JsonProperty(PropertyName = "ExpectedElements")]
    private readonly int _expectedElements;

    /// <summary>
    /// Acceptable error rate (false positives).
    /// </summary>
    [JsonIgnore]
    public double ErrorRate => _errorRate;

    [JsonProperty(PropertyName = "ErrorRate")]
    private readonly double _errorRate;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFilter"/> class. 
    /// </summary>
    /// <remarks>
    /// This particular constructor is only used for deserialization.
    /// </remarks>
    internal BaseFilter()
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFilter" /> class.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the required range.</exception>
    /// <param name="expectedElements">The expected number of elements in the bloom filter.</param>
    /// <param name="errorRate">The acceptable false positive error rate.</param>
    public BaseFilter(int expectedElements, double errorRate)
    {
        if (expectedElements < 1)
            throw new ArgumentOutOfRangeException(nameof(expectedElements), "Argument expectedElements value must be greater than 0.");
        if (errorRate >= 1 || errorRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(errorRate), "Argument errorRate value must be between 0 and 1, exclusive.");

        _expectedElements = expectedElements;
        _errorRate = errorRate;

        _capacity = BestM(expectedElements, errorRate);
        _hashes = BestK(expectedElements, Capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFilter" /> class.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the required range.</exception>
    /// <param name="capacity">The bloom filter capacity.</param>
    /// <param name="hashes">The number of hash functions to use.</param>
    public BaseFilter(int capacity, int hashes)
    {
        if (capacity < 1)
            throw new ArgumentOutOfRangeException("capacity", capacity, "capacity must be > 0");
        if (hashes < 1)
            throw new ArgumentOutOfRangeException("hashes", hashes, "hashes must be > 0");

        _capacity = capacity;
        _hashes = hashes;

        _expectedElements = BestN(hashes, capacity);
        _errorRate = BestP(hashes, capacity, ExpectedElements);
    }

    /// <summary>
    /// Add value to the filter.
    /// </summary>
    /// <param name="element">The value.</param>
    /// <returns>
    /// Boolean value <c>true</c> if it succeeds, <c>false</c> if it fails.
    /// </returns>
    /// <seealso cref="IBloomFilter.Add(string)"/>
    public abstract bool Add(string element);

    /// <summary>
    /// Tests whether the value is present in the filter.
    /// </summary>
    /// <param name="element">The value.</param>
    /// <returns>
    /// Boolean value <c>true</c> if the value is present in the filter, <c>false</c> if not.
    /// </returns>
    /// <seealso cref="IBloomFilter.Contains(string)"/>
    public abstract bool Contains(string element);

    /// <summary>
    /// Removes all values from the filter.
    /// </summary>
    /// <seealso cref="IBloomFilter.Clear()"/>
    public abstract void Clear();

    /// <summary>
    /// Calculates the hash.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>
    /// The calculated hash.
    /// </returns>
    protected int[] ComputeHash(byte[] data)
    {
        return Hash.ComputeHash(data, Capacity, Hashes);
    }

    /// <summary>
    /// Converts a string value to byte array.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>
    /// Byte array.
    /// </returns>
    protected virtual byte[] ToBytes(string value)
    {
        return Encoding.UTF8.GetBytes(value!);
    }

    /// <summary>
    /// Calculates the optimal size of the bloom filter in bits given expectedElements (expected number of elements in bloom filter) and falsePositiveProbability (tolerable false
    /// positive rate).
    /// </summary>
    /// <param name="n">Expected number of elements inserted in the bloom filter.</param>
    /// <param name="p">Tolerable false positive rate.</param>
    /// <returns>
    /// The optimal siz of the bloom filter in bits.
    /// </returns>
    public static int BestM(long n, double p)
    {
        return (int)Math.Ceiling(-1 * (n * Math.Log(p)) / Math.Pow(Math.Log(2), 2));
    }

    /// <summary>
    /// Calculates the optimal hashes (number of hash function) given expectedElements (expected number of elements in bloom filter) and size (size of bloom filter in bits).
    /// </summary>
    /// <param name="n">Expected number of elements inserted in the bloom filter.</param>
    /// <param name="m">The size of the bloom filter in bits.</param>
    /// <returns>
    /// The optimal amount of hash functions hashes.
    /// </returns>
    public static int BestK(long n, long m)
    {
        return (int)Math.Ceiling((Math.Log(2) * m) / n);
    }

    /// <summary>
    /// Calculates the amount of elements a bloom filter for which the given configuration of size and hashes is optimal.
    /// </summary>
    /// <param name="k">Number of hashes.</param>
    /// <param name="m">The size of the bloom filter in bits.</param>
    /// <returns>
    /// Number of elements a bloom filter for which the given configuration of size and hashes is optimal.
    /// </returns>
    public static int BestN(long k, long m)
    {
        return (int)Math.Ceiling((Math.Log(2) * m) / k);
    }

    /// <summary>
    /// Calculates the best-case (uniform hash function) false positive probability.
    /// </summary>
    /// <param name="k">Number of hashes.</param>
    /// <param name="m">The size of the bloom filter in bits.</param>
    /// <param name="insertedElements">Number of elements inserted in the filter.</param>
    /// <returns>
    /// The calculated false positive probability.
    /// </returns>
    public static double BestP(long k, long m, double insertedElements)
    {
        return Math.Pow((1 - Math.Exp(-k * insertedElements / (double)m)), k);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    /// <seealso cref="System.Object.ToString()"/>
    public override string ToString()
    {
        return $"Capacity:{Capacity},Hashes:{Hashes},ExpectedElements:{ExpectedElements},ErrorRate:{ErrorRate}";
    }
}