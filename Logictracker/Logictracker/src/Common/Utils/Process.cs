#region Usings

using System;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.Utils
{
    public static class Process
    {
        #region ExitCodes enum

        public enum ExitCodes
        {
            Voluntary = 0,
            InvalidConfiguration = 1,
            SpineLost = 2,
            AbortRequest = 3,
            ServiceStop = 4,
            SystemShutdown = 5,
            SessionLogoff = 6,
            ApplicationRestart = 7,
            Undefined = -1
        }

        #endregion

		#region Public Members

		public static void InitAppDomain()
		{
			Environment.CurrentDirectory = GetApplicationFolder();
			if (!AppDomain.CurrentDomain.IsDefaultAppDomain()) return;
			var remoting_config_file = Config.Remoting.RemotingConfiguration;
			if (File.Exists(remoting_config_file)) RemotingConfiguration.Configure(remoting_config_file, false);
		}

		public static String GetApplicationFolder(params String[] subfolder)
		{
		    var t = new TimeElapsed();
            var mainmod = System.Diagnostics.Process.GetCurrentProcess().MainModule;
            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", String.Format("GetApplicationFolder GetCurrentProcess({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
			var exe = mainmod != null ? mainmod.FileName : "unknown";
			var folder = exe.Substring(0, exe.LastIndexOf('\\'));
            t.Restart();
			folder = subfolder.Where(s => !String.IsNullOrEmpty(s)).Aggregate(folder, (current, s) => current + (@"\" + s));
            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", String.Format("GetApplicationFolder Where({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
            t.Restart();
			Directory.CreateDirectory(folder);
            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", String.Format("GetApplicationFolder CreateDirectory({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
			return folder;
		}

		#endregion
	}
}
