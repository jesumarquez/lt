#region Usings

using System;
using Urbetrack.Comm.Core.Codecs;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    [Serializable]
    public class Heartbeat : PDU
    {        
        public Heartbeat()
        {
            Saliente = true;
            CH = (byte)Codes.HighCommand.HeartBeat;
            CL = 0x00;
        }

        public Heartbeat(byte[] buffer, int pos)
        {
            Entrante = true;
            PDU this_pdu = this;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
            signature = UrbetrackCodec.DecodeString(buffer, ref pos);
        }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            UrbetrackCodec.EncodeString(ref buffer, ref pos, signature);
        }

        public override string Trace(string data)
        {
            var s = "";
            s += base.Trace("HEARTBEAT"); s += "\r\n";
            s += string.Format(" signature={0}", Signature);
            return s;
        }

        #region Propiedades Publicas
        public string Signature
        {
            get { return signature; }
            set { signature = value; }
        }

        public override bool RequiereACK
        {
            get { return false; }
        }

        public override bool RequiereRespuesta
        {
            get { return false; }
        }

        public override string IdTransaccion
        {
            get { return String.Format("net:{0}/dev:0/seq:{1}", Destino, Seq); }
        }

        #endregion

        #region Propiedades Privadas
        private string signature;
        #endregion

    }
}