using System;

namespace Queue;

class Program
{
    static void Main(string[] args)
    {
        var queue = new Queue<int>();

        if (queue.IsEmpty())
            Console.WriteLine("Queue is empty.");

        queue.Enqueue(5);
        queue.Enqueue(10);
        queue.Enqueue(15);
        Console.Write("Queue: ");
        queue.Print();

        Console.WriteLine("Front of the queue: " + queue.Peek());
        Console.WriteLine("Queue size: " + queue.GetSize());

        var value = queue.Dequeue();
        Console.WriteLine("Dequeued value: " + value);
        queue.Enqueue(20);
        Console.WriteLine("Enqueued value: 20");

        Console.WriteLine("After modifications, front of the queue: " + queue.Peek());
        Console.Write("Queue: ");
        queue.Print();
    }
}