using System;

namespace Bubble
{
    class Program
    {
        static int[] Sort(int[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data.Length - i - 1; j++)
                {
                    if (data[j] > data[j+1])
                    {
                        Swap(data, j, j+1);
                    }
                }
            }
            
            return data;
        }

        private static void Swap(int[] data, int first, int second)
        {
            (data[first], data[second]) = (data[second], data[first]);
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Bubble sort:");
            var data = new [] { 5, 4, 3, 2, 1 };

            var sorted = Sort(data);

            foreach (int i in sorted)
            {
                Console.WriteLine(i);
            }
        }
    }
}