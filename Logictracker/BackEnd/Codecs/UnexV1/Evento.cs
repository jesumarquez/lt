#region Usings

using System;
using Urbetrack.AVL;
using Urbetrack.Messaging;
using Urbetrack.Utils;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Model;

#endregion

namespace Urbetrack.Unetel.v1
{
	public class Evento
	{
		public static Event Parse(String asString, INode node)
		{
			var IdEquipo = Convert.ToInt32(asString.Substring(3, 5));
			if (node.Id != SafeConvert.ToInt32(IdEquipo, -1))
			{
				STrace.Debug(typeof(Evento).FullName, node.Id, "ENTRANTE DESCARTAD0, NO COINCIDE. raw_data={0}", asString);
				return null;
			}

			//se cambian algunos codigos por que hay conflictos
			var code = Convert.ToInt16(asString.Substring(1, 2));
			switch (code)
			{
				case 90: code = (short)MessageIdentifier.BateryReConected; break;
				case 91: code = (short)MessageIdentifier.PanicButtonOff; break;
				case 92: code = (short)MessageIdentifier.PanicButtonOn; break;
				case 93: code = (short)MessageIdentifier.SlowingTicket; break;
				case 94: code = (short)MessageIdentifier.EngineOn_internal; break;
				case 95: code = (short)MessageIdentifier.EngineOff_internal; break;
				case 96: code = (short)MessageIdentifier.SpeedingTicket; break;
				case 97: code = (short)MessageIdentifier.BateryDisconected; break;
				case 99: code = (short)MessageIdentifier.DisplayProblem; break;
			}

			var IdMsg = Convert.ToUInt32(asString.Substring(8, 5));

			return (Event)((MessageIdentifier)code)
				.FactoryEvent(node.Id, IdMsg, null, DateTime.UtcNow, null, null)
				.AddStringToSend(String.Format("GA{0:D5}", IdMsg));
		}
	}
}