#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Tracer
        {
            public static String[] TracerType { get { return ConfigurationBase.GetAsString("logictracker.tracer.type", Types.DataBase).Split(','); } }

            /// <summary>
            /// Gets the minimun log type to trace into database.
            /// </summary>
            public static String TracerMinLogType { get { return ConfigurationBase.GetAsString("logictracker.tracer.minlogtype", "Trace"); } }

            /// <summary>
            /// Gets the log trace retries.
            /// </summary>
            public static Int32 TracerMaxRetries { get { return ConfigurationBase.GetAsInt32("logictracker.tracer.maxretries", 5); } }

            /// <summary>
            /// Gets the error sleep interval.
            /// </summary>
            public static TimeSpan TracerErrorInterval { get { return TimeSpan.FromMilliseconds(ConfigurationBase.GetAsInt32("logictracker.tracer.errorinterval", 100)); } }

            /// <summary>
            /// Gets the tracer nhibernate mapping assembly.
            /// </summary>
            public static String TracerNhibernateAssembly { get { return ConfigurationBase.GetAsString("logictracker.tracer.hibernate.assembly", "Logictracker.DatabaseTracer"); } }

			public static int[] ConsoleFilter
	        {
		        get
		        {
					try
					{
						var idss = ConfigurationBase.GetAsString("logictracker.tracer.consolefilter", "").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						var ids = new List<int> {0};

						foreach (var s in idss)
						{
							int i;
							var success = int.TryParse(s, out i);
							if (i == 0 || !success) continue;
							ids.Add(i);
						}
						return ids.ToArray();
					}
					catch
					{
						return new int[0];
					}
		        }
	        }

	        public static class Types
			{
				public const String Fota = "Fota";
				public const String DataBase = "DataBase";
				public const String Log4Net = "Log4NET";
				public const String None = "None";
			}
        }
    }
}
