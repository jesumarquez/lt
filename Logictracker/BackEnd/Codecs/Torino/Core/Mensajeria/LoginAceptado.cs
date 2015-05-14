#region Usings

using System;
using Urbetrack.Comm.Core.Codecs;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    [Serializable]
    public class LoginAceptado : PDU
    {
        public LoginAceptado()
        {
            CH = (byte)Codes.HighCommand.LoginAceptado;
            CL = 0x00;
            Saliente = true;
        }

        public LoginAceptado(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Entrante = true;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
            IdAsignado = UrbetrackCodec.DecodeShort(buffer, ref pos);
        }

        public override string Trace(string data)
        {
            return string.Format("{0} O={1} CH={2} CL={3} {4}={5} dir={6} seq={7} sent_dev_id={8}", "LOGIN ACEPTADO", Options, CH, CL, (Entrante ? "src" : "dst"), Destino, (Entrante ? "IN" : "OUT"), Seq, IdAsignado);
        }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            UrbetrackCodec.EncodeShort(ref buffer, ref pos, IdAsignado);
            if (CL == 0x01)
            {
                UrbetrackCodec.EncodeByte(ref buffer, ref pos, MaxPDUSamples);
                UrbetrackCodec.EncodeByte(ref buffer, ref pos, FlushTimeout);
                UrbetrackCodec.EncodeShort(ref buffer, ref pos, RetrieveFlags);
            }
            if (CL == 0x02)
            {
                UrbetrackCodec.EncodeByte(ref buffer, ref pos, MaxPDUSamples);
                UrbetrackCodec.EncodeByte(ref buffer, ref pos, FlushTimeout);
                UrbetrackCodec.EncodeShort(ref buffer, ref pos, RetrieveFlags);
                UrbetrackCodec.EncodeInteger(ref buffer, ref pos, ResetToSample);
            }
        }

        public short IdAsignado { get; set; }

        public byte MaxPDUSamples { get; set; }

        public byte FlushTimeout { get; set; }
        
        public short RetrieveFlags { get; set; }

        public int ResetToSample { get; set; }

        public override string IdTransaccion
        {
            get { return String.Format("net:{0}/dev:LPR/seq:{1}", Destino, Seq); }
        }

        public short IdAnterior { get; set; }

    }
}