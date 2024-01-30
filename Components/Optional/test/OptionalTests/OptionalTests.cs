using Optional;

namespace OptionalTests;

public class OptionalTests
{
    [Test]
    public void WithValue()
    {
        var optional = Optional<string>.Of("foo");
        Assert.IsNotNull(optional);
        Assert.IsTrue(optional.HasValue());
        Assert.AreEqual("foo", optional.GetValue());
    }

    [Test]
    public void Empty()
    {
        var optional = Optional<string>.Empty();
        Assert.IsNotNull(optional);
        Assert.IsFalse(optional.HasValue());
        Assert.Throws<InvalidOperationException>(() => optional.GetValue());
    }
    
    [Test]
    public void OrElse()
    {
        var optional = Optional<string>.Empty();
        Assert.IsNotNull(optional);
        Assert.IsFalse(optional.HasValue());
        Assert.AreEqual("bar", optional.OrElse("bar"));
    }
    
    [Test]
    public void OrElseGet()
    {
        var optional = Optional<string>.Empty();
        Assert.IsNotNull(optional);
        Assert.IsFalse(optional.HasValue());
        Assert.AreEqual("bar", optional.OrElseGet(() => "bar"));
    }
    
    [Test]
    public void OrElseThrow()
    {
        var optional = Optional<string>.Empty();
        Assert.IsNotNull(optional);
        Assert.IsFalse(optional.HasValue());
        Assert.Throws<InvalidOperationException>(() => optional.OrElseThrow(() => new InvalidOperationException()));
    }
    
    [Test]
    public void Map()
    {
        var optional = Optional<string>.Of("foo");
        Assert.IsNotNull(optional);
        Assert.IsTrue(optional.HasValue());
        Assert.AreEqual("foo", optional.GetValue());
        Assert.AreEqual("FOO", optional.Map(s => s.ToUpper()).GetValue());
    }
    
    [Test]
    public void MapOrElse()
    {
        var foo = Optional<string>.Of("foo");
        Assert.AreEqual("FOO", MapOrElse(foo));

        var empty = Optional<string>.Empty();
        Assert.AreEqual("EMPTY", MapOrElse(empty));
    }

    private string MapOrElse(Optional<string> optional)
    {
        return optional
            .Map(x => x.ToUpper())
            .OrElse("EMPTY");
    }
}