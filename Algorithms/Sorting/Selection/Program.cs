using System;

namespace Selection
{
    class Program
    {
        static int[] Sort(int[] data)
        {
            for (int i = 0; i < data.Length - 2; i++)
            {
                var minAt = i;
                for (int j = i; j < data.Length; j++)
                {
                    if (data[j] < data[minAt])
                    {
                        minAt = j;
                    }
                }

                if (i != minAt)
                {
                    Swap(data, i, minAt);
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
            Console.WriteLine("Selection sort:");
            var data = new [] { 5, 4, 3, 2, 1 };

            var sorted = Sort(data);

            foreach (int i in sorted)
            {
                Console.WriteLine(i);
            }
        }
    }
}
