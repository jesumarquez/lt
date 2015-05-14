#region Usings

using System;
using System.Collections.Generic;
using Urbetrack.Comm.Core.Codecs;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    public class SetParameter : PDU
    {
        public SetParameter()
        {
            CH = (byte)Codes.HighCommand.SetParameter;
            CL = 0x00; // 00 RESERVADO
            Saliente = true;
        }

        public string Parameter { get; set; }

        public string Value { get; set; }

        public int Revision { get; set; }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            UrbetrackCodec.EncodeInteger(ref buffer, ref pos, Revision);
            UrbetrackCodec.EncodeString(ref buffer, ref pos, Parameter);
            UrbetrackCodec.EncodeString(ref buffer, ref pos, Value);
        }

        public override string Trace(string data)
        {
            return base.Trace("SET PARAMETER");
        }

        public List<string> TraceFull()
        {
            var lista = new List<string>();
            lista.Insert(0, base.Trace("SET PARAMETER"));
            lista.Insert(0, String.Format("\tParameter: {0} REV: {1} NAME: {2} VALUE: {3}", Parameter, Revision, Parameter, Value));
            return lista;
        }
    }
}