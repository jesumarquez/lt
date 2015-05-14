#region Usings

using System;

#endregion

namespace Logictracker.Web.Monitor
{
    public class MonitorEventArgs: EventArgs
    {
        public MonitorEventArgs(string eventArgument)
        {
            EventArgument = eventArgument;
        }

        public string EventArgument { get; set; }
    }
}