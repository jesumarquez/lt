#region Usings

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Microsoft.Win32;

#endregion

namespace Logictracker.WindowsServices
{
    [FrameworkElement(XName = "WinService", IsContainer = false)]
    public class WinService : FrameworkElement
    {
		#region Attributes
		
		/// <value>Path del xml del servicio</value>
		[ElementAttribute(XName = "HostApplication", IsSmartProperty = true, IsRequired = true)]
		public string HostApplication
		{
			get { return (string)GetValue("HostApplication"); }
			set { SetValue("HostApplication", value); }
		}

		/// <value>Nombre del servicio</value>
		[ElementAttribute(XName = "ServiceName", IsSmartProperty = true, IsRequired = true)]
		public string ServiceName
		{
			get { return (string)GetValue("ServiceName"); }
			set { SetValue("ServiceName", value); }
		}

		/// <value>Nombre visible en el Service Manager</value>
		[ElementAttribute(XName = "DisplayName", IsSmartProperty = true, IsRequired = true)]
		public string DisplayName
		{
			get { return (string)GetValue("DisplayName"); }
			set { SetValue("DisplayName", value); }
		}

		/// <value>Descripcion del servicio visible en el Service Manager</value>
        [ElementAttribute(XName = "Description", IsSmartProperty = true, IsRequired = false, DefaultValue = "Logictracker Framework 2.0, Servicio Windows.")]
		public string Description
		{
			get { return (string)GetValue("Description"); }
			set { SetValue("Description", value); }
		}

		/// <value>Modo de Inicio del serivio.</value>
		[ElementAttribute(XName = "StartMode", DefaultValue = ServiceStartMode.Manual)]
		public ServiceStartMode StartMode { get; set; }

		/// <value>Cuenta de usuario que inicia el servicio.</value>
		[ElementAttribute(XName = "ServiceAccount", DefaultValue = ServiceAccount.LocalService)]
		public ServiceAccount ServiceAccount { get; set; }

		/// <value>Accion ante la primera falla.</value>
		[ElementAttribute(XName = "FirstFailureAction", IsSmartProperty = true, IsRequired = false, DefaultValue = null)]
		public FailureAction FirstFailureAction
		{
			get { return (FailureAction)GetValue("FirstFailureAction"); }
			set { SetValue("FirstFailureAction", value); }
		}

		/// <value>Accion ante la segunda falla.</value>
		[ElementAttribute(XName = "SecondFailureAction", IsSmartProperty = true, IsRequired = false, DefaultValue = null)]
		public FailureAction SecondFailureAction
		{
			get { return (FailureAction)GetValue("SecondFailureAction"); }
			set { SetValue("SecondFailureAction", value); }
		}

		/// <value>Accion ante las fallas subsiguientes.</value>
		[ElementAttribute(XName = "SubsequentFailuresAction", IsSmartProperty = true, IsRequired = false, DefaultValue = null)]
		public FailureAction SubsequentFailuresAction
		{
			get { return (FailureAction)GetValue("SubsequentFailuresAction"); }
			set { SetValue("SubsequentFailuresAction", value); }
		}

		/// <value>Comando a ejecutarse frente a una falla.</value>
		[ElementAttribute(XName = "FailureRunCommand")]
		public string FailureRunCommand { get; set; }
		
		#endregion

		#region FrameworkElement

		public override bool LoadResources()
		{
			return true;
		}

    	#endregion

	    #region Internal Members

        internal static void EnableAndStart(IEnumerable<ServiceController> winServicesAll)
	    {
		    foreach (var service in winServicesAll)
		    {
			    _SetServiceStart(service.ServiceName, ServiceStartMode.Automatic); //***
			    service.Refresh();
			    if (service.Status == ServiceControllerStatus.Stopped)
			    {
				    try
				    {
					    service.Start();
				    }
				    catch (Exception e)
				    {
					    Console.WriteLine("Exception: " + e);
				    }
			    }
			    service.Close();
		    }
	    }

