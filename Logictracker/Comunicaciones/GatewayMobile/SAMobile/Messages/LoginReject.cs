using System;
using Urbetrack.Mobile.Comm.Messages;

namespace Urbetrack.Mobile.Comm.Messages
{
    class LoginReject : PDU
    {
        public LoginReject()
        {
            CH = (byte)Decoder.ComandoH.LoginReject;
            CL = 0x00;
        }

        public LoginReject(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Decoder.DecodeHeaders(buffer, ref this_pdu, ref pos);
        }

        public override string TransactionId
        {
            get { return String.Format("net:UDP/dev:LPR/seq:{0}", Seq); }
        }
    }
}