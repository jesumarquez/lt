#region Usings

using System;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Metrics
        {
            /// <summary>
            /// Gets the metrics error retries
            /// </summary>
            public static Int32 MetricsMaxRetries { get { return ConfigurationBase.GetAsInt32("logictracker.metrics.maxretries", 5); } }

            /// <summary>
            /// Gets the error sleep interval.
            /// </summary>
            public static TimeSpan MetricsErrorInterval { get { return TimeSpan.FromMilliseconds(ConfigurationBase.GetAsInt32("logictracker.metrics.errorinterval", 100)); } }

            /// <summary>
            /// Gets the interval in seconds to check if a metric should be generated.
            /// </summary>
            public static TimeSpan MetricsGenerationInterval { get { return TimeSpan.FromSeconds(ConfigurationBase.GetAsInt32("logictracker.metrics.generationinterval", 5)); } }

            /// <summary>
            /// Gets the scheduler timers configuration file.
            /// </summary>
            public static String LogictrackerMetricsConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.metrics.configuration", "metrics.xml")); } }

            /// <summary>
            /// Gets the tracer nhibernate mapping assembly.
            /// </summary>
            public static String MetricsNhibernateAssembly { get { return ConfigurationBase.GetAsString("logictracker.metrics.hibernate.assembly", "Logictracker.Metrics"); } }

        }
    }
}
