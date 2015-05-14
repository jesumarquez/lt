#region Usings

using System;
using Urbetrack.AVL;
using Urbetrack.Model;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.TR_203.v1.Parser
{
	public class Comando
	{
		public const string Reiniciar = "GSC,{0},LH";
		public const string SetId = "GSS,{0},3,0,O5={1}";
		public static IMessage Factory(string cmd, string value, ulong sequence, INode node)
		{
			var dgram = String.Format(cmd, node.Identifier, value);
			var str = String.Format("{0}*{1:X2}!", dgram, Utils.GetCheckSum(dgram));
			var msg = new UserMessage(node, sequence);
			Parser.AddStringToSend(str, msg);
			return msg;
		}
	}
}
