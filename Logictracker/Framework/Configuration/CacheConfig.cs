#region Usings

using System;
using System.Linq;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Cache
        {
            /// <summary>
            /// Logictracker cache name.
            /// </summary>
            public static String LogictrackerCachePoolName { get { return ConfigurationBase.GetAsString("logictracker.cache.poolname", "logictracker.cache"); } }

            /// <summary>
            /// Logictracker cache server.
            /// </summary>
            public static String[] LogictrackerCacheServers { get { return ConfigurationBase.GetAsString("logictracker.cache.servers", "127.0.0.1:11212").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray(); } }

        }
    }
}
