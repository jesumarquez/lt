using System;
using Urbetrack.Mobile.Comm.Transport;

namespace Urbetrack.Mobile.Comm.Messages
{
    public class PDU
    {
        public PDU()
        {
            Options = 0x00;
            Outgoing = true;
        }

        public virtual void FinalEncode(ref byte[] buffer, ref int pos)
        {
        }

        public virtual string Trace(string data)
        {
            if (data.Length == 0) data = "PDU";
            return string.Format("{0} O={1} CH={2} CL={3} {4}={5} dir={6} seq={7}",data, Options, CH, CL, (Incomming ? "src" : "dst"), Destination, (Incomming ? "IN" : "OUT"), Seq);
        }

        public short DeviceId { get; set; }

        public byte Seq { get; set; }

        public byte CH { get; set; }

        public byte CL { get; set; }

        public byte Options { get; set; }

        public bool Incomming { get; set; }

        public bool Outgoing
        {
            get { return !Incomming; }
            set { Incomming = !value; }
        }

        public virtual bool RequiresACK
        {
            get { return false; }
        }

        public virtual bool RequiresAnswer
        {
            get { return false; }
        }

        public virtual string TransactionId
        {
            get { return String.Format("net:UDP/dev:{0}/seq:{1}", DeviceId, Seq); }
        }

        public Destination Destination { get; set; }

        public AbstractTransport Transport { get; set; }

    }
}