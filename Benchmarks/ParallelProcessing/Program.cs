using BenchmarkDotNet.Running;

namespace ParallelProcessing;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<ParallelProcessors>();
    }
}