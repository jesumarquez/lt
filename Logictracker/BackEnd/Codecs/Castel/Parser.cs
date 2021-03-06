using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

namespace Logictracker.Castel
{
    [FrameworkElement(XName = "CastelParser", IsContainer = false)]
	public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Castel; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2011)]
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
			var imei = Encoding.ASCII.GetString(frame.Payload.Skip(4).Take(20).Where(b => b != 0).ToArray()).ToUpperInvariant(); //"DeviceId" en la documentacion de castel

			var dev = DataProvider.FindByIMEI(imei, this);
			if (dev == null)
			{
				STrace.Trace(GetType().FullName, String.Format("Dispositivo no registrado: Imei={0}", imei));
			}
			return dev;
        }

		public override IMessage Decode(IFrame frame)
        {
			if (ParserUtils.IsInvalidDeviceId(Id)) return null;
			//if ((Frame.Payload[0] != '$') || (Frame.Payload[1] != '$')) return null;
			//if ((Frame.Payload[Frame.Payload.Length - 2] != '\r') || (Frame.Payload[Frame.Payload.Length - 1] != '\n')) return null;
			if (!CheckChecksum(frame.Payload, frame.Payload.Length))
			{
				STrace.Debug(GetType().FullName, Id, String.Format("paquete descartado por checksum: {0}", frame.PayloadAsString));
				return null;
			}
			//var Length = GetLength(Frame.Payload, 2);
			//Debug.Assert(Frame.Payload.Length == Length);
			//"When GPS have not positioning, GPS time will show as system time."
			//return MessageIdentifier.GpsSignalOff.FactoryEvent(MessageIdentifier.GenericMessage, Id, msgId, pos, dt, chofer, null);

			//var dataLength = Frame.Payload.Length - 30;

			//String chofer = null;
			var type = BitConverter_BigEndian.ToInt16(frame.Payload, 24);
			var packettype = (PacketType)type;

			switch (packettype)
			{
				case PacketType.Login: return ParseLogin(frame);
				case PacketType.RollCallResponse: return ParseRollCallResponse(frame);
				case PacketType.Logout: return ParseLogout(frame);
				case PacketType.FixedUpload: return ParseFixedUpload(frame);
				case PacketType.VehicleSupportedDataStreamType: return ParseVehicleSupportedDataStreamType(frame);
				case PacketType.UploadFreezeFrameDataStream: return ParseUploadFreezeFrameDataStream(frame);
				case PacketType.UploadDataStreamInfo: return ParseUploadDataStreamInfo(frame);
				case PacketType.UploadStoreMalfunctionInfo: return ParseUploadMalfunctionInfo(frame, packettype);
				case PacketType.UploadPendingMalfunctionInfo: return ParseUploadMalfunctionInfo(frame, packettype);
				case PacketType.UploadAlarm: return ParseUploadAlarm(frame, packettype);
				case PacketType.UploadAlarmElimination: return ParseUploadAlarm(frame, packettype);
				case PacketType.TripStart: return ParseTripStart(frame);
				case PacketType.TripEnd: return ParseTripEnd(frame);
				default:
					STrace.Debug(GetType().FullName, Id, String.Format("RX 0x{0:X4}:{1}{2}", type, Environment.NewLine, GetData(frame)));
					return new UserMessage(Id, 0);
			}
		}

	    public override bool IsPacketCompleted(byte[] payload, int start, int count, out int detectedCount, out bool ignoreNoise)
		{
			ignoreNoise = false;
			detectedCount = 0;
			if (count < 6) return false;
			if (payload[start] == '$' && payload[start + 1] == '$')
			{
				detectedCount = BitConverter.ToInt16(payload, start + 2);
				return count >= detectedCount;
			}

			//hay basura
			ignoreNoise = true;
			var limit = start + count - 1;
			var newStart = start;
			while (payload[newStart] != '$' || payload[newStart + 1] != '$')
			{
				newStart = Array.IndexOf(payload, '$', newStart, limit);
				if (newStart != -1) continue;
				detectedCount = count;
				return true;
			}

			detectedCount = newStart - start;
			return true;
		}

	    #endregion

	    #region Private Members

		#region Packet Parsers

		//TODO: tripid, VState, NewParaFlag, conditionData, gSensorData, totalTripMileage, totalTripFuelConsumption

		private IMessage ParseLogin(IFrame frame)
	    {
			//decode input
			var tripid = BitConverter.ToInt32(frame.Payload, DataStart);
			var rtctime = ParseRtcTime(frame.Payload, DataStart + 4);
			var privacyFunctionenabledFlag = BitHelper.AreBitsSet(frame.Payload[DataStart + 8], 0x01);
			var dataFlag = frame.Payload[DataStart + 8];
			var gpsDataFlag = BitHelper.AreBitsSet(dataFlag, 0x02);
			GPSPoint pos = null;
		    var offset = DataStart + 9;
			if (!privacyFunctionenabledFlag && gpsDataFlag)
			{
				pos = ParseGpsData(frame.Payload, offset);
				offset += 19;
			}
			else
			{
				var limit = 19;
				if (frame.Payload.Length - offset < 19) limit = frame.Payload.Length - offset;
				STrace.Debug(GetType().FullName, Id, String.Format("No hay datos de gps, Flag={0:X2} pfeFlag={1} GpsFlag={2} payload={3}", dataFlag, privacyFunctionenabledFlag, gpsDataFlag, StringUtils.ByteArrayToHexString(frame.Payload, offset, limit)));
			}
			var vState = ParseVehicleState(frame.Payload, offset);
			var newParaFlag = frame.Payload[offset + 4] == 0x01; //1 byte

			STrace.Debug(GetType().FullName, Id, String.Format("Login: tripid={0} VState.Length={1} NewParaFlag={2} payload={3}", tripid, vState.Length, newParaFlag, GetData(frame)));

			//encode Response
			var reply = FactoryPacket(
				PacketType.GsmLoginResponse,
				new byte[]
			    {
					0x17, 0x00, 0x00, 0x00,	//Fixed package sequence number
				    0xFF, 0xFF, 0xFF, 0xFF,	//IP address ("0xFFFFFFFF" represents invalid IP address.)
				    0xFF, 0xFF,				//port
				    0x00, 0x00              //Reserved
			    },
				frame.Payload);

		    return new ConfigRequest(Id, 0)
			    {
					Tiempo = rtctime,
                    GeoPoint = pos,
					Response = reply,
			    };
	    }

		private IMessage ParseRollCallResponse(IFrame frame)
	    {
			//decode input
			var privacyFunctionenabledFlag = BitHelper.AreBitsSet(frame.Payload[DataStart], 0x01);
			var dataFlag = frame.Payload[DataStart];
			var gpsDataFlag = BitHelper.AreBitsSet(frame.Payload[DataStart], 0x02);
			GPSPoint pos = null;
		    var offset = DataStart + 1;
			if (!privacyFunctionenabledFlag && gpsDataFlag)
			{
				pos = ParseGpsData(frame.Payload, offset);
				offset += 19;
			}
			else
			{
				var limit = 19;
				if (frame.Payload.Length - offset < 19) limit = frame.Payload.Length - offset;
				STrace.Debug(GetType().FullName, Id, String.Format("No hay datos de gps, Flag={0:X2} pfeFlag={1} GpsFlag={2} payload={3}", dataFlag, privacyFunctionenabledFlag, gpsDataFlag, StringUtils.ByteArrayToHexString(frame.Payload, offset, limit)));
			}
			var vState = ParseVehicleState(frame.Payload, offset);
			var newParaFlag = frame.Payload[offset + 4] == 0x01; //1 byte

			STrace.Debug(GetType().FullName, Id, String.Format("RollCallResponse: VState.Length={0} NewParaFlag={1} payload={2}", vState.Length, newParaFlag, GetData(frame)));

			return pos.ToPosition(Id, 0);
	    }

		private IMessage ParseLogout(IFrame frame)
	    {
			//decode input
			var rtctime = ParseRtcTime(frame.Payload, DataStart);
			var privacyFunctionenabledFlag = BitHelper.AreBitsSet(frame.Payload[DataStart + 4], 0x01);
			var dataFlag = frame.Payload[DataStart + 4];
			var gpsDataFlag = BitHelper.AreBitsSet(frame.Payload[DataStart + 4], 0x02);
			GPSPoint pos = null;
		    var offset = DataStart + 5;
			if (!privacyFunctionenabledFlag && gpsDataFlag)
			{
				pos = ParseGpsData(frame.Payload, offset);
				offset += 19;
			}
			else
			{
				var limit = 19;
				if (frame.Payload.Length - offset < 19) limit = frame.Payload.Length - offset;
				STrace.Debug(GetType().FullName, Id, String.Format("No hay datos de gps, Flag={0:X2} pfeFlag={1} GpsFlag={2} payload={3}", dataFlag, privacyFunctionenabledFlag, gpsDataFlag, StringUtils.ByteArrayToHexString(frame.Payload, offset, limit)));
			}
			var vState = ParseVehicleState(frame.Payload, offset);

			STrace.Debug(GetType().FullName, Id, String.Format("Logout: VState.Length={0} payload={1}", vState.Length, GetData(frame)));

			var res = pos.ToPosition(Id, 0);
			res.Tiempo = rtctime;
			return res;
	    }

		private IMessage ParseFixedUpload(IFrame frame)
	    {
			//decoding
			var tripid = BitConverter.ToInt32(frame.Payload, DataStart);
		    var fixedPackageSequenceNumber = BitConverter.ToUInt32(frame.Payload, DataStart + 4);
		    var rtctime = ParseRtcTime(frame.Payload, DataStart + 8);
			//var TripMileage = BitConverter.ToInt32(Frame.Payload, dataStart + 12); //en millas
			var conditionflag = frame.Payload[DataStart + 16] == 0x01;
			var dataFlag = frame.Payload[DataStart + 17];
			var privacyFunctionenabledFlag = BitHelper.AreBitsSet(frame.Payload[DataStart + 17], 0x01);
			var gpsDataFlag = BitHelper.AreBitsSet(frame.Payload[DataStart + 17], 0x02);
			var gSensorFlag = frame.Payload[DataStart + 18] == 0x01;
			var pidTypes = ParsePidTypes(frame.Payload, DataStart + 19);
			var offset = DataStart + 20 + pidTypes.Length * 2;
			var cdpLen = frame.Payload[offset]; //length of each group of condition data package every 2 seconds
			var gcgmpLen = frame.Payload[offset + 1]; //the number of GCG mixed package
			offset += 2;
			var pos = new GPSPoint[gcgmpLen];
			var conditionData = !conditionflag? null : new byte[gcgmpLen][][];
			var gSensorData = !gSensorFlag ? null : new byte[gcgmpLen][][];
			for (var i = 0; i < gcgmpLen; i++)
			{
				if (!privacyFunctionenabledFlag && gpsDataFlag)
				{
					pos[i] = ParseGpsData(frame.Payload, offset);
					offset += 19;
				}
				else
				{
					var limit = 19;
					if (frame.Payload.Length - offset < 19) limit = frame.Payload.Length - offset;
					STrace.Debug(GetType().FullName, Id, String.Format("No hay datos de gps, Flag={0:X2} pfeFlag={1} GpsFlag={2} payload={3}", dataFlag, privacyFunctionenabledFlag, gpsDataFlag, StringUtils.ByteArrayToHexString(frame.Payload, offset, limit)));
				}
				if (conditionflag)
				{
					conditionData[i] = ParseConditionData(frame.Payload, offset, cdpLen);
					offset += 5*cdpLen;
				}
				if (gSensorFlag)
				{
					gSensorData[i] = ParseGSensorData(frame.Payload, offset);
					offset += 15;
				}
			}
			var vState = ParseVehicleState(frame.Payload, offset);

			STrace.Debug(GetType().FullName, Id, String.Format("FixedUpload: tripid={0} VState.Length={1}", tripid, vState.Length));

			var res = pos.ToPosition(Id, fixedPackageSequenceNumber);
			res.Tiempo = rtctime;
			return res;
	    }

	    private IMessage ParseVehicleSupportedDataStreamType(IFrame frame)
	    {
			//decoding
			var tripid = BitConverter.ToInt32(frame.Payload, DataStart);
			var fixedPackageSequenceNumber = BitConverter.ToUInt32(frame.Payload, DataStart + 4);
		    var rtctime = ParseRtcTime(frame.Payload, DataStart + 8);
		    var count = frame.Payload[DataStart + 12];
			var pidTypes = ParsePidTypes(frame.Payload, DataStart + 13, count);

			STrace.Debug(GetType().FullName, Id, String.Format("VehicleSupportedDataStreamType: tripid={0} pidTypes.Length={1} payload={2}", tripid, pidTypes.Length, GetData(frame)));

			return new UserMessage(Id, fixedPackageSequenceNumber) { Tiempo = rtctime };
	    }

		private IMessage ParseUploadFreezeFrameDataStream(IFrame frame)
	    {
			//decoding
			var tripid = BitConverter.ToInt32(frame.Payload, DataStart);
			var fixedPackageSequenceNumber = BitConverter.ToUInt32(frame.Payload, DataStart + 4);
		    var rtctime = ParseRtcTime(frame.Payload, DataStart + 8);
			var piddata = ParsePidDataStream(frame.Payload, DataStart + 12);

			STrace.Debug(GetType().FullName, Id, String.Format("UploadFreezeFrameDataStream: tripid={0} piddata.Length={1} payload={2}", tripid, piddata == null ? 0 : piddata.Length, GetData(frame)));
			
			return new UserMessage(Id, fixedPackageSequenceNumber) { Tiempo = rtctime };
		}

		private IMessage ParseUploadDataStreamInfo(IFrame frame)
	    {
			//decoding
			var tripid = BitConverter.ToInt32(frame.Payload, DataStart);
			var fixedPackageSequenceNumber = BitConverter.ToUInt32(frame.Payload, DataStart + 4);
		    var rtctime = ParseRtcTime(frame.Payload, DataStart + 8);
		    //var len = BitConverter.ToInt16(Frame.Payload, dataStart + 12);
			var piddata = ParsePidDataStream(frame.Payload, DataStart + 14);

			STrace.Debug(GetType().FullName, Id, String.Format("UploadDataStreamInfo: tripid={0} piddata.Length={1} payload={2}", tripid, piddata == null ? 0 : piddata.Length, GetData(frame)));
			
			return new UserMessage(Id, fixedPackageSequenceNumber) { Tiempo = rtctime };
		}

	    private IMessage ParseUploadMalfunctionInfo(IFrame frame, PacketType packettype)
	    {
			//decoding
			var tripid = BitConverter.ToInt32(frame.Payload, DataStart);
			var fixedPackageSequenceNumber = BitConverter.ToUInt32(frame.Payload, DataStart + 4);
		    var rtctime = ParseRtcTime(frame.Payload, DataStart + 8);
		    var malfunctioncount = frame.Payload[DataStart + 12];
		    var malfunctionId = malfunctioncount == 0x00 ? 0x0000 : BitConverter.ToInt16(frame.Payload, 13);

			STrace.Debug(GetType().FullName, Id, String.Format("{0:g}: tripid={1} malfunctioncount={2} malfunctionId={3} payload={4}", packettype, tripid, malfunctioncount, malfunctionId, GetData(frame)));
			
			return new UserMessage(Id, fixedPackageSequenceNumber) { Tiempo = rtctime };
		}

		private IMessage ParseUploadAlarm(IFrame frame, PacketType packettype)
		{
			//decoding
			var tripid = BitConverter.ToInt32(frame.Payload, DataStart);
			var fixedPackageSequenceNumber = BitConverter.ToUInt32(frame.Payload, DataStart + 4);
			var rtctime = ParseRtcTime(frame.Payload, DataStart + 8);
			//var TripMileage = BitConverter.ToInt32(Frame.Payload, dataStart + 12); //en millas
			var dataFlag = frame.Payload[DataStart + 16];
			var privacyFunctionenabledFlag = BitHelper.AreBitsSet(frame.Payload[DataStart + 16], 0x01);
			var gpsDataFlag = BitHelper.AreBitsSet(frame.Payload[DataStart + 16], 0x02);
			var offset = DataStart + 17;
		    GPSPoint pos = null;
			if (!privacyFunctionenabledFlag && gpsDataFlag)
			{
				pos = ParseGpsData(frame.Payload, offset);
				offset += 19;
			}
			else
			{
				var limit = 19;
				if (frame.Payload.Length - offset < 19) limit = frame.Payload.Length - offset;
				STrace.Debug(GetType().FullName, Id, String.Format("No hay datos de gps, Flag={0:X2} pfeFlag={1} GpsFlag={2} payload={3}", dataFlag, privacyFunctionenabledFlag, gpsDataFlag, StringUtils.ByteArrayToHexString(frame.Payload, offset, limit)));
			}
			var vState = ParseVehicleState(frame.Payload, offset);
			var alarmIdentifier = BitConverter.ToInt32(frame.Payload, offset + 4);
			var alarmDetailedDescriptionField = BitConverter.ToInt32(frame.Payload, offset + 8);

			STrace.Debug(GetType().FullName, Id, String.Format("{0}: tripid={1} VState.Length={2} AlarmIdentifier={3:X} AlarmDetailedDescriptionField={4:X} payload={5}", packettype, tripid, vState.Length, alarmIdentifier, alarmDetailedDescriptionField, GetData(frame)));

			var res = pos.ToPosition(Id, fixedPackageSequenceNumber);
			res.Tiempo = rtctime;
			return res;
		}

		private IMessage ParseTripStart(IFrame frame)
		{
			//decoding
			var tripid = BitConverter.ToInt32(frame.Payload, DataStart);
			var fixedPackageSequenceNumber = BitConverter.ToUInt32(frame.Payload, DataStart + 4);
			var rtctime = ParseRtcTime(frame.Payload, DataStart + 8);
			var vehicleReadnessData = BitConverter.ToInt32(frame.Payload, DataStart + 12); //en millas
			var ecuProtocolType = (EcuProtocolType)frame.Payload[DataStart + 16];
			var dataFlag = frame.Payload[DataStart + 17];
			var privacyFunctionenabledFlag = BitHelper.AreBitsSet(frame.Payload[DataStart + 17], 0x01);
			var gpsDataFlag = BitHelper.AreBitsSet(frame.Payload[DataStart + 17], 0x02);
			var offset = DataStart + 18;
		    GPSPoint pos = null;
			if (!privacyFunctionenabledFlag && gpsDataFlag)
			{
				pos = ParseGpsData(frame.Payload, offset);
				offset += 19;
			}
			else
			{
				var limit = 19;
				if (frame.Payload.Length - offset < 19) limit = frame.Payload.Length - offset;
				STrace.Debug(GetType().FullName, Id, String.Format("No hay datos de gps, Flag={0:X2} pfeFlag={1} GpsFlag={2} payload={3}", dataFlag, privacyFunctionenabledFlag, gpsDataFlag, StringUtils.ByteArrayToHexString(frame.Payload, offset, limit)));
			}
			var vState = ParseVehicleState(frame.Payload, offset);

			STrace.Debug(GetType().FullName, Id, String.Format("TripStart: tripid={0} VehicleReadnessData={1} ecuProtocolType={2} VState.Length={3} payload={4}", tripid, vehicleReadnessData, ecuProtocolType, vState.Length, GetData(frame)));

			var res = pos.ToPosition(Id, fixedPackageSequenceNumber);
			res.Tiempo = rtctime;
			return res;
		}

		private IMessage ParseTripEnd(IFrame frame)
		{
			//decoding
			var tripid = BitConverter.ToInt32(frame.Payload, DataStart);
			var fixedPackageSequenceNumber = BitConverter.ToUInt32(frame.Payload, DataStart + 4);
			var rtctime = ParseRtcTime(frame.Payload, DataStart + 8);
			//var totalTripMileage = BitConverter.ToInt32(Frame.Payload, dataStart + 12); //en millas
			//var totalTripFuelConsumption = BitConverter.ToInt32(Frame.Payload, dataStart + 16);
			var dataFlag = frame.Payload[DataStart + 20];
			var privacyFunctionenabledFlag = BitHelper.AreBitsSet(frame.Payload[DataStart + 20], 0x01);
			var gpsDataFlag = BitHelper.AreBitsSet(frame.Payload[DataStart + 20], 0x02);
			var offset = DataStart + 18;
		    GPSPoint pos = null;
			if (!privacyFunctionenabledFlag && gpsDataFlag)
			{
				pos = ParseGpsData(frame.Payload, offset);
				offset += 19;
			}
			else
			{
				var limit = 19;
				if (frame.Payload.Length - offset < 19) limit = frame.Payload.Length - offset;
				STrace.Debug(GetType().FullName, Id, String.Format("No hay datos de gps, Flag={0:X2} pfeFlag={1} GpsFlag={2} payload={3}", dataFlag, privacyFunctionenabledFlag, gpsDataFlag, StringUtils.ByteArrayToHexString(frame.Payload, offset, limit)));
			}
			var vState = ParseVehicleState(frame.Payload, offset);

			STrace.Debug(GetType().FullName, Id, String.Format("TripEnd: tripid={0} VState.Length={1} payload={2}", tripid, vState.Length, GetData(frame)));

			var res = pos.ToPosition(Id, fixedPackageSequenceNumber);
			res.Tiempo = rtctime;
			return res;
		}

		#endregion

	    #region Data Parsers

	    private DateTime ParseRtcTime(byte[] ba, int offset)
	    {
		    return DateTimeUtils.FromUnixTimeStamp(BitConverter.ToInt32(ba, offset));
	    }

	    private GPSPoint ParseGpsData(byte[] ba, int offset)
	    {
		    var mark = ba[offset + 17];
		    var invalidQuality = (mark & 0x0C) == 0x00;
		    if (invalidQuality) return null;

		    var dt = new DateTime((DateTime.UtcNow.Year/100)*100 + ba[offset + 2], ba[offset + 1], ba[offset], ba[offset + 3], ba[offset + 4], ba[offset + 5], DateTimeKind.Utc);
			var lat = (float)(BitConverter.ToInt32(ba, offset + 6) / 3600000.0) * (!BitHelper.AreBitsSet(mark, 0x02) ? -1 : 1);
		    var lon = (float)(BitConverter.ToInt32(ba, offset + 10) / 3600000.0) * (!BitHelper.AreBitsSet(mark, 0x01) ? -1 : 1);
		    var vel = (BitConverter.ToInt16(ba, offset + 14)*36)/1000; // reporta centimetros por segundo, multiplico por 0.036 para pasar a kilometros por segundo
		    var dir = BitConverter.ToInt16(ba, offset + 16);

		    //var cantsat = (flags & 0xF0) >> 4;

		    var res = GPSPoint.Factory(dt, lat, lon, vel, dir, 0, 0);
			STrace.Debug(typeof(Parser).FullName, Id, String.Format("GPS: {0}", res));
		    return res;
	    }

	    private MessageIdentifier[] ParseVehicleState(byte[] ba, int offset)
	    {
		    var mis = new List<MessageIdentifier>();

		    if (BitHelper.AreBitsSet(ba[offset], 0x01)) mis.Add(MessageIdentifier.BateryLow);
		    if (BitHelper.AreBitsSet(ba[offset], 0x02)) mis.Add(MessageIdentifier.SensorLowerLimitExceeded);
		    if (BitHelper.AreBitsSet(ba[offset], 0x04)) mis.Add(MessageIdentifier.SpeedingTicketEnd);
		    if (BitHelper.AreBitsSet(ba[offset], 0x08)) mis.Add(MessageIdentifier.SensorUpperLimitExceeded);
		    if (BitHelper.AreBitsSet(ba[offset], 0x10)) mis.Add(MessageIdentifier.AccelerationEvent);
		    if (BitHelper.AreBitsSet(ba[offset], 0x20)) mis.Add(MessageIdentifier.DesaccelerationEvent);
		    if (BitHelper.AreBitsSet(ba[offset], 0x40)) mis.Add(MessageIdentifier.StoppedEvent);
		    if (BitHelper.AreBitsSet(ba[offset], 0x80)) mis.Add(MessageIdentifier.ExcessiveExhaustAlarm);

		    if (BitHelper.AreBitsSet(ba[offset + 1], 0x01)) mis.Add(MessageIdentifier.RpmTicketEnd);
		    if (BitHelper.AreBitsSet(ba[offset + 1], 0x02)) mis.Add(MessageIdentifier.DeviceTurnedOn);

		    return mis.ToArray();
	    }

	    private Int16[] ParsePidTypes(byte[] payload, int offset)
	    {
		    var pidTypeList = new List<Int16>();
		    var numberOfUploadedPidType = payload[offset];
		    for (var i = 0; i < numberOfUploadedPidType; i++)
		    {
			    pidTypeList.Add(BitConverter.ToInt16(payload, offset + 1 + i*2));
		    }
		    return pidTypeList.ToArray();
	    }

	    private byte[][] ParseConditionData(byte[] payload, int offset, byte cdpLen)
	    {
		    var list = new List<byte[]>();
		    for (var i = 0; i < 5; i++)
		    {
			    var nn = new byte[cdpLen];
			    Array.Copy(payload, offset + i*cdpLen, nn, 0, cdpLen);
			    list.Add(nn);
		    }
		    return list.ToArray();
	    }

		private byte[][] ParseGSensorData(byte[] payload, int offset)
		{
		    var list = new List<byte[]>();
		    for (var i = 0; i < 5; i++)
		    {
			    var nn = new byte[3];
			    Array.Copy(payload, offset + i*3, nn, 0, 3);
			    list.Add(nn);
		    }
		    return list.ToArray();
		}

		private Int16[] ParsePidTypes(byte[] payload, int offset, int count)
		{
		    var list = new List<Int16>();
		    for (var i = 0; i < count; i++)
		    {
			    list.Add(BitConverter.ToInt16(payload, offset + i*2));
		    }
		    return list.ToArray();
		}

	    private PidData[] ParsePidDataStream(byte[] payload, int _offset)
	    {
			var pidlenTable = new Dictionary<int, int>
				{
					{0x210C, 2},
					{0x2110, 2},
					{0x2121, 2},
					{0x2123, 2},
					{0x2124, 2},
					{0x2152, 2},
					{0x2142, 2},
				};

			var offset = _offset;
			var res = new List<PidData>();
			var count = payload[offset]; //total number of ECU supported PID types
		    offset++;
			for (var i = 0; i < count; i++)
			{
				var pid = BitConverter.ToInt16(payload, offset);
				if ((pid & 0x2100) != 0x2100)
				{
					STrace.Debug(typeof(Parser).FullName, Id, String.Format("ParsePidDataStream, pid desconocido: {0}", StringUtils.ByteArrayToHexString(payload, _offset, payload.Length - (_offset+4))));
					return null;
				}
				offset += 2;
				var arithmeticnumber = BitConverter.ToInt16(payload, offset);
				offset += 2;
				var datalen = pidlenTable.ContainsKey(pid) ? pidlenTable[pid] : 1;
				var data = new byte[datalen];
				offset += datalen;
				res.Add(new PidData(pid, arithmeticnumber, data));
			}

		    return res.ToArray();
	    }

		private class PidData
		{
			//public Int16 Pid;
			//public Int16 ArithmeticNumber;
			//public byte[] Data;

// ReSharper disable UnusedParameter.Local
			public PidData(Int16 pid, Int16 arithmeticnumber, byte[] data)
// ReSharper restore UnusedParameter.Local
			{
				//Pid = pid;
				//ArithmeticNumber = arithmeticnumber;
				//Data = data;
			}
		}

	    #endregion

		private byte[] FactoryPacket(PacketType type, byte[] data, byte[] sourcePacket)
	    {
		    //A   Header, 2 bytes, defined as @@, ASCII code
		    //B   Length, 2 bytes, indicates the data length from Header to Tail ([Header, Tail)), statistical length range is A,B,C,D,E,F,G 
		    //C   Device ID (OBDII product ID), 20 bytes, ASCII bytes. If length < 20 bytes, filled up with '\0'.
		    //D   Type, 2 bytes, high byte represents to main identifier and low byte represent to sub identifier.
		    //E   Data, random length.
		    //F   Checksum, 2 bytes, calculate from Header to Data excluding Checksum and Tail. Checksum algorithm is CRC Checksum Algorithm (see appendix), calculating range is A,B,C,D,E
		    //G   Tail, 2 bytes, defined as "\r\n".

		    var len = (Int16) (2 + 2 + 20 + 2 + data.Length + 2 + 2);
		    var res = new byte[len];

		    Encoding.ASCII.GetBytes("@@").CopyTo(res, 0);
			BitConverter.GetBytes(len).CopyTo(res, 2);
		    Array.Copy(sourcePacket, 4, res, 4, 20); //Device ID
			BitConverter_BigEndian.GetBytes((Int16)type).CopyTo(res, 24);
		    Array.Copy(data, 0, res, 26, data.Length);
			BitConverter.GetBytes(GetChecksum(res, len - 4)).CopyTo(res, len - 4);
		    Encoding.ASCII.GetBytes("\r\n").CopyTo(res, len - 2);
		    return res;
	    }

		private static String GetData(IFrame frame)
		{
			return StringUtils.ByteArrayToHexString(frame.Payload, DataStart, frame.Payload.Length - 30);
		}

		private const int DataStart = 26;

	    #region Type Enums

	    private enum PacketType
	    {
		    //Unknown = 0,

		    //Monitoring Commands
		    GsmLoginResponse = 0x1001,
		    //VehicleRoll_Call = 0x1002,
		    //MonitorWorkConditionData = 0x100F,
		    //ClearDtcData = 0x1010,
		    //ResetObdDevice = 0x1012,
		    //StartSelfDiagnosis_StopSelfDiagnosis = 0x1014,
			//UploadSelfDiagnosisInfoResponse = 0x1015,
			//TimeTracking = 0x1016,
			//RemoteSwitchLock = 0x1017,
			//RemoteMonitoring = 0x1018,
			//SecurityAlarmConfirmation = 0x1019,
			//RelieveSecurityAlarm = 0x1020,

		    //Configuring Commands
			//RestoreDefaultSetting = 0x1101,
			//ClearObdDataField = 0x1102,
			//SetAlarmMonitoring = 0x1103,
			//SetNetworkParameter = 0x1104,
			//SetFixedUploadParameters = 0x1105,
			//SetPowerSavingMode = 0x1106,
		    //...

		    //Reading Commands
		    //xxx = 0x12,
		    //...

		    //Remote Update Commands
			//StartUpdateRequest = 0x4122,
			//SendDeviceSoftwareUpdatePacket = 0x4123,

		    //Send A-GPS Data to Device (A-GPS: Assisted GPS Technology)
			//AGpsDataUpdate = 0x4185,
			//SendAGpsData = 0x4186,

		    //Send Programming Font Library Commands To Device (Centre Platform /COM support)
			//StartFontLibraryProgrammingRequest = 0x4142,
			//SendCode_TableDataPacket = 0x4143,

		    //Test Socket Connection
			//TestSocketConnection = 0x9000,

		    //Regular Data Upload
		    Login = 0x2001,
		    RollCallResponse = 0x2002,
		    Logout = 0x2003,
		    FixedUpload = 0x2004,
		    VehicleSupportedDataStreamType = 0x2005,
		    UploadFreezeFrameDataStream = 0x2006,
		    UploadDataStreamInfo = 0x2007,
		    UploadStoreMalfunctionInfo = 0x2008, //Upload STORE malfunction info (historic malfunction)
		    UploadPendingMalfunctionInfo = 0x2009, //Upload PENDING malfunction info (current malfunction)
		    UploadAlarm = 0x200A,
		    UploadAlarmElimination = 0x200B,
		    TripStart = 0x200C,
		    TripEnd = 0x200D,
			//UploadRealTimeWorkConditionData = 0x200E,
			//UploadMonitoringWorkConditionData = 0x200F,
			//ClearDtcDataResponse = 0x2010,
			//FixedUploadInSleepMode = 0x2011,
			//ResetObdDeviceRespond = 0x2012,
		    //...

		    //Setting Commands Response
		    //...

		    //Read Response Commands
		    //...

		    //Remote Update Commands From OBDIISMART
		    //...

		    //A-GPS Data From OBDIISMART (A-GPS: Assisted GPS Technology)
		    //...

		    //Programming Font Library Commands From OBDIISMART (Centre Platform/COM support)
		    //...

		    //Detect Socket connection response command
			//DetectionSocketConnectionResponseCommand = 0x9000,

		    //Error Processing Mechanism in Information Interaction
			//InformationInteractionErrorProcessing = 0x9090,

	    }

	    /*private enum CommunicationStateTable //OBDII SMART communication module status value list
	    {
		    NoError = 0x00,
		    ReceiveSettingCommandFromCentre = 0x10,
		    Searching = 0x7D,
		    Processing = 0x7E,
		    FrameLengthError = 0x80,
		    FrameLheckError = 0x81,
		    CrCheckError = 0x82, //CR (0x0D) check error
		    LfCheckError = 0x83, //LF(0x0A) check error
		    UnrecognizedOrWrongCommand = 0x84,
		    VehicleEcuDoesNotSupportCorrespondingFunction = 0x8C,
		    VehicleEcuHasNoAnswer = 0x8D,
		    ConditionDoesNotMatch = 0x8E,
		    ParameterError = 0x8F,
		    GpsPositionedUnsuccessfully = 0x90,
		    NoGpsConfigurationOrDisconnectingToGps = 0x91,
		    VehicleEcuDoesNotSupportFuelConsumptionStatistics = 0x92,
		    DoNotSupportDataStreamMonitoring = 0x93,
		    BufferOverflow = 0xFF,
	    }//*/

		private enum EcuProtocolType : byte
		{
// ReSharper disable UnusedMember.Local
			J1850_PWM = 0x01,
			J1850_VPW = 0x02,
			ISO9141_2 = 0x03,
			KWP_5BPS = 0x04,
			KWP_25MS = 0x05, 
			CAN11__500K = 0x06,
			CAN29__500K = 0x07,
			CAN11__250K = 0x08,
			CAN29__250K = 0x09,
// ReSharper restore UnusedMember.Local
		}

	    #endregion

		#region Checksum

		private UInt16 GetChecksum(byte[] src, int len)
		{
			return (UInt16)(Body(src, len) ^ FCS_START);
		}

		private bool CheckChecksum(byte[] src, int len)
		{
			return FCS_FINAL == Body(src, len-2);
		}

		private UInt16 Body(byte[] src, int len)
		{
			var crc = FCS_START;
			for (var i = 0; i < len; i++)
			{
				crc = (UInt16)(crc >> 8 ^ FCSTAB[(crc ^ src[i]) & 0x00FF]);
			}
			return crc;
		}

		private const UInt16 FCS_START = 0xFFFF;
		private const UInt16 FCS_FINAL = 0xF0B8;
		private readonly UInt16[] FCSTAB =
			{
				0x0000, 0x1189, 0x2312, 0x329B, 0x4624, 0x57AD, 0x6536, 0x74BF,
				0x8C48, 0x9DC1, 0xAF5A, 0xBED3, 0xCA6C, 0xDBE5, 0xE97E, 0xF8F7,
				0x1081, 0x0108, 0x3393, 0x221A, 0x56A5, 0x472C, 0x75B7, 0x643E,
				0x9CC9, 0x8D40, 0xBFDB, 0xAE52, 0xDAED, 0xCB64, 0xF9FF, 0xE876,
				0x2102, 0x308B, 0x0210, 0x1399, 0x6726, 0x76AF, 0x4434, 0x55BD,
				0xAD4A, 0xBCC3, 0x8E58, 0x9FD1, 0xEB6E, 0xFAE7, 0xC87C, 0xD9F5,
				0x3183, 0x200A, 0x1291, 0x0318, 0x77A7, 0x662E, 0x54B5, 0x453C,
				0xBDCB, 0xAC42, 0x9ED9, 0x8F50, 0xFBEF, 0xEA66, 0xD8FD, 0xC974,
				0x4204, 0x538D, 0x6116, 0x709F, 0x0420, 0x15A9, 0x2732, 0x36BB,
				0xCE4C, 0xDFC5, 0xED5E, 0xFCD7, 0x8868, 0x99E1, 0xAB7A, 0xBAF3,
				0x5285, 0x430C, 0x7197, 0x601E, 0x14A1, 0x0528, 0x37B3, 0x263A,
				0xDECD, 0xCF44, 0xFDDF, 0xEC56, 0x98E9, 0x8960, 0xBBFB, 0xAA72,
				0x6306, 0x728F, 0x4014, 0x519D, 0x2522, 0x34AB, 0x0630, 0x17B9,
				0xEF4E, 0xFEC7, 0xCC5C, 0xDDD5, 0xA96A, 0xB8E3, 0x8A78, 0x9BF1,
				0x7387, 0x620E, 0x5095, 0x411C, 0x35A3, 0x242A, 0x16B1, 0x0738,
				0xFFCF, 0xEE46, 0xDCDD, 0xCD54, 0xB9EB, 0xA862, 0x9AF9, 0x8B70,
				0x8408, 0x9581, 0xA71A, 0xB693, 0xC22C, 0xD3A5, 0xE13E, 0xF0B7,
				0x0840, 0x19C9, 0x2B52, 0x3ADB, 0x4E64, 0x5FED, 0x6D76, 0x7CFF,
				0x9489, 0x8500, 0xB79B, 0xA612, 0xD2AD, 0xC324, 0xF1BF, 0xE036,
				0x18C1, 0x0948, 0x3BD3, 0x2A5A, 0x5EE5, 0x4F6C, 0x7DF7, 0x6C7E,
				0xA50A, 0xB483, 0x8618, 0x9791, 0xE32E, 0xF2A7, 0xC03C, 0xD1B5,
				0x2942, 0x38CB, 0x0A50, 0x1BD9, 0x6F66, 0x7EEF, 0x4C74, 0x5DFD,
				0xB58B, 0xA402, 0x9699, 0x8710, 0xF3AF, 0xE226, 0xD0BD, 0xC134,
				0x39C3, 0x284A, 0x1AD1, 0x0B58, 0x7FE7, 0x6E6E, 0x5CF5, 0x4D7C,
				0xC60C, 0xD785, 0xE51E, 0xF497, 0x8028, 0x91A1, 0xA33A, 0xB2B3,
				0x4A44, 0x5BCD, 0x6956, 0x78DF, 0x0C60, 0x1DE9, 0x2F72, 0x3EFB,
				0xD68D, 0xC704, 0xF59F, 0xE416, 0x90A9, 0x8120, 0xB3BB, 0xA232,
				0x5AC5, 0x4B4C, 0x79D7, 0x685E, 0x1CE1, 0x0D68, 0x3FF3, 0x2E7A,
				0xE70E, 0xF687, 0xC41C, 0xD595, 0xA12A, 0xB0A3, 0x8238, 0x93B1,
				0x6B46, 0x7ACF, 0x4854, 0x59DD, 0x2D62, 0x3CEB, 0x0E70, 0x1FF9,
				0xF78F, 0xE606, 0xD49D, 0xC514, 0xB1AB, 0xA022, 0x92B9, 0x8330,
				0x7BC7, 0x6A4E, 0x58D5, 0x495C, 0x3DE3, 0x2C6A, 0x1EF1, 0x0F78,
			};

		#endregion

		#endregion
	}
}