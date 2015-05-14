using System;
using Urbetrack.Mobile.Comm.Messages;

namespace Urbetrack.Mobile.Comm.Messages
{
    public class LoginRequest : PDU
    {
        public LoginRequest()
        {
            CH = (byte)Decoder.ComandoH.LoginRequest;
            CL = 0x04;
        }

        public LoginRequest(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Decoder.DecodeHeaders(buffer, ref this_pdu, ref pos);
            iMEI = Decoder.DecodeString(buffer, ref pos);
            password = Decoder.DecodeString(buffer, ref pos);
            firmware = Decoder.DecodeString(buffer, ref pos);
            tableVersion = Decoder.DecodeShort(buffer, ref pos);
        }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            Decoder.EncodeString(ref buffer, ref pos, iMEI);
            Decoder.EncodeString(ref buffer, ref pos, password);
            Decoder.EncodeString(ref buffer, ref pos, firmware);
            Decoder.EncodeShort(ref buffer, ref pos, tableVersion);
        }

        #region Propiedades Publicas
        public string IMEI
        {
            get { return iMEI; }
            set { iMEI = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string Firmware
        {
            get { return firmware; }
            set { firmware = value; }
        }

        public short TableVersion
        {
            get { return tableVersion; }
            set { tableVersion = value; }
        }

        public override bool RequiresACK
        {
            get { return false; }
        }

        public override bool RequiresAnswer
        {
            get { return true; }
        }

        public override string TransactionId
        {
            get { return String.Format("net:UDP/dev:LPR/seq:{0}", Seq); }
        }

        #endregion

        #region Propiedades Privadas
        private string iMEI;
        private string password;
        private string firmware;
        private short tableVersion = 0;
        #endregion

    }
}