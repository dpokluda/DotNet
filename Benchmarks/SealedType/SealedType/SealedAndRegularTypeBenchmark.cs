using BenchmarkDotNet.Attributes;

namespace SealedType;

public class SealedAndRegularTypeBenchmark
{
    [Benchmark]
    public void GetSealedType()
    {
        for (int i = 0; i < 100; i++)
        {
            var sealedType = new MySealed(i, i.ToString());
            var value = sealedType.Value;
            var name = sealedType.Name;
            var _ = name + value;
        }
    }

    [Benchmark]
    public void GetRegularType()
    {
        for (int i = 0; i < 100; i++)
        {
            var sealedType = new MyRegular(i, i.ToString());
            var value = sealedType.Value;
            var name = sealedType.Name;
            var _ = name + value;
        }
    }
}