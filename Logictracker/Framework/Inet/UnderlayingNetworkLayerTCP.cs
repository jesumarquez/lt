#region Usings

using System;
using System.Net;
using System.Text;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.InetLayer.TCP;
using Logictracker.Model;

#endregion

namespace Logictracker.InetLayer
{
    [FrameworkElement(XName = "TCPNetworkServer", IsContainer = false)]
    public class UnderlayingNetworkLayerTCP : FrameworkElement, IUnderlayingNetworkLayer
    {
        #region Attributes

        [ElementAttribute(XName = "Parser", IsSmartProperty = true, IsRequired = true)]
        public INode Parser
        {
            get { return (INode)GetValue("Parser"); }
            set { SetValue("Parser", value); }
        }

        [ElementAttribute(XName = "DataLinkLayer", IsSmartProperty = true, IsRequired = true)]
        public IDataLinkLayer DataLinkLayer
        {
            get { return (IDataLinkLayer)GetValue("DataLinkLayer"); }
            set { SetValue("DataLinkLayer", value); }
        }

        #endregion

        #region IUnderlayingNetworkLayer

        public bool OrderedTransfer
        {
            get { return true; }
        }

        public bool ErrorFree
        {
            get { return true; }
        }

        public bool CongestionControl
        {
            get { return true; }
        }

        public bool FlowControl
        {
            get { return true; }
        }

        public short Mtu
        {
            get { return 512; }
        }

        public int Bps
        {
            get { return 0x100000; }
        }

        public void SendFrame(ILink path, IFrame frame)
        {
            try
            {
                if (frame.Size < 1) return;
                var userState = path.UserState as Acceptor;
                if (userState == null) return;

                userState.Send(frame.Payload, frame.Payload.GetLength(0), path.GetDeviceId(), path.EndPoint.ToString());
            }
            catch
            {
            }
        }

        #region IService

        public bool ServiceStart()
        {
            STrace.Debug(GetType().FullName, String.Format("Iniciando servicio TCP listener en puerto: {0}", Parser.Port));

            _listenAddress = new IPEndPoint(IPAddress.Any, Parser.Port);
            _acceptor = new UnderlayingNetworkLayerTCPAcceptor(this, Parser);
            _listener = new Listener("", _listenAddress, _acceptor);

            return true;
        }

        public bool ServiceStop()
        {
            _listener.Close();
            return true;
        }

        #region ILayer

        public bool StackBind(ILayer bottom, ILayer top)
        {
            return true;
        }

        #endregion

        #endregion

        #endregion

        private IPEndPoint _listenAddress;
        private Listener _listener;
        private UnderlayingNetworkLayerTCPAcceptor _acceptor;

        private class UnderlayingNetworkLayerTCPAcceptor : Acceptor
        {
            private ILink _iLink;
            private int _deviceId;
            private readonly UnderlayingNetworkLayerTCP _parent;
            private readonly INode _codec;
            private readonly byte[] _activeBuffer = new byte[65535];
            private int _pendingCount;

            public UnderlayingNetworkLayerTCPAcceptor(UnderlayingNetworkLayerTCP parent, INode codec)
            {
                _parent = parent;
                _codec = codec;
            }

            public override void OnConnection()
            {
                SourceEndpoint = OsSocket.RemoteEndPoint as IPEndPoint;
                if (SourceEndpoint == null)
                {
                    throw new MissingFieldException("no se puede obtener el host remoto.");
                }
                STrace.Debug(GetType().FullName, String.Format("Recibio una conexion de {0}", SourceEndpoint));
            }

            protected override void OnDisconnect()
            {
                _parent.DataLinkLayer.OnNetworkSuspend(_iLink);
            }

