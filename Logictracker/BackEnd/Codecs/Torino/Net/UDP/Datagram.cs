#region Usings

using System;
using System.Net;
using System.Text;
using Urbetrack.Model;
using Urbetrack.Utils;

#endregion

namespace Urbetrack.Net.UDP
{
    public class Datagram : IFrame
    {
        internal EndPoint remoteAddress;
        public IPEndPoint RemoteAddress
        {
            get { return (IPEndPoint) remoteAddress; }
            set { remoteAddress = value; }
        }

        public IPEndPoint LocalAddress { get; set; }
        private byte[] payload;
        public byte[] Payload
        {
            get { return payload; }
            private set { payload = value; }
        }
		public String PayloadAsString { get; set; }
		private String _remoteAddressAsString;
		public String RemoteAddressAsString { get { return _remoteAddressAsString ?? (_remoteAddressAsString = String.Format("IP:{0}", remoteAddress)); } }

        private int size;
        public int Size
        {
            get { return size; }
            set {
                size = value;
                if (Payload == null)
                {
                    if (size == 0) return;
                    Payload = new byte[size];
                    return;
                }
                if (size == 0) payload = null;
                else
                Array.Resize(ref payload, size);
            }
        }

        public override string ToString()
        {
            return string.Format("<DGRAM length={0} remote_endpoint=\"{1}\">", size, remoteAddress);
        }

        public IFrame Reuse(string new_data)
        {
            return Reuse(Encoding.ASCII.GetBytes(new_data));
        }
        
        public IFrame Reuse(byte[] new_data)
        {
            return Reuse(new_data, new_data.GetLength(0));
        }
        
        public IFrame Reuse(byte[] new_data, int new_size)
        {
            payload = new_data;
            size = new_size;
            return this;
        }

		public byte[] Payload2 { get { return null; } }
		public int Size2 { get { return 0; } }
		public void PostSend(string new_data) { }
		public void PostSend(byte[] new_data) { }
		public void PostSend(byte[] new_data, int new_size) { }
		public bool HasPostSend { get { return false; } }
		public int DeviceId { get; set; }
    }
}
