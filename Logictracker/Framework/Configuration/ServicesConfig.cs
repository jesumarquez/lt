#region Usings

using System;
using System.Collections.Generic;
using System.Configuration;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Services
        {
            /// <summary>
            /// User for impersonating aspnet.
            /// </summary>
            public static String ImpersonateUser { get { return ConfigurationBase.GetAsString("logictracker.services.user",string.Empty); } }

            /// <summary>
            /// The domain of the user to be impersonated.
            /// </summary>
            public static String ImpersonateDomain { get { return ConfigurationBase.GetAsString("logictracker.services.domain", string.Empty); } }

            /// <summary>
            /// The password of the user to be impersonated.
            /// </summary>
            public static String ImpersonatePassword { get { return ConfigurationBase.GetAsString("logictracker.services.password", string.Empty); } }

            /// <summary>
            /// Gets the names of all services to be monitored.
            /// </summary>
            /// <returns></returns>
            public static IEnumerable<string> ServicesToMonitor
            {
                get
                {
                    var services = new List<String>();
                    var configServices = ConfigurationManager.AppSettings["logictracker.services"];

                    if (String.IsNullOrEmpty(configServices)) return services;

                    var splitted = configServices.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    services.AddRange(splitted);

                    return services;
                }
            }
        }
    }
}
