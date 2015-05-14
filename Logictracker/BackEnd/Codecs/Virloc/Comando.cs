#region Usings

using System;
using Urbetrack.AVL;
using Urbetrack.Model;

#endregion

namespace Urbetrack.FulMarV1.v1
{
	public static class Comando
	{
		public const string SetId = "SID{0}";
		public const string ForceIdReq = "QIME";

        /*public static IMessage Factory(string cmd, string value, ulong sequence, INode node)
        {
            var dgram = String.Format(">{0}{1};ID={2:D4};#CMD:{3:X4};*", cmd, value, node.DeviceId, sequence);
            var str = String.Format("{0}{1:X2}<", dgram, Utils.GetCheckSumFg(dgram));
            var msg = new UserMessage(node, sequence);
            msg.AddStringToSend(str);
            return msg;
        }//*/

        public static IMessage Factory(String cmd)
		{
			var dgram = String.Format(">{0};*", cmd);
            var chk = Node.GetCheckSumFg(dgram);
			var str = String.Format("{0}{1:X2}<", dgram, chk);
			var msg = new UserMessage(0, 0, str);
			return msg;
		}
	}
}
