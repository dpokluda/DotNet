﻿using System.Reflection;
using StackExchange.Redis;

namespace CacheProvider.Lua
{
    /// <summary>
    /// Helper class to load Lua scripts.
    /// </summary>
    internal class LuaResource
    {
        public static Lazy<LuaScript> _delete = new Lazy<LuaScript>(() => LoadLuaScript("Delete"));
        public static Lazy<LuaScript> _increment = new Lazy<LuaScript>(() => LoadLuaScript("Increment"));
        public static Lazy<LuaScript> _decrement = new Lazy<LuaScript>(() => LoadLuaScript("Decrement"));
        public static LuaScript Delete
        {
            get { return _delete.Value; }
        }
        public static LuaScript Increment
        {
            get { return _increment.Value; }
        }
        public static LuaScript Decrement
        {
            get { return _decrement.Value; }
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
