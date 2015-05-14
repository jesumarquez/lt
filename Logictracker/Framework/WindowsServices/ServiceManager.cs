using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Xml.Linq;
using Microsoft.Win32;
using Urbetrack.Model;
using Urbetrack.Toolkit;

namespace Urbetrack.WindowsServices
{
    /// <summary>
    /// Utilidad de administracion de servicios windows.
    /// </summary>
    /// <d:latesee>
    /// Esto quedo Ad-Hoc, utilizar una clase adicional que especifique
    /// toda la especificacion del servicio, permitira reuitilizar
    /// el codigo en cualquier tipo de proyecto.
    /// </d:latesee>
    public static class ServiceManager
    {
        public const string AgentServiceName = "Urbetrack.Agent";
        public const string AgentInstancePrefix = "Urbetrack.Instance$";

        public enum ServiceStartType
        {
            Boot = 0,
            System = 1,
            Automatic = 2,
            Manual = 3,
            Disabled = 4
        }

        /// <summary>
        /// Obtiene la lista de servicios
        /// </summary>
        /// <param name="prefix">si se establece selecciona unicamente aquellos servicios que su nombre este prefijado con "prefix"</param>
        /// <returns>Lista de los controladores de servicios</returns>
        public static List<ServiceController> GetWindowsServices(string prefix)
        {
            var result = new List<ServiceController>();
            foreach (var srv in ServiceController.GetServices())
            {
                if (srv.ServiceName.StartsWith(prefix) || string.IsNullOrEmpty(prefix))
                {
                    result.Add(srv);
                }
            }
            return result;
        }

        public static ServiceController GetWindowsService(string name)
        {
            foreach (var srv in ServiceController.GetServices())
                if (srv.ServiceName == name) return srv;
            return null;
        }

        public static int CountInstalledInstances(string prefix)
        {
            return GetWindowsServices(prefix).Count;
        }

        public static ServiceStartType GetServiceStart(string ServiceName)
        {
            var key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\" + ServiceName );
            if (key == null) throw new ApplicationException("El registro no tiene la entrada del servicio " + ServiceName);
            var start = (ServiceStartType)key.GetValue("Start" );
            key.Close();
            return (start);
        }

        public static void SetServiceStart(string ServiceName, ServiceStartType value)
        {
            var key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\" + ServiceName, true );
            if (key == null) throw new ApplicationException("El registro no tiene la entrada del servicio " + ServiceName);
            key.SetValue( "Start", (int)value );
            key.Close();
        }

        public static bool InstallOrUpdateService(XElement xElement, string ServiceProcess, string AgentAddress, string logfile)
        {
            try
            {
                var ServiceLabel = xElement.xLabel(xElement.Name.LocalName);
                var ServiceDescription = xElement.xDescription();
                var ServiceName = AgentInstancePrefix + xElement.xKey();
                var ServiceArgs = new string[0];
                var ServicesDependedOn = new string[0];
                var ServiceLogon = ServiceAccount.NetworkService;
                var ServiceStartMode = xElement.xAdminState() == "disabled"
                                           ? System.ServiceProcess.ServiceStartMode.Disabled : System.ServiceProcess.ServiceStartMode.Manual;

                switch (xElement.Name.LocalName)
                {
                    case "agent":
                        ServiceLabel = "Urbetrack Agent (" + ServiceLabel + ")";
                        ServiceName = AgentServiceName;
                        ServiceLogon = ServiceAccount.LocalSystem;
                        ServiceArgs = new[] { "-A" };
                        ServiceStartMode = System.ServiceProcess.ServiceStartMode.Automatic;
                        break;
                    case "instance":
                        ServiceLabel = "Urbetrack Instance (" + ServiceLabel + ")";
                        ServiceArgs = new[] { "-I", xElement.xKey(), AgentAddress };
                        ServicesDependedOn = new[] { AgentServiceName };
                        break;
                }

                var serviceController = GetWindowsService(ServiceName);

                /////// UPDATE
                if (serviceController != null)
                {
                    if (serviceController.DisplayName != ServiceLabel)
                        serviceController.DisplayName = ServiceLabel;

                    try
                    {
                        if (ServiceStartMode == ServiceStartMode.Automatic &&
                            GetServiceStart(serviceController.ServiceName) != ServiceStartType.Automatic)
                            SetServiceStart(serviceController.ServiceName, ServiceStartType.Automatic);

                        if (ServiceStartMode == ServiceStartMode.Disabled &&
                            GetServiceStart(serviceController.ServiceName) != ServiceStartType.Disabled)
                            SetServiceStart(serviceController.ServiceName, ServiceStartType.Disabled);
                    } catch (ApplicationException e)
                    {
                        T.EXCEPTION(e);
                    }
                    return true;
                }

                //////// INSTALL
                var path = String.Format("/assemblypath={0}", ServiceProcess);
                String[] cmdline = { path };
                var Context = new InstallContext(logfile, cmdline);
                
                var Runner = new Installer {Context = Context};
                

                var ProcesServiceInstaller = new ServiceProcessInstaller
                                                 {
                                                     Context = Context,
                                                     Account = ServiceLogon,
                                                     Username = null,
                                                     Password = null,
                                                     CmdLineArgs = ServiceArgs
                                                 };

                var ServiceInstallerObj = new ServiceInstaller
                                              {
                                                  Context = Context,
                                                  DisplayName = ServiceLabel,
                                                  Description = ServiceDescription,
                                                  ServiceName = ServiceName,
                                                  ServicesDependedOn = ServicesDependedOn,
                                                  StartType = ServiceStartMode,
                                                  StartOnInstall = false
                                              };

                ServiceInstallerObj.FailureActions.Add(new FailureAction(RecoverAction.Restart, 60000));
                ServiceInstallerObj.FailureActions.Add(new FailureAction(RecoverAction.Restart, 60000));
                ServiceInstallerObj.FailureActions.Add(new FailureAction(RecoverAction.Restart, 60000));
                ServiceInstallerObj.FailCountResetTime = 60 * 60 * 24; // 1 dia.

                Runner.Installers.Add(ProcesServiceInstaller);
                Runner.Installers.Add(ServiceInstallerObj);

                var state = new System.Collections.Specialized.ListDictionary();

                try
                {
                    Runner.Install(state);
                    Runner.Commit(state);
                } catch (Exception e)
                {
                    Context.LogMessage(Format.Join(Format.Exception(e,"Ejecutando el instaldor")));
                    Runner.Rollback(state);
                }
                
                return true;
            }
            catch(Exception e)
            {
                T.EXCEPTION(e);
                return false;
            }
        }

