#region Usings

using Quartz.Impl.AdoJobStore.Common;

#endregion

namespace Quartz.Impl.AdoJobStore
{
	/// <summary> 
	/// An interface for providing thread/resource locking in order to protect
	/// resources from being altered by multiple threads at the same time.
	/// </summary>
	/// <author>James House</author>
	public interface ISemaphore
	{
		/// <summary> 
		/// Grants a lock on the identified resource to the calling thread (blocking
		/// until it is available).
		/// </summary>
		/// <returns> true if the lock was obtained.
		/// </returns>
		bool ObtainLock(DbMetadata metadata, ConnectionAndTransactionHolder conn, string lockName);

		/// <summary> Release the lock on the identified resource if it is held by the calling
		/// thread.
		/// </summary>
		void ReleaseLock(ConnectionAndTransactionHolder conn, string lockName);

		/// <summary> 
		/// Determine whether the calling thread owns a lock on the identified
		/// resource.
		/// </summary>
		bool IsLockOwner(ConnectionAndTransactionHolder conn, string lockName);

        /// <summary>
        /// Whether this Semaphore implementation requires a database connection for
        /// its lock management operations.
        /// </summary>
        /// <seealso cref="IsLockOwner" />
        /// <seealso cref="ObtainLock" />
        /// <seealso cref="ReleaseLock" />
        bool RequiresConnection
        { 
            get;
        }
	}
}