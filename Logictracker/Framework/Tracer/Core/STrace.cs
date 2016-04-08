#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Enums;
using Logictracker.DatabaseTracer.Types;
using log4net;
using log4net.Config;

#endregion

namespace Logictracker.DatabaseTracer.Core
{
    public static class STrace
    {
        #region Module Service

        private static String _module;
        public static String Module
        {
            get { return _module ?? (_module = LogModules.LogictrackerGateway.GetDescription()); }
            set
            {
                _module = value;
                if (TracerType.Contains(Config.Tracer.Types.Log4Net)) XmlConfigurator.Configure(new FileInfo(GetAppPath() + "Log4NET.xml"));
            }
        }

        #endregion

        #region Log

        /// <summary>
        /// el resto de los metodos de logueo terminan llamando a este que es el que realmente hace el trabajo
        /// </summary>
        /// <param name="component"></param>
        /// <param name="e"></param>
        /// <param name="device"></param>
        /// <param name="logType"></param>
        /// <param name="context"></param>
        /// <param name="str"></param>
        public static void Log(String component, Exception e, int device, LogTypes logType, Dictionary<String, String> context, String str)
        {
            try
            {
                #region Console Trace
                if (Environment.UserInteractive && ((ConsoleFilter.Length == 1) || ConsoleFilter.Contains(device)))
                {
                    Console.WriteLine(str);

                    if (e != null)
                    {
                        Console.WriteLine("{1}{1}exception {0}{1}", e, Environment.NewLine);

                        if (e.InnerException != null)
                        {
                            Console.WriteLine("{1}innerexception {0}{1}{1}", e.InnerException, Environment.NewLine);
                        }
                    }
                }
                #endregion

                #region Log4NET Trace
                if (TracerType.Contains(Config.Tracer.Types.Log4Net))
                {
                    var logger = LogManager.GetLogger(component);

                    using (ThreadContext.Stacks["DeviceId"].Push(device.ToString(CultureInfo.InvariantCulture)))
                    {

                        switch (logType)
                        {
                            case LogTypes.Debug: logger.Debug(str, e); break;
                            case LogTypes.Trace: logger.Info(str, e); break;
                            case LogTypes.Error: logger.Error(str, e); break;
                            case LogTypes.Exception: logger.Fatal(str, e); break;
                            case LogTypes.Warning: logger.Warn(str, e); break;
                        }
                    } // value will be popped off the stack here
                }
                #endregion

                #region Fota Folder Trace
                /*if (TracerType.Contains(Config.Tracer.Types.Fota) && (device != 0) && ConsoleFilter.Contains(device))
				{
					ReformatCached(ref str, format, args);
					FotaTrace(device, str);
				}//*/
                #endregion

                #region Database Trace
                if (TracerType.Contains(Config.Tracer.Types.DataBase))
                {
                    var type = logType.GetValue();
                    if (!(type < MinLogTypeValue))
                    {
                        var log = new Log
                        {
                            DateTime = DateTime.UtcNow,
                            Module = Module,
                            Component = component,
                            Message = str,
                            Type = type,
                        };

                        if (e != null)
                        {
                            log.AddContext("exception", e.ToString());
                            log.AddContext("exception.message", e.Message);
                            log.AddContext("exception.stacktrace", e.StackTrace);

                            if (e.InnerException != null)
                            {
                                log.AddContext("exception.innerexception", e.InnerException.ToString());
                                log.AddContext("exception.innerexception.message", e.InnerException.Message);
                                log.AddContext("exception.innerexception.stacktrace", e.InnerException.StackTrace);
                            }
                        }
                        else if (type == LogTypes.Error.GetValue())
                        {
                            var stackTrace = new StackTrace();           // get call stack
                            var stackFrames = stackTrace.GetFrames();  // get method calls (frames)
                            var stacksb = new StringBuilder();
                            // write call stack method names
                            foreach (var stackFrame in stackFrames)
                            {
                                stacksb.AppendFormat("{0}{1}", stackFrame, Environment.NewLine);   // write method name
                            }
                            log.AddContext("stacktrace", stacksb.ToString());
                        }

                        if (context != null)
                        {
                            foreach (var ctx in context)
                            {
                                log.AddContext(ctx.Key, ctx.Value);
                            }
                        }

                        if (device != 0) log.Device = device;

                        Queue.Enqueue(log);
                    }
                }
                #endregion
            }
            catch (Exception ee)
            {
                var err = String.Format("Exception:{1}{0}{1}{1}Log original: Component={2}; Format={3}{1}{1}", ee, Environment.NewLine, component, str);
                if (!(ee is IndexOutOfRangeException)) File.AppendAllText(String.Format("logs/{0:yyyy-MM-dd HH mm} TraceErrors.txt", DateTime.Now), err);
                if (Environment.UserInteractive) Console.WriteLine(err);
            }
        }

