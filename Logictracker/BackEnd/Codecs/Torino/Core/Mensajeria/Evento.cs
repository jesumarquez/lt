#region Usings

using System;
using System.Text;
using Urbetrack.Backbone;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Utils;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    [Serializable]
    public class Evento : PDU
    {
        public enum HardwareChanges
        {
            EVTDATA_GPS_FAULT = 0x00,
            EVTDATA_GPS_READY = 0x80,
            EVTDATA_XBEE_FAULT = 0x01,
            EVTDATA_XBEE_READY = 0x81,
            EVTDATA_MODEM_FAULT = 0x02,
            EVTDATA_MODEM_READY = 0x82,
            EVTDATA_SDCI_FAULT = 0x03,
            EVTDATA_SDCI_READY = 0x83,
            EVTDATA_FLASH_FAULT = 0x04,
            EVTDATA_FLASH_READY = 0x84
        };

        public Evento()
        {
            CH = (byte) Codes.HighCommand.MsgEvento;
            CL = 0xFF; // Evento Generico
            Saliente = true;
        }

        public char SubTipoAuxiliar;

        public Evento(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Entrante = true;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
            var d = Devices.I().FindById(this_pdu.IdDispositivo);

            Posicion = UrbetrackCodec.DecodeGPSPointEx(buffer, ref pos, d);
            CodigoEvento = UrbetrackCodec.DecodeShort(buffer, ref pos);
            Datos = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            Payload = null;
            switch (CL)
            {
                case 0xFF:
                    PayloadSize = 0;
                    Extra = 0;
                    RiderRevision = -1;
                    if (d.Type == DeviceTypes.Types.URBETRACK_v1_0 ||
                        d.Type == DeviceTypes.Types.URBETRACK_v0_8 ||
                        d.Type == DeviceTypes.Types.URBETRACK_v0_8n)
                    {
                        RiderIdentifier = Encoding.ASCII.GetString(UrbetrackCodec.DecodeBytes(buffer, ref pos, 10));
                        if (String.IsNullOrEmpty(RiderIdentifier))
                            RiderIdentifier = "0000000000";
                    }
                    else
                    {
                        RiderIdentifier = "0000000000";
                    }
                    break;
                case 0xFE:
                    RiderIdentifier = Encoding.ASCII.GetString(UrbetrackCodec.DecodeBytes(buffer, ref pos, 10));
                    RiderRevision = UrbetrackCodec.DecodeInteger(buffer, ref pos);
                    Extra = UrbetrackCodec.DecodeInteger(buffer, ref pos);
                    PayloadSize = UrbetrackCodec.DecodeInteger(buffer, ref pos);
                    if (PayloadSize > 0)
                        Payload = UrbetrackCodec.DecodeBytes(buffer, ref pos, PayloadSize);
                    break;
            }
        }

        public string StringPayload()
        {
            return Payload == null ? "" : Encoding.ASCII.GetString(Payload);
        }

        public byte[] Payload { get; set; }

        public int PayloadSize { get; set; }

        public string RiderIdentifier { get; set; }

        public int RiderRevision { get; set; }

        public short CodigoEvento { get; set; }
        
        public int Datos { get; set; }

        public int Extra { get; set; }

        public GPSPoint Posicion { get; set; }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            UrbetrackCodec.EncodeGPSPoint(ref buffer, ref pos, Posicion);
            UrbetrackCodec.EncodeShort(ref buffer, ref pos, CodigoEvento);
            UrbetrackCodec.EncodeInteger(ref buffer, ref pos, Datos);
        }
    }
}