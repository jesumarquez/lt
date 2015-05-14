using System;
using System.Linq;
using Logictracker.Utils;

namespace Logictracker.Model.Utils
{
	public static class ParserUtils
	{
		public const int WithoutDeviceId = -1;
		public const int CeroDeviceId = 0;
		public const ulong MsgIdNotSet = 0x10001;
		public const ulong CeroMsgId = 0x10000;

		public delegate int GetCheckSumDelegate(String message);
		public static void CheckChecksumOk(String entrada, String iniciator, String terminator, GetCheckSumDelegate gcd)
		{
			var ini = entrada.IndexOf(iniciator) + iniciator.Length;
			if (ini < iniciator.Length) return; //no contiene checksum
			var lon = (terminator == null) ? 2 : (entrada.IndexOf(terminator, ini, StringComparison.Ordinal) - ini);
			var chckOrig = Convert.ToUInt32(entrada.Substring(ini, lon), 16);
			var chckCalc = gcd(entrada);
			if (chckOrig != chckCalc)
				throw new Exception(String.Format("Error de checksum en mensaje {0:X2};{1:X2} '{2}'", chckOrig, chckCalc, entrada));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		/// <returns>el checksum de un string como el XOR desde el primer caracter hasta el ultimo o el primer asterisco si lo hay</returns>
		public static int GetCheckSumNmea(String message)
		{
			return message.TakeWhile(car => car != '*').Aggregate(0, (current, car) => current ^ car);
		}

		public static Boolean IsInvalidDeviceId(int deviceId)
		{
			return ((deviceId == WithoutDeviceId) || (deviceId == CeroDeviceId));
		}

		public static ulong NormalizeMsgId(ulong msgId, INode dev)
		{
			return msgId == MsgIdNotSet || msgId == CeroMsgId || msgId == 0 ? dev != null ? dev.NextSequence : RandomUtils.RandomNumber(1, Int16.MaxValue) : msgId;
		}

		public static ulong GetMsgIdTaip(String buffer)
		{
			const string eventoFg = ">RFG";
			const int fgTrashLenght = 37;

			if (String.IsNullOrEmpty(buffer) || !buffer.Contains(";#")) return MsgIdNotSet;
			var trash = 0; if (buffer.StartsWith(eventoFg)) trash = fgTrashLenght;

			var ini = buffer.IndexOf(";#", trash, StringComparison.Ordinal) + 2;

			var mid = buffer.Substring(ini).Split(';')[0].Split('<')[0];
			if (mid.Contains(":")) mid = mid.Split(':')[1];
			var id = Convert.ToUInt64(mid, 16);

			if (id == 0) id = CeroMsgId;
			return id;
		}

		public static int GetDeviceIdTaip(String buffer)
		{
			var ini = buffer.IndexOf(";ID=");

			if (ini == -1) return WithoutDeviceId;

			ini += 4;

			var len = buffer.IndexOf(';', ini) - ini;

			var res = Convert.ToInt32(buffer.Substring(ini, len));
			if ((res == 0) || (res == 1)) res = CeroDeviceId;
			return res;
		}
	}
}