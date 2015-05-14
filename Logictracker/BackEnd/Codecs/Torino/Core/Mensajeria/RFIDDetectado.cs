#region Usings

using System;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Utils;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    [Serializable]
    public class RFIDDetectado : PDU
    {
        public RFIDDetectado()
        {
            CH = (byte) Codes.HighCommand.MsgEvento;
            CL = 0x00; // RFID Detectado
            Saliente = true;
        }

        public RFIDDetectado(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Entrante = true;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
            var d = Devices.I().FindById(this_pdu.IdDispositivo);
            switch (CL)
            {
                case 0x00:
                    IdTarjeta = UrbetrackCodec.DecodeString(buffer, ref pos);
                    Posicion = UrbetrackCodec.DecodeGPSPoint(buffer, ref pos);
                    Status = 0xFE;
                    break;
                case 0x20:
                    IdTarjeta = UrbetrackCodec.DecodeString(buffer, ref pos);
                    Posicion = d.SupportsGPSPointEx ? UrbetrackCodec.DecodeGPSPointEx(buffer, ref pos, d) : UrbetrackCodec.DecodeGPSPoint(buffer, ref pos);
                    Status = UrbetrackCodec.DecodeByte(buffer, ref pos);
                    break;
                default:
                    throw new Exception("No conincide CL con el Tipo RFID Detectado.");
            }
        }

        public string IdTarjeta { get; set; }

        public GPSPoint Posicion { get; set; }

        public Byte Status { get; set; }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            UrbetrackCodec.EncodeString(ref buffer, ref pos, IdTarjeta);
            UrbetrackCodec.EncodeGPSPoint(ref buffer, ref pos, Posicion);
        }
    }
}