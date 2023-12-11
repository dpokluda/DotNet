using System;
using System.Runtime.InteropServices;

namespace Heap
{
    class Program
    {
        static int[] Sort(int[] data)
        {
            int n = data.Length;
  
            // Build heap (rearrange array)
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(data, n, i);
  
            // One by one extract an element from heap
            for (int i = n - 1; i > 0; i--) {
                // Move current root to end
                Swap(data, 0, i);
  
                // call max heapify on the reduced heap
                Heapify(data, i, 0);
            }
            
            return data;
        }
        
        static void Heapify(int[] data, int n, int i)
        {
            int largest = i; // Initialize largest as root
            int l = 2 * i + 1; // left = 2*i + 1
            int r = 2 * i + 2; // right = 2*i + 2
  
            // If left child is larger than root
            if (l < n && data[l] > data[largest])
                largest = l;
  
            // If right child is larger than largest so far
            if (r < n && data[r] > data[largest])
                largest = r;
  
            // If largest is not root
            if (largest != i)
            {
                Swap(data, i, largest);
  
                // Recursively heapify the affected sub-tree
                Heapify(data, n, largest);
            }
        }
        
        private static void Swap(int[] data, int first, int second)
        {
            (data[first], data[second]) = (data[second], data[first]);
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Heap sort:");
            var data = new [] { 5, 4, 3, 2, 1 };

            var sorted = Sort(data);

            foreach (int i in sorted)
            {
                Console.WriteLine(i);
            }
        }
    }
}