        #endregion

        #region Debug

        public static void Debug(String component, int device, String str) { Log(component, null, device, LogTypes.Debug, null, str); }

        public static void Debug(String component, String str) { Debug(component, 0, str); }

        public static void Debug(String component, int device, Dictionary<String, String> context, String str) { Log(component, null, device, LogTypes.Debug, context, str); }

        #endregion

        #region Trace

        public static void Trace(String component, int device, String str) { Log(component, null, device, LogTypes.Trace, null, str); }

        public static void Trace(String component, String str) { Trace(component, 0, str); }

        #endregion

        #region Error

        public static void Error(String component, int device, String str) { Log(component, null, device, LogTypes.Error, null, str); }

        public static void Error(String component, String str) { Error(component, 0, str); }

        #endregion

        #region Exception

        public static void Exception(String component, Exception e, int device, Dictionary<String, String> context, String str) { Log(component, e, device, LogTypes.Exception, context, str); }

        public static void Exception(String component, Exception e, int device, String str) { Exception(component, e, device, null, str); }

        public static void Exception(String component, Exception e, int device) { Exception(component, e, device, e.Message); }

        public static void Exception(String component, Exception e, String str) { Exception(component, e, 0, str); }

        public static void Exception(String component, Exception e) { Exception(component, e, e.Message); }

        #endregion

        #region Warning

        public static void Warning(String component, Exception e, int device, Dictionary<String, String> context, String str) { Log(component, e, device, LogTypes.Warning, context, str); }

        public static void Warning(String component, Exception e, int device, String str) { Warning(component, e, device, null, str); }

        public static void Warning(String component, Exception e, int device) { Warning(component, e, device, e.Message); }

        public static void Warning(String component, Exception e, String str) { Warning(component, e, 0, str); }

        public static void Warning(String component, Exception e) { Warning(component, e, e.Message); }

        #endregion

        #region LogTypes Extensions

        internal static Int32 GetValue(this LogTypes logType)
        {
            switch (logType)
            {
                case LogTypes.All: return -1;
                case LogTypes.Debug: return 0;
                case LogTypes.Trace: return 1;
                case LogTypes.Error: return 2;
                case LogTypes.Exception: return 3;
                case LogTypes.Warning: return 4;
                case LogTypes.Nothing: return 999;
                default: return 1;
            }
        }

        #endregion

        #region Private Members

        private static int[] _consoleFilter;
        private static int[] ConsoleFilter { get { return (_consoleFilter ?? (_consoleFilter = Config.Tracer.ConsoleFilter)); } }

        private static String[] _tracerType;
        private static IEnumerable<String> TracerType { get { return _tracerType ?? (_tracerType = Config.Tracer.TracerType); } }

        private static readonly Int32 MinLogTypeValue = GetMinLogTypeValue(Config.Tracer.TracerMinLogType);
        private static Int32 GetMinLogTypeValue(String minLogTypeName)
        {
            var minLogType = String.IsNullOrEmpty(minLogTypeName) || !Enum.IsDefined(typeof(LogTypes), minLogTypeName)
                ? LogTypes.Trace
                : (LogTypes)Enum.Parse(typeof(LogTypes), minLogTypeName);

            return minLogType.GetValue();
        }

        private static String GetAppPath()
        {
            var myPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var i = myPath.LastIndexOf('\\');
            return myPath.Remove(i + 1);
        }

        /*private static void FotaTrace(int DeviceId, String str)
        {
            using (var sw = new StreamWriter(String.Format(@"{0}\{1:D4}.log", LogsFolder, DeviceId), true))
            {
                sw.WriteLine("{0} - {1}", DateTime.UtcNow, str);
                sw.Close();
            }
        }
        private static readonly String LogsFolder = GetAppPath() + @"\logs";
        //*/

        #endregion
    }

    #region Public Enums

    public enum LogTypes
    {
        All,
        Debug,
        Trace,
        Error,
        Exception,
        Warning,
        Nothing,
    };

    #endregion
}
