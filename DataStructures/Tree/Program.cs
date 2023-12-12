using System;

namespace Tree;

class Program
{
    static void Main(string[] args)
    {
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
    }
}