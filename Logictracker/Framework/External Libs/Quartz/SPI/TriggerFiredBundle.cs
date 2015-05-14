#region Usings

#if NET_20
using System;
using Quartz.Core;
using Quartz.Util;
using NullableDateTime = System.Nullable<System.DateTime>;
#else
using Nullables;
#endif

#endregion

namespace Quartz.Spi
{
    /// <summary>
    /// A simple class (structure) used for returning execution-time data from the
    /// JobStore to the <see cref="QuartzSchedulerThread" />.
    /// </summary>
    /// <seealso cref="QuartzScheduler" />
    /// <author>James House</author>
    [Serializable]
    public class TriggerFiredBundle
    {
        private readonly JobDetail job;
        private readonly Trigger trigger;
        private readonly ICalendar cal;
        private readonly bool jobIsRecovering;
        private readonly NullableDateTime fireTimeUtc;
        private readonly NullableDateTime scheduledFireTimeUtc;
        private readonly NullableDateTime prevFireTimeUtc;
        private readonly NullableDateTime nextFireTimeUtc;

        /// <summary>
        /// Gets the job detail.
        /// </summary>
        /// <value>The job detail.</value>
        public virtual JobDetail JobDetail
        {
            get { return job; }
        }

        /// <summary>
        /// Gets the trigger.
        /// </summary>
        /// <value>The trigger.</value>
        public virtual Trigger Trigger
        {
            get { return trigger; }
        }

        /// <summary>
        /// Gets the calendar.
        /// </summary>
        /// <value>The calendar.</value>
        public virtual ICalendar Calendar
        {
            get { return cal; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TriggerFiredBundle"/> is recovering.
        /// </summary>
        /// <value><c>true</c> if recovering; otherwise, <c>false</c>.</value>
        public virtual bool Recovering
        {
            get { return jobIsRecovering; }
        }

        /// <returns> 
        /// Returns the UTC fire time.
        /// </returns>
        public virtual NullableDateTime FireTimeUtc
        {
            get { return fireTimeUtc; }
        }

        /// <summary>
        /// Gets the next UTC fire time.
        /// </summary>
        /// <value>The next fire time.</value>
        /// <returns> Returns the nextFireTimeUtc.</returns>
        public virtual NullableDateTime NextFireTimeUtc
        {
            get { return nextFireTimeUtc; }
        }

        /// <summary>
        /// Gets the previous UTC fire time.
        /// </summary>
        /// <value>The previous fire time.</value>
        /// <returns> Returns the previous fire time. </returns>
        public virtual NullableDateTime PrevFireTimeUtc
        {
            get { return prevFireTimeUtc; }
        }

        /// <returns> 
        /// Returns the scheduled UTC fire time.
        /// </returns>
        public virtual NullableDateTime ScheduledFireTimeUtc
        {
            get { return scheduledFireTimeUtc; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerFiredBundle"/> class.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="trigger">The trigger.</param>
        /// <param name="cal">The calendar.</param>
        /// <param name="jobIsRecovering">if set to <c>true</c> [job is recovering].</param>
        /// <param name="fireTimeUtc">The fire time.</param>
        /// <param name="scheduledFireTimeUtc">The scheduled fire time.</param>
        /// <param name="prevFireTimeUtc">The previous fire time.</param>
        /// <param name="nextFireTimeUtc">The next fire time.</param>
        public TriggerFiredBundle(JobDetail job, Trigger trigger, ICalendar cal, bool jobIsRecovering,
                                  NullableDateTime fireTimeUtc, 
                                  NullableDateTime scheduledFireTimeUtc,
                                  NullableDateTime prevFireTimeUtc,
                                  NullableDateTime nextFireTimeUtc)
        {
            this.job = job;
            this.trigger = trigger;
            this.cal = cal;
            this.jobIsRecovering = jobIsRecovering;
            this.fireTimeUtc = DateTimeUtil.AssumeUniversalTime(fireTimeUtc);
            this.scheduledFireTimeUtc = DateTimeUtil.AssumeUniversalTime(scheduledFireTimeUtc);
            this.prevFireTimeUtc = DateTimeUtil.AssumeUniversalTime(prevFireTimeUtc);
            this.nextFireTimeUtc = DateTimeUtil.AssumeUniversalTime(nextFireTimeUtc);
        }
    }
}