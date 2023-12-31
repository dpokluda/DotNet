using CacheProvider;
using CacheProvider.Timestamp;

namespace CacheProviderTests;

[TestClass]
public class MemoryCacheTest
{
    private ManualTimestampProvider _timestampProvider = new ManualTimestampProvider();
    
    [TestMethod]
    public void Construct()
    {
        ICache cache = GetMemoryCache();
        Assert.IsNotNull(cache);
    }

    [TestMethod]
    public async Task SetAndGetSimpleStringValue()
    {
        ICache cache = GetMemoryCache();
        Assert.IsTrue(await cache.SetSimpleValueAsync("key", "value", false, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetSimpleValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetAndGetSimpleIntValue()
    {
        ICache cache = GetMemoryCache();
        await cache.SetSimpleValueAsync("key", 123, false, CancellationToken.None);
        Assert.AreEqual(123, await cache.GetSimpleValueAsync<int>("key"));
    }

    [TestMethod]
    public async Task SetSimpleValueIfNew()
    {
        ICache cache = GetMemoryCache();
        Assert.IsTrue(await cache.SetSimpleValueAsync("key", "value", true, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetSimpleValueAsync<string>("key"));

        Assert.IsFalse(await cache.SetSimpleValueAsync("key", "value2", true, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetSimpleValueAsync<string>("key"));
    }
    
    [TestMethod]
    public async Task GetNotFoundSimpleValue()
    {
        ICache cache = GetMemoryCache();
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task DeleteSimpleValue()
    {
        ICache cache = GetMemoryCache();
        Assert.IsTrue(await cache.SetSimpleValueAsync("key", "value", false, CancellationToken.None));
        Assert.IsTrue(await cache.DeleteSimpleValueAsync("key"));
    }
    
    [TestMethod]
    public async Task DeleteNotFoundSimpleValue()
    {
        ICache cache = GetMemoryCache();
        Assert.IsTrue(await cache.DeleteSimpleValueAsync("key"));
    }
    
    [TestMethod]
    public async Task DeleteWithSimpleValue()
    {
        ICache cache = GetMemoryCache();
        Assert.IsTrue(await cache.SetSimpleValueAsync("key", "value", false, CancellationToken.None));
        Assert.IsTrue(await cache.DeleteSimpleValueAsync("key", "value"));
    }
    
    [TestMethod]
    public async Task DeleteWithSimpleValueFail()
    {
        ICache cache = GetMemoryCache();
        Assert.IsTrue(await cache.SetSimpleValueAsync("key", "value", false, CancellationToken.None));
        Assert.IsFalse(await cache.DeleteSimpleValueAsync("key", "value2"));
    }

    [TestMethod]
    public async Task SetAndGetString()
    {
        ICache cache = GetMemoryCache();
        Assert.IsTrue(await cache.SetValueAsync("key", "value", TimeSpan.FromMilliseconds(1), false, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetAndGetInt()
    {
        ICache cache = GetMemoryCache();
        await cache.SetValueAsync("key", 123, TimeSpan.FromMilliseconds(1), false, CancellationToken.None);
        Assert.AreEqual(123, await cache.GetValueAsync<int>("key"));
    }

    [TestMethod]
    public async Task SetIfNew()
    {
        ICache cache = GetMemoryCache();
        Assert.IsTrue(await cache.SetValueAsync("key", "value", TimeSpan.FromMilliseconds(1), true, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetValueAsync<string>("key"));
        
        Assert.IsFalse(await cache.SetValueAsync("key", "value2", TimeSpan.FromMilliseconds(1), true, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetValueAsync<string>("key"));
    }
    
    [TestMethod]
    public async Task GetNotFound()
    {
        ICache cache = GetMemoryCache();
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>("key"));
    }
    
    [TestMethod]
    public async Task SetWithInvalidExpiration()
    {
        ICache cache = GetMemoryCache();
        await cache.SetValueAsync("key", "value", TimeSpan.FromMilliseconds(-1), false, CancellationToken.None);
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetWithExpiration()
    {
        ICache cache = GetMemoryCache();
        await cache.SetValueAsync("key", "value", TimeSpan.FromMilliseconds(1), false, CancellationToken.None);
        Assert.AreEqual("value", await cache.GetValueAsync<string>("key"));

        _timestampProvider.Value = 5;
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetAndGetWithInvalidType()
    {
        ICache cache = GetMemoryCache();
        await cache.SetValueAsync("key", "value", TimeSpan.FromMilliseconds(10), false, CancellationToken.None);
        await Assert.ThrowsExceptionAsync<InvalidCastException>(async () => await cache.GetValueAsync<int>("key"));
    }
    
    [TestMethod]
    public async Task Delete()
    {
        ICache cache = GetMemoryCache();
        Assert.IsTrue(await cache.SetValueAsync("key", "value", TimeSpan.FromMilliseconds(1), false, CancellationToken.None));
        Assert.IsTrue(await cache.DeleteValueAsync("key", "value"));
    }
    
    [TestMethod]
    public async Task DeleteWithValueFail()
    {
        ICache cache = GetMemoryCache();
        Assert.IsTrue(await cache.SetValueAsync("key", "value", TimeSpan.FromMilliseconds(1), false, CancellationToken.None));
        Assert.IsFalse(await cache.DeleteValueAsync("key", "value2"));
    }
    
    [TestMethod]
    public async Task IncrementAndDecrementCounter_Simple()
    {
        ICache cache = GetMemoryCache();
        Assert.AreEqual(0, await cache.GetCounterAsync("key"));
        Assert.AreEqual(0, await cache.DecrementCounterAsync("key", "1"));
        Assert.AreEqual(0, await cache.GetCounterAsync("key"));
        Assert.AreEqual(1, await cache.IncrementCounterAsync("key", "1", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.IncrementCounterAsync("key", "2", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.GetCounterAsync("key"));
        Assert.AreEqual(1, await cache.DecrementCounterAsync("key", "2"));
        Assert.AreEqual(0, await cache.DecrementCounterAsync("key", "1"));
        Assert.AreEqual(0, await cache.DecrementCounterAsync("key", "1"));
        Assert.AreEqual(0, await cache.GetCounterAsync("key"));
    }
    
    [TestMethod]
    public async Task IncrementAndDecrementCounter_MaxValue()
    {
        ICache cache = GetMemoryCache();
        Assert.AreEqual(0, await cache.GetCounterAsync("key"));
        Assert.AreEqual(0, await cache.DecrementCounterAsync("key", "1"));
        Assert.AreEqual(0, await cache.GetCounterAsync("key"));
        Assert.AreEqual(1, await cache.IncrementCounterAsync("key", "1", TimeSpan.FromMilliseconds(10), 2));
        Assert.AreEqual(2, await cache.IncrementCounterAsync("key", "2", TimeSpan.FromMilliseconds(10), 2));
        Assert.AreEqual(2, await cache.IncrementCounterAsync("key", "2", TimeSpan.FromMilliseconds(10), 2));
        Assert.AreEqual(2, await cache.GetCounterAsync("key"));
        Assert.AreEqual(1, await cache.DecrementCounterAsync("key", "2"));
        Assert.AreEqual(0, await cache.DecrementCounterAsync("key", "1"));
        Assert.AreEqual(0, await cache.DecrementCounterAsync("key", "1"));
        Assert.AreEqual(0, await cache.GetCounterAsync("key"));
    }

    [TestMethod]
    public async Task IncrementAndDecrementCounter_Expiration()
    {
        ICache cache = GetMemoryCache();
        Assert.AreEqual(1, await cache.IncrementCounterAsync("key", "1", TimeSpan.FromMilliseconds(10)));
        _timestampProvider.Value = 5;
        Assert.AreEqual(1, await cache.GetCounterAsync("key"));
        Assert.AreEqual(2, await cache.IncrementCounterAsync("key", "2", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.IncrementCounterAsync("key", "2", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.GetCounterAsync("key"));
        _timestampProvider.Value = 10;
        Assert.AreEqual(1, await cache.GetCounterAsync("key"));
        Assert.AreEqual(1, await cache.GetCounterAsync("key"));
        Assert.AreEqual(2, await cache.IncrementCounterAsync("key", "1", TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual(2, await cache.GetCounterAsync("key"));
        _timestampProvider.Value = 15;
        Assert.AreEqual(1, await cache.GetCounterAsync("key"));
        _timestampProvider.Value = 20;
        Assert.AreEqual(0, await cache.GetCounterAsync("key"));
    }

    private ICache GetMemoryCache()
    {
        _timestampProvider.Value = 0;

        return new MemoryCache(_timestampProvider);
    }
}