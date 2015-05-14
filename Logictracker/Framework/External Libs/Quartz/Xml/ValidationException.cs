#region Usings

using System;
using System.Collections;
using System.Text;

#endregion

namespace Quartz.Xml
{
	/// <summary> 
	/// Reports JobSchedulingDataProcessor validation exceptions.
	/// </summary>
	/// <author> <a href="mailto:bonhamcm@thirdeyeconsulting.com">Chris Bonham</a></author>
	[Serializable]
	public class ValidationException : Exception
	{
		private readonly ICollection validationExceptions = new ArrayList();

		/// <summary>
		/// Gets the validation exceptions.
		/// </summary>
		/// <value>The validation exceptions.</value>
		public virtual ICollection ValidationExceptions
		{
			get { return validationExceptions; }
		}

		/// <summary>
		/// Returns the detail message string.
		/// </summary>
		public override string Message
		{
			get
			{
				if (ValidationExceptions.Count == 0)
				{
					return base.Message;
				}

				var sb = new StringBuilder();

				var first = true;

				foreach (Exception e in ValidationExceptions)
				{
					if (!first)
					{
						sb.Append('\n');
						first = false;
					}

					sb.Append(e.Message);
				}

				return sb.ToString();
			}
		}

		/// <summary>
		/// Constructor for ValidationException.
		/// </summary>
		public ValidationException()    
		{
		}

		/// <summary>
		/// Constructor for ValidationException.
		/// </summary>
		/// <param name="message">exception message.</param>
		public ValidationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Constructor for ValidationException.
		/// </summary>
		/// <param name="errors">collection of validation exceptions.</param>
		public ValidationException(ICollection errors) : this()
		{
			validationExceptions = ArrayList.ReadOnly(new ArrayList(errors));
		}
	}
}