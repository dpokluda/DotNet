using System;

namespace Bubble
{
    class Program
    {
        static int[] Sort(int[] data)
        {
            for (int i = 1; i < data.Length; i++) {
                int value = data[i];
                int j = i - 1;
 
                while (j >= 0 && data[j] > value) {
                    data[j + 1] = data[j];
                    j = j - 1;
                }
                data[j + 1] = value;
            }
            
            return data;
        }
        
        private static void Swap(int[] data, int first, int second)
        {
            (data[first], data[second]) = (data[second], data[first]);
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Insertion sort:");
            var data = new [] { 5, 4, 3, 2, 1 };

            var sorted = Sort(data);

            foreach (int i in sorted)
            {
                Console.WriteLine(i);
            }
        }
    }
}