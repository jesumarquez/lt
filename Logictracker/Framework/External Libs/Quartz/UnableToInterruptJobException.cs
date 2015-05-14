#region Usings

using System;
using System.Runtime.Serialization;

#endregion

namespace Quartz
{
	/// <summary>
	/// An exception that is thrown to indicate that a call to 
	/// <see cref="IInterruptableJob.Interrupt" /> failed without interrupting the Job.
	/// </summary>
	/// <seealso cref="IInterruptableJob" />
	/// <author>James House</author>
	[Serializable]
	public class UnableToInterruptJobException : SchedulerException
	{
		/// <summary>
		/// Create a <see cref="UnableToInterruptJobException" /> with the given message.
		/// </summary>
		public UnableToInterruptJobException(string msg) : base(msg)
		{
		}

		/// <summary>
		/// Create a <see cref="UnableToInterruptJobException" /> with the given cause.
		/// </summary>
		public UnableToInterruptJobException(Exception cause) : base(cause)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToInterruptJobException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
        public UnableToInterruptJobException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
	}
}
