using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DamienG.Security.Cryptography;
using Logictracker.AVL.Messages;
using Logictracker.Cache;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Layers;
using Logictracker.Layers.DeviceCommandCodecs;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Utils;

namespace Logictracker.Trax.v1
{
    public static class GarminFmi
    {
        #region Garmin

        private const String CodeStr = "code=";

        #region MessageFactory

        private static string GetHexAckFor(byte packetId)
        {
            var ack = new byte[] { PacketIdCode.Ack, 0x02, packetId, 0x00};

            return BitConverter.ToString(ack).Replace("-", string.Empty);
        }

        #endregion MessageFactory

        #region Encode

        public static byte[]  EncodeDriverIdReceipt(UInt32 id)
        {
            return FactoryGarminFmi(
                BitConverter.GetBytes(FmiPacketId.DriverIdReceipt),
                BitConverter.GetBytes(id), new byte[] {1, 0, 0, 0});
        }

        public static byte[] EncodeStopStatusReceipt(UInt32 id)
        {

            return FactoryGarminFmi(
                BitConverter.GetBytes(FmiPacketId.ScStopStatusReceipt),
                BitConverter.GetBytes(id));
        }

        public static byte[] EncodeEtaDataReceipt(UInt32 id)
        {
            return FactoryGarminFmi(
                BitConverter.GetBytes(FmiPacketId.ScEtaDataReceipt),
                BitConverter.GetBytes(id));
        }


        public static byte[] EncodeMessageThrottlingControl(UInt16 packetId, bool newState)
        {
            return FactoryGarminFmi(BitConverter.GetBytes(FmiPacketId.ScMessageThrottlingCommand),
                                    BitConverter.GetBytes(packetId), BitConverter.GetBytes(newState ? (UInt16) 1 : (UInt16) 0));
        }

        private static byte[] RefactorArrayTo(byte[] src, int bytez)
        {
            var filledArr = new byte[bytez];
            Array.Copy(src, 0, filledArr, 0, (src.Length < bytez ? src.Length : bytez));
            return filledArr;
        }

        public static byte[] EncodeEnableFMI(bool unicodeSupport, bool A607Support, bool DriverPasswords, bool MultipleDrivers, bool AOBRD)
        {
            return FactoryGarminFmi(BitConverter.GetBytes(FmiPacketId.ScEnableFleetManagementProtocolRequest), new byte[] {0x05, 0x00, (byte) (!unicodeSupport?0x80:0x00),0x01, (byte) (!A607Support?0x80:0x00),0x02, (byte) (!DriverPasswords?0x80:0x00),0x0A, (byte) (!MultipleDrivers?0x80:0x00),0x0B, (byte) (!AOBRD?0x80:0x00),0x0C}); 
        }

        public static byte[] EncodeAutoArrival(UInt32? stopTime, UInt32? stopDistance)
        {
            return FactoryGarminFmi(BitConverter.GetBytes(FmiPacketId.ScAutoArrival),
                                    (stopTime != null
                                         ? BitConverter.GetBytes(stopTime.Value)
                                         : BitConverter.GetBytes(0xFFFFFFFF)),
                                    (stopDistance != null
                                         ? BitConverter.GetBytes(stopDistance.Value)
                                         : BitConverter.GetBytes(0xFFFFFFFF)));
        }

        public static byte[] EncodeUserInterfaceText(String str)
        {
            return FactoryGarminFmi(BitConverter.GetBytes(FmiPacketId.ScUserInterfaceText),
                                    BitConverter.GetBytes((Int32) 0), Encoding.ASCII.GetBytes(str), new byte[] { 0 } );
        }


        public static byte[] EncodeDataDeletionFor(DataDeletionProtocolId data)
        {
            return FactoryGarminFmi(BitConverter.GetBytes(FmiPacketId.ScDataDeletion), BitConverter.GetBytes((uint) data));
        }


        /// <summary>
        /// 	codifica un string a formato mensaje garmin
        /// </summary>
        /// <param name = "id">identificador para referenciar el id, si id es 0 entonces el mensaje no tiene id</param>
        /// <param name = "text"></param>
        /// <param name = "showImmediately"></param>
        /// <returns></returns>
        public static byte[] EncodeOpenTextMessage(UInt32 id, String text, bool showImmediately)
        {
            if (String.IsNullOrEmpty(text))
                throw new NullReferenceException(
                    "Garmin/FMI, el texto de la interface no puede estar vacio ni ser nulo.");
            if (text.Length > 199)
                throw new ApplicationException(
                    "Garmin/FMI, el texto de la interface no puede tener mas de 199 caracteres.");

            byte[] _id = BitConverter.GetBytes(id);
            byte[] pId = RefactorArrayTo(_id, 16);
            byte[] pPacketId = BitConverter.GetBytes(FmiPacketId.ScTextMessageA604OpenServer2Client);
            byte[] pDt = GetDateTimeGarminEncoded();
            var pIdSize = new[] {(byte) _id.Length};
            var pMessageType = new[] {(byte) (showImmediately ? 1 : 0)};
            byte[] pTextMessage = Encoding.ASCII.GetBytes(text);

            return FactoryGarminFmi(
                pPacketId,
                pDt,
                pIdSize,
                pMessageType,
                new byte[] {0, 0},
                pId,
                pTextMessage,
                new byte[] {0}
                );
        }

