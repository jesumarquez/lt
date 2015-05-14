using System;

namespace Logictracker.Configuration
{
	public static partial class Config
	{
		public static class MessageQueueMonitor
		{
			public static Int32 TrendSamples { get { return ConfigurationBase.GetAsInt32("message_queue_monitor.trend_samples", 30); } }
			public static Int32 TrendStableThreshold { get { return ConfigurationBase.GetAsInt32("message_queue_monitor.trend_stable_threshold", 5); } }
			public static Int32 QueryInterval { get { return ConfigurationBase.GetAsInt32("message_queue_monitor.query_interval", 1000); } }
		}
	}
}
