using Sharding;

namespace ShardingTests.Components;

public class TestableConsistentHashSharding : ConsistentHashSharding
{
    public TestableConsistentHashSharding(IEnumerable<string> nodes, int numberOfReplicas)
        : base(nodes, numberOfReplicas)
    { }

    public TestableConsistentHashSharding(IEnumerable<string> nodes)
        : base(nodes)
    { }

    public TestableConsistentHashSharding(int numberOfReplicas)
        : base(numberOfReplicas)
    { }

    public TestableConsistentHashSharding()
    { }

    public SortedDictionary<int, string> GetCircle()
    {
        return Circle;
    }
}