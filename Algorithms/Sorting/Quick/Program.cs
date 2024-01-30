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

            int pivotIndex = left - 1; // if all items will be bigger; then it will go to `+1` index (which is `left`)
            for (int i = left; i < right; i++)
            {
                if (data[i] < pivot)
                {
                    // since the data is smaller; pivotIndex needs to move to the right (by as many positions as many items are smaller than it)
                    pivotIndex++;
                    // replace them; the item that is smaller is replaced item at last position before pivot index so far
                    Swap(data, i, pivotIndex);
                }
            }
 
            // finally replace smaller item larger than pivot with pivot; now we have {all-smaller}-pivot-{all-larger}
            Swap(data, pivotIndex + 1, right);
            
            return (pivotIndex + 1);
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