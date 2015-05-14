#region Usings

using System;
using System.Net;
using System.Text;
using Logictracker.Model;

#endregion

namespace Logictracker.InetLayer.UDP
{
    public class Datagram : IFrame
    {
        internal EndPoint remoteAddress;
        public IPEndPoint RemoteAddress
        {
            get { return (IPEndPoint)remoteAddress; }
            set { remoteAddress = value; }
        }

        public int DeviceId { get; set; }

        public IPEndPoint LocalAddress { get; set; }
        private byte[] _payload;
        public byte[] Payload
        {
            get { return _payload; }
            private set { _payload = value; }
        }
        public String PayloadAsString { get; set; }
        private String _remoteAddressAsString;
        public String RemoteAddressAsString { get { return _remoteAddressAsString ?? (_remoteAddressAsString = String.Format("IP:{0}", remoteAddress)); } }

        private int _size;
        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                if (Payload == null)
                {
                    if (_size == 0) return;
                    Payload = new byte[_size];
                    return;
                }
                if (_size == 0) _payload = null;
                else
                    Array.Resize(ref _payload, _size);
            }
        }

        public override string ToString()
        {
            return string.Format("<DGRAM length={0} remote_endpoint=\"{1}\">", _size, remoteAddress);
        }

        public IFrame Reuse(string newData)
        {
            return Reuse(Encoding.ASCII.GetBytes(newData));
        }

        public IFrame Reuse(byte[] newData)
        {
            return Reuse(newData, newData.GetLength(0));
        }

        public IFrame Reuse(byte[] newData, int newSize)
        {
            _payload = newData;
            _size = newSize;
            return this;
        }

        private byte[] _payload2;
        public byte[] Payload2
        {
            get { return _payload2; }
            private set { _payload2 = value; }
        }
        private int _size2;
        public int Size2
        {
            get { return _size2; }
            set
            {
                _size2 = value;
                if (Payload2 == null)
                {
                    if (_size2 == 0) return;
                    Payload2 = new byte[_size2];
                    return;
                }
                if (_size2 == 0) _payload2 = null;
                else
                    Array.Resize(ref _payload2, _size2);
            }
        }
        public void PostSend(string newData)
        {
            PostSend(Encoding.ASCII.GetBytes(newData));
        }
        public void PostSend(byte[] newData)
        {
            PostSend(newData, newData.GetLength(0));
        }
        public void PostSend(byte[] newData, int newSize)
        {
            _payload2 = newData;
            _size2 = newSize;
        }
        public bool HasPostSend { get { return _size2 > 0; } }
    }
}