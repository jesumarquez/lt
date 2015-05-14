#region Usings

using System;
using System.Collections.Generic;
using Urbetrack.AVL;
using Urbetrack.Backbone;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Core.TransactionUsers;
using Urbetrack.Messaging;
using Urbetrack.Utils;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Model;
using Urbetrack.Layers;

#endregion

namespace Urbetrack.Gateway.Joint.MessageQueue
{
	internal class UrbetrackUT : ServerTU
	{
		public static bool decorate_fix_with_zone;
		internal static IDispatcherLayer Dispatcher;

		public UrbetrackUT()
		{
			AutoReport += UrbetrackUT_AutoReport;
			RFIDDetected += UrbetrackUT_RFIDDetected;
			ExcesoVelocidad += UrbetrackUT_ExcesoVelocidad;
			Evento += UrbetrackUT_Evento;
		}

		private static bool UrbetrackUT_Evento(object sender, Evento pdu)
		{
			var d = Devices.I().FindById(pdu.IdDispositivo);
			if (d == null) return false;

			if (DataTransportLayer.IsRetransmission(d.Id, pdu.Seq))
			{
				return false;
			}
	
			
			DecorateFixWithZone(pdu.Posicion);
			var data = pdu.Datos;
			var extra = pdu.Extra;

			var codigo = (MessageIdentifier) pdu.CodigoEvento;

			if (((pdu.CodigoEvento > 0xD0 && pdu.CodigoEvento < 0xD6) || pdu.CodigoEvento == 85))
				codigo = MessageIdentifier.GenericMessage;

			var msg = codigo.FactoryEvent(d.Id, pdu.Seq, pdu.Posicion, pdu.Posicion.GetDate(), pdu.RiderIdentifier, new List<int>(2) {data, extra});
			msg.Payload = pdu.Payload;
			msg.PayloadSize = pdu.PayloadSize;

			Dispatcher.Dispatch(msg);
			return true;
		}

		private static bool UrbetrackUT_AutoReport(object sender, Posicion pos)
		{
			var d = Devices.I().FindById(pos.IdDispositivo);
			if (d == null) return false;

			if (DataTransportLayer.IsRetransmission(d.Id, pos.Seq))
			{
				return false;
			}

			// decoro la zona de las GeoPoints.
			foreach (var point in pos.Puntos)
			{
				if (d.Type == DeviceTypes.Types.URBMOBILE_v0_1)
				{
					point.Date = point.Date.AddHours(3);
				}
				DecorateFixWithZone(point);
			}

			Dispatcher.Dispatch(pos.Puntos.ToPosition(d.Id, pos.Seq));

			return true;
		}

		private static bool UrbetrackUT_RFIDDetected(object sender, RFIDDetectado pdu)
		{
			var d = Devices.I().FindById(pdu.IdDispositivo);
			if (d == null) return false;

			if (DataTransportLayer.IsRetransmission(d.Id, pdu.Seq))
			{
				return false;
			}

			DecorateFixWithZone(pdu.Posicion);

			var msg = MessageIdentifierX.FactoryRfid(d.Id, pdu.Seq, pdu.Posicion, pdu.Posicion.GetDate(), pdu.IdTarjeta, pdu.Status);

			Dispatcher.Dispatch(msg);
			return true;
		}

		private static bool UrbetrackUT_ExcesoVelocidad(object sender, ExcesoVelocidad pdu)
		{
			var d = Devices.I().FindById(pdu.IdDispositivo);

			if (d == null) return false;

			// si alguna posicion DEL TICKET esta mocha...
			if (pdu.PosicionDeTicket == null || pdu.PosicionDeTicket.Lat == 0 || pdu.PosicionFinal == null || pdu.PosicionFinal.Lat == 0)
			{
				STrace.Debug(typeof(UrbetrackUT).FullName, d.Id, String.Format("CrapReceivedCounter -> Device[{0}]: TICKET[VELOCIDAD EXCEDIDA]/MALFORMACION DE DATOS", d.LogId));
				return true;
			}

			#region HACK FIX PARA LA VERSION 99 DEL FIRMWARE

			if (d.HackVersion99())
			{
				if (pdu.PosicionDeAviso != null && pdu.PosicionDeAviso.Lat != 0)
				{
					var diferencia = pdu.PosicionDeTicket.Date - pdu.PosicionDeAviso.Date;

					STrace.Debug(typeof(UrbetrackUT).FullName, d.Id,String.Format( "TICKET[VELOCIDAD EXCEDIDA/DEVICE={0}]: DIFERENCIA CALCULADA {1}", d.LogId, diferencia));

					if (diferencia.TotalSeconds > 21)
					{
						if (QuadTree.QuadTree.i() == null) return false;

						// esto me indica que aplico el b-u-g
						var claseAviso = QuadTree.QuadTree.i().GetPositionClass(pdu.PosicionDeAviso.Lat, pdu.PosicionDeAviso.Lon);
						var claseTicket = QuadTree.QuadTree.i().GetPositionClass(pdu.PosicionDeTicket.Lat, pdu.PosicionDeTicket.Lon);

						if (claseAviso == -1 || claseTicket == -1) return false;

						if (claseAviso == claseTicket) return true;

						STrace.Debug(typeof(UrbetrackUT).FullName, d.Id, String.Format("TICKET[VELOCIDAD EXCEDIDA/DEVICE={0}/RIDER={1}]: REPARANDO EVENTO MALFORMADO X BUG v99", d.LogId, pdu.RiderIdentifier));

						pdu.PosicionDeAviso = null;
					}
				}

				if (pdu.PosicionDeAviso != null && pdu.PosicionDeAviso.Lat != 0)
				{
					var duracion = pdu.PosicionFinal.Date.Subtract(pdu.PosicionDeAviso.Date).TotalSeconds;

					if (duracion < 21) return true;
				}
			}

			#endregion

			DecorateFixWithZone(pdu.PosicionDeTicket);
			DecorateFixWithZone(pdu.PosicionDeTicket);
			DecorateFixWithZone(pdu.PosicionFinal);

			if (DataTransportLayer.IsRetransmission(d.Id, pdu.Seq))
			{
				return false;
			}
			var msg = new SpeedingTicket(d.Id, pdu.Seq)
						 {
							 StartPoint = pdu.PosicionDeAviso,
							 UserIdentifier = pdu.RiderIdentifier,
							 TicketPoint = pdu.PosicionDeTicket,
							 EndPoint = pdu.PosicionFinal,
							 SpeedReached = Speed.KnotToKm(pdu.VelocidadMaximaAlcanzada),
							 SpeedLimit = Speed.KnotToKm(pdu.VelocidadMaximaPermitida),
							 Tiempo = (pdu.PosicionDeAviso ?? pdu.PosicionDeTicket).GetDate(),
						 };
			Dispatcher.Dispatch(msg);
			return true;
		}

		private static void DecorateFixWithZone(GPSPoint fix)
		{
			if (fix == null) return;

			fix.Speed = new Speed(Speed.KnotToKm(fix.Speed.Unpack()));

			if (!decorate_fix_with_zone) return;
			var zona = '?';
			try
			{
				zona = QuadTree.QuadTree.i().GetPositionZone(fix.Lat, fix.Lon);
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(UrbetrackUT).FullName, e, fix.DeviceId);
			}
			fix.Zona = zona;
		}
	}
}