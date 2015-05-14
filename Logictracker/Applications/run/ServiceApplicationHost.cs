#region Usings

using System;
using System.ServiceProcess;
using System.Threading;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Description;
using Urbetrack.Description.Common;
using Urbetrack.Description.Runtime;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Runtime
{
    /// <summary>
    /// Class for running a framework application as a windows service.
    /// </summary>
    partial class ServiceApplicationHost : ServiceBase
    {
        #region Private Properties

        /// <summary>
        /// The dll and class of the application to host.
        /// </summary>
        private string HostApplication { get; set; }

        /// <summary>
        /// The instanc eof the application to host.
        /// </summary>
        private FrameworkApplication _application;

        /// <summary>
        /// The main worker thread for the win service.
        /// </summary>
        private Thread _worker; 

        /// <summary>
        /// Defines a maximum amount of application restarts before killing the service.
        /// </summary>
        private const int MaxApplicationsRestarts = 10;

        /// <summary>
        /// Arguments to be passed to the hosted application.
        /// </summary>
        private string[] _arguments;

        #endregion

        #region Usings

        /// <summary>
        /// Main service thread for runnin ghosted application.
        /// </summary>
        private void ServiceThread()
        {
            Diagnostics.Debugger.LaunchDebugger(4);

            try
            {
                STrace.Debug(GetType().FullName,"Iniciando Worker de Servicio windows: {0}", ServiceName);

                var errorCounter = 0;

                var returnValue = _application.Run(HostApplication, RunMode.WinService, _arguments);

                while (returnValue is ElementException)
                {
                    errorCounter++;

                    if (errorCounter > (MaxApplicationsRestarts - 1)) throw returnValue as ElementException;

                    Thread.Sleep(100);

                    returnValue = _application.Run(HostApplication, RunMode.WinService, _arguments);
                }
            }
            catch (Exception e)
            {
				STrace.Exception(GetType().FullName,e, "Run Stage Exception");
            }
            finally
            {
                STrace.Debug(GetType().FullName,"Terminando Worker de Servicio windows: {0}", ServiceName);
            }
        }

        #endregion

        #region Usings

        /// <summary>
        /// Starts the win service for hosting the specified application.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            Diagnostics.Debugger.LaunchDebugger(3);

            STrace.Debug(GetType().FullName,"Iniciando servicio windows: {0}", ServiceName);

            _arguments = args;

            _application = new FrameworkApplication();

            _worker = new Thread(ServiceThread);

            _worker.Start();
        }

        /// <summary>
        /// Stops the current service and kills the hosted application.
        /// </summary>
        protected override void OnStop()
        {
            STrace.Debug(GetType().FullName,"Descargando Aplicacion: {0}", ServiceName);

            _application.Unload();

            STrace.Debug(GetType().FullName,"Terminando servicio windows: {0}", ServiceName);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            Process.Exit(Process.ExitCodes.ServiceStop, "Detenido por el usuario.");
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new service host for running the specified application.
        /// </summary>
        /// <param name="svcName"></param>
        /// <param name="hostApplication"></param>
        public ServiceApplicationHost(string svcName, string hostApplication)
        {
            HostApplication = hostApplication;
            ServiceName = svcName;

            _application = new FrameworkApplication();

            InitializeComponent();
        }

        #endregion
    }
}