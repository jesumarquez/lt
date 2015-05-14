#region Usings

using System;
using System.IO;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Qtree
        {
            /// <summary>
            /// Logictracker qtree directory.
            /// </summary>
            public static String QtreeDirectory { get { return ConfigurationBase.GetAsString("logictracker.qtree.directory", ConfigurationBase.GetLocalPath(@"Qtree\")); } }

            /// <summary>
            /// Logictracker gte qtree directory.
            /// </summary>
            public static String QtreeGteDirectory { get { return Path.Combine(QtreeDirectory, "Gte"); } }

            /// <summary>
            /// Logictracker torino qtree directory.
            /// </summary>
            public static String QtreeTorinoDirectory { get { return Path.Combine(QtreeDirectory, "Torino"); } }
        }
    }
}
