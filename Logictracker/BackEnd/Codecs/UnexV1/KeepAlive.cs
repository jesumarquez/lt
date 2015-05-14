#region Usings

using Urbetrack.AVL;
using Urbetrack.Model;

#endregion

namespace Urbetrack.Unetel.v1
{
	internal static class KeepAlive
	{
		internal static UserMessage Parse(INode node)
		{
			var msg = new UserMessage(node.GetDeviceId(), 0, "A");
			msg.UserSettings.Add("user_message_code", "KEEPALIVE");
			return msg;
		}
	}
}