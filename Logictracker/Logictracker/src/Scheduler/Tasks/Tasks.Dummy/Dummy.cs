using System;
using Urbetrack.Scheduler.Core.Tasks.BaseTasks;

namespace Urbetrack.Scheduler.Tasks.Dummy
{
    public class Dummy : BaseTask
    {
        protected override void OnExecute(Timer timer) { throw new NotImplementedException("Execute dummy task"); }
    }
}