	    internal static void StopAndDisable(IEnumerable<ServiceController> winServicesAll)
	    {
		    foreach (var service in winServicesAll)
		    {
			    service.Refresh();
			    if (service.CanStop)
			    {
				    service.Stop();
			    }
			    _SetServiceStart(service.ServiceName, ServiceStartMode.Disabled); //*****
			    service.Close();
		    }
	    }

        public static void RestartService(ServiceController service)
        {
            service.Refresh();
            if (service.CanStop)
            {
                service.Stop();
                Thread.Sleep(5000);
                service.Start();
            }
            service.Close();
        }

	    internal static ServiceStartMode GetServiceStart(String serviceName)
	    {
		    var key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\" + serviceName);

		    if (key == null) throw new ApplicationException("El registro no tiene la entrada del servicio " + serviceName);

		    var start = (ServiceStartMode) key.GetValue("Start");

		    key.Close();

		    return (start);
	    }

	    /// <summary>
	    /// Obtiene la lista de servicios
	    /// </summary>
	    /// <param name="prefix">si se establece selecciona unicamente aquellos servicios que su nombre este prefijado con "prefix"</param>
	    /// <returns>Lista de los controladores de servicios</returns>
	    internal static IEnumerable<ServiceController> GetWindowsServices(String prefix)
	    {
		    return ServiceController
			    .GetServices()
			    .Where(srv => srv.ServiceName.StartsWith(prefix) || String.IsNullOrEmpty(prefix))
			    .ToList();
	    }

        public static IEnumerable<ServiceController> GetLogictrackerServices()
        {
            return ServiceController
                .GetServices()
                .Where(srv => srv.ServiceName.Contains("Gateway") 
                           || srv.ServiceName.Contains("Dispatcher")
                           || srv.ServiceName.Contains("LogicTracker"))
                .ToList();
        }

	    internal static void InstallAllServices(bool AutomaticUninstall, bool Purge, bool AutomaticInstall, bool AutomaticUpdate, String ServiceNamePrefix, IEnumerable<WinService> winServicesToInstall, WinServiceInstaller parent)
	    {
		    if (AutomaticUninstall) UninstallUndefined(ServiceNamePrefix, Purge, winServicesToInstall);

		    foreach (var winService in winServicesToInstall)
		    {
			    if (AutomaticInstall) winService.InstallService(parent);
			    if (AutomaticUpdate) winService.UpdateService(parent);
		    }
	    }

	    #endregion

	    #region Private Members

		private void InstallService(WinServiceInstaller group)
	    {
		    try
		    {
			    var serviceKey = group.ServiceNamePrefix + ServiceName;

			    Console.WriteLine("Installing: {0}", serviceKey);

			    var serviceController = GetWindowsService(serviceKey);

			    if (serviceController != null)
			    {
				    Console.WriteLine(" Already Installed.");

				    return;
			    }

			    var installLogFile = Path.GetTempFileName();

			    Console.WriteLine("Log: {0}", installLogFile);

			    var context = new InstallContext(installLogFile, new[] {String.Format("/assemblypath={0}", Process.GetCurrentProcess().MainModule.FileName.Replace("vshost.", ""))});

			    var runner = new Installer {Context = context};

			    var procesServiceInstaller = new ServiceProcessInstaller
				    {
					    Context = context,
					    Account = ServiceAccount,
					    Username = null,
					    Password = null,
					    CmdLineArgs = new[] {String.Format("{0} {1}", HostApplication, serviceKey)}
				    };

			    var serviceInstallerObj = new ServiceInstaller
				    {
					    Context = context,
					    DisplayName = DisplayName,
					    Description = Description,
					    ServiceName = serviceKey,
					    ServicesDependedOn = default(string[]),
					    StartType = StartMode,
					    StartOnInstall = false,
					    FailRunCommand = FailureRunCommand
				    };

			    if (FirstFailureAction != null) serviceInstallerObj.FailureActions.Add(FirstFailureAction);
			    if (SecondFailureAction != null) serviceInstallerObj.FailureActions.Add(SecondFailureAction);
			    if (SubsequentFailuresAction != null) serviceInstallerObj.FailureActions.Add(SubsequentFailuresAction);

			    serviceInstallerObj.FailCountResetTime = default(int); // 1 dia.

			    runner.Installers.Add(procesServiceInstaller);
			    runner.Installers.Add(serviceInstallerObj);

			    var state = new ListDictionary();

			    try
			    {
				    runner.Install(state);
				    runner.Commit(state);
			    }
			    catch (Exception e)
			    {
				    Console.WriteLine("Exception: " + e);
				    runner.Rollback(state);
			    }
		    }
		    catch (Exception e)
		    {
			    Console.WriteLine("Exception: " + e);
		    }
	    }

