using System;

namespace Logictracker.Scheduler.Core.Tasks
{
    /// <summary>
    /// Timers tasks handlers common interface.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Executes on timer elapsed interval.
        /// </summary>
        /// <param name="timer"></param>
        void Execute(Timer timer);

        /// <summary>
        /// Sets tasks parameters.
        /// </summary>
        /// <param name="parameters"></param>
		void SetParameters(String parameters);
    }
}