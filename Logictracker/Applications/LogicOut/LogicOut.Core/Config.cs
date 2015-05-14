using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace LogicOut.Core
{
    public class Config
    {
        private const string HandlerPrefix = "logicout.handler.";
        private const string IntervalKey = "logicout.interval";
        private const string LogFileKey = "logicout.logfile";
        private const string LogLevelKey = "logicout.loglevel";
        private const string ServiceUrlKey = "logicout.serverurl";
        private const string UserNameKey = "logicout.user";
        private const string PasswordKey = "logicout.pass";
        private const string CompanyKey = "logicout.company";
        private const string BranchKey = "logicout.branch";
        private const string ServerNameKey = "logicout.servername";

        public static Dictionary<string, string> Handlers { get; private set; }
        public static int Interval { get; private set; }
        public static string LogFile { get; private set; }
        public static string LogLevel { get; private set; }
        public static string ServiceUrl { get; private set; }
        public static string UserName { get; private set; }
        public static string Password { get; private set; }
        public static string Company { get; private set; }
        public static string Branch { get; private set; }
        public static string ServerName { get; private set; }

        public static string SessionToken { get; set; }

        static Config()
        {
            // Handlers
            var handlers = ConfigurationManager.AppSettings.AllKeys.Where(k => k.ToLower().StartsWith(HandlerPrefix));
            
            Handlers = new Dictionary<string, string>(handlers.Count());

            foreach (var handler in handlers)
            {
                var name = handler.Substring(HandlerPrefix.Length);
                var value = ConfigurationManager.AppSettings[handler];
                Handlers.Add(name, value);
            }

            // Interval
            int inter;
            var interval = ConfigurationManager.AppSettings[IntervalKey];
            if (interval == null || !int.TryParse(interval, out inter)) inter = 5;
            Interval = inter;

            // Logfile
            var logfile = ConfigurationManager.AppSettings[LogFileKey];
            LogFile = logfile ?? "logicout.log";

            //LogLevel
            var loglevel = ConfigurationManager.AppSettings[LogLevelKey];
            LogLevel = loglevel ?? "Info";

            // ServiceUrl
            var service = ConfigurationManager.AppSettings[ServiceUrlKey];
            ServiceUrl = service ?? "http://plataforma.logictracker.com/App_Services/Export.asmx";

            // UserName
            UserName = ConfigurationManager.AppSettings[UserNameKey];

            // Pass
            Password = ConfigurationManager.AppSettings[PasswordKey];


            // Company
            Company = ConfigurationManager.AppSettings[CompanyKey] ?? string.Empty;

            // Branch
            Branch = ConfigurationManager.AppSettings[BranchKey] ?? string.Empty;

            // ServerName
            ServerName = ConfigurationManager.AppSettings[ServerNameKey] ?? string.Empty;
        }
    }
}
