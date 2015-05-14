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
using Logictracker.Utils;

namespace Logictracker.Melano.v1
{
    [FrameworkElement(XName = "MelanoParser", IsContainer = false)]
    public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Melano; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 50000)]
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
			var serial = BitConverter_BigEndian.ToInt16(frame.Payload, 1).ToString("D");
			var res = DataProvider.FindByIMEI(serial, this);
			if (res == null) STrace.Debug(GetType().FullName, String.Format("dispositivo no dado de alta, serial={0}", serial));
			return res;
		}

		public override IMessage Decode(IFrame frame)
		{
			var chR = BitConverter_BigEndian.ToInt32(frame.Payload, frame.Payload.Length-4);
			var chC = GetChecksum(frame.Payload);
			if (chC != chR)
			{
				STrace.Debug(GetType().FullName, Id, String.Format("Paquete descartado por checksum: checksum {0:X8}:{1:X8} {2}", chC, chR, StringUtils.MakeString(frame.Payload)));
				return null;
			}

			var tipo = (PacketType)frame.Payload[0];
			//var packetLenght = BitConverter_BigEndian.ToInt16(Frame.Payload, 3);
			var msgId = BitConverter_BigEndian.ToUInt32(frame.Payload, 5);

			var dt1 = (frame.Payload[11] == 0 || frame.Payload[10] == 0 || frame.Payload[9] == 0) ? DateTime.UtcNow : new DateTime(((DateTime.UtcNow.Year / 100) * 100) + frame.Payload[11], frame.Payload[10], frame.Payload[9], frame.Payload[12], frame.Payload[13], frame.Payload[14]);
			IMessage res;

			switch (tipo)
			{
				case PacketType.ReportPosition:
					{
						var data = Encoding.ASCII.GetString(frame.Payload, 15, frame.Payload.Length - 19).Split(',');

						var dt2 = DateTimeUtils.SafeParseFormat(data[10] + data[2].Split('.')[0], "ddMMyyHHmmss");
						var lat = GPSPoint.ResampleAxis(data[4]) * ((data[5] == "N") ? 1 : -1);
						var lon = GPSPoint.ResampleAxis(data[6]) * ((data[7] == "E") ? 1 : -1);
						var vel = Speed.KnotToKm(Convert.ToSingle(data[8], CultureInfo.InvariantCulture));
						var dir = Convert.ToSingle(data[9], CultureInfo.InvariantCulture);
						var pos = GPSPoint.Factory(dt2, lat, lon, vel, dir, 0, 0);

						res = pos.ToPosition(Id, msgId);
						var interval = BitConverter_BigEndian.GetBytes((Int16)1); //este numero multiplicado * 10'' es el intervalo de reporte
						var resdata = new []
							{
								(byte) DateTime.UtcNow.Day,
								(byte) DateTime.UtcNow.Month,
								(byte) DateTime.UtcNow.Year,
								(byte) DateTime.UtcNow.Hour,
								(byte) DateTime.UtcNow.Minute,
								(byte) DateTime.UtcNow.Second,
								interval[0],
								interval[1]
							};
						res.Response = FactoryResponse(PacketType.AckPosition, frame.Payload, resdata);
						STrace.Debug(GetType().FullName, Id, String.Format("dt {0} {1}", dt1, pos));
						break;
					}
				case PacketType.ReportModbusData:
					{
						var source = frame.Payload[15];
						var count = (frame.Payload.Length - Nondatalen) / Recordlen; //cantidad de bytes de los datos / 4 bytes por cada dato
						var sb = new StringBuilder();
						for (var i = 0; i < count; i++)
						{
							var key = GetShort(frame, i, 16);
							var value = GetShort(frame, i, 18) * 1.0;

							if (key == 40108) value /= 10; //para "Fuel Rate" el valor viene expresado en decilitros y es conveniente visualizarlo en "Litros"

							if ((value != 0x8000) && (value != 0x80000000)) sb.AppendFormat(CultureInfo.InvariantCulture, "Modbus_{0}_{1}:{2},", source, key, value);
						}

						res = MessageIdentifier.AnalogicInputs.FactoryEvent(MessageIdentifier.TelemetricData, Id, msgId, null, dt1, null, null);
						((Event)res).SensorsDataString = sb.ToString();
						res.Response = FactoryResponse(PacketType.AckModbusData, frame.Payload, null);
						STrace.Debug(GetType().FullName, Id, String.Format("ModbusData {0} dt {1} payload {2}", sb, dt1, StringUtils.MakeString(frame.Payload)));
						break;
					}
				case PacketType.ReplyModbusList:
					//pendiente Fota.Dequeue(PacketType.Command_ModbusList)
					STrace.Debug(GetType().FullName, Id, String.Format("ModbusList (dt {0}) {1}", dt1, StringUtils.MakeString(frame.Payload)));
					res = new UserMessage(Id, msgId);
					break;
				case PacketType.RequestBiosNewPage:
					{
						STrace.Debug(GetType().FullName, Id, String.Format("BiosNewPage (dt {0}) {1}", dt1, StringUtils.MakeString(frame.Payload)));
						//pendiente fota de bios
						//var requestedpagelen = BitConverter_BigEndian.ToUInt16(Frame.Payload, 15);
						//var requestedpagenumber = BitConverter_BigEndian.ToUInt16(Frame.Payload, 17);
						//var resdata = new byte[50];
						//Array.Copy(biosdata, 0, resdata, 0, 50);
						res = new UserMessage(Id, msgId);
						//res.Response = FactoryResponse(PacketType.BiosNewPage, Frame.Payload, resdata);
						break;
					}
				case PacketType.ReportEngineData:
					{
						var enginenum = frame.Payload[15];
						var engineStateNum = frame.Payload[16];
						var engineState = (EngineStates)engineStateNum;
						MessageIdentifier evcode;
						var isevent = new List<EngineStates> {EngineStates.Start, EngineStates.Stop}.Contains(engineState);
						if (_enginesStates.ContainsKey(enginenum) && (!isevent) && ((_enginesStates[enginenum]) == engineState))
						{
							evcode = MessageIdentifier.AnalogicInputs;
						}
						else
						{
							if (!_enginesStates.ContainsKey(enginenum))
							{
								_enginesStates.Add(enginenum, engineState);
							}
							switch (engineState)
							{
								case EngineStates.Start:
									evcode = MessageIdentifier.EngineOn;
									break;
								case EngineStates.Stop:
									evcode = MessageIdentifier.EngineOff;
									break;
								//case EngineStates.DataInfo:
								//case EngineStates.Unknown:
								default:
									evcode = MessageIdentifier.TelemetricData;
									break;
							}
						}

						var sb = new StringBuilder();

						var engHours = BitConverter_BigEndian.ToUInt32(frame.Payload, 17);
						var fuelUsed = BitConverter_BigEndian.ToUInt32(frame.Payload, 21);
						var kVAhours = BitConverter_BigEndian.ToUInt32(frame.Payload, 25);
						var kVAhoursParcial = BitConverter_BigEndian.ToUInt32(frame.Payload, 29);
						var segundosRunParcial = BitConverter_BigEndian.ToUInt32(frame.Payload, 33);

						sb.AppendFormat(CultureInfo.InvariantCulture, "Modbus_{0}_EngineState:{1},", enginenum, engineState);
						if ((engHours != 0x8000) && (engHours != 0x80000000)) sb.AppendFormat(CultureInfo.InvariantCulture, "Modbus_{0}_43587:{1},", enginenum, engHours);
						if ((fuelUsed != 0x8000) && (fuelUsed != 0x80000000)) sb.AppendFormat(CultureInfo.InvariantCulture, "Modbus_{0}_40126:{1},", enginenum, fuelUsed);
						if ((kVAhours != 0x8000) && (kVAhours != 0x80000000)) sb.AppendFormat(CultureInfo.InvariantCulture, "Modbus_{0}_43595:{1},", enginenum, kVAhours);
						if ((kVAhoursParcial != 0x8000) && (kVAhoursParcial != 0x80000000)) sb.AppendFormat(CultureInfo.InvariantCulture, "Modbus_{0}_40337:{1},", enginenum, kVAhoursParcial);
						if ((segundosRunParcial != 0x8000) && (segundosRunParcial != 0x80000000)) sb.AppendFormat(CultureInfo.InvariantCulture, "Modbus_{0}_SegundosRunParcial:{1},", enginenum, segundosRunParcial);

						var res_ = evcode.FactoryEvent(MessageIdentifier.TelemetricData, Id, msgId, null, dt1, null, null);
						res_.SensorsDataString = sb.ToString();
						STrace.Debug(GetType().FullName, Id, String.Format("EngineData: {0} Payload: {1}", res_.SensorsDataString, StringUtils.MakeString(frame.Payload)));
						res_.Response = FactoryResponse(PacketType.AckEngineData, frame.Payload, null);
						res = res_;
						break;
					}
				default:
					STrace.Debug(GetType().FullName, Id, String.Format("paquete no reconocido: {0}", StringUtils.MakeString(frame.Payload)));
					return null;
			}
			//Debug.Assert(res != null);
			res.Tiempo = dt1;
			return res;
		}

		private readonly Dictionary<byte, EngineStates> _enginesStates = new Dictionary<byte, EngineStates>();

	    private static ushort GetShort(IFrame frame, int i, int offset)
	    {
		    return BitConverter_BigEndian.ToUInt16(frame.Payload, offset + Recordlen * i);
	    }

	    private static byte[] FactoryResponse(PacketType t, byte[] packet, byte[] ba)
		{
			var len = ((ba != null)?ba.Length:0) + 21;
			var res = new byte[len];

			res[0] = (byte) t;

			Array.Copy(packet, 1, res, 1, 8);

			res[3] = (byte) ((len >> 8) & 0x00FF);
			res[4] = (byte) (len & 0x00FF);

			res[9] = 0x33;
			res[10] = 0x12;
			res[11] = 0x11;
			res[12] = 0x55;
			res[13] = 0xA5;
			res[14] = 0xC5;
			res[15] = 0x67;
			res[16] = 0x87;

			if (ba != null) Array.Copy(ba, 0, res, 17, ba.Length);

			var ch = GetChecksum(res);

			res[len - 4] = (byte) ((ch >> 24) & 0x000000FF);
			res[len - 3] = (byte) ((ch >> 16) & 0x000000FF);
			res[len - 2] = (byte) ((ch >>  8) & 0x000000FF);
			res[len - 1] = (byte) ((ch      ) & 0x000000FF);
			return res;
		}

		private static Int32 GetChecksum(byte[] ba)
		{
			return CRC32(ba, ba.Length - 4);
		}

