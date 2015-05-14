#region Usings

using System;
using System.Collections;
using System.Configuration.Install;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;

#endregion

namespace Logictracker.WindowsServices
{
    /// <summary>
    /// Logictracker framework custom installer.
    /// </summary>
    public class ServiceInstaller : System.ServiceProcess.ServiceInstaller
    {
        #region Misc Win32 Interop Stuff

        // The struct for setting the service description
        [StructLayout(LayoutKind.Sequential)]
        private struct ServiceDescription { public string lpDescription; }

        // The struct for setting the service failure actions
        [StructLayout(LayoutKind.Sequential)]
        private struct ServiceFailureActions
        {
            public int dwResetPeriod;
            public string lpRebootMsg;
            public string lpCommand;
            public int cActions;
            public int lpsaActions;
        }

        // Win32 function to open the service control manager
        [DllImport("advapi32.dll")]
        private static extern IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, int dwDesiredAccess);

        // Win32 function to open a service instance
        [DllImport("advapi32.dll")]
        private static extern IntPtr OpenService(IntPtr hScManager, string lpServiceName, int dwDesiredAccess);

        // Win32 function to lock the service database to perform write operations.
        [DllImport("advapi32.dll")]
        private static extern IntPtr LockServiceDatabase(IntPtr hScManager);

        // Win32 function to change the service config for the failure actions.
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
        private static extern bool ChangeServiceFailureActions(IntPtr hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref ServiceFailureActions lpInfo);

        // Win32 function to change the service config for the service description
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
        private static extern bool ChangeServiceDescription(IntPtr hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref ServiceDescription lpInfo);

        // Win32 function to close a service related handle.
        [DllImport("advapi32.dll")]
        private static extern bool CloseServiceHandle(IntPtr hScObject);

        // Win32 function to unlock the service database.
        [DllImport("advapi32.dll")]
        private static extern bool UnlockServiceDatabase(IntPtr hScManager);

        // The infamous GetLastError() we have all grown to love
        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        // Some Win32 constants I'm using in this app
        private const int ScManagerAllAccess = 0xF003F;
        private const int ServiceAllAccess = 0xF01FF;
        private const int ServiceConfigDescription = 0x1;
        private const int ServiceConfigFailureActions = 0x2;
        private const int ServiceNoChange = -1;
        private const int ErrorAccessDenied = 5;

        #endregion

        #region Shutdown Privilege Interop Stuff

        // Struct required to set shutdown privileges
        [StructLayout(LayoutKind.Sequential)]
        public struct LuidAndAttributes
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
        private struct TokenPrivileges
        {
            public int PrivilegeCount;
            public LuidAndAttributes Privileges;
        }

