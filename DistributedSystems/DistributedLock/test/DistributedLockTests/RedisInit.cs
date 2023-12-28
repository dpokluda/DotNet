using System.Diagnostics;

namespace DistributedLockTests
{
    [TestClass]
    public class RedisInit
    {
        private static Process _redis = null;
    
        [AssemblyInitialize]
        public static void StartRedis(TestContext testContext)
        {
            // Windows (local redis server)
            _redis = Process.Start("redis\\redis-server.exe");
            // MacOs (you need to install redis first: brew install redis)
            // _redis = Process.Start("/opt/homebrew/opt/redis/bin/redis-server","/opt/homebrew/etc/redis.conf");
        }

        [AssemblyCleanup]
        public static void StopRedis()
        {
            _redis.Kill(true);
        }
    }
}