        public static byte[] EncodeOpenLongTextMessage(UInt32 id, String text, bool showImmediately)
        {
            if (String.IsNullOrEmpty(text))
                throw new NullReferenceException(
                    "Garmin/FMI, el texto de la interface no puede estar vacio ni ser nulo.");
            if (text.Length > 1999)
                throw new ApplicationException(
                    "Garmin/FMI, el texto de la interface no puede tener mas de 1999 caracteres.");

            byte[] _id = BitConverter.GetBytes(id);
            byte[] pId = RefactorArrayTo(_id, 16);
            byte[] pPacketId = BitConverter.GetBytes(FmiPacketId.ScLongTextMessageA611LongServer2Client);
            byte[] pDt = GetDateTimeGarminEncoded();
            var pIdSize = new[] { (byte)_id.Length };
            var pMessageType = new[] { (byte)(showImmediately ? 1 : 0) };
            byte[] pTextMessage = Encoding.ASCII.GetBytes(text);

            return FactoryGarminFmi(
                pPacketId,
                pDt,
                pIdSize,
                pMessageType,
                new byte[] { 0, 0 },
                pId,
                pTextMessage,
                new byte[] { 0 }
                );
        }

        public static byte[] EncodePing()
        {
            byte[] packetId = BitConverter.GetBytes(FmiPacketId.PingCommunicationStatus);
            return FactoryGarminFmi(packetId);
        }

        public static byte[] EncodeTextMessageAckReceipt(UInt32 id)
        {
            var _pId = BitConverter.GetBytes(id);
            var pId = RefactorArrayTo(_pId, 16);
            var pCode = BitConverter.GetBytes(FmiPacketId.ScTextMessageAckReceipt);

            return FactoryGarminFmi(pCode, new byte[] {(byte) _pId.Length, 0, 0, 0}, pId);
        }

        public static byte[] EncodeDeleteTextmessage(UInt32 id)
        {
            byte[] _id = BitConverter.GetBytes(id);
            byte[] pId = RefactorArrayTo(_id, 16);
            byte[] pPacketId = BitConverter.GetBytes(FmiPacketId.ScDeleteTextMessage);
            var pIdSize = new[] {(byte) _id.Length};

            return FactoryGarminFmi(
                pPacketId,
                pIdSize,
                new byte[] {0, 0, 0},
                pId);
        }

        public static byte[] EncodeCannedResponseList(UInt32 id, uint[] responses)
        {
            int respCount = responses.Length;

            byte[] _id = BitConverter.GetBytes(id);
            byte[] pId = RefactorArrayTo(_id, 16);
            byte[] pPacketId = BitConverter.GetBytes(FmiPacketId.ScSetCannedResponseList);
            var pIdSize = new[] {(byte) _id.Length};
            var pRespCount = new[] {(byte) respCount};


            var allResponses = new List<byte>();
            foreach (var bArr in responses.Select(r => BitConverter.GetBytes(r)))
            {
                allResponses.AddRange(bArr);
            }

            return FactoryGarminFmi(
                pPacketId,
                pIdSize,
                pRespCount,
                new byte[] {0, 0},
                pId,
                allResponses.ToArray()
                );
        }

        public static byte[] EncodeSetCanned(int id, String text, UInt16 code)
        {
            if (String.IsNullOrEmpty(text))
                throw new NullReferenceException(
                    "Garmin/FMI, el texto de la interface no puede estar vacio ni ser nulo.");

            if (text.Length > 49)
                throw new ApplicationException("Garmin/FMI, el texto de la interface es muy largo.");

            byte[] pId = BitConverter.GetBytes(id);
            byte[] pCode = BitConverter.GetBytes(code);
            byte[] pText = Encoding.ASCII.GetBytes(text);

            return FactoryGarminFmi(
                pCode,
                pId,
                pText,
                new byte[] {0}
                );
        }

