#region Usings

using System;

#endregion

namespace Logictracker.DatabaseTracer.Types
{
    /// <summary>
    /// Class that represents a detail about the context of a log message.
    /// </summary>
    public class LogContext
    {
        #region Public Properties

        public virtual Int32 Id { get; set; }
        public virtual Log Log { get; set; }
        public virtual String Key { get; set; }
        public virtual String Value { get; set; }

        #endregion
    }
}