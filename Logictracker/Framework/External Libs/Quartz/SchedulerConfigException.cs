#region Usings

using System;
using System.Runtime.Serialization;

#endregion

namespace Quartz
{
	/// <summary>
	/// An exception that is thrown to indicate that there is a misconfiguration of
	/// the <see cref="ISchedulerFactory" />- or one of the components it
	/// configures.
	/// </summary>
	/// <author>James House</author>
	[Serializable]
	public class SchedulerConfigException : SchedulerException
	{
		/// <summary>
		/// Create a <see cref="JobPersistenceException" /> with the given message.
		/// </summary>
		public SchedulerConfigException(string msg) : base(msg, ErrorBadConfiguration)
		{
		}

		/// <summary>
		/// Create a <see cref="JobPersistenceException" /> with the given message
		/// and cause.
		/// </summary>
		public SchedulerConfigException(string msg, Exception cause) : base(msg, cause)
		{
			ErrorCode = ErrorBadConfiguration;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerConfigException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
        public SchedulerConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
	}
}