        public static byte[] EncodeDeleteCanned(int id, UInt16 code)
        {
            byte[] pId = BitConverter.GetBytes(id);
            byte[] pCode = BitConverter.GetBytes(code);

            return FactoryGarminFmi(
                pCode,
                pId
                );
        }

        public static byte[] EncodeA603StopProtocol(GPSPoint pos, int id, String text)
        {
            if (text.Length > 199)
                throw new ApplicationException("Garmin/FMI, el texto de la interface es muy largo.");
           
            var result = FactoryGarminFmi(
                BitConverter.GetBytes(FmiPacketId.ScA603Stop),
                GetDateTimeGarminEncoded(),
                DegreesToSc(pos.Lat),
                DegreesToSc(pos.Lon),
                BitConverter.GetBytes(id),
                Encoding.ASCII.GetBytes(text),
                new byte[] {0}
                );            

            return result;
        }

        public static byte[] EncodeA603StopStatusProtocol(int id, StopStatusValue stopStatus)
        {
            return EncodeA603StopStatusProtocol(id, stopStatus, 9999);
        }

        public static byte[] EncodeA603StopStatusProtocol(int id, StopStatusValue stopStatus, UInt16 stopIndexInList)
        {
            return FactoryGarminFmi(
                BitConverter.GetBytes(FmiPacketId.ScStopStatusRequest),
                BitConverter.GetBytes(id),
                BitConverter.GetBytes((ushort) stopStatus),
                (stopIndexInList == 9999 ? BitConverter.GetBytes(0xFFFF) : BitConverter.GetBytes(stopIndexInList)),
                new byte[] {0}
                );
        }


        public static byte[] EncodeSortStopListProtocol()
        {
            return FactoryGarminFmi(
                BitConverter.GetBytes(FmiPacketId.ScSortStopList)
                );
        }

        public static byte[] EncodeA602StopProtocol(GPSPoint pos, String text)
        {
            if (text.Length > 199)
                throw new ApplicationException("Garmin/FMI, el texto de la interface es muy largo.");

            return FactoryGarminFmi(
                BitConverter.GetBytes(FmiPacketId.ScA602Stop),
                GetDateTimeGarminEncoded(),
                DegreesToSc(pos.Lat),
                DegreesToSc(pos.Lon),
                Encoding.ASCII.GetBytes(text),
                new byte[] {0}
                );
        }

        public static byte[] EncodeServerToClientDriverIdUpdateProtocolA607(UInt32 changeId, String driver)
        {
            return FactoryGarminFmi(
                BitConverter.GetBytes(FmiPacketId.ScA607DriverIdUpdate),
                BitConverter.GetBytes(changeId),
                GetDateTimeGarminEncoded(),
                new byte[] {0}, // driver_idx solo un driver por el momento
                new byte[] {0, 0, 0},
                Encoding.ASCII.GetBytes(driver),
                new byte[] {0},
                new byte[] {0} // password, nada por el momento
                );
        }

        #endregion

        #region Funciones de Utilidad

        private static readonly Double Deg2Sc = Math.Pow(2, 31)/180.0;

        /// <summary>
        /// toma una serie de parametros en byte array en formato garmin y los envuelve en un paquete fmi listo para enviar
        /// </summary>
        /// <param name="args">el primer byte array debe ser un solo byte con el largo del payload</param>
        /// <returns>paquete fmi listo para enviar</returns>
        public static byte[] FactoryGarminFmi(params byte[][] args)
        {
            var bytes = new List<byte>();
            bytes.AddRange(PacketParts.FmiHeader);
            bytes.Add(0x00); // Size of Packet Data

            foreach (var param in args)
            {
                bytes[2] += (byte) param.Length;
                bytes.AddRange(param);
            }
            bytes.Add(Calculate8BitChecksum(bytes.Skip(1).ToArray()));
            bytes.AddRange(PacketParts.Trail);
            return bytes.ToArray();
        }

        private static byte[] GetDateTimeGarminEncoded()
        {
            var ini = new DateTime(1989, 12, 31, 0, 0, 0, DateTimeKind.Utc); //(12:00 am December 31, 1989 utc) ?
            return BitConverter.GetBytes(Convert.ToUInt32((DateTime.UtcNow - ini).TotalSeconds));
        }

        private static DateTime ToDateTime(this byte[] val, int index)
        {
            return new DateTime(1989, 12, 31, 0, 0, 0, DateTimeKind.Utc).AddSeconds(BitConverter.ToInt32(val, index));
            //(12:00 am December 31, 1989 utc) ?
        }

        private static String GetString(this byte[] text, int index)
        {
            try
            {
                return Encoding.ASCII.GetString(text, index, Array.IndexOf(text, (byte) 0, index) + 1 - index);
            }
            catch
            {
                return "0000000000";
            }
        }

