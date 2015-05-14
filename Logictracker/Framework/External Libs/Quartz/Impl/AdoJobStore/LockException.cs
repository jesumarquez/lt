#region Usings

using System;
using System.Runtime.Serialization;

#endregion

namespace Quartz.Impl.AdoJobStore
{
	/// <summary>
	/// Exception class for when there is a failure obtaining or releasing a
	/// resource lock.
	/// </summary>
	/// <seealso cref="ISemaphore" />
	/// <author>James House</author>
	[Serializable]
	public class LockException : JobPersistenceException
	{
		public LockException(string msg) : base(msg)
		{
		}

		public LockException(string msg, Exception cause) : base(msg, cause)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="LockException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
        public LockException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
	}
}