namespace AvlTree;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("BALANCED:");
        var tree = new Tree<int>();
        tree.Insert(5);
        tree.Insert(3);
        tree.Insert(8);
        tree.Insert(1);
        tree.Insert(4);
        tree.Insert(7);
        tree.Insert(9);

        Console.WriteLine("Inorder traversal: ");
        tree.Print();  // Expected: 1 3 4 5 7 8 9
        
        Console.WriteLine("Tree: ");
        tree.PrintTree();
        
        Console.WriteLine("\nUNBALANCED:");
        tree = new Tree<int>();
        for (int i = 9; i >0; i--)
        {
            tree.Insert(i);
        }

        Console.WriteLine("Inorder traversal: ");
        tree.Print();  // Expected: 1 3 4 5 7 8 9
        
        Console.WriteLine("Tree: ");
        tree.PrintTree();
    }
}