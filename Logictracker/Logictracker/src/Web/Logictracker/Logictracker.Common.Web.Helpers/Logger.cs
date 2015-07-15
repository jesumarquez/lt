using log4net;
using log4net.Config;

namespace Logictracker.Web.Helpers
{
    public static class Logger
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Logger));

        static Logger()
        {
            bool isConfigured = Log.Logger.Repository.Configured;
            if (!isConfigured)
            {
                // Setup RollingFileAppender
                var fileAppender = new log4net.Appender.RollingFileAppender
                {
                    Layout = new log4net.Layout.PatternLayout("%d [%3t] %-5p %c - %m%n"),
                    MaximumFileSize = "512KB",
                    MaxSizeRollBackups = 5,
                    RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size,
                    AppendToFile = true,
                    File = "ltwebapp.log",
                    Name = "XXXRollingFileAppender"
                };
                fileAppender.ActivateOptions(); // IMPORTANT, creates the file
                BasicConfigurator.Configure(fileAppender);
            }
        }

        public static void Debug(object message)
        {
            Log.Debug(message);
        }
        public static void Info(object message)
        {
            Log.Info(message);
        }
        public static void Warn(object message)
        {
            Log.Warn(message);
        }
        public static void Error(object message)
        {
            Log.Error(message);
        }
        public static void Fatal(object message)
        {
            Log.Fatal(message);
        }
    }
}