        #region Estructuras WIN32 de servicios
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SERVICE_DESCRIPTION
        {
            public IntPtr lpDescription;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_FAILURE_ACTIONS
        {
            public int dwResetPeriod;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpRebootMsg;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpCommand;
            public int cActions;
            public IntPtr lpsaActions;
        }
        #endregion

        #region DLLImport

        [DllImport("advapi32.dll")]
        public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);

        [DllImport("Advapi32.dll")]
        public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName,
                                                  int dwDesiredAccess, int dwServiceType, int dwStartType,
                                                  int dwErrorControl, string lpPathName,
                                                  string lpLoadOrderGroup, int lpdwTagId, string lpDependencies,
                                                  string lpServiceStartName, string lpPassword);

        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2W")]
        public static extern bool
            ChangeServiceFailureActions(IntPtr hService, int dwInfoLevel,
                                        [MarshalAs(UnmanagedType.Struct)] ref SERVICE_FAILURE_ACTIONS lpInfo);

        [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "ChangeServiceConfig2W")]
        public static extern bool ChangeServiceConfigDescription(
            IntPtr hService,
            uint dwInfoLevel,
            ref SERVICE_DESCRIPTION lpInfo);

        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2W")]
        public static extern bool
            ChangeServiceDescription(IntPtr hService, int dwInfoLevel,
                                     [MarshalAs(UnmanagedType.Struct)] ref SERVICE_DESCRIPTION lpInfo);

        [DllImport("advapi32.dll", CharSet=CharSet.Unicode, SetLastError = true)]
        public static extern bool
            ChangeServiceConfig(IntPtr hService, UInt32 nServiceType, 
                UInt32 nStartType, UInt32 nErrorControl, String lpBinaryPathName,
                String lpLoadOrderGroup, IntPtr lpdwTagId, [In] char[] lpDependencies, 
                String lpServiceStartName, String lpPassword, String lpDisplayName);
        
        [DllImport("advapi32.dll")]
        public static extern void CloseServiceHandle(IntPtr SCHANDLE);

        [DllImport("advapi32.dll")]
        public static extern int StartService(IntPtr SVHANDLE, int dwNumServiceArgs, string lpServiceArgVectors);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);

        [DllImport("advapi32.dll")]
        public static extern int DeleteService(IntPtr SVHANDLE);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        #endregion DLLImport

        /// <summary>
        /// Desintala un servcio previamente instalado del "Service Control Manager"
        /// </summary>
        /// <param name="svcName">Nombre del servicio a desinstalar.</param>
        public static bool UnInstallService(string svcName)
        {
            const int GENERIC_WRITE = 0x40000000;
            var sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);
            if (sc_hndl.ToInt32() == 0)
            {
                T.TRACE("Imposible abrir SCM");
                return false;
            }
            const int DELETE = 0x10000;
            var svc_hndl = OpenService(sc_hndl, svcName, DELETE);
            if (svc_hndl.ToInt32() == 0)
            {
                T.TRACE("Imposible hallar el servicio {0} para eliminarlo.", svcName);
                return false;
            }
            var i = DeleteService(svc_hndl);
            if (i != 0)
            {
                T.TRACE("Servicio {0} Eliminado", svcName);
                CloseServiceHandle(sc_hndl);
                return true;
            }
            T.TRACE("Imposible eliminar servicio {0}.",svcName);
            CloseServiceHandle(sc_hndl);
            return false;
        }
    }
}