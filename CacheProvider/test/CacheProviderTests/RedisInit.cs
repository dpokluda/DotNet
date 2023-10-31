using System.Diagnostics;

namespace CacheProviderTests;

[TestClass]
public class RedisInit
{
    private static Process _redis = null;

    [AssemblyInitialize]
    public static void StartRedis(TestContext testContext)
    {
        // _redis = Process.Start("redis\\redis-server.exe");
    }

    [AssemblyCleanup]
    public static void StopRedis()
    {
        // _redis.Kill(true);
    }
}