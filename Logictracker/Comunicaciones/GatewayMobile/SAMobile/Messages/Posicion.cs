using System;
using System.Collections.Generic;
using Urbetrack.Mobile.Comm.GEO;
using Urbetrack.Mobile.Comm.Messages;

namespace Urbetrack.Mobile.Comm.Messages
{
    public class Posicion : PDU
    {
        public Posicion()
        {
            CH = (byte) Decoder.ComandoH.MsgPosicion;
            CL = 0x01;
        }

        public Posicion(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Decoder.DecodeHeaders(buffer, ref this_pdu, ref pos);
            byte items = Decoder.DecodeByte(buffer, ref pos);
            puntos.Clear();
            while(items-- > 0)
                puntos.Add(DecodeGPSPoint(buffer, ref pos));
        }

        public void AddPoints(List<GPSPoint> _puntos)
        {
            puntos = _puntos;
        }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            if (puntos.Count > 255) throw new Exception("SA: Demasiados puntos en Posicion.");
            Decoder.EncodeByte(ref buffer, ref pos, Convert.ToByte(puntos.Count & 0xFF));
            foreach(GPSPoint point in puntos)
                EncodeGPSPoint(ref buffer, ref pos, point);
        }

        internal static void EncodeGPSPoint(ref byte[] buffer, ref int pos, GPSPoint point)
        {
            Decoder.EncodeByte(ref buffer, ref pos, Convert.ToByte(point.Date.Hour & 0xFF));
            Decoder.EncodeByte(ref buffer, ref pos, Convert.ToByte(point.Date.Minute & 0xFF));
            Decoder.EncodeByte(ref buffer, ref pos, Convert.ToByte(point.Date.Second & 0xFF));
            Decoder.EncodeByte(ref buffer, ref pos, Convert.ToByte(point.Date.Day & 0xFF));
            Decoder.EncodeByte(ref buffer, ref pos, Convert.ToByte(point.Date.Month & 0xFF));
            Decoder.EncodeByte(ref buffer, ref pos, Convert.ToByte((point.Date.Year - 2000) & 0xFF));
            Decoder.EncodeByte(ref buffer, ref pos, point.LcyStatus);
            Decoder.EncodeFloat(ref buffer, ref pos, point.Lat);
            Decoder.EncodeFloat(ref buffer, ref pos, point.Lon);
            Decoder.EncodeFloat(ref buffer, ref pos, point.Speed);
            Decoder.EncodeFloat(ref buffer, ref pos, point.Course);
        }

        internal static GPSPoint DecodeGPSPoint(byte[] buffer, ref int pos)
        {
            GPSPoint point = new GPSPoint();
            byte hh = Decoder.DecodeByte(buffer, ref pos);
            byte min = Decoder.DecodeByte(buffer, ref pos);
            byte seg = Decoder.DecodeByte(buffer, ref pos);
            byte day = Decoder.DecodeByte(buffer, ref pos);
            byte mm = Decoder.DecodeByte(buffer, ref pos);
            byte yy = Decoder.DecodeByte(buffer, ref pos);
            point.Date = new DateTime(yy, mm, day, hh, min, seg, 0);
            point.LcyStatus = Decoder.DecodeByte(buffer, ref pos);
            point.Lat = Decoder.DecodeFloat(buffer, ref pos);
            point.Lon = Decoder.DecodeFloat(buffer, ref pos);
            point.Speed = Decoder.DecodeFloat(buffer, ref pos);
            point.Course = Decoder.DecodeFloat(buffer, ref pos);
            return point;
        }

        private List<GPSPoint> puntos = new List<GPSPoint>();

        public int IdQueue
        {
            get { return idQueue; }
            set { idQueue = value; }
        }

        private int idQueue;
    }
}