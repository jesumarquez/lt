#region Usings

using System.Collections;

#endregion

namespace Quartz.Listener
{
    /// <summary>
    /// Holds a List of references to SchedulerListener instances and broadcasts all
    ///  events to them (in order).
    ///</summary>
    /// <remarks>
    /// This may be more convenient than registering all of the listeners
    /// directly with the Scheduler, and provides the flexibility of easily changing
    /// which listeners get notified.
    /// </remarks>
    /// <see cref="AddListener" />
    /// <see cref="RemoveListener" />
    /// <author>James House</author>
    public class BroadcastSchedulerListener : ISchedulerListener
    {
        private readonly IList listeners;

        public BroadcastSchedulerListener()
        {
            listeners = new ArrayList();
        }


        /// <summary>
        /// Construct an instance with the given List of listeners.
        /// </summary>
        /// <param name="listeners">The initial List of SchedulerListeners to broadcast to.</param>
        public BroadcastSchedulerListener(IList listeners)
        {
            this.listeners.Add(listeners);
        }


        public void AddListener(ISchedulerListener listener)
        {
            listeners.Add(listener);
        }

        public bool RemoveListener(ISchedulerListener listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
                return true;
            }
            else
            {
                return false;
            }
        }

        public IList GetListeners()
        {
            return ArrayList.ReadOnly(listeners);
        }

        public void JobScheduled(Trigger trigger)
        {
            foreach (ISchedulerListener l in listeners)
            {
                l.JobScheduled(trigger);
            }
        }

        public void JobUnscheduled(string triggerName, string triggerGroup)
        {
            foreach (ISchedulerListener l in listeners)
            {
                l.JobUnscheduled(triggerName, triggerGroup);
            }
        }

        public void TriggerFinalized(Trigger trigger)
        {
            foreach (ISchedulerListener l in listeners)
            {
                l.TriggerFinalized(trigger);
            }
        }

        public void TriggersPaused(string triggerName, string triggerGroup)
        {
            foreach (ISchedulerListener l in listeners)
            {
                l.TriggersPaused(triggerName, triggerGroup);
            }
        }

        public void TriggersResumed(string triggerName, string triggerGroup)
        {
            foreach (ISchedulerListener l in listeners)
            {
                l.TriggersResumed(triggerName, triggerGroup);
            }
        }

        public void JobsPaused(string jobName, string jobGroup)
        {
            foreach (ISchedulerListener l in listeners)
            {
                l.JobsPaused(jobName, jobGroup);
            }
        }

        public void JobsResumed(string jobName, string jobGroup)
        {
            foreach (ISchedulerListener l in listeners)
            {
                l.JobsResumed(jobName, jobGroup);
            }
        }

        public void SchedulerError(string msg, SchedulerException cause)
        {
            foreach (ISchedulerListener l in listeners)
            {
                l.SchedulerError(msg, cause);
            }
        }

        public void SchedulerShutdown()
        {
            foreach (ISchedulerListener l in listeners)
            {
                l.SchedulerShutdown();
            }
        }
    }
}