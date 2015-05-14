#region Usings

using System;
using System.Collections.Generic;
using Urbetrack.Comm.Core.Codecs;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    public class Query : PDU
    {
        public Query()
        {
            CH = (byte)Codes.HighCommand.Query;
            CL = 0x00; // 00 RESERVADO
            Saliente = true;
        }

        public int Sensor { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            UrbetrackCodec.EncodeInteger(ref buffer, ref pos, Sensor);
            UrbetrackCodec.EncodeDateTime(ref buffer, ref pos, StartTime);
            UrbetrackCodec.EncodeDateTime(ref buffer, ref pos, EndTime);
        }

        public override string Trace(string data)
        {
            return base.Trace("QUERY");
        }

        public List<string> TraceFull()
        {
            var lista = new List<string>();
            lista.Insert(0, base.Trace("QUERY"));
            lista.Insert(0, String.Format("\tSensor: {0} StartTime: {1} EndTime: {2}", Sensor, StartTime, EndTime));
            return lista;
        }
    }
}