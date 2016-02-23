#region Usings

using System;
using System.Globalization;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.Layers.DeviceCommandCodecs;
using Logictracker.Model;
using Logictracker.Model.Utils;

#endregion

namespace Alas
{
	public static class Mensaje
	{
        public const String GarminFm = ">SFM.,{0}<";
        public const String GarminFmTR1 = "STMTR1{0}";
        public const String GarminTurnOffPvtData = "SFM.,0A023200";

        public const String Sms = @"STMTRM#D1{0}@#D2{1}@#B50\2C100\2C0\2C0@";
        public const String Reiniciar = "SSR55AA";
        public const String ForceIdReq = "QUS00";
        public const String IMEIReq = "QGRI";
        public const String QueryVersion = "QVR";
	    public const String QueryPort = "QDU";
	    public const String SetId = "SID{0:D4},SHOW";
        public const String GetPictures = "QSDI,P,I{0},E{1}";
        public const String PictureAck = "SPC{0}";
        public const String TemperatureActivateReports = "SSG01{0:D1}";
        public const String TemperatureTimer = "STD07{0:D6}000000";

        public const String ReplyTime = "RTIME {0:ddMMyyHHmmss}";

        public static String Factory(INode dev, GTEDeviceCommand cmd)
        {
            var did = dev == null ? 0 : dev.Id;
            var res = cmd.ToString(true);

            return res;
        }

		public static T Factory<T>(ulong MessageId, INode dev, String cmd, params object[] values)
		{
			if ((MessageId == ParserUtils.MsgIdNotSet || MessageId == 0) && dev != null) MessageId = dev.NextSequence;

			var did = dev == null ? 0 : dev.Id;

			var resBuilder = new StringBuilder(!cmd.StartsWith(">")?">":"");

			if (values != null && values.Length > 0)
				resBuilder.AppendFormat(CultureInfo.InvariantCulture, cmd, values);
			else
				resBuilder.Append(cmd);

			if (did != 0)
				resBuilder.AppendFormat(";ID={0:D4}", did);

			if ((MessageId != ParserUtils.CeroMsgId) && (MessageId != 0))
				resBuilder.AppendFormat(";#{0:X4}", MessageId);

			resBuilder.Append(";*");

			resBuilder.AppendFormat("{0:X2}", GetCheckSum(resBuilder.ToString()));
            if (!cmd.StartsWith(">"))
                resBuilder.Append("<");

			var res = resBuilder.ToString();

			if (typeof(T) == typeof(String))
				return (T)(object)res;
            if (typeof(T) == typeof(UserMessage))
				return (T)(object)new UserMessage(did, MessageId).AddStringToSend(res);

			throw new NotSupportedException(String.Format(@"El Tipo ""{0}"" no esta implementado en {1}.{2}", typeof(T), typeof(Mensaje).FullName, "Factory"));
		}

		public static String Factory(ulong MessageId, INode dev, String cmd, params object[] values)
		{
			return Factory<String>(MessageId, dev, cmd, values);
		}

		public static T Factory<T>(INode dev, String cmd, params object[] values)
		{
			return Factory<T>(0, dev, cmd, values);
		}

		public static String Factory(INode dev, String cmd, params object[] values)
		{
			return Factory<String>(dev, cmd, values);
		}

		public static T Factory<T>(String cmd, params object[] values)
		{
			return Factory<T>(null, cmd, values);
		}

		public static String Factory(String cmd, params object[] values)
		{
			return Factory<String>(cmd, values);
		}

		public static int GetCheckSum(String message)
		{
			var lon = message.IndexOf(";*");
			if (lon == -1)
				lon = message.Length;
			else
				lon += 2;

			var chksum = 0;
			for (var i = 0; i < lon; i++)
				chksum ^= message[i];

			return chksum;
		}
	}
}