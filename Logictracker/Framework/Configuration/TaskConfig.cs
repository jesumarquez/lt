using System;

namespace Logictracker.Configuration
{
	public static partial class Config
	{
		public static class Task
		{
			public static Int32 JoinTimeout { get { return ConfigurationBase.GetAsInt32("task.join_timeout", 5000); } }
		}
	}
}
