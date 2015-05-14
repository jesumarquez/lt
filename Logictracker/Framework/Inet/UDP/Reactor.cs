#region Usings

using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.InetLayer.UDP
{
    public abstract class Reactor : IDisposable
    {
        private IPEndPoint _localAddress;

        private int Mtu { get; set; }

        private Socket Socket { get; set; }

        public void Open(IPEndPoint localAddress, int mtu)
        {
            Mtu = mtu;
            _localAddress = localAddress;
            CreateSocket();
            BeginRecive();
        }

        private void CreateSocket()
        {
            Exception ee = null;
            var level = LogTypes.Debug;
            try
            {
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Socket.Bind(_localAddress);
            }
            catch (Exception e)
            {
                ee = e;
                level = LogTypes.Trace;
            }
            STrace.Log(GetType().FullName, ee, 0, level, null, String.Format("Iniciando Udp Listener: {0}", _localAddress));
        }

        protected void SendTo(Datagram dgram)
        {
            if (Socket == null) return;

            var totalPayloadSize = dgram.Size + dgram.Size2;

            //STrace.Debug(GetType().FullName, dgram.DeviceId, String.Format("Send size={0} ip={1} text='{2}'+'{3}'", totalPayloadSize, dgram.RemoteAddressAsString, StringUtils.MakeString(dgram.Payload), StringUtils.MakeString(dgram.Payload2)));
            if (dgram.HasPostSend && totalPayloadSize < 140)
            {
                var totalPayload = new byte[totalPayloadSize];

                if (dgram.Size > 0)
                {
                    Array.Copy(dgram.Payload, totalPayload, dgram.Size);
                }

                if (dgram.Size2 > 0)
                {
                    Array.Copy(dgram.Payload2, 0, totalPayload, dgram.Size, dgram.Size2);
                }

                Socket.SendTo(totalPayload, totalPayloadSize, SocketFlags.None, dgram.RemoteAddress);
            }
            else
            {
                if (dgram.Size > 0)
                    Socket.SendTo(dgram.Payload, dgram.Size, SocketFlags.None, dgram.RemoteAddress);

                if (dgram.HasPostSend && dgram.Size2 > 0)
                    Socket.SendTo(dgram.Payload2, dgram.Size2, SocketFlags.None, dgram.RemoteAddress);
            }
        }

        public void SendTo(byte[] buffer, int bufferSize, IPEndPoint remoteEp) { if (Socket != null) Socket.SendTo(buffer, bufferSize, SocketFlags.None, remoteEp); }

        public void SendTo(string s, IPEndPoint remoteEp) { if (Socket != null) Socket.SendTo(Encoding.ASCII.GetBytes(s.ToCharArray()), 0, s.Length, SocketFlags.None, remoteEp); }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                if (Socket == null) return;

                var dgram = (Datagram)ar.AsyncState;

                dgram.Size = Socket.EndReceiveFrom(ar, ref dgram.remoteAddress);

                OnReceive(dgram);
            }
            catch (SocketException)
            {
                Close();
                CreateSocket();
            }
            catch (Exception e)
            {
                Close();
                CreateSocket();
                STrace.Exception(GetType().FullName, e);
            }
            finally
            {
                BeginRecive();
            }
        }

        private void BeginRecive()
        {
            var seguir = true;
            while (seguir)
            {
                try
                {
                    var dgram = new Datagram { RemoteAddress = new IPEndPoint(IPAddress.Any, 0), LocalAddress = _localAddress, Size = Mtu };

                    Socket.BeginReceiveFrom(dgram.Payload, 0, dgram.Size, 0, ref dgram.remoteAddress, ReadCallback, dgram);
                    seguir = false;
                }
                catch (ThreadAbortException e)
                {
                    STrace.Log(GetType().FullName, e, 0, LogTypes.Error, null, "ThreadAbortException en Reactor BeginRecive");
                    seguir = false;
                }
                catch (SecurityException e)
                {
                    STrace.Log(GetType().FullName, e, 0, LogTypes.Error, null, "SecurityException en Reactor BeginRecive");
                    seguir = false;
                }
                catch (Exception e)
                {
                    STrace.Log(GetType().FullName, e, 0, LogTypes.Debug, null, "Exception controlada en Reactor BeginRecive");
                    Thread.Sleep(1000);
                    Close();
                    CreateSocket();
                    seguir = true;
                }
            }
        }

        protected virtual void OnReceive(Datagram dgram) { }

        public void Close()
        {
            try
            {
                if (Socket != null) Socket.Close();
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName, e);
            }
            finally
            {
                Socket = null;
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}