            protected override void OnReceive(StreamBlock block)
            {
                if (block.TotalBytes < 5)
                {
                    STrace.Debug(GetType().FullName, String.Format("chunk invalido={0}", block.Data));
                    return;
                }

                Array.Copy(block.Data, 0, _activeBuffer, _pendingCount, block.TotalBytes);
                _pendingCount += block.TotalBytes;
                var start = 0;

                while (_pendingCount > 0)
                {
                    int cant;
                    bool ignorenoise;
                    var completed = _codec.IsPacketCompleted(_activeBuffer, start, _pendingCount, out cant, out ignorenoise);

                    if (ignorenoise)
                    {
                        if (cant == _pendingCount)
                        {
                            _pendingCount = 0;
                            return;
                        }
                        start += cant;
                        _pendingCount -= cant;
                        completed = _codec.IsPacketCompleted(_activeBuffer, start, _pendingCount, out cant, out ignorenoise);
                        //Debug.Assert(ignorenoise == false);
                    }

                    if (!completed)
                    {
                        if ((start != 0) && (_pendingCount != 0))
                        {
                            var tba = new byte[_pendingCount];
                            Array.Copy(_activeBuffer, start, tba, 0, _pendingCount);
                            Array.Copy(tba, 0, _activeBuffer, 0, _pendingCount);
                        }
                        return;
                    }

                    var activeChunk = new Chunk(_activeBuffer, start, cant, SourceEndpoint, _deviceId);
                    start += cant;
                    _pendingCount -= cant;

                    if (_iLink == null)
                    {
                        _iLink = _parent.DataLinkLayer.OnLinkTranslation(_parent, SourceEndpoint, activeChunk);

                        if (_iLink != null)
                        {
                            _iLink.UserState = this;
                            _deviceId = _iLink.Device.GetDeviceId();
                            activeChunk.DeviceId = _deviceId;
                            SendTo(activeChunk);
                        }
                    }


                    if (_parent.DataLinkLayer.OnFrameReceived(_iLink, activeChunk, _parent.Parser))
                    {
                        SendTo(activeChunk);
                    }
                }
            }

            private void SendTo(Chunk chunk)
            {
                Send(chunk.Payload, chunk.Size, _deviceId, SourceEndpoint.ToString());
                if (!chunk.HasPostSend) return;
                Send(chunk.Payload2, chunk.Size2, _deviceId, SourceEndpoint.ToString());
                chunk.Size2 = 0;
            }

            public override object Clone()
            {
                return new UnderlayingNetworkLayerTCPAcceptor(_parent, _codec);
            }
        }

        private class Chunk : IFrame
        {
            public int Size { get; private set; }

            private byte[] _payload;
            public byte[] Payload { get { return _payload; } private set { _payload = value; } }
            public String PayloadAsString { get; set; }

            private readonly IPEndPoint _remoteAddress;
            private String _remoteAddressAsString;

            public String RemoteAddressAsString { get { return _remoteAddressAsString ?? (_remoteAddressAsString = String.Format("IP='{0}'", _remoteAddress)); } }

            public IFrame Reuse(String newData)
            {
                return Reuse(Encoding.ASCII.GetBytes(newData));
            }

            public IFrame Reuse(byte[] newData)
            {
                return Reuse(newData, newData.GetLength(0));
            }

            public IFrame Reuse(byte[] newData, int newSize)
            {
                Payload = newData;
                Size = newSize;
                return this;
            }

            public byte[] Payload2 { get { return _payload2; } }
            private byte[] _payload2 = new byte[0];

            public int Size2 { get; set; }

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
                // El postSend se copia a diferencia del payload que no se 
                if (_payload2.Length < newSize)
                {
                    Array.Resize(ref _payload2, newSize);
                }
                Array.Copy(newData, _payload2, newSize);
                Size2 = newSize;
            }

            public bool HasPostSend { get { return _payload2 != null && _payload2.Length > 0; } }
            private const int ReceiveBlockSize = 1024;

            public Chunk(byte[] buffer, int start, int size, IPEndPoint sourceEndpoint, int deviceId)
            {
                _remoteAddress = sourceEndpoint;
                Size = size;
                if (size > ReceiveBlockSize) throw new Exception("CHUNK recibio un bloque mayor a lo permitido.");
                if (Payload == null) Payload = new byte[size];
                if (size > Payload.GetLength(0)) Array.Resize(ref _payload, size);
                Array.Copy(buffer, start, Payload, 0, size);
                DeviceId = deviceId;
            }
            public int DeviceId { get; set; }
        }
    }
}
