using System;
using System.Runtime.InteropServices;

namespace UrbeMobile
{

    public class LINEGENERALINFO {
        public int dwManufacturerOffset;
        public int dwManufacturerSize;
        public int dwModelOffset;
        public int dwModelSize;
        public int dwNeededSize;
        public int dwRevisionOffset;
        public int dwRevisionSize;
        public int dwSerialNumberOffset;
        public int dwSerialNumberSize;
        public int dwSubscriberNumberOffset;
        public int dwSubscriberNumberSize;
        public int dwTotalSize;
        public int dwUsedSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LINEEXTENSIONID
    {
        public IntPtr dwExtensionID0;
        public IntPtr dwExtensionID1;
        public IntPtr dwExtensionID2;
        public IntPtr dwExtensionID3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LINEINITIALIZEEXPARAMS
    {
        public uint dwTotalSize;
        public uint dwNeededSize;
        public uint dwUsedSize;
        public uint dwOptions;
        public IntPtr hEvent;
        public IntPtr hCompletionPort;
        public uint dwCompletionKey;
    }

    /// <summary>
    /// simple managed wrapper for the Telephony API
    /// </summary>
    public class Tapi
    {
        [DllImport("coredll")]
        public static extern int lineInitializeEx(out IntPtr lpm_hLineApp, IntPtr hInstance, IntPtr lpfnCallback, string lpszFriendlyAppName, out int lpdwNumDevs, ref int lpdwAPIVersion, ref LINEINITIALIZEEXPARAMS lpLineInitializeExParams);

        [DllImport("coredll")]
        public static extern int lineOpen(IntPtr m_hLineApp, int dwDeviceID, out IntPtr lphLine, int dwAPIVersion, int dwExtVersion, IntPtr dwCallbackInstance, int dwPrivileges, int dwMediaModes, IntPtr lpCallParams);

        [DllImport("coredll")]
        public static extern int lineNegotiateAPIVersion(IntPtr m_hLineApp, int dwDeviceID, int dwAPILowVersion, int dwAPIHighVersion, out int lpdwAPIVersion, out LINEEXTENSIONID lpExtensionId);

        [DllImport("cellcore")]
        public static extern int lineGetGeneralInfo(IntPtr hLine, byte[] bytes);

        [DllImport("cellcore")]
        public static extern int lineGetGeneralInfo(IntPtr hLine, ref LINEGENERALINFO lineGenerlInfo);

        [DllImport("coredll")]
        public static extern int lineClose(IntPtr hLine);

        [DllImport("coredll")]
        public static extern int lineShutdown(IntPtr m_hLineApp);
        
    }


}
