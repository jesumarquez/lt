#region Usings

using System;
using System.Xml.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;

#endregion

namespace Logictracker.Scheduler
{
    [FrameworkElement(XName = "LogictrackerScheduler", XNamespace = ConfigScheduler.LogictrackerSchedulerNamespaceUri, IsContainer = true)]
    public class LogictrackerScheduler : FrameworkElement, ILayer
    {
        #region Public Methods

        public bool ServiceStart()
        {
            try
            {
				STrace.Trace(GetType().FullName, "Starting Logictracker.Scheduler");

                Core.Scheduler.Instance.Start();

                return true;
            }
            catch (Exception ex)
            {
				STrace.Exception(GetType().FullName, ex);

                return false;
            }
        }

        public bool ServiceStop()
        {
            try
            {
				STrace.Trace(GetType().FullName, "Stopping Logictracker.Scheduler");

                Core.Scheduler.Instance.Stop();

				GC.Collect();

                return true;
            }
            catch (Exception ex)
            {
				STrace.Exception(GetType().FullName, ex);

                return false;
            }
        }

        public bool StackBind(ILayer bottom, ILayer top)
        {
            return true;
        }

        #endregion
    }

	public class ConfigScheduler
	{
		public const String LogictrackerSchedulerNamespaceUri = "http://walks.ath.cx/e/logictrackerscheduler";
		public static readonly XNamespace LogictrackerSchedulerNamespace = XNamespace.Get(LogictrackerSchedulerNamespaceUri);
	}
}