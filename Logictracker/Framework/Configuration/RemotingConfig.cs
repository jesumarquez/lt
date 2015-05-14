using System;

namespace Logictracker.Configuration
{
	public static partial class Config
	{
		public static class Remoting
		{
			public static String RemotingConfiguration
			{
				get { return ConfigurationBase.GetAsString("core.remoting_configuration", GetConfigurationFileName()); }
			}

			private static string GetConfigurationFileName()
			{
				var configFilename = "(no especificado)";
				var mainmod = System.Diagnostics.Process.GetCurrentProcess().MainModule;
				if (mainmod != null)
					configFilename = mainmod.FileName + ".config";
				return configFilename;
			}
		}
	}
}


