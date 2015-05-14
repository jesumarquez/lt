#region Usings

using System;
using Quartz;

#endregion

namespace Logictracker.Utils
{
    public class TimerJob : IJob
    {
        public delegate void Launch(int timer_id);
        
        public void Execute(JobExecutionContext context)
        {
            var jdm = context.JobDetail.JobDataMap;
            var timer_id = jdm.GetInt("timer_id");
            if (timer_id <= 0)
                throw new Exception("Scheduler Exception: no hay timer_id para ejecutar el timer.");
            var callback = jdm.Get("delegate") as Launch;
            if (callback == null)
                throw new Exception("Scheduler Exception: no hay delegado para ejecutar el timer.");
            callback(timer_id);
        }
    }

    public class StateJob : IJob
    {
        public delegate void Launch(object state);

        public void Execute(JobExecutionContext context)
        {
            var jdm = context.JobDetail.JobDataMap;
            var job_state = jdm.Get("job_state");
            var callback = jdm.Get("delegate") as Launch;
            if (callback == null)
                throw new Exception("Scheduler Exception: no hay delegado para ejecutar el state timer.");
            callback(job_state);
        }
    }
}