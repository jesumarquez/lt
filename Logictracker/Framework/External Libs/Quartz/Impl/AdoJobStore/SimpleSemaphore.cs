#region Usings

using System;
using System.Globalization;
using System.Threading;
using Common.Logging;
using Quartz.Collection;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Util;

#endregion

namespace Quartz.Impl.AdoJobStore
{
	/// <summary> 
	/// Internal in-memory lock handler for providing thread/resource locking in 
    /// order to protect resources from being altered by multiple threads at the 
    /// same time.
	/// </summary>
	/// <author>James House</author>
	public class SimpleSemaphore : ISemaphore
	{
	    private const string KeyThreadLockOwners = "qrtz_ssemaphore_lock_owners";

		private readonly ILog log = LogManager.GetLogger(typeof(SimpleSemaphore));
		private readonly HashSet locks = new HashSet();

        /// <summary>
        /// Gets the thread locks.
        /// </summary>
        /// <value>The thread locks.</value>
		private static HashSet ThreadLocks
		{
			get
			{
				var threadLocks = (HashSet) LogicalThreadContext.GetData(KeyThreadLockOwners);
				if (threadLocks == null)
				{
					threadLocks = new HashSet();
					LogicalThreadContext.SetData(KeyThreadLockOwners, threadLocks);
				}
				return threadLocks;
			}
		}

		/// <summary> 
		/// Grants a lock on the identified resource to the calling thread (blocking
		/// until it is available).
		/// </summary>
		/// <returns>True if the lock was obtained.</returns>
		public virtual bool ObtainLock(DbMetadata metadata, ConnectionAndTransactionHolder conn, string lockName)
		{
			lock (this)
			{
				lockName = String.Intern(lockName);

				if (log.IsDebugEnabled)
					log.Debug("Lock '" + lockName + "' is desired by: " + Thread.CurrentThread.Name);

				if (!IsLockOwner(conn, lockName))
				{
					if (log.IsDebugEnabled)
					{
						log.Debug("Lock '" + lockName + "' is being obtained: " + Thread.CurrentThread.Name);
					}
					
					while (locks.Contains(lockName))
					{
						try
						{
							Monitor.Wait(this);
						}
						catch (ThreadInterruptedException)
						{
							if (log.IsDebugEnabled)
							{
								log.Debug("Lock '" + lockName + "' was not obtained by: " + Thread.CurrentThread.Name);
							}
						}
					}

					if (log.IsDebugEnabled)
					{
						log.Debug(string.Format(CultureInfo.InvariantCulture, "Lock '{0}' given to: {1}", lockName, Thread.CurrentThread.Name));
					}
					ThreadLocks.Add(lockName);
					locks.Add(lockName);
				}
				else if (log.IsDebugEnabled)
				{
					log.Debug(string.Format(CultureInfo.InvariantCulture, "Lock '{0}' already owned by: {1} -- but not owner!", lockName, Thread.CurrentThread.Name), new Exception("stack-trace of wrongful returner"));
				}

				return true;
			}
		}

		/// <summary> Release the lock on the identified resource if it is held by the calling
		/// thread.
		/// </summary>
		public virtual void ReleaseLock(ConnectionAndTransactionHolder conn, string lockName)
		{
			lock (this)
			{
				lockName = String.Intern(lockName);

				if (IsLockOwner(conn, lockName))
				{
					if (log.IsDebugEnabled)
					{
						log.Debug(string.Format(CultureInfo.InvariantCulture, "Lock '{0}' retuned by: {1}", lockName, Thread.CurrentThread.Name));
					}
					ThreadLocks.Remove(lockName);
					locks.Remove(lockName);
					Monitor.PulseAll(this);
				}
				else if (log.IsDebugEnabled)
				{
					log.Debug(string.Format(CultureInfo.InvariantCulture, "Lock '{0}' attempt to retun by: {1} -- but not owner!", lockName, Thread.CurrentThread.Name), new Exception("stack-trace of wrongful returner"));
				}
			}
		}

		/// <summary> 
		/// Determine whether the calling thread owns a lock on the identified
		/// resource.
		/// </summary>
		public virtual bool IsLockOwner(ConnectionAndTransactionHolder conn, String lockName)
		{
			lock (this)
			{
				lockName = String.Intern(lockName);

				return ThreadLocks.Contains(lockName);
			}
		}

        /// <summary>
        /// Whether this Semaphore implementation requires a database connection for
        /// its lock management operations.
        /// </summary>
        /// <value></value>
        /// <seealso cref="IsLockOwner"/>
        /// <seealso cref="ObtainLock"/>
        /// <seealso cref="ReleaseLock"/>
	    public bool RequiresConnection
	    {
            get { return false; }
	    }
	}
}