using System.Diagnostics;
using Sharding;

namespace ShardingTests;

[TestClass]
public class Demo
{
    [TestMethod]
    public void Run()
    {
        // create sharding component (with default 100 replicas per node)
        var sharding = new ConsistentHashSharding();

        // define nodes
        var nodes = new List<string> { "Node-1", "Node-2", "Node-3", "Node-4", "Node-5" };
        
        // register sharding nodes
        foreach (var node in nodes)
        {
            sharding.AddNode(node);
        }
        RunSteps(nodes, sharding);
        
        // let's remove one node
        nodes.Remove("Node-3");
        sharding.RemoveNode("Node-3");
        RunSteps(nodes, sharding);
        
        // add the node back in
        nodes.Add("Node-3");
        sharding.AddNode("Node-3");
        RunSteps(nodes, sharding);
        
        // add a brand new node
        nodes.Add("Node-6");
        sharding.AddNode("Node-6");
        RunSteps(nodes, sharding);
    }

    private static void RunSteps(List<string> nodes, ConsistentHashSharding sharding)
    {
        // sharding keys to existing nodes
        var counts = new Dictionary<string, int>();
        foreach (var node in nodes)
        {
            counts.Add(node, 0);
        }

        for (int i = 1; i < 100; i++)
        {
            var key = $"ValueKey-{i}";
            var node = sharding.GetNodeForKey(key);
            counts[node]++;
        }

        // display stats
        Trace.WriteLine($"Stats for {nodes.Count} nodes:");
        foreach (var node in nodes)
        {
            Trace.WriteLine($"{node}: {counts[node]}");
        }

        Trace.WriteLine("--------------------------");
    }

    private void RunStep()
    {
        
    }
}