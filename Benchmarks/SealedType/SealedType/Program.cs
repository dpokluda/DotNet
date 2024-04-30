using BenchmarkDotNet.Running;

namespace SealedType;

partial class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<SealedAndRegularTypeBenchmark>();
        Console.WriteLine(summary);
    }
}