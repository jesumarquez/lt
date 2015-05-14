#region Usings

using System;
using System.Linq;
using System.Threading;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks;

#endregion

namespace Logictracker.Scheduler.Core.Timers
{
    /// <summary>
    /// Timer associated to a group of Tasks
    /// </summary>
    public class TaskTimer : System.Timers.Timer
    {
        #region Private Properties

        /// <summary>
        /// The associated configuration timer.
        /// </summary>
        private Timer Config { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initial set up and configuration.
        /// </summary>
        /// <param name="timer"></param>
        public TaskTimer(Timer timer)
        {
            Config = timer;

            AutoReset = true;
            Enabled = Config.Enabled;
            Interval = Config.GetIntervalToStart();
            Elapsed += TaskTimerElapsed;

			STrace.Trace(GetType().FullName, String.Format("TaskTimer: Name={0} Interval={2} tasks={1}", Config.Name, GetTasks(), Interval));

            Start();

			if (Config.RunAtStart) new Thread(() => TaskTimerElapsed(null, null)).Start();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs all associated tasks on elapsed timer interval.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                NHibernateProfiler.Initialize();

                if (!Config.IsEnabled())
                {
                    STrace.Trace(GetType().FullName, String.Format("Timer vencido: timer = {0}", Config.Name));

                    Stop();
                }
                else
                {
                    STrace.Trace(GetType().FullName, String.Format("Tiempo vencido para Timer: timer = {0}", Config.Name));

                    var start = DateTime.Now;

                    //Establishes a high value in order to force the timer to wait until all the tasks finishes.
                    Interval = Int32.MaxValue;

					if (Config.Task != null)
					{
						foreach (var handler in Config.Task)
						{
							STrace.Trace(GetType().FullName, String.Format("Ejecutando Tarea: timer = {0} - tarea = {1}", Config.Name, handler.Class));

							var task = GetTask(handler.Class, handler.Params);

							if (task == null) continue;

							task.Execute(Config);

							STrace.Trace(GetType().FullName, String.Format("Finalizo ejecucion de Tarea: timer = {0} - tarea = {1}", Config.Name, handler.Class));
						}
					}

                    SetNewTimerInterval(start);
                }
            }
            catch (Exception ex)
            {
				STrace.Exception(GetType().FullName, ex);

                throw;
            }
            finally { GC.Collect(); }
        }

        /// <summary>
        /// Sets the new interval till the next timer elapsed time.
        /// </summary>
        /// <param name="start"></param>
        private void SetNewTimerInterval(DateTime start)
        {
            var elapsed = DateTime.Now.Subtract(start).TotalMilliseconds;
            var originalInterval = Config.GetInterval();
            var interval = originalInterval - elapsed;

            while (interval < 0) interval += originalInterval;

            Interval = interval;
        }

        /// <summary>
        /// Dinamically loads the specified class.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
		private ITask GetTask(String className, String parameters)
        {
            try
            {
                var t = Type.GetType(className, true);

                if (t == null)
                {
                    STrace.Trace(GetType().FullName, String.Format("No se puede cargar el tipo: {0}", className));

                    return null;
                }

                var constInfo = t.GetConstructor(new Type[0]);

                if (constInfo == null)
                {
                    STrace.Trace(GetType().FullName, String.Format("No se puede construir la clase:{0}", className));

                    return null;
                }

                var task = (ITask)constInfo.Invoke(null);

                task.SetParameters(parameters);

                return task;
            }
            catch (Exception ex)
            {
				STrace.Exception(GetType().FullName, ex);

                return null;
            }
        }

        /// <summary>
        /// Returns a string containing associated tasks classes.
        /// </summary>
        /// <returns></returns>
        private String GetTasks()
        {
            if (Config == null || Config.Task == null || Config.Task.Count().Equals(0)) return String.Empty;

            return String.Join(",", (from task in Config.Task select task.Class).ToArray());
        }

        #endregion
    }
}