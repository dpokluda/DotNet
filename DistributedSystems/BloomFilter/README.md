# Bloom Filter
This project implements serializable Bloom Filter in .NET Core. It has a very simple interface 

``` csharp
public interface IBloomFilter
{
    bool Add(string element);
    bool Contains(string element);
    void Clear();
}
```

The actual filter is implemented in memory and supports serialization to JSON document.

## Resources
The projects is built using the following sources:
- [BloomFilter.NetCore](https://github.com/vla/BloomFilter.NetCore) project
- BitArray serialization is based on the following [StackOverflow](https://stackoverflow.com/questions/58279263/how-to-deserialize-bitarray-using-jsonserializer/58279913#58279913) answer
- BitArray serialization is based on [M5.BitArraySerialization.Json](https://github.com/MILL5/M5.BloomFilter/tree/main/M5.BitArraySerialization.Json) project
