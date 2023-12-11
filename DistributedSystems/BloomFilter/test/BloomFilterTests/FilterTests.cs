namespace BloomFilter.Tests;

[TestClass]
public class FilterTests
{
    [TestMethod]
    public void Construct()
    {
        Assert.IsNotNull(new BloomFilter(100, 0.01));
        Assert.IsNotNull(new BloomFilter(100, 2));
    }
    
    [TestMethod]
    public void Invalid()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BloomFilter(0, 0.01));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BloomFilter(100, 1.1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BloomFilter(0, 2));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BloomFilter(100, 0));
    }

    [TestMethod]
    public void Properties()
    {
        var filter = new BloomFilter(100, 0.01);
        Assert.AreEqual(959, filter.Capacity);
        Assert.AreEqual(0.01, filter.ErrorRate);
        Assert.AreEqual(100, filter.ExpectedElements);
        Assert.AreEqual(7, filter.Hashes);
        Assert.AreEqual("Capacity:959,Hashes:7,ExpectedElements:100,ErrorRate:0.01", filter.ToString());
        
        filter = new BloomFilter(959, 7);
        Assert.AreEqual(959, filter.Capacity);
        Assert.AreEqual(7, filter.Hashes);
    }
    
    [TestMethod]
    public void Add()
    {
        var filter = new BloomFilter(100, 0.01);
        Assert.IsTrue(filter.Add("one"));
        Assert.IsTrue(filter.Add("two"));
        
        Assert.IsFalse(filter.Add("one"));
    }
    
    [TestMethod]
    public void Contains()
    {
        var filter = new BloomFilter(100, 0.01);
        Assert.IsTrue(filter.Add("one"));
        Assert.IsTrue(filter.Add("two"));
        
        Assert.IsTrue(filter.Contains("one"));
        Assert.IsTrue(filter.Contains("two"));
        Assert.IsFalse(filter.Contains("three"));
    }
    
    [TestMethod]
    public void Clear()
    {
        var filter = new BloomFilter(100, 0.01);
        Assert.IsTrue(filter.Add("one"));
        Assert.IsTrue(filter.Add("two"));
        
        Assert.IsTrue(filter.Contains("one"));
        Assert.IsTrue(filter.Contains("two"));
        
        filter.Clear();
        
        Assert.IsFalse(filter.Contains("one"));
        Assert.IsFalse(filter.Contains("two"));
    }
}