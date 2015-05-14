#region Usings

using System;
using System.Collections.Generic;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Utils;
using Urbetrack.DatabaseTracer.Core;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    public class Posicion : PDU
    {
        public byte Posiciones { get; set; }

        public Posicion()
        {
            CH = (byte) Codes.HighCommand.MsgPosicion;
            CL = 0x01;
            Saliente = true;
        }

        public Posicion(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Entrante = true;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
            puntos.Clear();
            switch (CL)
            {
                case 0x00:
                    try
                    {
                        var point = UrbetrackCodec.DecodeGPSPoint(buffer, ref pos);
                        if (point != null) puntos.Add(point);
                    } catch (Exception e)
                    {
                        STrace.Exception(GetType().FullName,e);
                    }
                    break;
                case 0x01:
                    {
                        var items = UrbetrackCodec.DecodeByte(buffer, ref pos);
                        Posiciones = items;
                        while (items-- > 0){
                            try
                            {
                                var point = UrbetrackCodec.DecodeGPSPoint(buffer, ref pos);
                                if (point != null) puntos.Add(point);
                            } catch (Exception e)
                            {
                                STrace.Exception(GetType().FullName,e);
                            }
                        }
                    }
                    break;
                case 0x02:
                    {
                        var items = UrbetrackCodec.DecodeByte(buffer, ref pos);
                        var d = Devices.I().FindById(this_pdu.IdDispositivo);
                        while (items-- > 0)
                        {
                            try
                            {
                                var point = UrbetrackCodec.DecodeGPSPointEx(buffer, ref pos, d);
                                if (point != null) puntos.Add(point);
                            }
                            catch (Exception e)
                            {
                                STrace.Exception(GetType().FullName,e);
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception("DAC, Subtipo de mensaje de posicion desconocido.");
            }
        }

        public void AddPoints(List<GPSPoint> _puntos)
        {
            puntos = _puntos;
        }

        
        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            if (puntos.Count > 255) throw new Exception("SA: Demasiados puntos en Posicion.");
            UrbetrackCodec.EncodeByte(ref buffer, ref pos, Convert.ToByte(puntos.Count & 0xFF));
            foreach (var point in puntos)
            {
                UrbetrackCodec.EncodeGPSPoint(ref buffer, ref pos, point);
            }
        }

        private List<GPSPoint> puntos = new List<GPSPoint>();
        public List<GPSPoint> Puntos
        {
            get { return puntos; }
            set { puntos = value; }
        }

        public int IdQueue { get; set; }

        public override string Trace(string data)
        {
            return string.Format("{0} O={1} CH={2} CL={3} {4}={5} dir={6} seq={7} fixes={8}", "POSICION", Options, CH, CL, (Entrante ? "src" : "dst"), Destino, (Entrante ? "IN" : "OUT"), Seq, Puntos.Count);
        }
    }
}