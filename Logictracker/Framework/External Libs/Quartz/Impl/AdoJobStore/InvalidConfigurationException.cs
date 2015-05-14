#region Usings

using System;
using System.Runtime.Serialization;

#endregion

namespace Quartz.Impl.AdoJobStore
{
	/// <summary>
	/// Exception class for when a driver delegate cannot be found for a given
	/// configuration, or lack thereof.
	/// </summary>
	/// <author>  <a href="mailto:jeff@binaryfeed.org">Jeffrey Wescott</a>
	/// </author>
	[Serializable]
	public class InvalidConfigurationException : SchedulerException
	{
		public InvalidConfigurationException(string msg) : base(msg)
		{
		}

		public InvalidConfigurationException()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
        public InvalidConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

	}
}
