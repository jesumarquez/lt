#region Usings

using System;

#endregion

namespace Logictracker.Unetel.v1
{
	internal static class HardwareStatus
	{
		internal static AVL.Messages.HardwareStatus Parse(String asString)
		{
			return new AVL.Messages.HardwareStatus(Convert.ToInt32(asString.Substring(2, 5)), 0)
			{
				Datos = asString.Substring(7),
			};
		}
	}
}