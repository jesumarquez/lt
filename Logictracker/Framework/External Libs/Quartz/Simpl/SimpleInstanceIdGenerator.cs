#region Usings

using System;
using System.Net;
using Quartz.Spi;

#endregion

namespace Quartz.Simpl
{
	/// <summary> 
	/// The default InstanceIdGenerator used by Quartz when instance id is to be
	/// automatically generated.  Instance id is of the form HOSTNAME + CURRENT_TIME.
	/// </summary>
	/// <seealso cref="IInstanceIdGenerator">
	/// </seealso>
	/// <seealso cref="HostnameInstanceIdGenerator">
	/// </seealso>
	public class SimpleInstanceIdGenerator : IInstanceIdGenerator
	{
		/// <summary>
		/// Generate the instance id for a <see cref="IScheduler" />
		/// </summary>
		/// <returns>The clusterwide unique instance id.</returns>
		public virtual string GenerateInstanceId()
		{
			try
			{
#if NET_20
                return Dns.GetHostName() + DateTime.UtcNow.Ticks;
#else
				return
					Dns.GetHostByAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString()).HostName +
					DateTime.UtcNow.Ticks;
#endif
            }
			catch (Exception e)
			{
				throw new SchedulerException("Couldn't get host name!", e);
			}
		}
	}
}