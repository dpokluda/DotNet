using System;

namespace Trie;

class Program
{
    static void Main(string[] args)
    {
        var trie = new Trie();
        trie.Insert("apple");
        trie.Insert("app");

        Console.WriteLine("apple: " + (trie.Search("apple") ? " true" : "false")); 
        Console.WriteLine("app: " + (trie.Search("app") ? " true" : "false"));
        Console.WriteLine("appl: " + (trie.Search("appl") ? " true" : "false"));
    }
}