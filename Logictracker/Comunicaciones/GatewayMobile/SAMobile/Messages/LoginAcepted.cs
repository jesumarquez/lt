using System;
using Urbetrack.Mobile.Comm.Messages;

namespace Urbetrack.Mobile.Comm.Messages
{
    class LoginAcepted : PDU
    {
        public LoginAcepted()
        {
            CH = (byte)Decoder.ComandoH.LoginAcepted;
            CL = 0x00;
        }

        public LoginAcepted(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Decoder.DecodeHeaders(buffer, ref this_pdu, ref pos);
            idAsignado = Decoder.DecodeShort(buffer, ref pos);
        }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            Decoder.EncodeShort(ref buffer, ref pos, idAsignado);
        }

        public short IdAsignado
        {
            get { return idAsignado;  }
            set { idAsignado = value;  }
        }

        public override string TransactionId
        {
            get { return String.Format("net:UDP/dev:LPR/seq:{0}", Seq); }
        }

        private short idAsignado;
    }
}