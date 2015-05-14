using System;
using System.Runtime.InteropServices;

namespace Urbetrack.Mobile.Install
{
    public class MSMQ
    {
        public class ProcessInfo
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public Int32 ProcessId;
            public Int32 ThreadId;
        }

        [DllImport("CoreDll.DLL", SetLastError = true)]
        private extern static
            int CreateProcess(String imageName,
                              String cmdLine,
                              IntPtr lpProcessAttributes,
                              IntPtr lpThreadAttributes,
                              Int32 boolInheritHandles,
                              Int32 dwCreationFlags,
                              IntPtr lpEnvironment,
                              IntPtr lpszCurrentDir,
                              IntPtr lpsiStartInfo,
                              ProcessInfo pi);
        
        [DllImport("CoreDll.dll")]
        private extern static
            Int32 GetLastError();

        [DllImport("CoreDll.dll")]
        private extern static
            Int32 GetExitCodeProcess(IntPtr hProcess, out Int32 exitcode);

        [DllImport("CoreDll.dll")]
        private extern static
            Int32 CloseHandle(IntPtr hProcess);

        [DllImport("CoreDll.dll")]
        private extern static
            IntPtr ActivateDevice(
            string lpszDevKey,
            Int32 dwClientInfo);
        
        [DllImport("CoreDll.dll")]
        private extern static
            Int32 WaitForSingleObject(IntPtr Handle,
                                      Int32 Wait);

        public static bool CreateProcess(String ExeName, String CmdLine)
        {
            Int32 INFINITE;
            unchecked { INFINITE = (int)0xFFFFFFFF; }

            var pi = new ProcessInfo();

            if (CreateProcess(ExeName, CmdLine, IntPtr.Zero, IntPtr.Zero,
                              0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, pi) == 0)
            {
                return false;
            }

            WaitForSingleObject(pi.hProcess, INFINITE);

            Int32 exitCode;

            if (GetExitCodeProcess(pi.hProcess, out exitCode) == 0)
            {
                //MessageBox.Show("ERROR de comunicacion con Windows, (GetExitCodeProcess)");

                CloseHandle(pi.hThread);
                CloseHandle(pi.hProcess);
                return false;
            }
            CloseHandle(pi.hThread);
            CloseHandle(pi.hProcess);

            return exitCode == 0;
        }

        private const String MSMQ_ADM = @"\windows\msmqadm.exe";
        private const String MSMQ_DRIVER_REG = @"Drivers\BuiltIn\MSMQD";

        /*private static void CopyFilesRequired()
        {
            string _path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (!File.Exists(@"\windows\msmqadm.exe"))
                File.Copy(_path + "\\msmqadm.exe", @"\windows\msmqadm.exe");
            //if (!File.Exists(@"\windows\msmqadmext.dll"))
            //    File.Copy(_path + "\\msmqadmext.dll", @"\windows\msmqadmext.dll");
            if (!File.Exists(@"\windows\msmqd.dll"))
                File.Copy(_path + "\\msmqd.dll", @"\windows\msmqd.dll");
            if (!File.Exists(@"\windows\msmqrt.dll"))
                File.Copy(_path + "\\msmqrt.dll", @"\windows\msmqrt.dll");

            //File.Delete(_path + "\\msmqadm.exe");
            //File.Delete(_path + "\\msmqadmext.dll");
            //File.Delete(_path + "\\msmqd.dll");
            //File.Delete(_path + "\\msmqrt.dll");
        }*/

        public static void Install()
        {
            //CopyFilesRequired();

            if (!(CreateProcess(MSMQ_ADM, "-f \\temp\\mqactivate.txt status")))
            {
                var handle1 = ActivateDevice(MSMQ_DRIVER_REG, 0);
                CloseHandle(handle1);
                //Let us check if MSMQ is running
                if (CreateProcess(MSMQ_ADM, "-f \\temp\\mqactst.txt status"))
                {
                    return;
                }
            }

            //Check status of MSMQ (is it installed and running yet?
            if (!(CreateProcess(MSMQ_ADM, "-f \\temp\\mqstatus.txt status")))
            {
                //Deletes the MSMQ registry key and store directory.
                //All messages are lost.

                CreateProcess(MSMQ_ADM, "-f \\temp\\mqregcl.txt register cleanup");
                //Installs MSMQ as device drivcers.

                if (!CreateProcess(MSMQ_ADM, "-f \\temp\\mqregins.txt register install"))
                {
                    //MessageBox.Show("ERROR al Instalar MSMQ! System ERROR = " + GetLastError());
                    return;
                }

                //Creates the MSMQ Configuration in Registry

                if (!CreateProcess(MSMQ_ADM, "-f \\temp\\mqreg.txt register"))
                {
                    //MessageBox.Show("ERROR al registrar! System ERROR = " + GetLastError());
                    return;
                }

                //Enables the native MSMQ protocol
                if (!CreateProcess(MSMQ_ADM, "-f \\temp\\mqenable.txt enable binary"))
                {
                    //MessageBox.Show("ERROR al habilitar MSMQ binaria! System ERROR = " + GetLastError());
                    return;
                }

                //Starts the MSMQ service

                if (!CreateProcess(MSMQ_ADM, "-f \\temp\\mqstart.txt start"))
                {
                    //This is one additional step that is needed for PocketPCs
                    //The Device Drivers have to be loaded before the service
                    //can be started
                    //ActivateDevice will load the device drivers

                    IntPtr handle = ActivateDevice(MSMQ_DRIVER_REG, 0);
                    CloseHandle(handle);

                    //Let us check if MSMQ is running
                    if (!CreateProcess(MSMQ_ADM, "-f \\temp\\mqrstatus.txt status"))
                    {
                        //MessageBox.Show("ERROR al inicializar el servicio MSMQ! System ERROR = " + GetLastError());
                    }
                }
            }
        }
    }
}