#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Queue
		{
			#region Public Methods

			public static String QueueUser { get { return ConfigurationBase.GetAsString("logictracker.queues.user", GetUsersGroupLocalizedName()); } }

			public static String QueueTransactional { get { return ConfigurationBase.GetAsString("logictracker.queues.transactional", ""); } }

			public static String QueueRecoverable { get { return ConfigurationBase.GetAsString("logictracker.queues.recoverable", ""); } }

			public static String QueueFormater { get { return ConfigurationBase.GetAsString("logictracker.queues.formater", ""); } }

			/// <summary>
			/// Gets a list of the system queues currently beeing monitored.
			/// </summary>
			public static IEnumerable<string> LogictrackerQueues
			{
				get
				{
					var queues = ConfigurationBase.GetAsString("logictracker.queues", String.Empty);

					return queues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
				}
			}
			
			#endregion

			#region Private Methods

			/// <summary>
			/// Gets windows os users group name in localized form ("Users", "Usuarios", etc).
			/// </summary>
			/// <returns></returns>
			private static String GetUsersGroupLocalizedName()
			{
				IntPtr psid;
				int use;
				var cbName = 0;
				var cbDom = 0;

				ConvertStringSidToSid("S-1-5-32-545", out psid);

				LookupAccountSid(null, psid, null, ref cbName, null, ref cbDom, out use);

				var name = new StringBuilder(cbName);
				var dom = new StringBuilder(cbDom);

				LookupAccountSid(null, psid, name, ref cbName, dom, ref cbDom, out use);

				Marshal.FreeHGlobal(psid);

				return name.ToString();
			}

			[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			private static extern Boolean ConvertStringSidToSid(String stringSid, out IntPtr psid);

			[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			private static extern Boolean LookupAccountSid(String lpSystemName, IntPtr sid, [Out] StringBuilder name, ref Int32 cbName, [Out] StringBuilder referencedDomainName, ref Int32 cbReferencedDomainName, out Int32 peUse);
 
			#endregion 
		}
    }
}
