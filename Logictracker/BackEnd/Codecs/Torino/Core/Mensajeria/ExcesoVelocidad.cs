#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Utils;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    [Serializable]
    public class ExcesoVelocidad : PDU
    {
        public ExcesoVelocidad()
        {
            CH = (byte)Codes.HighCommand.MsgEvento;
            CL = 0x01; // Exceso de Velocidad
            Saliente = true;
        }

        public ExcesoVelocidad(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Entrante = true;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
            switch (CL)
            {
                case 0x10:
                    {
                        var d = Devices.I().FindById(IdDispositivo);
                        PosicionDeAviso = null;
                        PosicionDeTicket = d.SupportsGPSPointEx ? UrbetrackCodec.DecodeGPSPointEx(buffer, ref pos, d) : UrbetrackCodec.DecodeGPSPoint(buffer, ref pos);
                        PosicionFinal = d.SupportsGPSPointEx ? UrbetrackCodec.DecodeGPSPointEx(buffer, ref pos, d) : UrbetrackCodec.DecodeGPSPoint(buffer, ref pos);
                        VelocidadMaximaPermitida = UrbetrackCodec.DecodeFloat(buffer, ref pos);
                        VelocidadMaximaAlcanzada = UrbetrackCodec.DecodeFloat(buffer, ref pos);
                    }
                    break;
                case 0x11:
                    {
                        var d = Devices.I().FindById(IdDispositivo);
                        PosicionDeAviso = UrbetrackCodec.DecodeGPSPointEx(buffer, ref pos, d);
                        PosicionDeTicket = UrbetrackCodec.DecodeGPSPointEx(buffer, ref pos, d);
                        PosicionFinal = UrbetrackCodec.DecodeGPSPointEx(buffer, ref pos, d);
                        VelocidadMaximaPermitida = UrbetrackCodec.DecodeFloat(buffer, ref pos);
                        VelocidadMaximaAlcanzada = UrbetrackCodec.DecodeFloat(buffer, ref pos);
                        RiderIdentifier = Encoding.ASCII.GetString(UrbetrackCodec.DecodeBytes(buffer, ref pos, 10));
                    }
                    break;
            }
        }

        public GPSPoint PosicionDeAviso { get; set; }
        public GPSPoint PosicionDeTicket { get; set; }
        public GPSPoint PosicionFinal { get; set; }

        public float VelocidadMaximaAlcanzada { get; set; }
        public float VelocidadMaximaPermitida { get; set; }

        public string RiderIdentifier { get; set; }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            //Mensajeria.Posicion.EncodeGPSPoint(ref buffer, ref pos, Posicion);
            //if (CL == 0x02) UnetelCodec.EncodeShort(ref buffer, ref pos, VelocidadMaximaAlcanzada);
        }

        public override string Trace(string data)
        {
            return base.Trace("EXCESO DE VELOCIDAD");
        }

        public List<string> TraceFull()
        {
            var posi = base.Trace("Velocidad Maxima");
            var lista = new List<string>();
            lista.Insert(0, posi);
            lista.Insert(0, String.Format("\tPosicion Inicial: {0}", PosicionDeTicket));
            lista.Insert(0, String.Format("\tPosicion Final: {0}", PosicionFinal));
            lista.Insert(0, String.Format("\tVelocidad Maxima Alcanzada: {0}", VelocidadMaximaAlcanzada));
            lista.Insert(0, String.Format("\tVelocidad Maxima Permitida: {0}", VelocidadMaximaPermitida));
            return lista;
        }
    }
}