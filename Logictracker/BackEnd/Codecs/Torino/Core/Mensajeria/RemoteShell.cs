#region Usings

using Urbetrack.Comm.Core.Codecs;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    public class RemoteShell : PDU
    {
        public int Order { get; set; }

        public string CommandLine { get; set; }

        
        public RemoteShell()
        {
            CH = (byte) Codes.HighCommand.RemoteShell;
            CL = 0x00; // RESERVADO
            Saliente = true;
        }

        public RemoteShell(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Entrante = true;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
            Order = UrbetrackCodec.DecodeInteger(buffer, ref pos);
            CommandLine = UrbetrackCodec.DecodeString(buffer, ref pos);
        }

 
        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            UrbetrackCodec.EncodeInteger(ref buffer, ref pos, Order);
            UrbetrackCodec.EncodeString(ref buffer, ref pos, CommandLine);
        }

        public override string Trace(string data)
        {
            return base.Trace("REMOTE SHELL");
        }
    }
}