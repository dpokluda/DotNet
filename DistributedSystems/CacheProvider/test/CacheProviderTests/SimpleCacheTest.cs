using CacheProvider;

namespace CacheProviderTests;

[TestClass]
public class SimpleCacheTest
{
    [TestMethod]
    public void Construct()
    {
        ICache cache = new SimpleCache();
        Assert.IsNotNull(cache);
    }

    [TestMethod]
    public async Task SetAndGetString()
    {
        ICache cache = new SimpleCache();
        Assert.IsTrue(await cache.SetValueAsync("key", "value", false, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetAndGetInt()
    {
        ICache cache = new SimpleCache();
        await cache.SetValueAsync("key", 123, false, CancellationToken.None);
        Assert.AreEqual(123, await cache.GetValueAsync<int>("key"));
    }

    [TestMethod]
    public async Task SetIfNew()
    {
        ICache cache = new SimpleCache();
        Assert.IsTrue(await cache.SetValueAsync("key", "value", true, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetIfNewFail()
    {
        ICache cache = new SimpleCache();
        Assert.IsTrue(await cache.SetValueAsync("key", "value", true, CancellationToken.None));
        Assert.IsFalse(await cache.SetValueAsync("key", "value2", true, CancellationToken.None));
        Assert.AreEqual("value", await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetWithRelativeExpirationAndGet()
    {
        ICache cache = new SimpleCache();
        await cache.SetValueAsync("key", "value", TimeSpan.FromMinutes(1), false, CancellationToken.None);
        Assert.AreEqual("value", await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task GetNotFound()
    {
        ICache cache = new SimpleCache();
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>("key"));
    }


    [TestMethod]
    public async Task SetWithRelativeExpirationAndGetExpired()
    {
        ICache cache = new SimpleCache();
        await cache.SetValueAsync("key", "value", TimeSpan.FromMinutes(-1), false, CancellationToken.None);
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetAndGetWithInvalidType()
    {
        ICache cache = new SimpleCache();
        await cache.SetValueAsync("key", "value", false, CancellationToken.None);
        await Assert.ThrowsExceptionAsync<InvalidCastException>(async () => await cache.GetValueAsync<int>("key"));
    }
    
    [TestMethod]
    public async Task Delete()
    {
        ICache cache = new SimpleCache();
        Assert.IsTrue(await cache.SetValueAsync("key", "value", false, CancellationToken.None));
        Assert.IsTrue(await cache.DeleteAsync("key"));
    }
    
    [TestMethod]
    public async Task DeleteWithValue()
    {
        ICache cache = new SimpleCache();
        Assert.IsTrue(await cache.SetValueAsync("key", "value", false, CancellationToken.None));
        Assert.IsTrue(await cache.DeleteAsync("key", "value"));
    }
    
    [TestMethod]
    public async Task DeleteWithValueFail()
    {
        ICache cache = new SimpleCache();
        Assert.IsTrue(await cache.SetValueAsync("key", "value", false, CancellationToken.None));
        Assert.IsFalse(await cache.DeleteAsync("key", "value2"));
    }
}