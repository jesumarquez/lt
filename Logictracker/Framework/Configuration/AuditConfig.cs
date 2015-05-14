#region Usings

using System;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Audit
        {
            /// <summary>
            /// Gets the Audit configuration.
            /// </summary>
            public static String AuditConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.web.auditfile", "Logictracker.Audit.config")); } }
        }
    }
}
