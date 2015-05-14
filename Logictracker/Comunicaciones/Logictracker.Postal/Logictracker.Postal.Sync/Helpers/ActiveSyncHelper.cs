#region Usings

using System;
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace Urbetrack.Postal.Sync.Helpers
{
    /// <summary>
    /// Helper class for accesing the mobile device.
    /// </summary>
    public static class ActiveSyncHelper
    {
        #region Rapi.dll import

        [DllImport("rapi.dll")]
        static extern void CeRapiInitEx(ref Rapiinit pRapiInit);

        [DllImport("rapi.dll")]
        static extern int CeRapiUninit();

        [DllImport("rapi.dll")]
        static extern int CeCloseHandle(IntPtr hObject);

        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr CeCreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, int lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);

        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        static extern int CeWriteFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfbytesToWrite, ref int lpNumberOfbytesWritten, int lpOverlapped); 

        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        static extern int CeReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfbytesToRead, ref int lpNumberOfbytesRead, int lpOverlapped);

        #endregion

        #region Private Types

        [StructLayout(LayoutKind.Sequential)]
        private struct Rapiinit
        {
            internal int cbsize;
            private readonly IntPtr heRapiInit;
            private readonly UInt32 hrRapiInit;
        };

        #endregion

        #region Constant Properties

        const int ErrorSuccess = 0;
        const int OpenExisting = 3;
        const int InvalidHandleValue = -1;
        const int FileAttributeNormal = 0x80;
        const uint GenericRead = 0x80000000;

        const int BufferSize = 1024 * 5; // 5k transfer buffer
        const int CreateAlways = 2;
        const uint GenericWrite = 0x40000000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Copies the specified file into the device.
        /// </summary>
        /// <param name="localFile"></param>
        /// <param name="remoteFile"></param>
        public static void CopyToDevice(string localFile, string remoteFile)
        {
            var r = new Rapiinit();

            r.cbsize = Marshal.SizeOf(r);

            CeRapiInitEx(ref r);

            try
            {
                var filePtr = CeCreateFile(remoteFile, GenericWrite, 0, 0, CreateAlways, FileAttributeNormal, 0);

                if (filePtr == new IntPtr(InvalidHandleValue)) return;

                using (var localFileStream = new FileStream(localFile, FileMode.Open, FileAccess.Read))
                {
                    var byteswritten = 0;
                    var position = 0;
                    var buffer = new byte[BufferSize];

                    var bytesread = localFileStream.Read(buffer, position, BufferSize);

                    while (bytesread > 0)
                    {
                        position += bytesread;

                        if (CeWriteFile(filePtr, buffer, bytesread, ref byteswritten, 0) == ErrorSuccess) return;

                        try { bytesread = localFileStream.Read(buffer, 0, BufferSize); }
                        catch { bytesread = 0; }
                    }
                }
            }
            finally { CeRapiUninit(); }
        }

        /// <summary>
        /// Copies the specified file from the device.
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <param name="localFile"></param>
        public static void CopyFromDevice(string remoteFile, string localFile)
        {
            var r = new Rapiinit();

            r.cbsize = Marshal.SizeOf(r);

            CeRapiInitEx(ref r);

            try
            {
                var remoteFilePtr = CeCreateFile(remoteFile, GenericRead, 0, 0, OpenExisting, FileAttributeNormal, 0);

                if (remoteFilePtr.ToInt32() == InvalidHandleValue) return;

                var localFileStream = new FileStream(localFile, FileMode.Create, FileAccess.Write);

                var read = 0;

                const int size = 1024*4;

                var data = new byte[size];

                CeReadFile(remoteFilePtr, data, size, ref read, 0);

                while (read > 0)
                {
                    localFileStream.Write(data, 0, read);

                    if (CeReadFile(remoteFilePtr, data, size, ref read, 0) != 0) continue;

                    CeCloseHandle(remoteFilePtr);

                    localFileStream.Close();

                    return;
                }

                CeCloseHandle(remoteFilePtr);

                localFileStream.Flush();
                localFileStream.Close();

                if (!File.Exists(localFile)) throw new FileNotFoundException("The file was not copied to the desktop");
            }
            finally { CeRapiUninit(); }
        }

        /// <summary>
        /// Reads the identifier of the currently connected device.
        /// </summary>
        public static String GetDeviceCode()
        {
            var remoteIniFile = Path.Combine(Configuration.PdaIniFilePath, Configuration.PdaIniFileName);
            var localIniFile = Path.Combine(Configuration.TemporaryPath, Configuration.PdaIniFileName);

            if (!Directory.Exists(Configuration.TemporaryPath)) Directory.CreateDirectory(Configuration.TemporaryPath);
            else if (File.Exists(localIniFile)) File.Delete(localIniFile);

            ActiveSyncHelper.CopyFromDevice(remoteIniFile, localIniFile);

            using (var file = File.OpenText(localIniFile)) return file.ReadLine();
        }

        #endregion
    }
}