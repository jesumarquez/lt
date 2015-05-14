using System;
using System.Runtime.InteropServices;
using System.Text;

namespace UrbeMobile
{
    public static class PhoneInfo
    {
        //original code location:
        //http://www.developersdex.com/vb/message.asp?p=2916&ID=%3C68D1F07B-ECB0-4A15-AFCA-2A911FC3234C%40microsoft.com%3E
        public static void Get(out string manufacturer, out string model, out string revision, out string serialNumber, out string subsciberId)
        {
            IntPtr hLine;
            int dwNumDev;
            int num1 = 0x20000;
            var lineInitializeParams = new LINEINITIALIZEEXPARAMS();
            lineInitializeParams.dwTotalSize = (uint)Marshal.SizeOf(lineInitializeParams);
            lineInitializeParams.dwNeededSize = lineInitializeParams.dwTotalSize;
            lineInitializeParams.dwOptions = 2;
            lineInitializeParams.hEvent = IntPtr.Zero;
            lineInitializeParams.hCompletionPort = IntPtr.Zero;

            #region lineInitializeEx
            int result = Tapi.lineInitializeEx(out hLine, IntPtr.Zero,
            IntPtr.Zero, null, out dwNumDev, ref num1, ref lineInitializeParams);
            if (result != 0)
            {
                throw new ApplicationException(string.Format("lineInitializeEx failed!\n\nError Code:{0}", result));
            }
            #endregion

            #region lineNegotiateAPIVerison
            int version;
            const int dwAPIVersionLow = 0x10004;
            const int dwAPIVersionHigh = 0x20000;
            LINEEXTENSIONID lineExtensionID;
            result = Tapi.lineNegotiateAPIVersion(hLine, 0, dwAPIVersionLow,dwAPIVersionHigh, out version, out lineExtensionID);
            if (result != 0)
            {
                throw new ApplicationException(string.Format("lineNegotiateAPIVersion failed!\n\nError Code: {0}", result));
            }
            #endregion

            #region lineOpen
            IntPtr hLine2;
            result = Tapi.lineOpen(hLine, 0, out hLine2, version, 0,IntPtr.Zero, 0x00000002, 0x00000004, IntPtr.Zero);
            if (result != 0)
            {
                throw new ApplicationException(string.Format("lineNegotiateAPIVersion failed!\n\nError Code: {0}", result));
            }
            #endregion

            #region lineGetGeneralInfo
            var structSize = Marshal.SizeOf(new LINEGENERALINFO());
            var bytes = new byte[structSize];
            var tmpBytes = BitConverter.GetBytes(structSize);

            for (int index = 0; index < tmpBytes.Length; index++)
            {
                bytes[index] = tmpBytes[index];
            }
            #endregion

            #region make initial query to retrieve necessary size
            Tapi.lineGetGeneralInfo(hLine2, bytes);

            // get the needed size
            var neededSize = BitConverter.ToInt32(bytes, 4);

            // resize the array
            bytes = new byte[neededSize];

            // write out the new allocated size to the byte stream
            tmpBytes = BitConverter.GetBytes(neededSize);
            for (int index = 0; index < tmpBytes.Length; index++)
            {
                bytes[index] = tmpBytes[index];
            }

            // fetch the information with properly size buffer
            result = Tapi.lineGetGeneralInfo(hLine2, bytes);

            if (result != 0)
            {
                throw new ApplicationException(Marshal.GetLastWin32Error().ToString());
            }

            #endregion

            #region actual data fetching

            // manufacture
            var size = BitConverter.ToInt32(bytes, 12);
            var offset = BitConverter.ToInt32(bytes, 16);
            manufacturer = Encoding.Unicode.GetString(bytes, offset, size);
            manufacturer = manufacturer.Substring(0, manufacturer.IndexOf('\0'));

            // model
            size = BitConverter.ToInt32(bytes, 20);
            offset = BitConverter.ToInt32(bytes, 24);
            model = Encoding.Unicode.GetString(bytes, offset, size);
            model = model.Substring(0, model.IndexOf('\0'));

            // revision
            size = BitConverter.ToInt32(bytes, 28);
            offset = BitConverter.ToInt32(bytes, 32);
            revision = Encoding.Unicode.GetString(bytes, offset, size);
            revision = revision.Substring(0, revision.IndexOf('\0'));

            // serial number
            size = BitConverter.ToInt32(bytes, 36);
            offset = BitConverter.ToInt32(bytes, 40);
            serialNumber = Encoding.Unicode.GetString(bytes, offset, size);
            serialNumber = serialNumber.Substring(0, serialNumber.IndexOf('\0'));

            // subscriber id
            size = BitConverter.ToInt32(bytes, 44);
            offset = BitConverter.ToInt32(bytes, 48);
            subsciberId = Encoding.Unicode.GetString(bytes, offset, size);
            subsciberId = subsciberId.Substring(0, subsciberId.IndexOf('\0'));

            #endregion

            //tear down
            Tapi.lineClose(hLine2);            
            Tapi.lineShutdown(hLine);

        }
    }

}