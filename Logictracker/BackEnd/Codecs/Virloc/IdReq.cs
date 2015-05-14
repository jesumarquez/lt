#region Usings

using System;
using Urbetrack.AVL;
using Urbetrack.Model;

#endregion

namespace Urbetrack.FulMarV1.v1
{
	public static class IdReq
	{
        public static IMessage Factory(ulong sequence, int newDeviceId, string[] datos, int FormerDeviceId)
		{
            if (ParserUtils.IsInvalidDeviceId(newDeviceId)) return null;

            var msg = new ConfigRequest(newDeviceId, sequence);
            if (ParserUtils.IsInvalidDeviceId(FormerDeviceId))
            {
				var cmds = String.Format(">SID{0:D4}<", newDeviceId);
				msg.AddStringToSend(cmds);
			}

			//var imei = Parser.GetImei(datos[0]);
			return msg;
		}
	}
}