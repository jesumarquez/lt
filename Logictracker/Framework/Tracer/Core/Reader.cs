#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Enums;
using Logictracker.DatabaseTracer.NHibernateManagers;
using Logictracker.DatabaseTracer.Types;
using Logictracker.DatabaseTracer.ValueObjects;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DatabaseTracer.Core
{
    /// <summary>
    /// Class for reading logs entry from database.
    /// </summary>
    public class Reader : IDisposable
    {
        #region Private Properties

        /// <summary>
        /// NHibernate data access session.
        /// </summary>
        private ISession _session;

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new database log entries reader and its data access nhibernate session.
        /// </summary>
        public Reader() { _session = NHibernateHelper.GetSession(); }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves from database all log entries that match the specified search criterias.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="type"></param>
        /// <param name="module"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public List<LogEntry> GetEntries(DateTime from, DateTime to, LogTypes? type, LogModules? module, LogComponents? component)
        {
            var logType = type.HasValue ? type.Value.GetValue() : 0;
            var logModule = module.HasValue ? module.Value.GetDescription() : string.Empty;
            var logComponent = component.HasValue ? component.Value.GetDescription() : string.Empty;

            var logs = _session.Query<Log>()
                               .Where(log => log.DateTime >= from 
                                          && log.DateTime <= to 
                                          && log.Type >= logType 
                                          && (logModule.Equals(string.Empty) || log.Module.Equals(logModule))
                                          && (logComponent.Equals(string.Empty) || log.Component.Equals(logComponent)))
                               .ToList();

            var entries = new List<LogEntry>();

            if (logs.Any()) entries.AddRange(logs.Select(log => new LogEntry(log)));

            return entries;
        }

        public List<Log> GetLastEntries(int dispositivo, int mintype, DateTime mindate, int maxresults)
        {
            return _session.Query<Log>()
                .Where(log => log.Device == dispositivo && log.Type >= mintype)
                .Where(log => log.DateTime >= mindate)
                .OrderByDescending(log=>log.DateTime)
                .Take(maxresults)
                .ToList();
        }

        /// <summary>
        /// Loads the associated context for the givenn log entry.
        /// </summary>
        /// <param name="idLogEntry"></param>
        /// <returns></returns>
        public List<LogContext> GetContext(int idLogEntry) { return _session.Query<LogContext>().Where(c => c.Log != null && c.Log.Id == idLogEntry).ToList(); }

        /// <summary>
        /// Retrieves from database all log entries that match the specified search criterias.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<LogEntry> GetEntries(DateTime from, DateTime to, LogTypes type)
        {
            return GetEntries(from, to, type, null, null);
        }

        /// <summary>
        /// Retrieves from database all log entries that match the specified search criterias.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public List<LogEntry> GetEntries(DateTime from, DateTime to, LogModules module)
        {
            return GetEntries(from, to, null, module, null);
        }

        /// <summary>
        /// Retrieves from database all log entries that match the specified search criterias.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<LogEntry> GetEntries(DateTime from, DateTime to)
        {
            return GetEntries(from, to, null, null, null);
        }

        /// <summary>
        /// Deallocate all assigned resources.
        /// </summary>
        public void Dispose()
        {
            _session.Clear();

            _session.Close();

            _session = null;
        }

        #endregion
    }
}
