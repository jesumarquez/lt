using System;
using System.Runtime.InteropServices;

namespace UrbeMobile
{
    /// <summary>
    /// Class wraps P/Invokes to the windows system functions
    /// </summary>
    public class SystemCalls
    {
        [DllImport("coredll.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(IntPtr lpClassName, string lpWindowName);

        [DllImport("coredll.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("coredll.dll")]
        private extern static int GetDeviceUniqueID([In, Out] byte[] appdata,
                                            int cbApplictionData,
                                            int dwDeviceIDVersion,
                                            [In, Out] byte[] deviceIDOuput,
                                            out uint pcbDeviceIDOutput);

        static public byte[] GetDeviceID(string AppString)
        {
            // Call the GetDeviceUniqueID
            byte[] AppData = new byte[AppString.Length];
            for (int count = 0; count < AppString.Length; count++)
                AppData[count] = (byte)AppString[count];

            int appDataSize = AppData.Length;
            byte[] DeviceOutput = new byte[20];
            uint SizeOut = 20;
            GetDeviceUniqueID(AppData, appDataSize, 1, DeviceOutput, out SizeOut);
            return DeviceOutput;

        }
    }
}
