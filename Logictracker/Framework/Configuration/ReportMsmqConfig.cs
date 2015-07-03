using System;

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class ReportMsmq
        {
            public static String QueueName
            {
                get { return ConfigurationBase.GetAsString("logictracker.reports.queuename", ""); }
            }

            public static String QueueType
            {
                get { return ConfigurationBase.GetAsString("logictracker.reports.queuetype", ""); }
            }
        }
    }
}
