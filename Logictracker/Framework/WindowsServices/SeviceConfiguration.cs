using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Urbetrack.Toolkit;

namespace Urbetrack.WindowsServices
{
    /// <summary>
    /// Configuración de un Servicio Windows.
    /// </summary>
    public class SeviceConfiguration
    {
        /// <value>Computer Name, Nombre del equipo donde esta el servicio</value>
        /// <remarks>Actualmente ReadOnly, apunta al equipo local.</remarks>
        public string ComputerName { get { return "."; } }

        /// <value>Service Name, Codigo univoco del servicio</value>
        public string ServiceName          { get; set; }
        
        /// <value>Display Name, Nombre visible en el Service Manager</value>
        public string DisplayName          { get; set; }
        
        /// <value>Descripcion del servicio visible en el Service Manager</value>
        public string Description          { get; set; }
        
        /// <value>Ruta completa al ejecutable del sericio.</value>
        public string BinaryPath           { get; set; }
        
        /// <value>Argumentos de linea de comando con que se inicia el serivicio.</value>
        public string StartArguments { get; set; }
        
        /// <value>Modo de Inicio del serivio.</value>
        public ServiceStartMode StartMode  { get; set; }
        
        /// <value>Cuenta de usuario que inicia el serivcio.</value>
        public ServiceAccount ServiceLogon { get; set; }
        
        /// <value>Usuario con se inicia el servicio (Si ServiceLogon = ServiceAccount.User)</value>
        public string UserName             { get; set; }
        
        /// <value>Password con se inicia el servicio (Si ServiceLogon = ServiceAccount.User)</value>
        public string Password             { get; set; }
        
        /// <value>Lista de Dependencia a otros Servicios, la relacion se hace con el ServiceName</value>
        public string[] ServicesDependedOn { get; set; }
        
        /// <value>Accion ante la primera falla.</value>
        public FailureAction FirstFailureAction { get; set; }
        
        /// <value>Accion ante la segunda falla.</value>
        public FailureAction SecondFailureAction { get; set; }
        
        /// <value>Accion ante las fallas subsiguientes.</value>
        public FailureAction SubsequentFailuresAction { get; set; }
        
        /// <value>Cantidad de tiempo transcurrido sin fallas (en segundos) para resetear el contador de fallas.</value>
        public int ResetFailCounterAfter      { get; set; }
        
        /// <value>Mensaje a enviarse por la red antes de reinicar el sistema.</value>
        public string NetworkMessageBeforeReboot  { get; set; }
        
        /// <value>Comando a ejecutarse frente a una falla.</value>
        public string FailureRunCommand       { get; set; }
        
        /// <value>Actualizar automaticamente si esta instalado un servicio con el mismo service name.</value>
        public bool UpdateIfInstalled { get; set; }
        
        /// <value>Iniciar automaticamente luego de la instalación</value>
        public bool StartAfterInstall { get; set; }

        /// <value>Indica si el servicio ya esta instalado en el sistema</value>
        public bool IsInstalled {
            get
            {
                if (!string.IsNullOrEmpty(ServiceName))
                    return ServiceManager.GetWindowsService(ServiceName) != null;
                return false;
            }
        }

        /// <value>Indica que hubo errores al interactuar con el SCM</value>
        public bool SCMErrorState { get; private set; }

