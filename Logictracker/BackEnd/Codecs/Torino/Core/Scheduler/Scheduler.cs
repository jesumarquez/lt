#region Usings

using System;
using Quartz;
using Quartz.Impl;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Comm.Core.Scheduler
{
    public static class Scheduler
    {
        // First we must get a reference to a scheduler
        static private ISchedulerFactory sf;
        static private IScheduler sched;
        static private int next_timer_id;

        static Scheduler()
        {
            next_timer_id = 1;
            sf = new StdSchedulerFactory();
            sched = sf.GetScheduler();
            // start the schedule 
            sched.Start();
        }

        public static void Close()
        {
            sched.Shutdown();
            sched = null;
            sf = null;
        }


        static public int AddTimer(StateJob.Launch callback, object state, int ms_timeout)
        {
            // define the job and tie it to our HelloJob class
            var this_timer_id = next_timer_id++;
            var job = new JobDetail(String.Format("JOB:TIMER:{0}", this_timer_id), null, typeof(StateJob));
            job.JobDataMap.Add("delegate", callback);
            job.JobDataMap.Add("job_state", state);
            var runTime = DateTime.UtcNow.AddMilliseconds(ms_timeout);
            var trigger = new SimpleTrigger(String.Format("JOB:TRIGGER:{0}", this_timer_id), null, runTime);
            sched.ScheduleJob(job, trigger);
            return this_timer_id;
        }

        static public int AddTimer(TimerJob.Launch callback, int ms_timeout)
        {
            var this_timer_id = next_timer_id++;
            // define the job and tie it to our HelloJob class
            var job = new JobDetail(String.Format("JOB:TIMER:{0}",this_timer_id), null, typeof(TimerJob));
            job.JobDataMap.Add("delegate", callback);
            job.JobDataMap.Add("timer_id", this_timer_id);
            var runTime = DateTime.UtcNow.AddMilliseconds(ms_timeout);
            var trigger = new SimpleTrigger(String.Format("JOB:TRIGGER:{0}",this_timer_id), null, runTime);
            sched.ScheduleJob(job, trigger);
            return this_timer_id;
        }

        static public void DelTimer(int timer_id)
        {
            if (sched.DeleteJob(String.Format("JOB:TIMER:{0}", timer_id), null))
            {
                STrace.Debug(typeof(Scheduler).FullName, String.Format("Scheduler: imposible eliminar timer id={0}", timer_id));
            }
        }
        
    }
}