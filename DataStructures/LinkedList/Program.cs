using System;
using System.Linq;

namespace LinkedList;

class Program
{
    static void Main(string[] args)
    {
        var list = new LinkedList<int>(Enumerable.Range(1, 3).ToArray());
        list.PushBack(5);
        list.PushBack(10);
        list.PushBack(15);

        Console.Write("Initial list: ");
        list.Print();

        list.PushFront(0);

        Console.Write("After adding 0 to the front: ");
        list.Print();

        list.Remove(10);

        Console.Write("After removing 10: ");
        list.Print();

        Console.WriteLine("Removing first element: " + list.PopFront());
        Console.WriteLine("Removing last element: " + list.PopBack());
        Console.Write("Final queue: ");
        list.Print();
    }
}