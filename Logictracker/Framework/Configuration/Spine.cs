#region Usings

using System;

#endregion

namespace Logictracker.Configuration
{
	public static partial class Config
	{
		public static class Spine
		{
			public static Int32 DisconnectedComfortTimeout { get { return ConfigurationBase.GetAsInt32("spine_client_wrap.disconnected_comfort_timeout", 10000); } }
			public static Int32 ConnectedComfortTimeout { get { return ConfigurationBase.GetAsInt32("spine_client_wrap.connected_comfort_timeout", 1000); } }
		}
	}
}
