namespace CacheProvider.Timestamp;

public class UnixTimestampProvider : ITimestampProvider
{
    public long Get()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}