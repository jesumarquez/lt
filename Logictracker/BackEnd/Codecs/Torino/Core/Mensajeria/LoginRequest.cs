#region Usings

using System;
using System.Text;
using Urbetrack.Backbone;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Utils;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    [Serializable]
    public class LoginRequest : PDU
    {
       

        public LoginRequest()
        {
            CH = (byte)Codes.HighCommand.LoginRequest;
            CL = 0x00;
            Saliente = true;
        }

        public LoginRequest(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Entrante = true;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
            IMEI = UrbetrackCodec.DecodeString(buffer, ref pos);
            Password = UrbetrackCodec.DecodeString(buffer, ref pos);
            Firmware = UrbetrackCodec.DecodeString(buffer, ref pos);
            ConfigRevision = UrbetrackCodec.DecodeShort(buffer, ref pos);
            var d = Fleet.Devices.I().FindById(IdDispositivo);
            switch (CL)
            {
                case 0x00:
                    DetectedDeviceType = DeviceTypes.Types.SISTELCOM_v1;
                    break;
                case 0x01:
                    PendingMessages = UrbetrackCodec.DecodeShort(buffer, ref pos);
                    DetectedDeviceType = DeviceTypes.Types.SISTELCOM_v2;
                    break;
                case 0x02:
                    DetectedDeviceType = DeviceTypes.Types.URB_v0_5;
                    break;
                case 0x04:
                    DetectedDeviceType = DeviceTypes.Types.URBMOBILE_v0_1;
                    break;
                case 0x06:
                    DetectedDeviceType = DeviceTypes.Types.URB_v0_7;
                    QTreeRevision = UrbetrackCodec.DecodeInteger(buffer, ref pos);
                    XbeeHardware = UrbetrackCodec.DecodeString(buffer, ref pos);
                    XbeeFirmware = UrbetrackCodec.DecodeString(buffer, ref pos);
                    break;
                case 0x10:
                    DetectedDeviceType = DeviceTypes.Types.URBETRACK_v0_8n;
                    QTreeRevision = UrbetrackCodec.DecodeInteger(buffer, ref pos);
                    XbeeHardware = UrbetrackCodec.DecodeString(buffer, ref pos);
                    XbeeFirmware = UrbetrackCodec.DecodeString(buffer, ref pos);
                    SecureId = UrbetrackCodec.DecodeString(buffer, ref pos);
                    break;
                case 0x11:
                case 0x12:
                case 0x13:
                case 0x14:
                    DetectedDeviceType = CL <= 0x12 ? DeviceTypes.Types.URBETRACK_v0_8 : DeviceTypes.Types.URBETRACK_v1_0;
                    QTreeRevision = UrbetrackCodec.DecodeInteger(buffer, ref pos);
                    if (CL > 0x12) 
                    {
                        MessagesRevision = UrbetrackCodec.DecodeInteger(buffer, ref pos);
                    }
                    XbeeHardware = UrbetrackCodec.DecodeString(buffer, ref pos);
                    XbeeFirmware = UrbetrackCodec.DecodeString(buffer, ref pos);
                    SecureId = UrbetrackCodec.DecodeString(buffer, ref pos);
                    if (CL == 0x11 || CL == 0x13) {
                       GPSPoint = UrbetrackCodec.DecodeGPSPointEx(buffer, ref pos, d);
                       RiderIdentifier = Encoding.ASCII.GetString(UrbetrackCodec.DecodeBytes(buffer, ref pos, 10));
                    }
                    break;
                default:
                    DetectedDeviceType = DeviceTypes.Types.UNKNOW_DEVICE;
                    break;
            }
        }

        public override string Trace(string data)
        {
            return base.Trace("LOGIN REQUEST");
        }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            UrbetrackCodec.EncodeString(ref buffer, ref pos, IMEI);
            UrbetrackCodec.EncodeString(ref buffer, ref pos, Password);
            UrbetrackCodec.EncodeString(ref buffer, ref pos, Firmware);
            UrbetrackCodec.EncodeShort(ref buffer, ref pos, ConfigRevision);
        }

        #region Propiedades Publicas

        public string IMEI { get; set; }

        public string Password { get; set; }

        public string Firmware { get; set; }

        public short ConfigRevision { get; set; }

        public int QTreeRevision { get; set; }

        public int MessagesRevision { get; set; }

        public string XbeeFirmware { get; set; }

        public string XbeeHardware { get; set; }

        public string SecureId { get; set; }

        public string OldId { get; set; }

        public short PendingMessages { get; set; }

        public GPSPoint GPSPoint { get; set; }

        public string RiderIdentifier { get; set; }
        
        public DeviceTypes.Types DetectedDeviceType { get; set; }

        public override bool RequiereACK
        {
            get { return false; }
        }

        public override bool RequiereRespuesta
        {
            get { return true; }
        }

        public override string IdTransaccion
        {
            get { return String.Format("net:{0}/dev:LPR/seq:{1}", Destino, Seq); }
        }

        #endregion

        #region Propiedades Privadas

        #endregion

    }
}