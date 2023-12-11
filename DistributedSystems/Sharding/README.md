# Sharding
This project implements simple sharding component. The basic interface is simple:

``` csharp
public interface ISharding
{
    void Add(string node);
    void Remove(string node);
    string GetNode(string key);
}
```

The project contains consistent-hash sharding implementation.

## Resources
TBD
