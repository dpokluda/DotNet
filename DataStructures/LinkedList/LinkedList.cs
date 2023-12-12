using System;

namespace LinkedList;

public class LinkedList<T>
{
    private class Node
    {
        public T Data { get; set; }
        public Node Next { get; set; }
        
        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }
    
    private Node _head;

    public LinkedList()
    {
        _head = null;
    }

    public LinkedList(T[] array)
    {
        foreach(T item in array)
        {
            PushBack(item);
        }
    }
    
    public void PushBack(T item)
    {
        if (_head == null)
        {
            _head = new Node(item);
            return;
        }
        
        Node current = _head;
        while (current.Next != null)
        {
            current = current.Next;
        }
        current.Next = new Node(item);
    }
    
    public void PushFront(T item)
    {
        Node node = new Node(item);
        node.Next = _head;
        _head = node;
    }
    
    public T PopFront()
    {
        if (_head == null)
        {
            throw new InvalidOperationException("List is empty");
        }
        
        T data = _head.Data;
        _head = _head.Next;
        return data;
    }
    
    public T PopBack()
    {
        if (_head == null)
        {
            throw new InvalidOperationException("List is empty");
        }
        
        if (_head.Next == null)
        {
            T data = _head.Data;
            _head = null;
            return data;
        }
        
        Node current = _head;
        while (current.Next.Next != null)
        {
            current = current.Next;
        }
        T result = current.Next.Data;
        current.Next = null;
        return result;
    }
    
    public void Remove(T item)
    {
        if (_head == null)
        {
            throw new InvalidOperationException("List is empty");
        }
        
        if (_head.Data.Equals(item))
        {
            _head = _head.Next;
            return;
        }
        
        Node current = _head;
        while (current.Next != null && !current.Next.Data.Equals(item))
        {
            current = current.Next;
        }
        
        if (current.Next == null)
        {
            throw new InvalidOperationException("Item not found");
        }
        
        current.Next = current.Next.Next;
    }
    
    public void Print()
    {
        Node current = _head;
        while (current != null)
        {
            Console.Write(current.Data + " -> ");
            current = current.Next;
        }
        Console.WriteLine("null");
    }
}