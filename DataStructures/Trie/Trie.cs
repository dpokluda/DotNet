using System.Collections.Generic;

namespace Trie;

public class Trie
{
    private class Node
    {
        public Dictionary<char, Node> Children;
        public bool IsTerminalNode;

        public Node()
        {
            Children = new Dictionary<char, Node>();
            IsTerminalNode = false;
        }
    }
    
    private Node _root;

    public Trie()
    {
        _root = new Node();
    }

    public void Insert(string item)
    {
        var currentNode = _root;
        foreach(char ch in item)
        {
            if (currentNode.Children.ContainsKey(ch))
            {
                currentNode = currentNode.Children[ch];
            }
            else
            {
                var newNode = new Node();
                currentNode.Children.Add(ch, newNode);
                currentNode = newNode;
            }
        }
        currentNode.IsTerminalNode = true;
    }

    public bool Search(string item)
    {
        var currentNode = _root;
        foreach(char ch in item)
        {
            if (currentNode.Children.ContainsKey(ch))
            {
                currentNode = currentNode.Children[ch];
            }
            else
            {
                return false;
            }
        }
        return currentNode.IsTerminalNode;
    }
}
