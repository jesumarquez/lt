using log4net;
using log4net.Config;
using log4net.Core;

namespace LogicOut.Core
{
    public static class Logger
    {

        private static readonly ILog Log =  LogManager.GetLogger(typeof(Logger));

        static Logger()
        {
            bool isConfigured = Log.Logger.Repository.Configured;
            if (!isConfigured)
            {               
                // Setup RollingFileAppender
                var fileAppender = new log4net.Appender.RollingFileAppender
                                       {
                                           Layout = new log4net.Layout.PatternLayout("%d [%t]%-5p %c [%x] - %m%n"),
                                           MaximumFileSize = "512KB",
                                           MaxSizeRollBackups = 5,
                                           RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size,
                                           AppendToFile = true,
                                           File = Config.LogFile,
                                           Name = "XXXRollingFileAppender",
                                           Threshold = GetLevel(Config.LogLevel)
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

        private static Level GetLevel(string levelName)
        {
            var level = levelName.ToLower();
            switch (level)
            {
                case "off": return Level.Off;
                case "fatal": return Level.Fatal;
                case "error": return Level.Error;
                case "warn": return Level.Warn;
                case "info": return Level.Info;
                case "debug": return Level.Warn;
                case "all": return Level.All;
                default: return Level.Info;
            }
        }
    }
}
