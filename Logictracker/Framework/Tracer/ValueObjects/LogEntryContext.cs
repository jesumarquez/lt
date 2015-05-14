#region Usings

using System;
using Logictracker.DatabaseTracer.Types;

#endregion

namespace Logictracker.DatabaseTracer.ValueObjects
{
    /// <summary>
    /// Class that represents a database log entry context.
    /// </summary>
    public class LogEntryContext
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new log entry context based on the givenn database log context.
        /// </summary>
        /// <param name="context"></param>
        public LogEntryContext(LogContext context)
        {
            Id = context.Id;
            Key = context.Key;
            Value = context.Value;
        }

        #endregion

        #region Public Properties

        public Int32 Id { get; set; }
        public String Key { get; set; }
        public String Value { get; set; }

        #endregion
    }
}