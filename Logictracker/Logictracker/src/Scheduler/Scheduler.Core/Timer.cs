#region Usings

using System;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.Scheduler
{
    /// <summary>
    /// Extends the auto generated Timer class.
    /// </summary>
    public partial class Timer
    {
        #region Public Methods

        /// <summary>
        /// Gets the interval to the next excecution start date.
        /// </summary>
        /// <returns></returns>
        public double GetInterval()
        {
            switch (Periodicity)
            {
                case TimerPeriodicity.Day: return Frequency * 86400000;
                case TimerPeriodicity.Hour: return Frequency * 3600000;
                case TimerPeriodicity.Minute: return Frequency * 60000;
                case TimerPeriodicity.Second: return Frequency * 1000;
            }

            return 0;
        }

        /// <summary>
        /// Gets the interval to the first excecution start date.
        /// </summary>
        /// <returns></returns>
        public double GetIntervalToStart()
        {
            var start = StartDate == default(DateTime) ? DateTime.Now.Date : StartDate;
	        var start1 = start;

            start = start.Add(StartTime.TimeOfDay);
			var start2 = start;

            start = GetNextStartDate(start);
			var start3 = start;

			STrace.Debug(GetType().FullName, String.Format("GetIntervalToStart: 1={0} 2={1} 3={2} Periodicity={3} StartTime={4}", start1, start2, start3, Periodicity, StartTime));

			return start.Subtract(DateTime.Now).TotalMilliseconds;
        }

        /// <summary>
        /// Determines if the timer is enables based on its enabled attribute and its active time span.
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled() { return Enabled && IsActive(); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the next execution scheduled date for the timer.
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        private DateTime GetNextStartDate(DateTime start)
        {
            while (Periodicity == TimerPeriodicity.Day && start < DateTime.Now) start = start.AddDays(Frequency);
            while (Periodicity == TimerPeriodicity.Hour && start < DateTime.Now) start = start.AddHours(Frequency);
            while (Periodicity == TimerPeriodicity.Minute && start < DateTime.Now) start = start.AddMinutes(Frequency);
            while (Periodicity == TimerPeriodicity.Second && start < DateTime.Now) start = start.AddSeconds(Frequency);

            return start;
        }

        /// <summary>
        /// Determines if the timer is within its active time span.
        /// </summary>
        /// <returns></returns>
        private bool IsActive()
        {
            if (EndDate == default(DateTime)) return true;

            var end = EndDate.Add(StartTime == default(DateTime) ? TimeSpan.Zero : StartTime.TimeOfDay);

            return end >= DateTime.Now;
        }

        #endregion
    }
}