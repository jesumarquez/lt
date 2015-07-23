using System;
using System.Globalization;
using System.Text;

namespace TraxEmu
{
    class Utils
    {
        private static uint _nextSequence;

        public static int GetCheckSum(String message)
        {
            var lon = message.IndexOf(";*", StringComparison.Ordinal);
            if (lon == -1)
                lon = message.Length;
            else
                lon += 2;

            var chksum = 0;
            for (var i = 0; i < lon; i++)
                chksum ^= message[i];

            return chksum;
        }

        private static UInt32 NextSequence { get { return _nextSequence++; } }

        public static string Factory(int devId, String cmd, object[] values)
        {
            var messageId = NextSequence;

            var did = devId;// dev == null ? 0 : dev.Id;

            var resBuilder = new StringBuilder(!cmd.StartsWith(">") ? ">" : "");

            if (values != null && values.Length > 0)
                resBuilder.AppendFormat(CultureInfo.InvariantCulture, cmd, values);
            else
                resBuilder.Append(cmd);

            if (did != 0)
                resBuilder.AppendFormat(";ID={0:D4}", did);

            resBuilder.AppendFormat(";#{0:X4}", messageId);

            resBuilder.Append(";*");

            resBuilder.AppendFormat("{0:X2}", GetCheckSum(resBuilder.ToString()));
 
            if (!cmd.StartsWith(">"))
                resBuilder.Append("<");

            var res = resBuilder.ToString();

            return res;
        }
    }
}
