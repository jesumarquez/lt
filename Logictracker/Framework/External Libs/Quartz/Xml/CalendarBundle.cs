#region Usings

using System;
using Quartz.Util;

#endregion

namespace Quartz.Xml
{
	/// <summary> 
	/// Wraps a <see cref="ICalendar" />.
	/// </summary>
	/// <author><a href="mailto:bonhamcm@thirdeyeconsulting.com">Chris Bonham</a></author>
	[Serializable]
	public class CalendarBundle : ICalendar
	{
	    private string typeName;
		private ICalendar calendar;

	    /// <summary>
	    /// Gets or sets the name of the calendar.
	    /// </summary>
	    /// <value>The name of the calendar.</value>
	    public virtual string CalendarName { get; set; }

	    /// <summary>
		/// Gets or sets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		public virtual string TypeName
		{
			get { return typeName; }
			set
			{
				typeName = value;
				CreateCalendar();
			}
		}

		/// <summary>
		/// Gets or sets the calendar.
		/// </summary>
		/// <value>The calendar.</value>
		public virtual ICalendar Calendar
		{
			get { return calendar; }
			set { calendar = value; }
		}

	    public virtual bool Replace { get; set; }

	    /// <summary>
		/// Gets or sets a description for the <see cref="ICalendar" /> instance - may be
		/// useful for remembering/displaying the purpose of the calendar, though
		/// the description has no meaning to Quartz.
		/// </summary>
		/// <value></value>
		public virtual string Description
		{
			get { return calendar.Description; }
			set { calendar.Description = value; }
		}

		/// <summary>
		/// Set a new base calendar or remove the existing one.
		/// </summary>
		/// <value></value>
		public virtual ICalendar CalendarBase
		{
			get { return calendar.CalendarBase; }
			set
			{
				if (value is CalendarBundle)
				{
					value = ((CalendarBundle) value).Calendar;
				}
				calendar.CalendarBase = value;
			}
		}

		public virtual bool IsTimeIncluded(DateTime timeStamp)
		{
			return calendar.IsTimeIncluded(timeStamp);
		}

		public virtual DateTime GetNextIncludedTimeUtc(DateTime timeStamp)
		{
			return calendar.GetNextIncludedTimeUtc(timeStamp);
		}

		protected virtual void CreateCalendar()
		{
			var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new SchedulerConfigException("Unknown calendar type " + typeName);
            }
			Calendar = (ICalendar) ObjectUtils.InstantiateType(type);
		}
	}
}