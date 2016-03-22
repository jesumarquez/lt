#region Usings
using System;
using System.ServiceProcess;
using System.Threading;
using Logictracker.DatabaseTracer.Core;
#endregion

namespace Logictracker.Description.Runtime
{
    /// <summary>
    /// Class for running a framework application as a windows service.
    /// </summary>
    partial class ServiceApplicationHost : ServiceBase
    {
        #region Private Members

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
        /// Main service thread for runnin ghosted application.
        /// </summary>
        private void ServiceThread()
        {
         //   Debugger.LaunchDebugger(4);

            try
            {
                STrace.Debug(GetType().FullName, String.Format("Iniciando Worker de Servicio windows: {0}", ServiceName));

                var errorCounter = 0;

                var returnValue = _application.Run(HostApplication);

                while (returnValue is ElementException)
                {
                    errorCounter++;

                    if (errorCounter > (MaxApplicationsRestarts - 1)) throw returnValue as ElementException;

                    Thread.Sleep(100);

                    returnValue = _application.Run(HostApplication);
                }
            }
            catch (Exception e)
            {
				STrace.Exception(GetType().FullName, e);
            }
            finally
            {
                STrace.Debug(GetType().FullName, String.Format("Terminando Worker de Servicio windows: {0}", ServiceName));
            }
        }

        #endregion

		#region ServiceBase

		/// <summary>
        /// Starts the win service for hosting the specified application.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
     //       Debugger.LaunchDebugger(3);

            STrace.Debug(GetType().FullName, String.Format("Iniciando servicio windows: {0}", ServiceName));

            _application = new FrameworkApplication();

            _worker = new Thread(ServiceThread);

            _worker.Start();
        }

        /// <summary>
        /// Stops the current service and kills the hosted application.
        /// </summary>
        protected override void OnStop()
        {
			//_application.Unload();

            Environment.Exit(0);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new service host for running the specified application.
        /// </summary>
        /// <param name="svcName"></param>
        /// <param name="hostApplication"></param>
        public ServiceApplicationHost(String svcName, String hostApplication)
        {
            ServiceName = svcName;
            HostApplication = hostApplication;

            InitializeComponent();
        }

        #endregion
    }
}