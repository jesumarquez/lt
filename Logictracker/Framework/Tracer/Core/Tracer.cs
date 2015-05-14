using System;
using System.Collections.Generic;
using Urbetrack.Common.Configuration;
using Urbetrack.DatabaseTracer.Enums;
using Urbetrack.DatabaseTracer.Types;

namespace Urbetrack.DatabaseTracer.Core
{
    #region Public Classes

    /// <summary>
    /// Urbetrack database logger main class.
    /// </summary>
    public class Tracer : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new urbetrack database tracer.
        /// </summary>
        /// <param name="logModule">The current log module context.</param>
        /// <param name="component">The current log module component.</param>
        public Tracer(LogModules logModule, String component)
        {
            _module = logModule.GetDescription();
            _component = component;

            _log = new Log();
        }

        /// <summary>
        /// Instanciates a new urbetrack database tracer.
        /// </summary>
        /// <param name="module">The current log module context.</param>
        /// <param name="component">The current log module component.</param>
        public Tracer(String module, String component)
        {
            _module = module;
            _component = component;

            _log = new Log();
        }
        #endregion

        #region Private Properties

        /// <summary>
        /// The current tracer log module context.
        /// </summary>
        private String _module;

        /// <summary>
        /// The current tracer class context.
        /// </summary>
        private String _component;

        /// <summary>
        /// The current log.
        /// </summary>
        private Log _log;

        /// <summary>
        /// The min log type to be traced into database.
        /// </summary>
        private static readonly Int32 MinLogTypeValue = GetMinLogTypeValue(Config.Tracer.TracerMinLogType);

        #endregion

        #region Public Methods

        /// <summary>
        /// Instanciates a new database log.
        /// </summary>
        /// <param name="message">The message associated to the log.</param>
        /// <returns>A new database log.</returns>
        public Tracer CreateLog(String message)
        {
            _log = new Log
                       {
                           DateTime = DateTime.UtcNow,
                           Module = _module,
                           Component = _component,
                           Message = message
                       };

            return this;
        }

        /// <summary>
        /// Instanciates a new database log.
        /// </summary>
        /// <param name="component">The current log module component.</param>
        /// <param name="message">The message associated to the log.</param>
        /// <returns>A new database log.</returns>
        public Tracer CreateLog(String component, String message)
        {
            _log = new Log
            {
                DateTime = DateTime.UtcNow,
                Module = _module,
                Component = component,
                Message = message
            };

            return this;
        }

        /// <summary>
        /// Adds a relatinship between the current log and the specified vehicle id (and its assigned device).
        /// </summary>
        /// <param name="vehicleId">The if of vehicle that would be associated to the log.</param>
        /// <returns></returns>
        public Tracer AddVehicle(Int32 vehicleId)
        {
            if (_log == null) throw new Exception("A log must be created in order to add a vehicle to it.");

            _log.Vehicle = vehicleId;

            return this;
        }

        /// <summary>
        /// Adds a relatinship between the current log and the specified device id.
        /// </summary>
        /// <param name="deviceId">The if of device that would be associated to the log.</param>
        /// <returns></returns>
        public Tracer AddDevice(Int32 deviceId)
        {
            if (_log == null) throw new Exception("A log must be created in order to add a device to it.");

            _log.Device = deviceId;

            return this;
        }

        /// <summary>
        /// Adds a new log context detail with the specified parameters.
        /// </summary>
        /// <param name="key">The key of the context detail.</param>
        /// <param name="value">The value of the context detail.</param>
        public Tracer AddContext(String key, String value)
        {
            if (_log == null) throw new Exception("A log must be created in order to add a context to it.");

            _log.Context.Add(
                new LogContext
                    {
                        Log = _log,
                        Key = key,
                        Value = value
                    });

            return this;
        }

        /// <summary>
        /// Save the current log into database in debug mode.
        /// </summary>
        public void Debug()
        {
            Trace(LogTypes.Debug);
        }

        /// <summary>
        /// Save the current log into database in trace mode.
        /// </summary>
        public void Trace()
        {
            Trace(LogTypes.Trace);
        }

        /// <summary>
        /// Save the current log into database in error mode.
        /// </summary>
        public void Error()
        {
            Trace(LogTypes.Error);
        }

        /// <summary>
        /// Save the current log into database in exception mode.
        /// </summary>
        public void Exception()
        {
            Trace(LogTypes.Exception);
        }

        /// <summary>
        /// Trace into database the givenn exception.
        /// </summary>
        /// <param name="exception"></param>
        public void TraceException(Exception exception)
        {
            CreateLog(exception.Message).AddContext("exception.stacktrace", exception.StackTrace);

            if (exception.InnerException != null) AddContext("exception.innerexception", exception.InnerException.Message);

            Exception();
        }

        /// <summary>
        /// Trace into database the givenn exception.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="exception"></param>
        public void TraceException(String component, Exception exception)
        {
            CreateLog(component, exception.Message).AddContext("exception.stacktrace", exception.StackTrace);

            if (exception.InnerException != null) AddContext("exception.innerexception", exception.InnerException.Message);

            Exception();
        }

        /// <summary>
        /// Trace into database the givenn exception and context.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        public void TraceException(Exception exception, Dictionary<String, String> context)
        {
            CreateLog(exception.Message).AddContext("exception.stacktrace", exception.StackTrace);

            if (exception.InnerException != null) AddContext("exception.innerexception", exception.InnerException.Message);

            foreach (var pair in context) AddContext(pair.Key, pair.Value);

            Exception();
        }

        /// <summary>
        /// Dispose all allocated resources.
        /// </summary>
        public void Dispose()
        {
            _module = null;
            _component = null;
            _log = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the min log type to be traced into database.
        /// </summary>
        /// <param name="minLogTypeName"></param>
        /// <returns></returns>
        private static Int32 GetMinLogTypeValue(String minLogTypeName)
        {
            var minLogType = String.IsNullOrEmpty(minLogTypeName) || !Enum.IsDefined(typeof (LogTypes), minLogTypeName) ? LogTypes.Trace
                : (LogTypes)Enum.Parse(typeof (LogTypes), minLogTypeName);

            return minLogType.GetValue();
        }

        /// <summary>
        /// Saves the current log into database with the specified log type.
        /// </summary>
        /// <param name="traceType"></param>
        private void Trace(LogTypes traceType)
        {
            if (_log == null) throw new Exception("A log must be created in order to save it into database.");

            var logTypeValue = traceType.GetValue();

            if (logTypeValue >= MinLogTypeValue)
            {
                _log.Type = logTypeValue;

                Queue.Enqueue(_log);
            }

            _log = null;
        }

        #endregion
    }

    #endregion
}
