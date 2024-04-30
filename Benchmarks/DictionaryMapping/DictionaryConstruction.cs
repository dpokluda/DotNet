using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace DictionaryMapping
{
    public class DictionaryConstruction
    {
        [Benchmark]
        public void DictionaryWithoutCapacity()
        {
            var d = new Dictionary<string, object>();
            foreach (var item in Mappings.DefaultValues)        
            {
                d.Add(item.Key, item.Value);
            }
        }

        [Benchmark]
        public void DictionaryWithCapacity()
        {
            var d = new Dictionary<string, object>(Mappings.DefaultValues.Count);
            foreach (var item in Mappings.DefaultValues)        
            {
                d.Add(item.Key, item.Value);
            }
        }
        
        [Benchmark]
        public void DictionaryWithConstructor()
        {
            var d = new Dictionary<string, object>(Mappings.DefaultValues);
        }
    }
}
