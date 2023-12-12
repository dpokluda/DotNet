using System;

namespace Heap;

class Program
{
    static void Main(string[] args)
    {
        var heap = new Heap<int>();
        if (heap.IsEmpty())
            Console.WriteLine("Heap is empty.");

        heap.Enqueue(3);
        heap.Enqueue(1);
        heap.Enqueue(5);
        heap.Enqueue(10);
        heap.Enqueue(2);
        heap.Enqueue(4);

        Console.WriteLine("After populating the heap, the heap content is:");
        heap.Print();

        var max = heap.Dequeue();
        Console.WriteLine("\nMax value: " + max);
        heap.Print();

        max = heap.Dequeue();
        Console.WriteLine("\nMax value: " + max);
        heap.Print();
    }
}
