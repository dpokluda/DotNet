using CacheProvider;
using CacheProvider.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RedisCacheProvider;

namespace CacheProviderTests;

[TestClass]
public class RedisCacheProviderTest
{
    private const string LocalRedisConfiguration = "127.0.0.1:6379";
    
    [TestMethod]
    public void Construct()
    {
        ICacheProvider provider = new RedisCacheProvider.RedisCacheProvider(
            Options.Create(new RedisConfiguration
            {
                ConnectionString = LocalRedisConfiguration
            }), 
            new NullLogger<RedisCache>());
        Assert.IsNotNull(provider);
        
        ICache cache = provider.GetCache();
        Assert.IsNotNull(cache);
    }
}