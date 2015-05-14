using System;
using System.Configuration;

namespace Logictracker.Process.Import.Client
{
    public static class ConfigImportClient
    {
        public static string UserName {get { return GetValue("username");  }}
        public static string Password {get { return GetValue("password");  }}
        public static string Company {get { return GetValue("company");  }}
        public static string Branch {get { return GetValue("branch");  }}
        public static int Interval {get { return Convert.ToInt32(GetValue("interval") ?? "5");  }}
        public static string LogFile { get { return GetValue("logfile") ?? "lLogictracker.sync.log"; } }
        public static string ServerUrl {get { return GetValue("serverurl") ?? "http://plataforma.logictracker.com/App_Services/Import.asmx";  }}
        public static string StrategyName { get { return GetValue("strategy"); } }
        public static string MappingFile { get { return GetValue("mappingfile") ?? "logictracker.mapping.xml"; } }

        private static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