        private static String GetRfidFromDriverId(String driverId)
        {
            //por el momento viene directamente el rfid, si se quiere hacer mapeo dejo esto preparado
            return driverId;
        }

        private static readonly Double sc2deg = 180.0 / Math.Pow(2, 31);

        private static byte[] DegreesToSc(Double deg)
        {
            return BitConverter.GetBytes(Convert.ToInt32(deg*Deg2Sc));
        }

        private static Double ScToDegrees(this byte[] text, int index)
		{
			return BitConverter.ToInt32(text, index) * sc2deg;
		}

/*
        device_id_ETA_ReceivedAt
        device_id_ETA_Estimated
        device_id_ETA_DistanceTo
*/
        private static String makeIDFor(Int32 Id, String keySuffix)
        {
            return String.Format("device_{0}_{1}", Id, keySuffix);
        }

        private static void setETAReceivedAt(Int32 Id, DateTime? value)
        {
            var key = makeIDFor(Id, "ETA_ReceivedAt");
            if (value == null)
                LogicCache.Delete(key);
            else
                LogicCache.Store(typeof(String), key, value.Value.ToString("O"));
        }

        private static void setETAEstimated(Int32 Id, DateTime? value)
        {
            var key = makeIDFor(Id, "ETA_Estimated");
            if (value == null)
                LogicCache.Delete(key);
            else
                LogicCache.Store(typeof(String), key, value.Value.ToString("O"));
        }

        private static void setETADistanceTo(Int32 Id, UInt32? value)
        {
            var key = makeIDFor(Id, "ETA_DistanceTo");
            if (value == null)
                LogicCache.Delete(key);
            else
                LogicCache.Store(typeof(String), key, value.Value.ToString());
        }

        #endregion

        #region Decode

