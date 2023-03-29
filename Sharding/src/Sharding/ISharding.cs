namespace Sharding;

/// <summary>
/// Interface for key sharding.
/// </summary>
public interface ISharding
{
    /// <summary>
    /// Adds a sharding node.
    /// </summary>
    /// <param name="node">The node name/identifier.</param>
    void AddNode(string node);

    /// <summary>
    /// Removes a sharding node.
    /// </summary>
    /// <param name="node">The node name/identifier.</param>
    void RemoveNode(string node);

    /// <summary>
    /// Gets node for a given key.
    /// </summary>
    /// <param name="key">The key to shard to a node.</param>
    /// <returns>
    /// The node name/identifier where the key belongs.
    /// </returns>
    string GetNodeForKey(string key);
}