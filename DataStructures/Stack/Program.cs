using System;

namespace Stack;

class Program
{
    static void Main(string[] args)
    {
        var stack = new Stack<int>();

        Console.WriteLine("Stack is empty: " + stack.IsEmpty());

        stack.Push(5);
        stack.Push(10);
        stack.Push(15);
        Console.Write("Stack: ");
        stack.Print();

        Console.WriteLine("Top of the stack: " + stack.Peek());
        Console.WriteLine("Size of the stack: " + stack.GetSize());

        var value = stack.Pop();
        Console.WriteLine("Popped value: " + value);
        stack.Push(20);
        Console.WriteLine("Pushed value: " + 20);

        Console.WriteLine("After modifications, top of the stack: " + stack.Peek());
        Console.Write("Stack: ");
        stack.Print();
    }
}