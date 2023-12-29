using System.Diagnostics;

namespace CacheProviderTests;

[TestClass]
public class RedisInit
{
    private static Process _redis = null;

    [AssemblyInitialize]
    public static void StartRedis(TestContext testContext)
    {
        if (OperatingSystem.IsWindows())
        {
            // Windows (local redis server)
            _redis = Process.Start("redis\\redis-server.exe");
        }
        else if (OperatingSystem.IsMacOS())
        {
            // MacOs (you need to install redis first: brew install redis)
            _redis = Process.Start("/opt/homebrew/opt/redis/bin/redis-server","/opt/homebrew/etc/redis.conf");
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    [AssemblyCleanup]
    public static void StopRedis()
    {
        _redis.Kill(true);
    }
}