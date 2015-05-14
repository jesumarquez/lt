#region Usings

using System;

#endregion

namespace Logictracker.Configuration
{
    /// <summary>
    /// Class for getting configuration values for the application.
    /// </summary>
    public static partial class Config
    {
        public static class HttpGateway
        {
            /// <summary>
            /// Gets the application title.
            /// </summary>
            public static String Hostname { get { return ConfigurationBase.GetAsString("logictracker.httpgateway.host", "127.0.0.1"); } }

            /// <summary>
            /// Gets the current host name.
            /// </summary>
            public static  int Port { get { return ConfigurationBase.GetAsInt32("logictracker.httpgateway.port", 2007); } }
        
        }
	}
}
