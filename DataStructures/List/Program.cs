using System;
using System.Linq;

namespace List;

class Program
{
    static void Main(string[] args)
    {
        List<int> list = new List<int>(Enumerable.Range(1, 4).ToArray());

        list.PushBack(5);
        list.PushBack(10);
        list.PushBack(15);

        Console.WriteLine("List size: " + list.GetSize());
        for (int i = 0; i < list.GetSize(); ++i) {
            Console.Write(list[i] + " ");
        }
        Console.WriteLine();

        list.PopBack();
        list.PushBack(20);

        Console.WriteLine("Modified list: ");
        for (int i = 0; i < list.GetSize(); ++i) {
            Console.Write(list[i] + " ");
        }
        Console.WriteLine();
    }
}