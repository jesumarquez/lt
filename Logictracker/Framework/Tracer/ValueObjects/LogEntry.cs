#region Usings

using System;
using System.Collections.Generic;
using Logictracker.DatabaseTracer.Types;

#endregion

namespace Logictracker.DatabaseTracer.ValueObjects
{
    /// <summary>
    /// Value object for representing a database log entry.
    /// </summary>
    public class LogEntry
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new log entry based on the givenn database log.
        /// </summary>
        /// <param name="log"></param>
        public LogEntry(Log log)
        {
            Id = log.Id;
            Component = log.Component;
            DateTime = log.DateTime;
            Device = log.Device;
            Message = log.Message;
            Module = log.Module;
            Type = log.Type;
            Vehicle = log.Vehicle;

            AddContext(log.Context);
        }

        #endregion

        #region Public Properties

        public Int32 Id { get; set; }
        public String Component { get; set; }
        public DateTime DateTime { get; set; }
        public Int32? Device { get; set; }
        public String Message { get; set; }
        public String Module { get; set; }
        public Int32 Type { get; set; }
        public Int32? Vehicle { get; set; }
        public List<LogEntryContext> Context { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the specified context to the current entry.
        /// </summary>
        /// <param name="details"></param>
        private void AddContext(ICollection<LogContext> details)
        {
            Context = new List<LogEntryContext>();

            if (details == null || details.Count.Equals(0)) return;

            foreach (var detail in details) Context.Add(new LogEntryContext(detail));
        }

        #endregion
    }
}