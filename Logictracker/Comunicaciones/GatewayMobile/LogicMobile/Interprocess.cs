using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Microsoft.WindowsCE.Forms;

namespace UrbeMobile
{
    /// <summary>
    /// Class for interprocess commnucation
    /// </summary>
    public class Interprocess
    {
        public const string ServiceName = "UrbeMobile";
        
        /// <summary>
        /// USER messages starts from 0x400
        /// </summary>
        public const int WM_USER = 0x400;
        /// <summary>
        /// QUIT message
        /// </summary>
        public const int WM_QUIT_SERVICE = WM_USER + 5001;
        /// <summary>
        /// CLEAR_LOG message
        /// </summary>
        public const int WM_CLEAR_LOG = WM_USER + 5002;

        private static string ServiceExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + Path.DirectorySeparatorChar + "UrbeMobile.exe";

        /// <summary>
        /// Sends quit message to service
        /// </summary>
        public static void StopService()
        {
            IntPtr hwnd = SystemCalls.FindWindow(IntPtr.Zero, ServiceName);            
            Message msg = Message.Create(hwnd, WM_QUIT_SERVICE, (IntPtr)0, (IntPtr)0);
            MessageWindow.SendMessage(ref msg);
        }

        /// <summary>
        /// Sends clearlog message to service
        /// </summary>
        public static void ClearLog()
        {
            IntPtr hwnd = SystemCalls.FindWindow(IntPtr.Zero, ServiceName);
            Message msg = Message.Create(hwnd, WM_CLEAR_LOG, (IntPtr)0, (IntPtr)0);
            MessageWindow.SendMessage(ref msg);
        }

        /// <summary>
        /// Starts service
        /// </summary>
        public static void StartService()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = ServiceExe;
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
        }

        /// <summary>
        /// Determines whether the service is running or not
        /// </summary>      
        public static bool IsServiceRunning
        {
            get
            {
                IntPtr hwnd = SystemCalls.FindWindow(IntPtr.Zero, ServiceName);
                if (hwnd.ToInt64() > 0)
                    return true;
                else
                    return false;
            }
        }
    }
}
