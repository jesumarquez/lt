using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Win32
{
    public static class CoreDLL
    {
        [Flags]
        public enum PlaySoundFlags
        {
            SND_SYNC = 0x0000, /* play synchronously (default) */
            SND_ASYNC = 0x0001, /* play asynchronously */
            SND_NODEFAULT = 0x0002, /* silence (!default) if sound not found */
            SND_MEMORY = 0x0004, /* pszSound points to a memory file */
            SND_LOOP = 0x0008, /* loop the sound until next and PlaySound */
            SND_NOSTOP = 0x0010, /* don't stop any currently playing sound */
            SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
            SND_ALIAS = 0x00010000, /* name is a registry alias */
            SND_ALIAS_ID = 0x00110000, /* alias is a predefined ID */
            SND_FILENAME = 0x00020000, /* name is file name */
            SND_RESOURCE = 0x00040004 /* name is resource name or atom */
        }

        /*[DllImport("CoreDll")]
        public static extern bool CeSetThreadPriority(IntPtr hThread, int nPriority);*/

        /*
        enum PowerEventType
        {
            PBT_TRANSITION = 0x00000001,
            PBT_RESUME = 0x00000002,
            PBT_POWERSTATUSCHANGE = 0x00000004,
            PBT_POWERINFOCHANGE = 0x00000008,
        }
        */


        [Flags]
        public enum PowerState
        {
            POWER_STATE_ON = (0x00010000),
            POWER_STATE_OFF = (0x00020000),

            POWER_STATE_CRITICAL = (0x00040000),
            POWER_STATE_BOOT = (0x00080000),
            POWER_STATE_IDLE = (0x00100000),
            POWER_STATE_SUSPEND = (0x00200000),
            POWER_STATE_RESET = (0x00800000),
        }

        //[DllImport("coredll")]
        //static extern IntPtr RequestPowerNotifications(IntPtr hMsgQ, PowerEventType Flags);

        [DllImport("CoreDLL")]
        public static extern int ReleasePowerRequirement(IntPtr hPowerReq);

        [DllImport("CoreDll")]
        public static extern bool PlaySound(string szSound, IntPtr hMod, PlaySoundFlags flags);

        [DllImport("CoreDLL", SetLastError=true)]
        public static extern IntPtr SetPowerRequirement
        (
            string pDevice,
            CEDEVICE_POWER_STATE DeviceState,
            DevicePowerFlags DeviceFlags,
            IntPtr pSystemState,
            uint StateFlagsZero
        );

        [DllImport("CoreDLL", SetLastError = true)]
        public static extern IntPtr SetDevicePower
            (
                string pDevice,
                DevicePowerFlags DeviceFlags,
            CEDEVICE_POWER_STATE DevicePowerState
            );

        [DllImport("CoreDLL")]
        public static extern int GetDevicePower(string device, DevicePowerFlags flags, out CEDEVICE_POWER_STATE PowerState);

         [DllImport("CoreDLL")]
        public static extern int PowerPolicyNotify(
          PPNMessage dwMessage,
            int option
        //    DevicePowerFlags);
        );

        [DllImport("CoreDLL")]
        public static extern int GetSystemPowerStatusEx2(
             SYSTEM_POWER_STATUS_EX2 statusInfo, 
            int length,
            int getLatest
                );

        
        public static SYSTEM_POWER_STATUS_EX2 GetSystemPowerStatus()
        {
           var retVal = new SYSTEM_POWER_STATUS_EX2();
           GetSystemPowerStatusEx2( retVal, Marshal.SizeOf(retVal) , 1);
           return retVal;
        }

        [DllImport("CoreDLL")]
        public static extern int SystemParametersInfo
        (
            SPI Action,
            uint Param, 
            ref int  result, 
            int updateIni
        );

        [DllImport("CoreDLL")]
        public static extern int SystemIdleTimerReset();

        [DllImport("CoreDLL")]
        public static extern int CeRunAppAtTime(string application, SystemTime startTime);

        [DllImport("CoreDLL")]
        public static extern int CeRunAppAtEvent(string application, int EventID);

        [DllImport("CoreDLL")]
        public static extern int FileTimeToSystemTime(ref long lpFileTime, SystemTime lpSystemTime);
        
        [DllImport("CoreDLL")]
        public static extern int FileTimeToLocalFileTime(ref long lpFileTime, ref long lpLocalFileTime);

        // For named events
        //[DllImport("CoreDLL", SetLastError = true)]
        //internal static extern bool EventModify(IntPtr hEvent, EVENT ef);

        //[DllImport("CoreDLL", SetLastError = true)]
        //internal static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        //[DllImport("CoreDLL", SetLastError = true)]
        //internal static extern bool CloseHandle(IntPtr hObject);

        //[DllImport("CoreDLL", SetLastError = true)]
        //internal static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        //[DllImport("CoreDLL", SetLastError = true)]
        //internal static extern int WaitForMultipleObjects(int nCount, IntPtr[] lpHandles, bool fWaitAll, int dwMilliseconds);

        public static void Beep(int c)
        {
            for (int i = 0; i < c; ++i)
            {
                PlaySound("\\windows\\voicbeep.wav", IntPtr.Zero,
                                           PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_ASYNC);
                Thread.Sleep(100);
            }
        }
    }
}
