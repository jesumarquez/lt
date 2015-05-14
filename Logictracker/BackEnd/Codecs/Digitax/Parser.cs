using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

namespace Logictracker.Digitax
{
    [FrameworkElement(XName = "DigitaxParser", IsContainer = false)]
	public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Digitax; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2009)]
		public override int Port { get; set; }

		#endregion

		#region BaseCodec

        protected override UInt32 NextSequenceMin()
        {
            return 0x0000;
        }

        protected override UInt32 NextSequenceMax()
        {
            return 0xFFFF;
        }

		public override INode Factory(IFrame frame, int formerId)
        {
			//if (!PacketOk(Frame)) return null;
            var imei = Encoding.ASCII.GetString(frame.Payload, 111, 15);

			var dev = DataProvider.FindByIMEI(imei, this);
			if (dev == null)
			{
                var code = (frame.Payload[2] + (frame.Payload[3] << 8)).ToString(CultureInfo.InvariantCulture);
				STrace.Trace(GetType().FullName, 0, String.Format("Dispositivo no registrado: Imei={0} Code={1} ({2}) Payload={3}", imei, code, frame.RemoteAddressAsString, frame.PayloadAsString));
			}
			return dev;
        }

		public override IMessage Decode(IFrame frame)
        {
            if (ParserUtils.IsInvalidDeviceId(Id)) return null;

			//Debug.Assert(Frame.Payload[5] == 0x77);
			//var SENDOK = Frame.Payload[7];
			//var IDLETIME = Frame.Payload[13] + Frame.Payload[14] << 8;

			var msgId = BitConverter.ToUInt32(frame.Payload, 87);
            var tipoReporte = frame.Payload[4];

			var vel = Speed.KnotToKm(frame.Payload[26]);

			var hour8 = frame.Payload[27];
			if (hour8 < 0x80)
			{
				STrace.Trace(GetType().FullName, Id, "Descartando reporte por hora invalida");
				return null;
			}

			var year = Convert.ToInt32(frame.Payload[25].ToString("X2")) + ((DateTime.UtcNow.Year / 100) * 100);
			var month = Convert.ToInt32(frame.Payload[24].ToString("X2"));
			var day = Convert.ToInt32(frame.Payload[23].ToString("X2"));
			var minute = Convert.ToInt32(frame.Payload[28].ToString("X2"));
			var second = Convert.ToInt32(frame.Payload[29].ToString("X2"));

			var hour = Convert.ToInt32((hour8 - 0x80).ToString("X2"));
			var date = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
			var lat = (float)(BitConverter.ToInt32(frame.Payload, 30) / (-600000.0));
			var lon = (float)(BitConverter.ToInt32(frame.Payload, 34) / (-600000.0));

			var pos = GPSPoint.Factory(date, lat, lon, vel);

			//var TIMESPEED = Frame.Payload[38];
			//var SPEED = Frame.Payload[39-49];

			var chofer = (frame.Payload[62] + frame.Payload[63] << 8).ToString("X10");

			lock (ChoferesLock)
			{
				if (Choferes.ContainsKey(Id))
				{
					var lastChofer = Choferes[Id];
					ProcessChoferState(pos, date, msgId, lastChofer, 1); //logout
					ProcessChoferState(pos, date, msgId, chofer, 0); //login
				}
				else
				{
					Choferes.Add(Id, chofer);
				}
			}

            switch (tipoReporte)
            {
                case 0x17: //Reset del equipo completo.
		            return MessageIdentifier.DeviceShutdown.FactoryEvent(MessageIdentifier.GenericMessage, Id, msgId, pos, date, chofer, null);
                case 0x18: //Inicio (power up) del equipo.
					return MessageIdentifier.DeviceTurnedOn.FactoryEvent(MessageIdentifier.GenericMessage, Id, msgId, pos, date, chofer, null);
                case 0x19: //Intento de reconexión del modulo GSM.
					return MessageIdentifier.GsmSignalOn.FactoryEvent(MessageIdentifier.GenericMessage, Id, msgId, pos, date, chofer, null);
                case 0x25: //Reset del modulo GPS.
					return MessageIdentifier.GpsSignalOff.FactoryEvent(MessageIdentifier.GenericMessage, Id, msgId, pos, date, chofer, null);
                case 0x63: //Recupero de información.//posicion encolada
				case 0x64: //Información actual.//posicion online
				case 0x65: //Información actual conectado a capturador de datos.//posicion online
					return pos.ToPosition(Id, msgId);
				default:
					STrace.Debug(GetType().FullName, Id, String.Format("Llego reporte de tipo no soportado: tipo=0x{0:X2} lat={1} lon={2} date={3} chofer={4}", tipoReporte, pos.Lat, pos.Lon, date, chofer));
		            return null;
            }
        }

		public override bool IsPacketCompleted(byte[] payload, int start, int count, out int detectedCount, out bool ignoreNoise)
		{
			ignoreNoise = false;
			detectedCount = 0;
			if (count < 6) return false;
			if (payload[start] == 0x3C && payload[start + 1] == 0x29)
			{
				detectedCount = payload[start + 5] + 9;
				return count >= detectedCount;
			}

			//hay basura
			ignoreNoise = true;
			var limit = start + count - 1;
			var newStart = start;
			while (payload[newStart] != 0x3C || payload[newStart + 1] != 0x29)
			{
				newStart = Array.IndexOf(payload, 0x3C, newStart, limit);
				if (newStart != -1) continue;
				detectedCount = count;
				return true;
			}

			detectedCount = newStart - start;
			return true;
		}

	    #endregion

	    #region Private Members

		private readonly static Object ChoferesLock = new Object();
		private readonly static Dictionary<int, String> Choferes = new Dictionary<int, String>();

		private void ProcessChoferState(GPSPoint pos, DateTime date, uint msgId, String chofer, int state)
		{
			if ((chofer == "0000000000") || (chofer == "000000FFFF")) return;
			var ev = MessageIdentifierX.FactoryRfid(Id, msgId, pos, date, chofer, state);
			DataTransportLayer.DispatchMessage(this, ev);
		}

		/*private bool PacketOk(IFrame Frame)
	    {
			if (Frame.Payload[0] != 0x3C || Frame.Payload[1] != 0x29) return false;

		    var crcnew = CalcCrc(Frame.Payload, 0, Frame.Payload.Length - 4);
		    var crcold = Frame.Payload[Frame.Payload.Length - 2] + ((Frame.Payload[Frame.Payload.Length - 1] << 8) & 0xFF00);
		    if (crcold != crcnew)
		    {
			    STrace.Debug(GetType().FullName, Id, "reporte descartado por checksum: '{0:X2}' '{1:X2}' '{2}' ", crcold, crcnew, Frame.Payload);
				return false;
		    }
			STrace.Debug(GetType().FullName, Id, "reporte: '{0}'", Frame.Payload);
			return true;
	    }

		private static ushort CalcCrc(byte[] data, int index, int len)
		{
			//ushort crc = 0xFFFF;
			ushort crc = 0x0000; //la documentacion de digitax dice "es el crc 0x1021 con el inicio en cero"

			for (var i = index; i < len; i++)
				crc = UpdateCrc(crc, data[i]);

			return crc;
		}

		private static ushort UpdateCrc(ushort crc, byte b)
		{
			crc ^= (ushort)(b << 8);

			for (var i = 0; i < 8; i++)
			{
				if ((crc & 0x8000) > 0)
					crc = (ushort)((crc << 1) ^ 0x1021);
				else
					crc <<= 1;
			}

			return crc;
		}//*/

	    #endregion
	}
}