        /*
        private void QuerySCMAndFillProperties()
        {
            try
            {
                SCMErrorState = false;
                // Handlers
                var scmHndl = IntPtr.Zero;
                var svcHndl = IntPtr.Zero;
                var tmpBuf = IntPtr.Zero;
                var svcLock = IntPtr.Zero;

                // Control de Errores
                var rslt = false;

                // Abro Service control manager
                scmHndl = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);

                if (scmHndl.ToInt32() <= 0)
                {
                    Trace.TraceError("Error al abrir Service Control Manager");
                    SCMErrorState = true;
                    return;
                }
                
                // Lockeo SCM

                svcLock = LockServiceDatabase(scmHndl);
                if (svcLock.ToInt32() <= 0)
                {
                    Trace.TraceError("Error bloqueando el SCM para escritura.");
                    return;
                }

                
            }
            catch(UnauthorizedAccessException e)
            {
                T.EXCEPTION(e);
            }
            catch(Exception e)
            {
                T.EXCEPTION(e);
            }
        }


        #region Misc Win32 Interop Stuff

        // The struct for setting the service description
        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_DESCRIPTION
        {

            public string lpDescription;

        }

        // The struct for setting the service failure actions
        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_FAILURE_ACTIONS
        {

            public int dwResetPeriod;
            public string lpRebootMsg;
            public string lpCommand;
            public int cActions;
            public int lpsaActions;

        }

        // Win32 function to open the service control manager
        [DllImport("advapi32.dll")]
        public static extern
            IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, int dwDesiredAccess);

        // Win32 function to open a service instance
        [DllImport("advapi32.dll")]
        public static extern IntPtr
            OpenService(IntPtr hSCManager, string lpServiceName, int dwDesiredAccess);

        // Win32 function to lock the service database to perform write operations.
        [DllImport("advapi32.dll")]
        public static extern IntPtr
            LockServiceDatabase(IntPtr hSCManager);

        // Win32 function to change the service config for the failure actions.
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
        public static extern bool
            ChangeServiceFailureActions(IntPtr hService, int dwInfoLevel,
                                        [MarshalAs(UnmanagedType.Struct)] ref SERVICE_FAILURE_ACTIONS lpInfo);

        // Win32 function to change the service config for the service description
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
        public static extern bool
            ChangeServiceDescription(IntPtr hService, int dwInfoLevel,
                                     [MarshalAs(UnmanagedType.Struct)] ref SERVICE_DESCRIPTION lpInfo);

        // Win32 function to close a service related handle.
        [DllImport("advapi32.dll")]
        public static extern bool
            CloseServiceHandle(IntPtr hSCObject);

        // Win32 function to unlock the service database.
        [DllImport("advapi32.dll")]
        public static extern bool
            UnlockServiceDatabase(IntPtr hSCManager);

        // The infamous GetLastError() we have all grown to love
        [DllImport("kernel32.dll")]
        public static extern int
            GetLastError();

        // Some Win32 constants I'm using in this app

        private const int SC_MANAGER_ALL_ACCESS = 0xF003F;
        private const int SERVICE_ALL_ACCESS = 0xF01FF;
        private const int SERVICE_CONFIG_DESCRIPTION = 0x1;
        private const int SERVICE_CONFIG_FAILURE_ACTIONS = 0x2;
        private const int SERVICE_NO_CHANGE = -1;
        private const int ERROR_ACCESS_DENIED = 5;


        #endregion

        #region Shutdown Privilege Interop Stuff

        // Struct required to set shutdown privileges
        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES
        {

            public long Luid;
            public int Attributes;

        }

        // Struct required to set shutdown privileges. The Pack attribute specified here
        // is important. We are in essence cheating here because the Privileges field is
        // actually a variable size array of structs.  We use the Pack=1 to align the Privileges
        // field exactly after the PrivilegeCount field when marshalling this struct to
        // Win32. You do not want to know how many hours I had to spend on this alone!!!

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TOKEN_PRIVILEGES
        {

            public int PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privileges;

        }

        // This method adjusts privileges for this process which is needed when
        // specifying the reboot option for a service failure recover action.
        [DllImport("advapi32.dll")]
        public static extern bool
            AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges,
                                  [MarshalAs(UnmanagedType.Struct)] ref TOKEN_PRIVILEGES NewState, int BufferLength,
                                  IntPtr PreviousState, ref int ReturnLength);


        // Looks up the privilege code for the privilege name
        [DllImport("advapi32.dll")]
        public static extern bool
            LookupPrivilegeValue(string lpSystemName, string lpName, ref long lpLuid);

        // Opens the security/privilege token for a process handle
        [DllImport("advapi32.dll")]
        public static extern bool
            OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

        // Gets the current process handle
        [DllImport("kernel32.dll")]
        public static extern IntPtr
            GetCurrentProcess();

        // Close a windows handle
        [DllImport("kernel32.dll")]
        public static extern bool
            CloseHandle(IntPtr hndl);

        // Token adjustment stuff
        private const int TOKEN_ADJUST_PRIVILEGES = 32;
        private const int TOKEN_QUERY = 8;
        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        private const int SE_PRIVILEGE_ENABLED = 2;

        #endregion
        */
    }
}