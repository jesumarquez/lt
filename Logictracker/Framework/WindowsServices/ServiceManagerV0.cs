using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Urbetrack.Toolkit;

namespace Urbetrack.WindowsServices
{
    /// <summary>
    /// Utilidad de administracion de servicios windows.
    /// </summary>
    public static class ServiceManagerV0
    {
        /// <summary>
        /// Obtiene la lista de servicios
        /// </summary>
        /// <param name="prefix">si se establece filtra todo aquel servicios que no comienze con el texto dado (en el nombre)</param>
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
        private static extern bool ChangeServiceConfigDescription(
            IntPtr hService,
            uint dwInfoLevel,
            ref SERVICE_DESCRIPTION lpInfo);

        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2W")]
        public static extern bool
            ChangeServiceDescription(IntPtr hService, int dwInfoLevel,
                                     [MarshalAs(UnmanagedType.Struct)] ref SERVICE_DESCRIPTION lpInfo);

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
        /// This method installs and runs the service in the service control manager.
        /// </summary>
        /// <param name="svcPath">The complete path of the service.</param>
        /// <param name="svcName">Nombre univoco del servicio.</param>
        /// <param name="svcDispName">Titulo de servicio en MMC.</param>
        /// <param name="arguments">Argumentos de lina de comando, que el controlador de servicio utilizara cuando inicie el servicio.</param>
        /// <param name="startInmediatly">Si se pasa verdadero (true), se intentara inicar el servicio inmediatamente.</param>
        /// <returns>True if the process went thro successfully. False if there was any</returns>
        public static bool InstallService(string svcPath, string svcName, string svcDispName, string arguments, bool startInmediatly)
        {
            #region Constants declaration.

            var SC_MANAGER_CREATE_SERVICE = 0x0002;
            var SERVICE_WIN32_OWN_PROCESS = 0x00000010;
            //var SERVICE_DEMAND_START = 0x00000003;
            var SERVICE_ERROR_NORMAL = 0x00000001;
            var STANDARD_RIGHTS_REQUIRED = 0xF0000;
            var SERVICE_QUERY_CONFIG = 0x0001;
            var SERVICE_CHANGE_CONFIG = 0x0002;
            var SERVICE_QUERY_STATUS = 0x0004;
            var SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
            var SERVICE_START = 0x0010;
            var SERVICE_STOP = 0x0020;
            var SERVICE_PAUSE_CONTINUE = 0x0040;
            var SERVICE_INTERROGATE = 0x0080;
            var SERVICE_USER_DEFINED_CONTROL = 0x0100;
            var SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
                                      SERVICE_QUERY_CONFIG |
                                      SERVICE_CHANGE_CONFIG |
                                      SERVICE_QUERY_STATUS |
                                      SERVICE_ENUMERATE_DEPENDENTS |
                                      SERVICE_START |
                                      SERVICE_STOP |
                                      SERVICE_PAUSE_CONTINUE |
                                      SERVICE_INTERROGATE |
                                      SERVICE_USER_DEFINED_CONTROL);
            var SERVICE_AUTO_START = 0x00000002;

            #endregion Constants declaration.

            var sc_handle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
            if (sc_handle.ToInt32() != 0)
            {
                var sv_handle = CreateService(sc_handle, svcName, svcDispName, SERVICE_ALL_ACCESS,
                                              SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START, SERVICE_ERROR_NORMAL,
                                              svcPath + " " + arguments, null, 0, null, null /*@"NT AUTHORITY\NetworkService"*/, null);
                if (sv_handle.ToInt32() == 0)
                {
                    T.TRACE("Imposible crear servicio");
                    CloseServiceHandle(sc_handle);
                    return false;
                }
                if (startInmediatly)
                {
                    //now trying to start the service
                    var i = StartService(sv_handle, 0, null);
                    if (i == 0)
                    {
                        T.TRACE("Imposible iniciar servicio");
                        return false;
                    }
                }
                CloseServiceHandle(sc_handle);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Desintala un servcio previamente instalado del "Service Control Manager"
        /// </summary>
        /// <param name="svcName">Nombre del servicio a desinstalar.</param>
        public static bool UnInstallService(string svcName)
        {
            var GENERIC_WRITE = 0x40000000;
            var sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);
            if (sc_hndl.ToInt32() == 0)
            {
                T.TRACE("Imposible abrir SCM");
                return false;
            }
            var DELETE = 0x10000;
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
            T.TRACE("Imposible eliminar servicio {0}.", svcName);
            CloseServiceHandle(sc_hndl);
            return false;
        }
    }
}