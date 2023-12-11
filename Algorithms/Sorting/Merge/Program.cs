using System;

namespace Merge
{
    class Program
    {
        static int[] Sort(int[] data)
        {
            MergeSort(data, 0, data.Length - 1);
            return data;
        }

        static void MergeSort(int[] data, int left, int right)
        {
            if (left < right)
            {
                int middle = left + (right - left) / 2;
                MergeSort(data, left, middle);
                MergeSort(data, middle+1, right);
                Merge(data, left, middle, right);
            }
        }

        static void Merge(int[] data, int left, int middle, int right)
        {
            //create subarrays
            int[] la = new int[middle - left + 1];
            int[] ra = new int[right - middle];
            for (int i = 0; i < la.Length; i++)
            {
                la[i] = data[left + i];
            }

            for (int i = 0; i < ra.Length; i++)
            {
                ra[i] = data[middle + 1 + i];
            }
            
            //merge
            int li = 0;
            int ri = 0;
            int k = left;
            while (li < la.Length && ri < ra.Length)
            {
                if (la[li] <= ra[ri])
                {
                    data[k] = la[li];
                    li++;
                }
                else
                {
                    data[k] = ra[ri];
                    ri++;
                }

                k++;
            }
            
            //copy rest
            while (li < la.Length)
            {
                data[k] = la[li];
                li++;
                k++;
            }
            while (ri < ra.Length)
            {
                data[k] = ra[ri];
                ri++;
                k++;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Merge sort:");
            var data = new [] { 5, 4, 3, 2, 1 };

            var sorted = Sort(data);

            foreach (int i in sorted)
            {
                Console.WriteLine(i);
            }
        }
    }
}