#region Usings

using System;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Orbcomm
        {
            /// <summary>
            /// Orbcomm WebGateway Url.
            /// </summary>
            public static String WebGatewayUrl
            {
                get
                {
                    var url = ConfigurationBase.GetAsString("logictracker.orbcomm.url", "http://ipgwy.orbcomm.net/");
                    return url.EndsWith("/") ? url : url + '/';
                }
            }
        }
    }
}
