namespace Quartz
{
    /// <summary>
    /// Instructs Scheduler what to do with a trigger and job.
    /// </summary>
    public enum SchedulerInstruction
    {        
        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that the <see cref="Trigger" />
        /// has no further instructions.
        /// </summary>
        NoInstruction,

        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that the <see cref="Trigger" />
        /// wants the <see cref="JobDetail" /> to re-Execute
        /// immediately. If not in a 'RECOVERING' or 'FAILED_OVER' situation, the
        /// execution context will be re-used (giving the <see cref="IJob" /> the
        /// abilitiy to 'see' anything placed in the context by its last execution).
        /// </summary>      
        ReExecuteJob,

        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that the <see cref="Trigger" />
        /// should be put in the <see cref="TriggerState.Complete" /> state.
        /// </summary>
        SetTriggerComplete,

        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that the <see cref="Trigger" />
        /// wants itself deleted.
        /// </summary>
        DeleteTrigger,

        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that all <see cref="Trigger" />
        /// s referencing the same <see cref="JobDetail" /> as
        /// this one should be put in the <see cref="TriggerState.Complete" /> state.
        /// </summary>
        SetAllJobTriggersComplete,

        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that all <see cref="Trigger" />
        /// s referencing the same <see cref="JobDetail" /> as
        /// this one should be put in the <see cref="TriggerState.Error" /> state.
        /// </summary>
        SetAllJobTriggersError,

        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that the <see cref="Trigger" />
        /// should be put in the <see cref="TriggerState.Error" /> state.
        /// </summary>
        SetTriggerError
    }
}