        private static IMessage DecodeGarminFmi(byte[] text, ulong msgId, Parser node)
        {
            byte packetId = text[1];
            //byte packetSize = text[2];
            ulong msgid = 0;

            //bool needsAck = true;

            var response = "";
            switch (packetId)
            {
                case PacketIdCode.Ack:
                    byte packetIdAcked = text[3];
                    STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                 "GARMIN ACK FOR PACKET ID: 0x" + StringUtils.ByteToHexString(packetIdAcked));
                    //needsAck = false;
                    break;
                case PacketIdCode.UnitIdEsn:
                    var unitIdEsn = BitConverter.ToUInt32(text, 3);
                    var otherStuff = BitConverter.ToUInt32(text, 7);
                    STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                 "GARMIN PACKET ID: UnitId/ESN (Command Response)");
                    STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                 "GARMIN UNIT ID/ESN: " + unitIdEsn.ToString("#0"));
                    STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                 "GARMIN OTHER STUFF: " + otherStuff.ToString("#0"));
                    break;
                case PacketIdCode.DateTimeData:
                    STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(), "GARMIN PACKET ID: DateTime Data");
                    break;
                case PacketIdCode.FleetManagementPacket:
                    ushort fmiPacketId = BitConverter.ToUInt16(text, 3);
                    string hexFmiPacketId = StringUtils.UInt16ToHexString(fmiPacketId);

                    STrace.Debug("GarminTalk", node.Id, String.Format("RX: {0}", BitConverter.ToString(text)));

                    switch (fmiPacketId)
                    {
                        case FmiPacketId.CsProductIdData:
                        case FmiPacketId.CsProductSupportData:
                        case FmiPacketId.CsUnicodeSupportRequest:
                            STrace.Debug(typeof (GarminFmi).FullName, node.Id,
                                         "La función FMI Packet ID " + hexFmiPacketId + " no esta implementada.");
                            break;
                        case FmiPacketId.CsTextMessageAcknowledgement:
                            // when a canned response list text message is replied with a response, this acknowledgement is received with the response selected.
                            {
                                var dt = text.ToDateTime(5);
                                var idSize = Convert.ToByte(text[9]);
                                var id = BitConverter.ToUInt32(text, 13);
                                var responseId = BitConverter.ToUInt32(text, 29);

                                STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                             String.Format("GarminTextMessageCannedResponse: {0}",
                                                           BitConverter.ToString(text)));
                                STrace.Debug(typeof(GarminFmi).FullName, node.GetDeviceId(),
                                             String.Format(
                                                 "GarminTextMessageCannedResponse: dt={0}, IdSize={1}, Id={2}, ResponseId={3}",
                                                 dt, idSize, id, responseId));
                                STrace.Debug(typeof(GarminFmi).FullName, node.GetDeviceId(),
                                             String.Format("GarminTextMessageCannedResponse: {0}", response));

                                var salida = (IMessage) MessageIdentifier.GarminTextMessageCannedResponse
                                    .FactoryEvent(node.Id, (UInt32) msgid, null, DateTime.UtcNow, null,
                                                  new List<Int64> {node.GetDeviceId(), id, responseId});


                                salida.AddStringToSend(EncodeTextMessageAckReceipt(id).ToTraxFM(node).ToString(true));

                                var bcdArr = new[] { EncodeDeleteTextmessage(id).ToTraxFM(node) };
                                STrace.Debug(typeof(GarminFmi).FullName, node.Id, "StopResponse: Stop ID=" + Convert.ToString(id) + " - " + EncodeDeleteTextmessage(id).ToTraxFM(node, false));
                                Fota.EnqueueOnTheFly(node, 0, bcdArr, ref salida);

                                return salida;
                            }
                        case FmiPacketId.CsTextMessageOpenClient2Server:
                            {
                                msgid = node.NextSequence; // por si hace falta lo tengo a mano
                                var dt = text.ToDateTime(5);
                                var strText1 = Encoding.ASCII.GetString(text, 13, text[2] - 11);
                                var strText2 = StringUtils.ByteArrayToHexString(text, 9, 4);
                                STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                             "GARMIN A607 Client to Server Text Message (" + hexFmiPacketId + "): ID=" +
                                             msgid + ", Text=" + strText1);

                                return FactoryEvento(node, msgid, strText1, strText2, dt);
                            }

                        case FmiPacketId.CsA607Client2ServerTextMessage:
                            {
                                //Parser.EnsureMessagingDeviceIsGarmin(node);
                                var dt = text.ToDateTime(5);
                                msgid = BitConverter.ToUInt32(text, 17);
                                var strText1 = Encoding.ASCII.GetString(text, 41, text[2] - 39);
                                var strText2 = StringUtils.ByteArrayToHexString(text, 17, 4);
                                STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                             "GARMIN A607 Client to Server Text Message (" + hexFmiPacketId + "): ID=" +
                                             msgid + ", Text=" + strText1);

                                return FactoryEvento(node, msgid, strText1, strText2, dt);
                            }
                        case FmiPacketId.CsCannedResponseListReceipt:
                            //STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(), "GARMIN PACKET ID: CANNED_RESPONSE_LIST_RECEIPT (NOT IMPLEMENTED)");
                            STrace.Trace(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                         "GARMIN PACKET ID: CANNED_RESPONSE_LIST_RECEIPT:" +
                                         StringUtils.ByteArrayToHexString(text, 0, text.Length));
                            break;
                        case FmiPacketId.CsTextMessageReceiptA604OpenServer2Client:
                            {
                                Parser.EnsureMessagingDeviceIsGarmin(node);
                                var a604TmoprDatetime = text.ToDateTime(5);
                                var a604TmoprIdsize = (int) text[9];
                                var a604TmoprResultCode = (ushort) (text[10] == 0 ? 0 : 1);
                                var a604TmoprUid = (ulong) 0;
                                if (a604TmoprIdsize > 0) a604TmoprUid = BitConverter.ToUInt32(text, 13);
                                return FactoryEvento(node, packetId, a604TmoprUid, a604TmoprDatetime,
                                                     a604TmoprResultCode);
                            }
                        case FmiPacketId.CsSetCannedResponseReceipt:
                        case FmiPacketId.CsDeleteCannedResponseReceipt:
                            break;
                        case FmiPacketId.CsRequestCannedResponseListRefresh:
//                            node.ReloadMessages(0);
                            break;
                        case FmiPacketId.CsTextMessageStatus:
                            break;
                        case FmiPacketId.CsSetCannedMessageReceipt:
                            // Sucede cuando se envian los mensajes predeterminados
/*                            STrace.Debug(typeof(GarminFmi).FullName, node.Id, "La función FMI Packet ID " + hexFMIPacketId +
                               " no esta implementada."); */
                            break;
                        case FmiPacketId.CsDeleteCannedMessageReceipt:
                            // Sucede cuando se envian los mensajes predeterminados y se borran los mensajes no utilizados
