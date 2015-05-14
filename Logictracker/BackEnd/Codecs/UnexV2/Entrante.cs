using System;
using System.Collections.Generic;
using System.Globalization;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Utils;

namespace Logictracker.Unetel.v2
{
	public class Entrante
	{
		public static IMessage Parse(String[] partes, INode node)
		{
			if (partes[partes.Length - 1].EndsWith("*"))
			{
				partes[partes.Length - 1] = partes[partes.Length - 1].TrimEnd('*');
			}

			if (Convert.ToInt32(partes[2]) != node.Id) return null;

			var evpos = Posicion.ParsearPosicionCorta(partes[4]);

			var mid = Convert.ToUInt32(partes[1]);

			var code = partes[3];
			IMessage res;

			switch (code)
			{
				case EvCode.SpeedingTicket:
					res = new SpeedingTicket(
						node.Id,
						mid,
						Posicion.ParsearPosicionCorta(partes[5]),
						evpos,
						Speed.KnotToKm(Convert.ToInt16(partes[7])),
						Speed.KnotToKm(Convert.ToInt16(partes[6])),
						null);
					break;
				case EvCode.AcceleracionBrusca:
					res = FactoryBrusca(partes, node, evpos, MessageIdentifier.AccelerationEvent, mid);
					break;
				case EvCode.FrenadaBrusca:
					res = FactoryBrusca(partes, node, evpos, MessageIdentifier.DesaccelerationEvent, mid);
					break;
				default:
					res = TranslateLegacyCode(code, partes)
						.FactoryEvent(node.Id, mid, evpos, evpos.GetDate(), null, new List<Int64> { 0 });
					break;
			}
			return res.AddStringToSend(String.Format("REV;{0}", partes[1]));
		}

		private static IMessage FactoryBrusca(String[] partes, INode node, GPSPoint evpos, MessageIdentifier ev, ulong mid)
		{
			var res = ev.FactoryEvent(node.Id, mid, evpos, evpos.GetDate(), null, null);

			res.SensorsDataString = String.Format(
				CultureInfo.InvariantCulture,
				"VelocidadInicial:{0},VelocidadFinal:{1},LapsoProgramado:{2}",
				Speed.KnotToKm(Convert.ToInt16(partes[5])),
				Speed.KnotToKm(Convert.ToInt16(partes[6])),
				Convert.ToInt16(partes[7]));

			STrace.Debug(typeof(Entrante).FullName, node.Id, String.Format("{0} {1}", ev, res.SensorsDataString));

			return res;
		}

		private static MessageIdentifier TranslateLegacyCode(String evString, String[] partes)
		{
			if (evString.StartsWith("A0") || evString.StartsWith("C0"))
				return (MessageIdentifier)Convert.ToInt32(evString.Substring(2));

			var ev = Convert.ToInt32(evString, 16);
			switch (ev)
			{
				case EvCode.SpinCode: return GetSpinCode(partes[5]);
				case EvCode.PanicButtonOn: return MessageIdentifier.PanicButtonOn;
				case EvCode.PanicButtonOff: return MessageIdentifier.PanicButtonOff;
				case EvCode.SirenOn: return MessageIdentifier.SirenOn;
				case EvCode.SirenOff: return MessageIdentifier.SirenOff;
				case EvCode.SwitchingOn: return MessageIdentifier.EngineOnInternal;
				case EvCode.SwitchingOff: return MessageIdentifier.EngineOffInternal;
				case EvCode.HopperOn: return MessageIdentifier.TolvaActivated;
				case EvCode.HopperOff: return MessageIdentifier.TolvaDeactivated;
				case EvCode.BateryDisconected: return MessageIdentifier.BateryDisconected;
				case EvCode.BateryReConected: return MessageIdentifier.BateryReConected;
				case EvCode.StoppedEventSampeV2: return MessageIdentifier.StoppedEventSampeV2;
				case EvCode.DoorClosed: return MessageIdentifier.DoorClosed;
				case EvCode.DoorOpened: return MessageIdentifier.DoorOpenned;
				case EvCode.TrailerUnHooked: return MessageIdentifier.TrailerUnHooked;
				case EvCode.TrailerHooked: return MessageIdentifier.TrailerHooked;
				case EvCode.FuelEnabled: return MessageIdentifier.FuelEnabled;
				case EvCode.FuelDisabled: return MessageIdentifier.FuelDisabled;
				default:
					return MessageIdentifier.UnsuportedCmd;
			}
		}

		private static MessageIdentifier GetSpinCode(String s)
		{
			var val = Convert.ToInt16(s);
			switch (val)
			{
				case 0: return MessageIdentifier.MixerStopped;
				case 1: return MessageIdentifier.MixerCounterClockwise;
				case 2: return MessageIdentifier.MixerClockwise;
				case 3: return MessageIdentifier.PosibleSpinLeft;
				case 4: return MessageIdentifier.PosibleSpinRight;
				case 5: return MessageIdentifier.MixerCounterClockwiseFast;
				case 6: return MessageIdentifier.MixerClockwiseFast;
				case 7: return MessageIdentifier.MixerCounterClockwiseSlow;
				case 8: return MessageIdentifier.MixerClockwiseSlow;
				default:
					return MessageIdentifier.SpinStop2;
			}
		}

		private abstract class EvCode
		{
			public const int PanicButtonOff = 0xB010;
			public const int PanicButtonOn = 0xB011;
			public const int SirenOff = 0xB012;
			public const int SirenOn = 0xB013;
			public const int DoorClosed = 0xB014;
			public const int DoorOpened = 0xB015;
			public const int TrailerUnHooked = 0xB016;
			public const int TrailerHooked = 0xB017;
			public const int SwitchingOff = 0xB020;
			public const int SwitchingOn = 0xB021;
			public const int HopperOff = 0xB022;
			public const int HopperOn = 0xB023;
			public const int BateryDisconected = 0xB030;
			public const int BateryReConected = 0xB031;
			public const int FuelEnabled = 0xB040;
			public const int FuelDisabled = 0xB041;
			public const int StoppedEventSampeV2 = 0xB100;
			public const int SpinCode = 0xF100;

			public const String AcceleracionBrusca = "F404";
			public const String FrenadaBrusca = "F405";
			public const String SpeedingTicket = "F401";
		}
	}
}