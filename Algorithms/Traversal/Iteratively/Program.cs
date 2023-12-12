using System;
using System.Collections.Generic;

namespace Iteratively;

class Program
{
    static void Main(string[] args)
    {
        var root = new Node<int>(1);
        root.Left = new Node<int>(2);
        root.Right = new Node<int>(3);
        root.Left.Left = new Node<int>(4);
        root.Left.Right = new Node<int>(5);

        Console.WriteLine("\n\nIn-order:");
        List<int> ordered = InOrder(root);
        foreach (var item in ordered)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();

        Console.WriteLine("\n\nLevel-order:");
        ordered = LevelOrder(root);
        foreach (var item in ordered)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();
    }

    private static List<int> InOrder(Node<int> root)
    {
        var result = new List<int>();
        var stack = new Stack<Node<int>>();

        var current = root;
        while (current != null || stack.Count > 0)
        {
            while (current != null)
            {
                stack.Push(current);
                current = current.Left;
            }

            current = stack.Pop();
            result.Add(current.Data);
            current = current.Right;
        }

        return result;
    }

    private static List<int> LevelOrder(Node<int> root)
    {
        var result = new List<int>();
        var queue = new Queue<Node<int>>();

        queue.Enqueue(root);
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            result.Add(node.Data);

            if (node.Left != null)
                queue.Enqueue(node.Left);
            if (node.Right != null)
                queue.Enqueue(node.Right);
        }

        return result;
    }
}