namespace CacheProvider.Configuration;

public class RedisConfiguration
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; set; }
}