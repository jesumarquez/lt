#region Usings

using System;
using Urbetrack.Comm.Core.Codecs;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    [Serializable]
    class LoginRechazado : PDU
    {
        public LoginRechazado()
        {
            CH = (byte)Codes.HighCommand.LoginRechazado;
            CL = 0x00;
            Saliente = true;
        }

        public LoginRechazado(byte[] buffer, int pos)
        {
            Entrante = true;
            PDU this_pdu = this;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
        }

        public override string Trace(string data)
        {
            return base.Trace("LOGIN RECHAZADO");
        }

        public override string IdTransaccion
        {
            get { return String.Format("net:{0}/dev:LPR/seq:{1}", Destino, Seq); }
        }
    }
}