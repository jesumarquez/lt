#region Usings

using System;
using Logictracker.AVL.Messages;
using Logictracker.Model;
using Logictracker.Model.Utils;

#endregion

namespace Logictracker.FulMar
{
	public static class Mensaje
	{
		private const String SetId = "SID{0:D4}";
		public const String ForceIdReq = "QIME";

		private static String Factory(String cmd, params object[] values)
		{
			var tmpcmd = values != null ? String.Format(cmd, values) : cmd;
			var dgram = String.Format(">{0};*", tmpcmd);
			return String.Format("{0}{1:X2}<", dgram, GetCheckSumFg(dgram));
		}
		
		public static IMessage FactorySetId(ulong sequence, int newDeviceId, int formerDeviceId)
		{
			if (ParserUtils.IsInvalidDeviceId(newDeviceId)) return null;

			var msg = new ConfigRequest(newDeviceId, sequence);
			if (ParserUtils.IsInvalidDeviceId(formerDeviceId))
			{
				var cmds = Factory(String.Format(SetId, newDeviceId));
				msg.AddStringToSend(cmds);
			}

			//var imei = Parser.GetImei(datos[0]);
			return msg;
		}


		public static int GetCheckSumFg(String message)
		{
			var lon = message.LastIndexOf(";*", StringComparison.Ordinal);
			if (lon == -1)
				lon = message.Length;
			else
				lon += 1;

			var chksum = 0;
			for (var i = 0; i < lon; i++)
				chksum ^= message[i];

			return chksum;
		}
	}
}
