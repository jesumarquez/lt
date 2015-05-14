#region Usings

using Quartz.Simpl;

#endregion

namespace Quartz.Spi
{
	/// <summary>
	/// An IInstanceIdGenerator is responsible for generating the clusterwide unique 
	/// instance id for a <see cref="IScheduler" /> nodde.
	/// <p>
	/// This interface may be of use to those wishing to have specific control over 
	/// the mechanism by which the <see cref="IScheduler" /> instances in their 
	/// application are named.
	/// </p>
	/// 
	/// </summary>
	/// <seealso cref="SimpleInstanceIdGenerator" />
	public interface IInstanceIdGenerator
	{
		/// <summary> Generate the instance id for a <see cref="IScheduler" />
		/// 
		/// </summary>
		/// <returns> The clusterwide unique instance id.
		/// </returns>
		string GenerateInstanceId();
	}
}