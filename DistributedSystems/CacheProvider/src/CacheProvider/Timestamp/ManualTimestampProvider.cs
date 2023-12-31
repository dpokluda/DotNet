namespace CacheProvider.Timestamp;

public class ManualTimestampProvider : ITimestampProvider
{
    private long _value;

    public long Value
    {
        set
        {
            _value = value;
        }
    }

    public long Get()
    {
        return _value;
    }

    public void Increment()
    {
        _value++;
    }
}