using System.Text;
using Sharding.HashAlgorithms;

namespace Sharding;

/// <summary>
/// Sharding implementation using consistent hashing.
/// </summary>
/// <seealso cref="Sharding.ISharding"/>
public class ConsistentHashSharding : ISharding
{
    private const int DefaultNumberOfReplicas = 100;
    private static readonly HashFunction _hash = new Murmur2();

    protected readonly SortedDictionary<int, string> Circle;
    private readonly int _numberOfReplicas;
    private int[] _keys;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsistentHashSharding"/> class.
    /// </summary>
    /// <param name="nodes">The sharding nodes.</param>
    /// <param name="numberOfReplicas">Number of replicas.</param>
    public ConsistentHashSharding(IEnumerable<string> nodes, int numberOfReplicas)
    {
        Circle = new SortedDictionary<int, string>();
        _numberOfReplicas = numberOfReplicas;
        foreach (string node in nodes)
        {
            AddNodeToCircle(node);
        }

        _keys = Circle.Keys.ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsistentHashSharding"/> class.
    /// </summary>
    /// <param name="nodes">The sharding nodes.</param>
    public ConsistentHashSharding(IEnumerable<string> nodes)
        : this(nodes, DefaultNumberOfReplicas)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsistentHashSharding"/> class.
    /// </summary>
    /// <param name="numberOfReplicas">Number of replicas.</param>
    public ConsistentHashSharding(int numberOfReplicas)
    {
        Circle = new SortedDictionary<int, string>();
        _numberOfReplicas = numberOfReplicas;
        _keys = Array.Empty<int>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsistentHashSharding"/> class.
    /// </summary>
    public ConsistentHashSharding()
        : this(DefaultNumberOfReplicas)
    { }

    /// <summary>
    /// Adds a sharding node.
    /// </summary>
    /// <param name="node">The node name/identifier.</param>
    /// <seealso cref="ISharding.AddNode(string)"/>
    public void AddNode(string node)
    {
        AddNodeToCircle(node);
        _keys = Circle.Keys.ToArray();
    }

    /// <summary>
    /// Removes a sharding node.
    /// </summary>
    /// <param name="node">The node name/identifier.</param>
    /// <seealso cref="ISharding.RemoveNode(string)"/>
    public void RemoveNode(string node)
    {
        for (int i = 0; i < _numberOfReplicas; i++)
        {
            int hash = _hash.ComputeHash(ToBytes(node + i));
            Circle.Remove(hash);
        }
        
        _keys = Circle.Keys.ToArray();
    }

    /// <summary>
    /// Gets node for a given key.
    /// </summary>
    /// <param name="key">The key to shard to a node.</param>
    /// <returns>
    /// The node name/identifier where the key belongs.
    /// </returns>
    /// <seealso cref="ISharding.GetNodeForKey(string)"/>
    public string GetNodeForKey(string key)
    {
        int hash = _hash.ComputeHash(ToBytes(key));
        int first = FindFirstNode(_keys, hash);
        if (_keys.Length > 0 && first >= _keys.Length)
        {
            first %= _keys.Length;
        }

        return Circle[_keys[first]];
    }

    /// <summary>
    /// Converts an element to the bytes.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// Element as a byte[].
    /// </returns>
    protected virtual byte[] ToBytes(string element)
    {
        return Encoding.UTF8.GetBytes(element!);
    }

    private void AddNodeToCircle(string node)
    {
        for (int i = 0; i < _numberOfReplicas; i++)
        {
            int hash = _hash.ComputeHash(ToBytes(node + i));
            Circle[hash] = node;
        }
    }

    private int FindFirstNode(int[] ay, int val)
    {
        int result = Array.BinarySearch(ay, 0, ay.Length, val);
        if (result < 0)
        {
            // not found; let's return the first value greater than our value
            return ~result;
        }

        return result;
    }
}