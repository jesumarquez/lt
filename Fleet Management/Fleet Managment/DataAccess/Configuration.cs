using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Urbetrack.FleetManagment.DataAccess
{
    public class Configuration
    {

        internal static String GetAsString(String key, String defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];

            return String.IsNullOrEmpty(value) ? defaultValue : value;
        }

    }
}