        // This method adjusts privileges for this process which is needed when
        // specifying the reboot option for a service failure recover action.
        [DllImport("advapi32.dll")]
        private static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, bool disableAllPrivileges, [MarshalAs(UnmanagedType.Struct)] ref TokenPrivileges newState, int bufferLength,
            IntPtr previousState, ref int returnLength);

        // Looks up the privilege code for the privilege name
        [DllImport("advapi32.dll")]
        private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref long lpLuid);

        // Opens the security/privilege token for a process handle
        [DllImport("advapi32.dll")]
        private static extern bool OpenProcessToken(IntPtr processHandle, int desiredAccess, ref IntPtr tokenHandle);

        // Gets the current process handle
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        // Close a windows handle
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hndl);

        // Token adjustment stuff
        private const int TokenAdjustPrivileges = 32;
        private const int TokenQuery = 8;
        private const string SeShutdownName = "SeShutdownPrivilege";
        private const int SePrivilegeEnabled = 2;

        #endregion

        private string _description = String.Empty;
        private int _failResetTime = ServiceNoChange;
        private string _failRebootMsg = String.Empty;
        private string _failRunCommand = String.Empty;
        private bool _setDescription;
        private bool _setFailActions;
        private bool _startOnInstall;
        private int _startTimeout = 15000;
        private readonly string _logMsgBase;

        // List of Failure Actions
        public readonly ArrayList FailureActions;

        // Constructor: Init the failure actions and register for the Committed event
        public ServiceInstaller(){

            FailureActions = new ArrayList();

            // Multicast delegates are cool. By registering and activating here, we
            // shield user from having to deal with the event handlers. They simply
            // set the properties and we do the work for them.

            // Register the event handlers for post install operations
            Committed += UpdateServiceConfig;
            Committed += StartIfNeeded;

            // Set the Log Msg Base
            _logMsgBase = String.Concat("ServiceInstaller : ", ServiceName, " : ");
        }

        // Property for setting the service description
        public new string Description
        {
            set
            {
                _description = value;
                _setDescription = true;
            }
        }

        // Property to set fail count reset time
        public int FailCountResetTime
        {
            set
            {
                _failResetTime = value;
                _setFailActions = true;
            }
        }

        // Property to set fail reboot msg
        public string FailRebootMsg
        {
            set
            {
                _failRebootMsg = value;
                _setFailActions = true;
            }
        }

        // Property to set fail run command.
        public string FailRunCommand
        {
            set
            {
                _failRunCommand = value;
                _setFailActions = true;
            }
        }

        // Property style access to configure the service to start on install
        public bool StartOnInstall { set { _startOnInstall = value; } }

        // Property to set the start timeout for the service.
        public int StartTimeout { set { _startTimeout = value; } }

        /// <summary>
        /// Method to log to console and event log.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="msg"></param>
        private void LogInstallMessage(EventLogEntryType logLevel, string msg)
        {
            Console.WriteLine(msg);

            try { EventLog.WriteEntry(ServiceName, msg, logLevel); }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        private bool GrandShutdownPrivilege()
        {
            // This code mimics the MSDN defined way to adjust privilege for shutdown
            // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/sysinfo/base/shutting_down.asp

            var retRslt = false;

            var hToken = IntPtr.Zero;

            var tkp = new TokenPrivileges();

            long luid = 0;
            var retLen = 0;

            try
            {
                var myProc = GetCurrentProcess();

                var rslt = OpenProcessToken(myProc, TokenAdjustPrivileges | TokenQuery, ref hToken);

                if (!rslt) return false;

                LookupPrivilegeValue(null, SeShutdownName, ref luid);

                tkp.PrivilegeCount = 1;
                tkp.Privileges.Luid = luid;
                tkp.Privileges.Attributes = SePrivilegeEnabled;

                AdjustTokenPrivileges(hToken, false, ref tkp, 0, IntPtr.Zero, ref retLen);

                if (GetLastError() != 0) throw new Exception("Failed to grant shutdown privilege");

                retRslt = true;

            }
            catch (Exception ex) { LogInstallMessage(EventLogEntryType.Error, ex.Message); }
            finally { if (hToken != IntPtr.Zero) CloseHandle(hToken); }

            return retRslt;
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // The worker method to set all the extension properties for the service
        private void UpdateServiceConfig(object sender, InstallEventArgs ea)
        {
            // Determine if we need to set fail actions

            _setFailActions = false;

            var numActions = FailureActions.Count;

            if (numActions > 0) _setFailActions = true;

            // Do we need to do any work that the base installer did not do already?
            if (!(_setDescription || _setFailActions)) return;

            // We've got work to do
            var scmHndl = IntPtr.Zero;
            var svcHndl = IntPtr.Zero;
            var tmpBuf = IntPtr.Zero;
            var svcLock = IntPtr.Zero;

            // Place all our code in a try block
            try
            {
                // Open the service control manager
                scmHndl = OpenSCManager(null, null, ScManagerAllAccess);

                if (scmHndl.ToInt32() <= 0)
                {
                    LogInstallMessage(EventLogEntryType.Error, "Failed to Open Service Control Manager");

                    return;
                }

                // Lock the Service Database
                svcLock = LockServiceDatabase(scmHndl);

                if (svcLock.ToInt32() <= 0)
                {
                    LogInstallMessage(EventLogEntryType.Error, "Failed to Lock Service Database for Write");

                    return;
                }

                // Open the service
                svcHndl = OpenService(scmHndl, ServiceName, ServiceAllAccess);

                if (svcHndl.ToInt32() <= 0)
                {
                    LogInstallMessage(EventLogEntryType.Information, "Failed to Open Service ");

                    return;
                }

                // Need to set service failure actions. the API lets us set as many as
                // we want, yet the Service Control Manager GUI only lets us see the first 3.
                // Bill is aware of this and has promised no fixes. Also the API allows
                // granularity of seconds whereas GUI only shows days and minutes.

                bool rslt;

                if (_setFailActions)
                {
                    // We're gonna serialize the SA_ACTION structs into an array of ints
                    // for simplicity in marshalling this variable length ptr to win32

                    var actions = new int[numActions * 2];

                    var currInd = 0;

                    var needShutdownPrivilege = false;

                    foreach (FailureAction fa in FailureActions)
                    {
                        actions[currInd] = (int)fa.RecoverAction;
                        actions[++currInd] = fa.Delay;

                        currInd++;

                        if (fa.RecoverAction == RecoverAction.Reboot) needShutdownPrivilege = true;
                    }

                    // If we need shutdown privilege, then grant it to this process
                    if (needShutdownPrivilege)
                    {
                        rslt = GrandShutdownPrivilege();

                        if (!rslt) return;
                    }

                    // Need to pack 8 bytes per struct
                    tmpBuf = Marshal.AllocHGlobal(numActions * 8);

                    // Move array into marshallable pointer
                    Marshal.Copy(actions, 0, tmpBuf, numActions * 2);

                    // Set the SERVICE_FAILURE_ACTIONS struct
                    var sfa = new ServiceFailureActions
                                  {
                                      cActions = numActions,
                                      dwResetPeriod = _failResetTime,
                                      lpCommand = _failRunCommand,
                                      lpRebootMsg = _failRebootMsg,
                                      lpsaActions = tmpBuf.ToInt32()
                                  };

                    // Call the ChangeServiceFailureActions() abstraction of ChangeServiceConfig2()
                    rslt = ChangeServiceFailureActions(svcHndl, ServiceConfigFailureActions, ref sfa);

                    //Check the return
                    if (!rslt)
                    {
                        var err = GetLastError();

                        if (err == ErrorAccessDenied) throw new Exception(_logMsgBase + "Access Denied while setting Failure Actions");
                    }

                    // Free the memory
                    Marshal.FreeHGlobal(tmpBuf); tmpBuf = IntPtr.Zero;
                }

                // Need to set the description field?
                if (_setDescription)
                {
                    var sd = new ServiceDescription {lpDescription = _description};

                    // Call the ChangeServiceDescription() abstraction of ChangeServiceConfig2()
                    rslt = ChangeServiceDescription(svcHndl, ServiceConfigDescription, ref sd);

                    // Error setting description?
                    if (!rslt) throw new Exception(_logMsgBase + "Failed to set description");
                }
            }
            catch (Exception e) { LogInstallMessage(EventLogEntryType.Error, e.Message); }
            finally
            {
                if (scmHndl != IntPtr.Zero)
                {
                    // Unlock the service database
                    if (svcLock != IntPtr.Zero) UnlockServiceDatabase(svcLock);

                    // Close the service control manager handle
                    CloseServiceHandle(scmHndl);
                }

                // Close the service handle
                if (svcHndl != IntPtr.Zero) CloseServiceHandle(svcHndl);

                // Free the memory
                if (tmpBuf != IntPtr.Zero) Marshal.FreeHGlobal(tmpBuf);
            } 
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // Method to start the service automatically after installation
        private void StartIfNeeded(object sender, InstallEventArgs e)
        {
            // Do we need to do any work?
            if (!_startOnInstall) return;

            try
            {
                var waitTo = new TimeSpan(0, 0, _startTimeout);

                // Get a handle to the service
                var sc = new ServiceController(ServiceName);

                // Start the service and wait for it to start
                sc.Start();

                sc.WaitForStatus(ServiceControllerStatus.Running, waitTo);

                // Be good and release our handle
                sc.Close();
            }
            catch (Exception ex) { LogInstallMessage(EventLogEntryType.Error, ex.Message); }
        }
    }
}