		private void UpdateService(WinServiceInstaller group)
	    {
		    try
		    {
			    var serviceKey = group.ServiceNamePrefix + ServiceName;

			    Console.Write("Updating: {0}", serviceKey);

			    var serviceController = GetWindowsService(serviceKey);

			    /////// UPDATE
			    if (serviceController != null)
			    {
				    serviceController.DisplayName = DisplayName;

				    try
				    {
					    if (StartMode != GetServiceStart(serviceKey)) _SetServiceStart(serviceKey, StartMode);
				    }
				    catch (ApplicationException e)
				    {
					    Console.WriteLine("Exception: " + e);
				    }

				    Console.WriteLine(" Done.");

				    return;
			    }

			    Console.WriteLine(" Not Found.");

		    }
		    catch (Exception e)
		    {
			    Console.WriteLine("Exception: " + e);
		    }
	    }

	    private static void _SetServiceStart(String serviceName, ServiceStartMode value)
	    {
		    var key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\" + serviceName, true);

		    if (key == null) return;

		    key.SetValue("Start", (int) value);

		    key.Close();
	    }

	    private static ServiceController GetWindowsService(String name)
	    {
		    return ServiceController.GetServices().FirstOrDefault(srv => srv.ServiceName == name);
	    }

	    private static void UninstallUndefined(String ServiceNamePrefix, bool Purge, IEnumerable<WinService> services)
	    {
		    Console.WriteLine("Uninstalling Undefined Services:");
		    var c = 0;
		    if (ServiceNamePrefix.Length < 12)
		    {
			    Console.WriteLine("ServiceNamePrefix menos de 12 caracteres, ignoro.");
			    return;
		    }

		    var installed = GetWindowsServicesNamesList(ServiceNamePrefix);

		    var todelete = (Purge || services == null || !services.Any())
			                   ? installed
			                   : installed.Except(services.Select(t => ServiceNamePrefix + t.ServiceName));

		    foreach (var svcname in todelete)
		    {
			    c++;
			    UnInstallService(svcname);
		    }
		    Console.WriteLine(" Total: {0}", c);
	    }

	    private static IEnumerable<String> GetWindowsServicesNamesList(string prefix)
	    {
		    return ServiceController.GetServices().Where(srv => srv.ServiceName.StartsWith(prefix) || String.IsNullOrEmpty(prefix)).Select(name => name.ServiceName).ToList();
	    }

	    private static void UnInstallService(string svcName)
	    {
		    Console.Write("  Service {0} ", svcName);

		    const int GENERIC_WRITE = 0x40000000;
		    var sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);
		    if (sc_hndl.ToInt32() == 0)
		    {
			    Console.WriteLine(" OpenSCManager Error!!");
			    return;
		    }
		    const int DELETE = 0x10000;
		    var svc_hndl = OpenService(sc_hndl, svcName, DELETE);
		    if (svc_hndl.ToInt32() == 0)
		    {
			    Console.WriteLine(" OpenService Error!!");
			    return;
		    }
		    var i = DeleteService(svc_hndl);
		    if (i == 0)
		    {
			    Console.WriteLine(" DeleteService Error!!");
			    CloseServiceHandle(sc_hndl);
			    return;
		    }

		    Console.WriteLine(" uninstall success");
		    CloseServiceHandle(sc_hndl);
	    }

	    [DllImport("advapi32.dll")]
	    private static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);

	    [DllImport("advapi32.dll", SetLastError = true)]
	    private static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);

	    [DllImport("advapi32.dll")]
	    private static extern int DeleteService(IntPtr SVHANDLE);

	    [DllImport("advapi32.dll")]
	    private static extern void CloseServiceHandle(IntPtr SCHANDLE);

	    #endregion
	}
}