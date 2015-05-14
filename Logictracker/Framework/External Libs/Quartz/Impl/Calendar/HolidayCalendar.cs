#region Usings

using System;
using Quartz.Collection;

#endregion

namespace Quartz.Impl.Calendar
{
	/// <summary>
	/// This implementation of the Calendar stores a list of holidays (full days
	/// that are excluded from scheduling).
    /// </summary>
	/// <remarks>
	/// The implementation DOES take the year into consideration, so if you want to
	/// exclude July 4th for the next 10 years, you need to add 10 entries to the
	/// exclude list.
	/// </remarks>
	/// <author>Sharada Jambula</author>
	/// <author>Juergen Donnerstag</author>
	[Serializable]
	public class HolidayCalendar : BaseCalendar, ICalendar
	{
		/// <summary>
		/// Returns a <see cref="ISortedSet" /> of Dates representing the excluded
		/// days. Only the month, day and year of the returned dates are
		/// significant.
		/// </summary>
		public virtual ISortedSet ExcludedDates
		{
			get { return TreeSet.UnmodifiableTreeSet(dates); }
		}

		// A sorted set to store the holidays
		private readonly TreeSet dates = new TreeSet();

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayCalendar"/> class.
        /// </summary>
		public HolidayCalendar()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayCalendar"/> class.
        /// </summary>
        /// <param name="baseCalendar">The base calendar.</param>
		public HolidayCalendar(ICalendar baseCalendar)
		{
			CalendarBase = baseCalendar;
		}

		/// <summary>
		/// Determine whether the given time (in milliseconds) is 'included' by the
		/// Calendar.
		/// <p>
		/// Note that this Calendar is only has full-day precision.
		/// </p>
		/// </summary>
		public override bool IsTimeIncluded(DateTime timeStampUtc)
		{
			if (!base.IsTimeIncluded(timeStampUtc))
			{
				return false;
			}

			var lookFor = timeStampUtc.Date;

			return !(dates.Contains(lookFor));
		}

		/// <summary>
		/// Determine the next time (in milliseconds) that is 'included' by the
		/// Calendar after the given time.
		/// <p>
		/// Note that this Calendar is only has full-day precision.
		/// </p>
		/// </summary>
		public override DateTime GetNextIncludedTimeUtc(DateTime timeUtc)
		{
			// Call base calendar implementation first
			var baseTime = base.GetNextIncludedTimeUtc(timeUtc);
			if ((timeUtc != DateTime.MinValue) && (baseTime > timeUtc))
			{
				timeUtc = baseTime;
			}

			// Get timestamp for 00:00:00
			var day = timeUtc.Date;

			while (!IsTimeIncluded(day))
			{
				day = day.AddDays(1);
			}

#if !NET_35
            return day.ToUniversalTime();
#else
            return TimeZoneInfo.ConvertTimeToUtc(day);
#endif
		}

		/// <summary>
		/// Add the given Date to the list of excluded days. Only the month, day and
		/// year of the returned dates are significant.
		/// </summary>
		public virtual void AddExcludedDate(DateTime excludedDateUtc)
		{
			var date = excludedDateUtc.Date;
			dates.Add(date);
		}

		/// <summary>
		/// Removes the excluded date.
		/// </summary>
		/// <param name="dateToRemoveUtc">The date to remove.</param>
		public virtual void RemoveExcludedDate(DateTime dateToRemoveUtc)
		{
			var date = dateToRemoveUtc.Date;
			dates.Remove(date);
		}

        public override int GetHashCode()
        {
            var baseHash = 0;
            if (GetBaseCalendar() != null)
                baseHash = GetBaseCalendar().GetHashCode();

            return ExcludedDates.GetHashCode() + 5 * baseHash;
        }

        public bool Equals(HolidayCalendar obj)
        {
            if (obj == null)
                return false;
            var baseEqual = GetBaseCalendar() != null ?
                             GetBaseCalendar().Equals(obj.GetBaseCalendar()) : true;

            return baseEqual && (ExcludedDates.Equals(obj.ExcludedDates));

        }


        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is HolidayCalendar))
                return false;
            else
                return Equals((HolidayCalendar)obj);


        }
 
	}
}
