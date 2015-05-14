#region Usings

using System;
using Iesi.Collections.Generic;
using System.Diagnostics;

#endregion

namespace Logictracker.DatabaseTracer.Types
{
    /// <summary>
    /// Class that represents a database trace.
    /// </summary>
    public class Log
    {
        #region Private Properties
        
        private ISet<LogContext> _context;

        #endregion

        #region Public Properties

        public virtual Int32 Id { get; set; }
        public virtual DateTime DateTime { get; set; }
        public virtual Int32 Type { get; set; }
        public virtual String Module { get; set; }
        public virtual String Component { get; set; }
        public virtual String Message { get; set; }
        public virtual ISet<LogContext> Context { get { return _context ?? (_context = new HashedSet<LogContext>()); } }
        public virtual Int32? Vehicle { get; set; }
        public virtual Int32? Device { get; set; }

        public virtual Int32 Retries { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if the givenn object equals the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var castObj = obj as Log;

            return castObj != null && DateTime.Equals(castObj.DateTime) && Type.Equals(castObj.Type) && Module.Equals(castObj.Module) && Component.Equals(castObj.Component)
                && Message.Equals(castObj.Message);
        }

        /// <summary>
        /// Gets the hash code of the current object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return DateTime.GetHashCode() + Type.GetHashCode() + Module.GetHashCode() + Component.GetHashCode() + Message.GetHashCode();
        }

        #endregion
    }

    public static class LogX
    {
        /// <summary>
        /// Adds a new log context detail with the specified parameters.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="key">The key of the context detail.</param>
        /// <param name="value">The value of the context detail.</param>
        public static void AddContext(this Log log, String key, String value)
        {
            Debug.Assert(log != null);
            log.Context.Add(
                new LogContext
                {
                    Log = log,
                    Key = key,
                    Value = value
                });
        }
    }
}