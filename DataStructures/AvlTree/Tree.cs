namespace AvlTree;

public class Tree<T> where T : IComparable<T>
{
    public class Node<T>
    {
        public T Data { get; set; }
        public int Height { get; set; }
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }

        public Node(T data)
        {
            Data = data;
            Height = 1;
            Left = null;
            Right = null;
        }
    }
    
    private Node<T> _root;

    public Tree()
    {
        _root = null;
    }
    
    // Insert method
    public void Insert(T value)
    {
        _root = Insert(_root, value);
    }

    private Node<T> Insert(Node<T> node, T value)
    {
        if (node == null)
            return new Node<T>(value);

        if (value.CompareTo(node.Data) < 0)
            node.Left = Insert(node.Left, value);
        else if (value.CompareTo(node.Data) > 0)
            node.Right = Insert(node.Right, value);
        else
            return node;  // Duplicate values are not allowed

        UpdateHeight(node);
        return Balance(node);
    }

    public void Print()
    {
        Print(_root);
        Console.WriteLine();
    }
    
    public void PrintTree()
    {
        PrintTree(_root);
        Console.WriteLine();
    }
    
    // Update the height of a node
    private void UpdateHeight(Node<T> node)
    {
        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
    }

    // Get the height of a node
    private int GetHeight(Node<T> node)
    {
        return node == null ? 0 : node.Height;
    }

    // Get the balance factor of a node
    // > 1: Left heavy
    // < -1: Right heavy
    private int GetBalanceFactor(Node<T> node)
    {
        return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
    }

    // Balance the node
    private Node<T> Balance(Node<T> node)
    {
        int balanceFactor = GetBalanceFactor(node);

        // Left heavy
        if (balanceFactor > 1)
        {
            // Left-right heavy
            if (GetBalanceFactor(node.Left) < 0)
            {
                node.Left = LeftRotate(node.Left);
            }
            return RightRotate(node);
        }

        // Right heavy
        if (balanceFactor < -1)
        {
            // Right-left heavy
            if (GetBalanceFactor(node.Right) > 0)
            {
                node.Right = RightRotate(node.Right);
            }
            return LeftRotate(node);
        }

        return node;
    }

    private Node<T> RightRotate(Node<T> y)
    {
        Node<T> x = y.Left;
        Node<T> T2 = x.Right;

        // Perform rotation
        x.Right = y;
        y.Left = T2;

        // Update heights
        UpdateHeight(y);
        UpdateHeight(x);

        // Return new root
        return x;
    }

    private Node<T> LeftRotate(Node<T> x)
    {
        Node<T> y = x.Right;
        Node<T> T2 = y.Left;

        // Perform rotation
        y.Left = x;
        x.Right = T2;

        // Update heights
        UpdateHeight(x);
        UpdateHeight(y);

        // Return new root
        return y;
    }
    
    private void Print(Node<T> node)
    {
        if (node == null)
        {
            return;
        }
        Print(node.Left);
        Console.Write(node.Data + " ");
        Print(node.Right);
    }
    
    private void PrintTree(Node<T> root)
    {
        var currentLevel = new Queue<Node<T>>();
        var nextLevel = new Queue<Node<T>>();

        currentLevel.Enqueue(root);
        while (currentLevel.Count > 0 || nextLevel.Count > 0)
        {
            if (currentLevel.Count == 0)
            {
                currentLevel = nextLevel;
                nextLevel = new Queue<Node<T>>();
                Console.WriteLine();
            }
            var node = currentLevel.Dequeue();
            Console.Write(node.Data + " ");

            if (node.Left != null)
                nextLevel.Enqueue(node.Left);
            if (node.Right != null)
                nextLevel.Enqueue(node.Right);
        }
    }
}