/*                            STrace.Debug(typeof(GarminFmi).FullName, node.Id, "La función FMI Packet ID " + hexFMIPacketId +
                               " no esta implementada."); */
                            break;
                        case FmiPacketId.CsRefreshCannedMessageList:
                        case FmiPacketId.CsLongTextMessageReceiptA611LongServer2Client:
                        case FmiPacketId.CsSortStopListAcknowledgement:
                        case FmiPacketId.CsCreateWaypointReceipt:
                        case FmiPacketId.WaypointDeleted:
                        case FmiPacketId.CsDeleteWaypointByCategoryReceipt:
                        case FmiPacketId.CsCreateWaypointByCategoryReceipt:
                            STrace.Debug(typeof (GarminFmi).FullName, node.Id,
                                         "La función FMI Packet ID " + hexFmiPacketId + " no esta implementada.");
                            break;
                        case FmiPacketId.CsEtaData:
                            {

                                msgid = BitConverter.ToUInt32(text, 5);
                                var dt = text.ToDateTime(9);
                                var distanceTo = BitConverter.ToUInt32(text, 13);
                                var pos_lat = text.ScToDegrees(15);
                                var pos_long = text.ScToDegrees(17);
                                STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                    String.Format("GARMIN ETA DATA ({0}): ID={1}, LAT/LONG={2}/{3}, ETA={4}", hexFmiPacketId, msgid, pos_lat, pos_long,
                                        dt.ToString()));

                                setETAReceivedAt(node.Id, DateTime.UtcNow);
                                setETAEstimated(node.Id, dt);
                                setETADistanceTo(node.Id, distanceTo);

                                var salida = (IMessage) MessageIdentifier.GarminETA.FactoryEvent(node.Id, 0, null, DateTime.UtcNow, null, null);
                                salida.AddStringToSend(EncodeEtaDataReceipt((UInt32) msgid).ToTraxFM(node).ToString(true));

                                return salida;
                            }
                        case FmiPacketId.CsStopStatus:
                            {
                                var uid = BitConverter.ToUInt32(text, 5);
                                var ss = BitConverter.ToUInt16(text, 9);
                                STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                             "GARMIN STOP STATUS (0x" + hexFmiPacketId + "): ID=" + uid);

                                var salida = (IMessage) TranslateStopStatus((StopStatusValue) ss, node.Id)
                                    .FactoryEvent(node.Id, 0, null, DateTime.UtcNow, null, new List<Int64> {(int) uid});

                                STrace.Debug(typeof(GarminFmi).FullName, node.Id, "StopStatus: Stop ID=" + Convert.ToString(uid) + " - " + EncodeStopStatusReceipt(uid).ToTraxFM(node, false));

                                salida.AddStringToSend(EncodeStopStatusReceipt(uid).ToTraxFM(node).ToString(true));

                                return salida;
                            }
                        case FmiPacketId.CsUserInterfaceTextReceipt:
                            STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                         "GARMIN USER INTERFACE TEXT RECEIPT (0x" + hexFmiPacketId + ")");
                            // NOTE:    everytime UserInterfaceText is changed, 
                            //          we receive this event and the gateway doesnt expect to do nothing by now.
                            break;
                        case FmiPacketId.CsMessageThrottlingResponse:
                            break;
                        case FmiPacketId.CsMessageThrottlingQueryResponse:
                        case FmiPacketId.PingCommunicationStatus:
                            STrace.Debug(typeof(GarminFmi).FullName, node.Id, "La función FMI Packet ID " + hexFmiPacketId + " no esta implementada.");
                            //needsAck = false;
                            break;
                        case FmiPacketId.PingCommunicationStatusResponse:
                            node.IsGarminConnected = true;
                            STrace.Debug(typeof(GarminFmi).FullName, node.GetDeviceId(), "GARMIN PONG received");
                            //needsAck = false;
                            break;
                        case FmiPacketId.CsGpiFileStartReceipt:
                        case FmiPacketId.CsGpiPacketReceipt:
                        case FmiPacketId.CsGpiFileEndReceipt:
                        case FmiPacketId.CsGpiFileInformation:
                        case FmiPacketId.CsSetDriverStatusListItemReceipt:
                        case FmiPacketId.CsDeleteDriverStatusListItemReceipt:
                        case FmiPacketId.CsDriverStatusListRefresh:
                            STrace.Debug(typeof(GarminFmi).FullName, node.Id, "La función FMI Packet ID " + hexFmiPacketId + " no esta implementada.");
                            break;
                        case FmiPacketId.DriverIdUpdate:
                            {
                                msgid = node.NextSequence;
                                var changeId = BitConverter.ToUInt32(text, 5);
                                var changeTime = text.ToDateTime(9);
                                var driverId = text.GetString(13);
                                var rfid = GetRfidFromDriverId(driverId);
                                STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                             "GARMIN DRIVER ID UPDATE (" + hexFmiPacketId + "): ID=" + driverId +
                                             ", RFID=" + rfid);
                                

                                var salida = (IMessage) MessageIdentifierX.FactoryRfid(node.Id, msgid, null, changeTime, rfid, 0);

                                salida.AddStringToSend(EncodeDriverIdReceipt(changeId).ToTraxFM(node).ToString(true));

                                return salida;
                            }
                        case FmiPacketId.DriverIdReceipt:
                            STrace.Debug(typeof(GarminFmi).FullName, node.Id, "La función FMI Packet ID " + hexFmiPacketId + " no esta implementada.");
                            break;
                        case FmiPacketId.ScA607DriverIdUpdate:
                            {
                                msgid = node.NextSequence;
                                var changeId = BitConverter.ToUInt32(text, 5);
                                var changeTime = text.ToDateTime(9);
                                var driverId = text.GetString(17);
                                var rfid = GetRfidFromDriverId(driverId);

                                STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                                             "GARMIN DRIVER ID UPDATE (" + hexFmiPacketId + "): ID=" + driverId +
                                             ", RFID=" + rfid);


                                var salida = (IMessage) MessageIdentifierX.FactoryRfid(node.Id, msgid, null, changeTime, rfid, 0);
                                salida.AddStringToSend(EncodeDriverIdReceipt(changeId).ToTraxFM(node).ToString(true));
                                return salida;
                            }
                        case FmiPacketId.DriverStatusUpdate:
                        case FmiPacketId.DriverStatusReceipt:
                        case FmiPacketId.A607DriverStatusUpdate:
                        case FmiPacketId.CsFmiSafeModeReceipt:
                        case FmiPacketId.CsSpeedLimitAlertSetupReceipt:
                        case FmiPacketId.CsSpeedLimitAlert:
                            STrace.Debug(typeof(GarminFmi).FullName, node.Id, "La función FMI Packet ID " + hexFmiPacketId + " no esta implementada.");
                            break;
                    }
                    break;
                case PacketIdCode.NAck:
                    node.LastSent = null;
                    STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(), "GARMIN PACKET ID: NACK (NOT IMPLEMENTED)");
                    //needsAck = false;
                    break;
                case PacketIdCode.PvtData:
                    msgid = node.NextSequence;
                    STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(), "GARMIN PACKET ID: PVT DATA");
                    STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(), "GARMIN PVTDATA:" + msgid);
                    //needsAck = false;
                    break;
                case PacketIdCode.StreetPilotStopMessage:
                    STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(), "GARMIN PACKET ID: STREET PILOT STOP MESSAGE");
                    break;
                case PacketIdCode.StreetPilotTextMessage:
                    STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(), "GARMIN PACKET ID: STREET PILOT TEXT MESSAGE");
                    break;
            }

            return null;

            //if (needsAck)
            //{
            //    var salida = (IMessage) new UserMessage(node.Id, 0);
            //    var cmd = BaseDeviceCommand.createFrom(String.Format(Mensaje.GarminFm, GetHexAckFor(packetId)), node, DateTime.UtcNow.AddSeconds(30)).ToString(true);
            //    salida.AddStringToSend(cmd);
            //    return salida;
            //} 
            
            //return null;
        }

        private static MessageIdentifier TranslateStopStatus(StopStatusValue ssv, int devId)
        {
            MessageIdentifier res;
            switch (ssv)
            {
                case StopStatusValue.StopStatusActive:
                    res = MessageIdentifier.GarminStopStatusActive;
                    break;
                case StopStatusValue.StopStatusDone:
                    res = MessageIdentifier.GarminStopStatusDone;
                    break;
                case StopStatusValue.StopStatusUnreadInactive:
                    res = MessageIdentifier.GarminStopStatusUnreadInactive;
                    break;
                case StopStatusValue.StopStatusReadInactive:
                    res = MessageIdentifier.GarminStopStatusReadInactive;
                    break;
                case StopStatusValue.StopStatusDeleted:
                    res = MessageIdentifier.GarminStopStatusDeleted;
                    break;
                default:
                    res = MessageIdentifier.GarminStopStatusNa;
                    break;
            }
            STrace.Debug(typeof (GarminFmi).FullName, devId,
                         String.Format("GARMIN STOP STATUS RECEIVED: {0:d} - {0:G}", res));
            return res;
        }

        private static IMessage FactoryEvento(IFoteable node, ulong msgid, String texto, String gmid, DateTime dt)
        {
            IMessage salida;
            if (texto.StartsWith(CodeStr))
            {
                STrace.Debug(typeof (FmiPacketId).FullName, node.Id,
                             String.Format("llego mensaje predefinido: {0}", texto));
                var code =
                    (MessageIdentifier)
                    Convert.ToUInt32(texto.Substring(CodeStr.Length, texto.IndexOf(';', CodeStr.Length) - CodeStr.Length));
                salida = code.FactoryEvent(node.Id, msgid, null, dt, null, null);
            }
            else
            {
                STrace.Debug(typeof (FmiPacketId).FullName, node.Id,
                             String.Format("llego mensaje personalizado: {0}", texto));
                salida = new TextEvent(node.Id, msgid, dt) { Text = texto, };            
            }

            var dc = BaseDeviceCommand.createFrom(String.Format(Mensaje.GarminFm, "A1062500" + gmid), node, null);
            salida.AddStringToSend(dc.ToString(true));
//            Fota.EnqueueOnTheFly(node, 0, new[] { dc }, ref salida);

            return salida;
        }

        private static IMessage FactoryEvento(IFoteable node, byte packetId, ulong msgid, DateTime dt, ushort resultCode)
        {
            STrace.Debug(typeof (FmiPacketId).FullName, node.Id, String.Format("llego confirmacion de lectura del ultimo mensaje enviado."));
            IMessage salida = new TextEvent(node.Id, msgid, dt)
                                  {
                                      Text =
                                          (resultCode == 0
                                               ? "No se ha podido entregar el ultimo mensaje enviado."
                                               : "Confirmacion de lectura del ultimo mensaje enviado.")
                                  };

/*            var response = BaseDeviceCommand.createFrom(String.Format(Mensaje.GarminFm, GetHexAckFor(packetId)), node, null).ToString(true);
            salida.AddStringToPostSend(response);

            return salida;
 */
            return null;
        }

        #endregion

        #endregion

        #region Trax

        public static BaseDeviceCommand ToTraxFM(this byte[] message, INode node)
        {
            STrace.Debug("GarminTalk", node.Id, String.Format("TX: {0}", BitConverter.ToString(message)));
            return BaseDeviceCommand.createFrom(
                String.Format(Mensaje.GarminFm, StringUtils.ByteArrayToHexString(message, 1, message.Length - 4)),
                node, null);
        }

        public static String ToTraxFM(this byte[] message, INode node, bool addNewLine)
        {
            return ToTraxFM(message, node).ToString(false) + Environment.NewLine;
        }

        public static String ToTraxFM(this byte[] message, ulong msgid, INode node, bool addNewLine)
        {
            return ToTraxFM(message, node).ToString(false) + Environment.NewLine;
        }

        private static String encodeByteArrayForFmTr(byte[] arr2Encode)
        {
            return String.Join("", arr2Encode.Select(b => String.Format(@"\{0:X2}", b)).ToArray());
        }

        private static T ToTraxFmTR1<T>(this byte[] message, INode node)
        {
            //Debug.Assert(node != null);
            return Mensaje.Factory<T>(node, Mensaje.GarminFmTR1, encodeByteArrayForFmTr(message));
        }

        public static T ToTraxFmTR1<T>(this byte[] message, ulong mid, INode node)
        {
            //Debug.Assert(node != null);
            return Mensaje.Factory<T>(mid, node, Mensaje.GarminFmTR1, encodeByteArrayForFmTr(message));
        }

        private static byte Calculate8BitChecksum(byte[] bytes)
        {
            //byte[] byteToCalculate = bytes;
            //int checksum = byteToCalculate.Aggregate(0, (current, chData) => current + chData);
            //checksum &= 0xff;
            int chkSum = bytes.Aggregate(0, (s, b) => s += b) & 0xff;
            chkSum = (0x100 - chkSum) & 0xff;

            return (byte) chkSum;
        }

        private static byte[] CalcCrc32(byte[] bytes)
        {
            var crc32Calc = new Crc32();
            byte[] result = crc32Calc.ComputeHash(bytes);
            return result;
        }

        public static IMessage DecodeTraxFmTR1(String message, ulong msgId, Parser node)
        {
            return null;
        }

        public static IMessage DecodeTraxFm(String message, ulong msgId, Parser node)
        {
            //BitConverter.ToString(Bytes);
            //const int fmlen = 6;
            var startIndex = message.IndexOf(",", message.IndexOf(">")) + 1;
            var endIndex = message.IndexOf(";") - startIndex;
            var bytes = StringUtils.HexStringToByteList(message, startIndex, endIndex);
            // ignoro ">RFMP," y "<"            

            bytes.Add(Calculate8BitChecksum(bytes.ToArray()));

            bytes.Insert(0, PacketParts.Dle);

            bytes.AddRange(PacketParts.Trail);

/*            STrace.Debug(typeof (GarminFmi).FullName, node.GetDeviceId(),
                         "EXTRACTED: " + BitConverter.ToString(bytes.ToArray()));
*/
            return DecodeGarminFmi(bytes.ToArray(), msgId, node);
        }

        #endregion
    }
}