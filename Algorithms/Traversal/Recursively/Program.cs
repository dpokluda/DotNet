namespace Recursively;

class Program
{
    static void Main(string[] args)
    {
        var root = new Node<int>(1);
        root.Left = new Node<int>(2);
        root.Right = new Node<int>(3);
        root.Left.Left = new Node<int>(4);
        root.Left.Right = new Node<int>(5);

        Console.WriteLine("Pre-order:");
        PreOrder(root);

        Console.WriteLine("\n\nIn-order:");
        InOrder(root);

        Console.WriteLine("\n\nPost-order:");
        PostOrder(root);

        Console.WriteLine("\n\nLevel-order:");
        LevelOrder(root);
    }

    private static void PreOrder(Node<int> root)
    {
        if (root == null)
            return;

        Console.Write($"{root.Data} ");
        PreOrder(root.Left);
        PreOrder(root.Right);
    }

    private static void InOrder(Node<int> root)
    {
        if (root == null)
            return;

        InOrder(root.Left);
        Console.Write($"{root.Data} ");
        InOrder(root.Right);
    
    }

    private static void PostOrder(Node<int> root)
    {
        if (root == null)
            return;

        PostOrder(root.Left);
        PostOrder(root.Right);
        Console.Write($"{root.Data} ");
    }
    
    private static void LevelOrder(Node<int> root)
    {
        if (root == null)
            return;

        var queue = new Queue<Node<int>>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            Console.Write($"{node.Data} ");

            if (node.Left != null)
                queue.Enqueue(node.Left);

            if (node.Right != null)
                queue.Enqueue(node.Right);
        }
    }
}