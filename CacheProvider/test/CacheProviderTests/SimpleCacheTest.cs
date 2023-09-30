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
        await cache.SetValueAsync("key", "value", CancellationToken.None);
        Assert.AreEqual("value", await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetAndGetInt()
    {
        ICache cache = new SimpleCache();
        await cache.SetValueAsync("key", 123, CancellationToken.None);
        Assert.AreEqual(123, await cache.GetValueAsync<int>("key"));
    }

    [TestMethod]
    public async Task SetWithAbsoluteExpirationAndGet()
    {
        ICache cache = new SimpleCache();
        await cache.SetValueAsync("key", "value", DateTimeOffset.UtcNow + TimeSpan.FromMinutes(1), CancellationToken.None);
        Assert.AreEqual("value", await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetWithRelativeExpirationAndGet()
    {
        ICache cache = new SimpleCache();
        await cache.SetValueAsync("key", "value", TimeSpan.FromMinutes(1), CancellationToken.None);
        Assert.AreEqual("value", await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task GetNotFound()
    {
        ICache cache = new SimpleCache();
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>("key"));
    }


    [TestMethod]
    public async Task SetWithAbsoluteExpirationAndGetExpired()
    {
        ICache cache = new SimpleCache();
        await cache.SetValueAsync("key", "value", DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1), CancellationToken.None);
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetWithRelativeExpirationAndGetExpired()
    {
        ICache cache = new SimpleCache();
        await cache.SetValueAsync("key", "value", TimeSpan.FromMinutes(-1), CancellationToken.None);
        await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () => await cache.GetValueAsync<string>("key"));
    }

    [TestMethod]
    public async Task SetAndGetWithInvalidType()
    {
        ICache cache = new SimpleCache();
        await cache.SetValueAsync("key", "value", CancellationToken.None);
        await Assert.ThrowsExceptionAsync<InvalidCastException>(async () => await cache.GetValueAsync<int>("key"));
    }
}