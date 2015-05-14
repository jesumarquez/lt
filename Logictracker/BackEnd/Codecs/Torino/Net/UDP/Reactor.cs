using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Urbetrack.DatabaseTracer.Core;

namespace Urbetrack.Net.UDP
{
    public abstract class Reactor : IDisposable
    {
        private IPEndPoint _localAddress;
        private int Mtu { get; set; }
        private Socket Socket { get; set; }

        public virtual void Open(IPEndPoint localAddress, int mtu)
        {
            Mtu = mtu;

            _localAddress = localAddress;

            CreateSocket();

            BeginRecieve();
        }

        private void CreateSocket()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            Socket.Bind(_localAddress);

			STrace.Debug(GetType().FullName, String.Format("Starting Udp engine at address: {0}", _localAddress));
        }

        private void BeginRecieve()
        {
            var dgram = new Datagram { RemoteAddress = new IPEndPoint(IPAddress.Any, 0), LocalAddress = _localAddress, Size = Mtu };

            Socket.BeginReceiveFrom(dgram.Payload, 0, dgram.Size, 0, ref dgram.remoteAddress, _ReadCallback, dgram);
        }

        public virtual void Close() { Socket.Close(); }

        public void SendTo(Datagram dgram) { Socket.SendTo(dgram.Payload, dgram.Size, SocketFlags.None, dgram.RemoteAddress); }

        public void SendTo(byte[] buffer, int bufferSize, IPEndPoint remoteEp) { Socket.SendTo(buffer, bufferSize, SocketFlags.None, remoteEp); }

        public void SendTo(string s, IPEndPoint remoteEp) { Socket.SendTo(Encoding.ASCII.GetBytes(s.ToCharArray()), 0, s.Length, SocketFlags.None, remoteEp); }

        private void _ReadCallback(IAsyncResult ar)
        {
            try
            {
                var dgram = (Datagram) ar.AsyncState;

                dgram.Size = Socket.EndReceiveFrom(ar, ref dgram.remoteAddress);
                
                OnReceive(dgram);
            }
            catch (Exception e)
            {
                Dispose();

                CreateSocket();

                STrace.Exception(GetType().FullName,e);
            }
            finally
            {
                BeginRecieve();
            }
        }

        public virtual void OnReceive(Datagram dgram) { }

        public void Dispose()
        {
            Socket.Close();

            Socket = null;
        }
    }
}