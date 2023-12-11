using System;

namespace Quick
{
    class Program
    {
        static int[] Sort(int[] data)
        {
            QuickSort(data, 0, data.Length - 1);
            return data;
        }

        static void QuickSort(int[] data, int left, int right)
        {
            if (left < right)
            {
 
                // pi is partitioning index, arr[p]
                // is now at right place
                int middle = Partition(data, left, right);
 
                // Separately sort elements before
                // partition and after partition
                QuickSort(data, left, middle - 1);
                QuickSort(data, middle + 1, right);
            }
        }

        private static int Partition(int[] data, int left, int right)
        {
 
            // pivot
            int pivot = data[right];
 
            // Index of smaller element and
            // indicates the right position
            // of pivot found so far
            int i = (left - 1);
 
            for (int j = left; j <= right - 1; j++)
            {
                // If current element is smaller
                // than the pivot
                if (data[j] < pivot)
                {
                    i++;
                    Swap(data, i, j);
                }
            }
            
            Swap(data, i + 1, right);
            
            return (i + 1);
        }
        
        private static void Swap(int[] data, int first, int second)
        {
            (data[first], data[second]) = (data[second], data[first]);
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Quick sort:");
            var data = new [] { 5, 4, 3, 2, 1 };

            var sorted = Sort(data);

            foreach (int i in sorted)
            {
                Console.WriteLine(i);
            }
        }
    }
}