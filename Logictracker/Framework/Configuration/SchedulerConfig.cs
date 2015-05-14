#region Usings

using System;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Scheduler
        {
            /// <summary>
            /// Gets the scheduler timers configuration file.
            /// </summary>
            public static String SchedulerTimersConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.scheduler.timers.configuration", "configuration.xml")); } }

            public static String SchedulerTemplateDirectory { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.scheduler.reportscheduler.templatedirectory", string.Empty)); } }

        }
    }
}
