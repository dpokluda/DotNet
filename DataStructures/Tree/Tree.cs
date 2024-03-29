﻿using System;
using System.Collections.Generic;

namespace Tree;

public class Tree<T>
{
    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }

        public Node(T data)
        {
            Data = data;
            Left = null;
            Right = null;
        }
    }
    
    private Node<T> _root;

    public Tree()
    {
        _root = null;
    }

    public void Insert(T item)
    {
        if (_root == null)
        {
            _root = new Node<T>(item);
        }
        else
        {
            Insert(_root, item);
        }
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
    
    private void Insert(Node<T> node, T item)
    {
        if (Comparer<T>.Default.Compare(item, node.Data) < 0)
        {
            if (node.Left == null)
            {
                node.Left = new Node<T>(item);
            }
            else
            {
                Insert(node.Left, item);
            }
        }
        else
        {
            if (node.Right == null)
            {
                node.Right = new Node<T>(item);
            }
            else
            {
                Insert(node.Right, item);
            }
        }
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