// ReSharper disable UnusedMember.Local
		//Report Reply Request: Server <- EC400
		//Ack Command Info:     Server -> EC400
		private enum PacketType : byte
		{
			Unknown = 0x00,
			ReportPosition = 0x01, //*
			AckPosition = 0x02,
			ReportModbusData = 0x03, //*
			AckModbusData = 0x04,
			//Command_ModbusList = 0x05,
			ReplyModbusList = 0x06, //*
			//0x07
			//0x08
			//0x09
			//FotaStart = 0x10,
			RequestBiosNewPage = 0x11, //*
			//BiosNewPage = 0x12,
			ReportEngineData = 0x21, //*
			AckEngineData = 0x22,
		}

		private enum EngineStates : byte //1 = arranque, 2 = parada
		{
			Unknown = 0x00,
			Start = 0x01,
			Stop = 0x02,
			DataInfo = 0x03,
		}
// ReSharper restore UnusedMember.Local

		private const int Nondatalen = 15 + 1 + 4;
		private const int Recordlen = 4;

		#region Tabla de CRC

		private static int CRC32(byte[] xvar, int n)
		{
			var CRC = 0xFFFFFFFF;
			for (var i = 0; i < n; i++)
			{
				var var2 = ((CRC >> 24) & 0x000000FF) ^ xvar[i];
				var Y = TCRC[var2];

				var j = (CRC << 8) & 0xFFFFFF00;

				CRC = Y ^ j;
			}
			return (int)(CRC ^ 0xFFFFFFFF);
		}

		private static readonly uint[] TCRC = new uint[]
		                               	{
		                               		0x00000000,
		                               		0x04C11DB7,
		                               		0x09823B6E,
		                               		0x0D4326D9,
		                               		0x130476DC,
		                               		0x17C56B6B,
		                               		0x1A864DB2,
		                               		0x1E475005,
		                               		0x2608EDB8,
		                               		0x22C9F00F,
		                               		0x2F8AD6D6,
		                               		0x2B4BCB61,
		                               		0x350C9B64,
		                               		0x31CD86D3,
		                               		0x3C8EA00A,
		                               		0x384FBDBD,
		                               		0x4C11DB70,
		                               		0x48D0C6C7,
		                               		0x4593E01E,
		                               		0x4152FDA9,
		                               		0x5F15ADAC,
		                               		0x5BD4B01B,
		                               		0x569796C2,
		                               		0x52568B75,
		                               		0x6A1936C8,
		                               		0x6ED82B7F,
		                               		0x639B0DA6,
		                               		0x675A1011,
		                               		0x791D4014,
		                               		0x7DDC5DA3,
		                               		0x709F7B7A,
		                               		0x745E66CD,
		                               		0x9823B6E0,
		                               		0x9CE2AB57,
		                               		0x91A18D8E,
		                               		0x95609039,
		                               		0x8B27C03C,
		                               		0x8FE6DD8B,
		                               		0x82A5FB52,
		                               		0x8664E6E5,
		                               		0xBE2B5B58,
		                               		0xBAEA46EF,
		                               		0xB7A96036,
		                               		0xB3687D81,
		                               		0xAD2F2D84,
		                               		0xA9EE3033,
		                               		0xA4AD16EA,
		                               		0xA06C0B5D,
		                               		0xD4326D90,
		                               		0xD0F37027,
		                               		0xDDB056FE,
		                               		0xD9714B49,
		                               		0xC7361B4C,
		                               		0xC3F706FB,
		                               		0xCEB42022,
		                               		0xCA753D95,
		                               		0xF23A8028,
		                               		0xF6FB9D9F,
		                               		0xFBB8BB46,
		                               		0xFF79A6F1,
		                               		0xE13EF6F4,
		                               		0xE5FFEB43,
		                               		0xE8BCCD9A,
		                               		0xEC7DD02D,
		                               		0x34867077,
		                               		0x30476DC0,
		                               		0x3D044B19,
		                               		0x39C556AE,
		                               		0x278206AB,
		                               		0x23431B1C,
		                               		0x2E003DC5,
		                               		0x2AC12072,
		                               		0x128E9DCF,
		                               		0x164F8078,
		                               		0x1B0CA6A1,
		                               		0x1FCDBB16,
		                               		0x018AEB13,
		                               		0x054BF6A4,
		                               		0x0808D07D,
		                               		0x0CC9CDCA,
		                               		0x7897AB07,
		                               		0x7C56B6B0,
		                               		0x71159069,
		                               		0x75D48DDE,
		                               		0x6B93DDDB,
		                               		0x6F52C06C,
		                               		0x6211E6B5,
		                               		0x66D0FB02,
		                               		0x5E9F46BF,
		                               		0x5A5E5B08,
		                               		0x571D7DD1,
		                               		0x53DC6066,
		                               		0x4D9B3063,
		                               		0x495A2DD4,
		                               		0x44190B0D,
		                               		0x40D816BA,
		                               		0xACA5C697,
		                               		0xA864DB20,
		                               		0xA527FDF9,
		                               		0xA1E6E04E,
		                               		0xBFA1B04B,
		                               		0xBB60ADFC,
		                               		0xB6238B25,
		                               		0xB2E29692,
		                               		0x8AAD2B2F,
		                               		0x8E6C3698,
		                               		0x832F1041,
		                               		0x87EE0DF6,
		                               		0x99A95DF3,
		                               		0x9D684044,
		                               		0x902B669D,
		                               		0x94EA7B2A,
		                               		0xE0B41DE7,
		                               		0xE4750050,
		                               		0xE9362689,
		                               		0xEDF73B3E,
		                               		0xF3B06B3B,
		                               		0xF771768C,
		                               		0xFA325055,
		                               		0xFEF34DE2,
		                               		0xC6BCF05F,
		                               		0xC27DEDE8,
		                               		0xCF3ECB31,
		                               		0xCBFFD686,
		                               		0xD5B88683,
		                               		0xD1799B34,
		                               		0xDC3ABDED,
		                               		0xD8FBA05A,
		                               		0x690CE0EE,
		                               		0x6DCDFD59,
		                               		0x608EDB80,
		                               		0x644FC637,
		                               		0x7A089632,
		                               		0x7EC98B85,
		                               		0x738AAD5C,
		                               		0x774BB0EB,
		                               		0x4F040D56,
		                               		0x4BC510E1,
		                               		0x46863638,
		                               		0x42472B8F,
		                               		0x5C007B8A,
		                               		0x58C1663D,
		                               		0x558240E4,
		                               		0x51435D53,
		                               		0x251D3B9E,
		                               		0x21DC2629,
		                               		0x2C9F00F0,
		                               		0x285E1D47,
		                               		0x36194D42,
		                               		0x32D850F5,
		                               		0x3F9B762C,
		                               		0x3B5A6B9B,
		                               		0x0315D626,
		                               		0x07D4CB91,
		                               		0x0A97ED48,
		                               		0x0E56F0FF,
		                               		0x1011A0FA,
		                               		0x14D0BD4D,
		                               		0x19939B94,
		                               		0x1D528623,
		                               		0xF12F560E,
		                               		0xF5EE4BB9,
		                               		0xF8AD6D60,
		                               		0xFC6C70D7,
		                               		0xE22B20D2,
		                               		0xE6EA3D65,
		                               		0xEBA91BBC,
		                               		0xEF68060B,
		                               		0xD727BBB6,
		                               		0xD3E6A601,
		                               		0xDEA580D8,
		                               		0xDA649D6F,
		                               		0xC423CD6A,
		                               		0xC0E2D0DD,
		                               		0xCDA1F604,
		                               		0xC960EBB3,
		                               		0xBD3E8D7E,
		                               		0xB9FF90C9,
		                               		0xB4BCB610,
		                               		0xB07DABA7,
		                               		0xAE3AFBA2,
		                               		0xAAFBE615,
		                               		0xA7B8C0CC,
		                               		0xA379DD7B,
		                               		0x9B3660C6,
		                               		0x9FF77D71,
		                               		0x92B45BA8,
		                               		0x9675461F,
		                               		0x8832161A,
		                               		0x8CF30BAD,
		                               		0x81B02D74,
		                               		0x857130C3,
		                               		0x5D8A9099,
		                               		0x594B8D2E,
		                               		0x5408ABF7,
		                               		0x50C9B640,
		                               		0x4E8EE645,
		                               		0x4A4FFBF2,
		                               		0x470CDD2B,
		                               		0x43CDC09C,
		                               		0x7B827D21,
		                               		0x7F436096,
		                               		0x7200464F,
		                               		0x76C15BF8,
		                               		0x68860BFD,
		                               		0x6C47164A,
		                               		0x61043093,
		                               		0x65C52D24,
		                               		0x119B4BE9,
		                               		0x155A565E,
		                               		0x18197087,
		                               		0x1CD86D30,
		                               		0x029F3D35,
		                               		0x065E2082,
		                               		0x0B1D065B,
		                               		0x0FDC1BEC,
		                               		0x3793A651,
		                               		0x3352BBE6,
		                               		0x3E119D3F,
		                               		0x3AD08088,
		                               		0x2497D08D,
		                               		0x2056CD3A,
		                               		0x2D15EBE3,
		                               		0x29D4F654,
		                               		0xC5A92679,
		                               		0xC1683BCE,
		                               		0xCC2B1D17,
		                               		0xC8EA00A0,
		                               		0xD6AD50A5,
		                               		0xD26C4D12,
		                               		0xDF2F6BCB,
		                               		0xDBEE767C,
		                               		0xE3A1CBC1,
		                               		0xE760D676,
		                               		0xEA23F0AF,
		                               		0xEEE2ED18,
		                               		0xF0A5BD1D,
		                               		0xF464A0AA,
		                               		0xF9278673,
		                               		0xFDE69BC4,
		                               		0x89B8FD09,
		                               		0x8D79E0BE,
		                               		0x803AC667,
		                               		0x84FBDBD0,
		                               		0x9ABC8BD5,
		                               		0x9E7D9662,
		                               		0x933EB0BB,
		                               		0x97FFAD0C,
		                               		0xAFB010B1,
		                               		0xAB710D06,
		                               		0xA6322BDF,
		                               		0xA2F33668,
		                               		0xBCB4666D,
		                               		0xB8757BDA,
		                               		0xB5365D03,
		                               		0xB1F740B4
		                               	};

		#endregion

        #endregion
    }
}
