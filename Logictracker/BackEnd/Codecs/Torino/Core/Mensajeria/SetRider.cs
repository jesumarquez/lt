#region Usings

using System;
using System.Collections.Generic;
using Urbetrack.Comm.Core.Codecs;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    public class SetRider : PDU
    {
        public SetRider()
        {
            CH = (byte) Codes.HighCommand.SetRider;
            CL = 0x00; // RESERVADO
            Saliente = true;
        }

        public int Revision { get; set; }

        public string Identifier { get; set; }
        
        public string Description { get; set; }

        public string File { get; set; }

        public byte Flags { get; set; }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            UrbetrackCodec.EncodeString(ref buffer, ref pos, Identifier);
            UrbetrackCodec.EncodeString(ref buffer, ref pos, Description);
            UrbetrackCodec.EncodeString(ref buffer, ref pos, File);
            UrbetrackCodec.EncodeInteger(ref buffer, ref pos, Revision);
            UrbetrackCodec.EncodeByte(ref buffer, ref pos, Flags);
        }

        public override string Trace(string data)
        {
            return base.Trace("RIDER");
        }

        public List<string> TraceFull()
        {
            var lista = new List<string>();
            lista.Insert(0, base.Trace("RIDER"));
            lista.Insert(0, String.Format("\tIdentifier: {0} Description: {1} Revision: {2}", Identifier, Description, Revision));
            return lista;
        }
    }
}