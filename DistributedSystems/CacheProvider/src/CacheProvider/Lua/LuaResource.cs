using System.Reflection;
using StackExchange.Redis;

namespace CacheProvider.Lua
{
    /// <summary>
    /// Helper class to load Lua scripts.
    /// </summary>
    internal class LuaResource
    {
        private static readonly Lazy<LuaScript> _deleteValue = new Lazy<LuaScript>(() => LoadLuaScript("DeleteValue"));
        private static readonly Lazy<LuaScript> _incrementCounter = new Lazy<LuaScript>(() => LoadLuaScript("IncrementCounter"));
        private static readonly Lazy<LuaScript> _decrementCounter = new Lazy<LuaScript>(() => LoadLuaScript("DecrementCounter"));
        private static readonly Lazy<LuaScript> _incrementCounterWithExpiration = new Lazy<LuaScript>(() => LoadLuaScript("IncrementCounterWithExpiration"));
        private static readonly Lazy<LuaScript> _decrementCounterWithExpiration = new Lazy<LuaScript>(() => LoadLuaScript("DecrementCounterWithExpiration"));
        private static readonly Lazy<LuaScript> _getCounter = new Lazy<LuaScript>(() => LoadLuaScript("GetCounter"));
        
        public static LuaScript DeleteValue
        {
            get { return _deleteValue.Value; }
        }
        public static LuaScript IncrementCounter
        {
            get { return _incrementCounter.Value; }
        }
        public static LuaScript DecrementCounter
        {
            get { return _decrementCounter.Value; }
        }
        public static LuaScript IncrementCounterWithExpiration
        {
            get { return _incrementCounterWithExpiration.Value; }
        }
        public static LuaScript DecrementCounterWithExpiration
        {
            get { return _decrementCounterWithExpiration.Value; }
        }
        public static LuaScript GetCounter
        {
            get { return _getCounter.Value; }
        }

        /// <summary>
        /// Loads Lua script.
        /// </summary>
        /// <param name="name">The resource name.</param>
        /// <param name="assembly">(Optional) The assembly.</param>
        /// <returns>
        /// The lua script.
        /// </returns>
        public static LuaScript LoadLuaScript(string name, Assembly assembly = null)
        {
            var resource = LoadResource(name, assembly);
            var luaScript = LuaScript.Prepare(resource);
            return luaScript;
        }

        private static string LoadResource(string name, Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetCallingAssembly();

            var resourceName = $"CacheProvider.Lua.{name}.lua";
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new ArgumentException($"Resource '{resourceName}' not found in assembly '{assembly.FullName}'.");
            }

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
