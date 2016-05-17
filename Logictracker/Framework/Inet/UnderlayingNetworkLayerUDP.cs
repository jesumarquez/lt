#region Usings

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.InetLayer.UDP;
using Logictracker.Layers;
using Logictracker.Model;
using Logictracker.Statistics;
using Logictracker.Utils;

#endregion

namespace Logictracker.InetLayer
{
    /// <summary>
    /// Nivel de red subyacente que implementa UDP con la tecnica Reactor,
    /// lo que se traduce procesa los datagramas sincronica y secuencialmente.
    /// </summary>
    /// <remarks>
    /// Esta implementacion, asegura que el orden de procesamiento respeta el orden
    /// en que el sistema operativo entrega los datagramas, la ventaja es su simpleza,
    /// y la desventaja que no aprovecha los recursos de procesamiento disposibles, y
    /// se espera que el rendimiento no varie en funcion del numero de procesadores 
    /// disponibles.
    /// </remarks>
    [FrameworkElement(XName = "UDPNetworkServer", IsContainer = false)]
    public class UnderlayingNetworkLayerUDP : FrameworkElement, IUnderlayingNetworkLayer
    {
        #region Attributes

        [ElementAttribute(XName = "Parser", IsSmartProperty = true, IsRequired = true)]
        public INode Parser
        {
            get { return (INode)GetValue("Parser"); }
            set { SetValue("Parser", value); }
        }

        [ElementAttribute(XName = "LinksTree", IsSmartProperty = true, IsRequired = true)]
        public LinksTree LinksTree
        {
            get { return (LinksTree)GetValue("LinksTree"); }
            set { SetValue("LinksTree", value); }
        }

        [ElementAttribute(XName = "DataLinkLayer", IsSmartProperty = true, IsRequired = true)]
        public IDataLinkLayer DataLinkLayer
        {
            get { return (IDataLinkLayer)GetValue("DataLinkLayer"); }
            set { SetValue("DataLinkLayer", value); }
        }

        [ElementAttribute(XName = "LogBandwidth", DefaultValue = false)]
        public bool LogBandwidth { get; set; }

        [ElementAttribute(XName = "ReplicationClient", DefaultValue = null)]
        public String ReplicationClient { get; set; }

        [ElementAttribute(XName = "ReplicationClientPort", DefaultValue = 5050)]
        public int ReplicationClientPort { get; set; }

        [ElementAttribute(XName = "NullPort", DefaultValue = 1025)]
        public int NullPort { get; set; }

        #endregion

        #region Public Members

        private Gauge64 SentBytes { get; set; }

        private Gauge64 ReceivedBytes { get; set; }

        public UnderlayingNetworkLayerUDP()
        {
            SentBytes = new Gauge64();
            ReceivedBytes = new Gauge64();
        }

        #endregion

        #region IUnderlayingNetworkLayer

        public bool OrderedTransfer
        {
            get { return false; }
        }

        public bool ErrorFree
        {
            get { return false; }
        }

        public bool CongestionControl
        {
            get { return false; }
        }

        public bool FlowControl
        {
            get { return false; }
        }

        public short Mtu
        {
            get { return 1024; }
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

                _reactor.LogBandwidthUsed(0, frame.Size);

                SentBytes.Inc((ulong)frame.Size);
                _reactor.SendTo(frame.Payload, frame.Size, (IPEndPoint)path.EndPoint);
            }
            catch
            {
            }
        }

        #endregion

        #region Private Members

        private UdpReactor _reactor;

        private class UdpReactor : Reactor
        {
            private delegate void Proactivate(Datagram datagram);
            private readonly UnderlayingNetworkLayerUDP _udp;
            private readonly Proactivate _deleg;
            private readonly bool _logBandwidthFlag;
            private readonly String _replicationClient;
            private readonly int _replicationClientPort;
            private readonly int _nullPort;

            public UdpReactor(UnderlayingNetworkLayerUDP udp, bool logBandwidthFlag, String replicationClient, int replicationClientPort, int nullPort)
            {
                _udp = udp;
                _deleg = Proaction;
                _logBandwidthFlag = logBandwidthFlag;
                _replicationClient = replicationClient;
                _replicationClientPort = replicationClientPort;
                _nullPort = nullPort;
            }

            protected override void OnReceive(Datagram dgram)
            {
                try
                {
                    _deleg.BeginInvoke(dgram, OnProactionComplete, _deleg);
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(UnderlayingNetworkLayerUDP).FullName, e);
                }
            }

            private static void OnProactionComplete(IAsyncResult ar)
            {
                try
                {
                    var deleg = (Proactivate)ar.AsyncState;
                    deleg.EndInvoke(ar);
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(UnderlayingNetworkLayerUDP).FullName, e);
                }
            }

            private void Proaction(Datagram dgram)
            {
                try
                {
                    _udp.ReceivedBytes.Inc((ulong)dgram.Size);

                    //Update Performance Counters
                    var cat = new BackendCategory();
                    PerformanceCounterHelper.Increment(cat.CategoryName, cat.GatewayUDP);

                    LogBandwidthUsed(dgram.Size, 0);

                    ReplicateReport(dgram);

                    var link = _udp.DataLinkLayer.OnLinkTranslation(_udp, dgram.RemoteAddress, dgram);

                    if (link != null) dgram.DeviceId = link.GetDeviceId();

                    if (!_udp.DataLinkLayer.OnFrameReceived(link, dgram, _udp.Parser)) return;

                    LogBandwidthUsed(0, dgram.Size + dgram.Size2);

                    SendTo(dgram);
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(UnderlayingNetworkLayerUDP).FullName, e);
                }
            }

            public void LogBandwidthUsed(int rx, int tx)
            {
                try
                {
                    if (!_logBandwidthFlag) return;

                    if (rx != 0)
                    {
                        _rxBytes += rx;
                        _rxPackets++;
                    }
                    if (tx != 0)
                    {
                        _txBytes += tx;
                        _txPackets++;
                    }
                    if (_lastLoggedTime.AddMinutes(1) > DateTime.Now) return;

                    var myPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    var i = myPath.LastIndexOf('\\');
                    var path = String.Format("{0}\\logs\\{1:yyyyMMdd} - {2}.log", myPath.Remove(i + 1), DateTime.Now, STrace.Module);
                    using (var sw = new StreamWriter(path, true))
                    {
                        sw.WriteLine("{0:yyyy-MM-dd hh mm ss};rxBytes={1};rxPackets={2};txBytes={3};txPackets={4}", DateTime.Now, _rxBytes, _rxPackets, _txBytes, _txPackets);
                        sw.Close();
                    }
                    _rxBytes = 0;
                    _rxPackets = 0;
                    _txBytes = 0;
                    _txPackets = 0;
                    _lastLoggedTime = DateTime.Now;
                }
                catch (Exception e)
                {
                    STrace.Exception("LogBandwidthUsed", e);
                }
            }
            private int _rxBytes;
            private int _rxPackets;
            private int _txBytes;
            private int _txPackets;
            private DateTime _lastLoggedTime = DateTime.MinValue;

            private void ReplicateReport(Datagram dgram)
            {
                try
                {
                    if (String.IsNullOrEmpty(_replicationClient)) return;

                    ReplicateClientSocket.SendTo(dgram.Payload, _replicateClientEp);
                }
                catch (Exception e)
                {
                    STrace.Log(GetType().FullName, e, 0, LogTypes.Debug, null, "ReplicateReport: exception al replicar");
                }
            }

            private Socket _replicateClientSocket;
            private IPEndPoint _replicateClientEp;

            private Socket ReplicateClientSocket
            {
                get
                {
                    if (_replicateClientSocket != null) return _replicateClientSocket;

                    _replicateClientEp = new IPEndPoint(IPAddress.Parse(_replicationClient), _replicationClientPort);

                    _replicateClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    _replicateClientSocket.Bind(new IPEndPoint(IPAddress.Any, _nullPort));
                    return _replicateClientSocket;
                }
            }

        }

        #endregion

        #region ILayer

        public bool StackBind(ILayer bottom, ILayer top)
        {
            return true;
        }

        public bool ServiceStart()
        {
            if (_reactor != null) return true;
            if (LinksTree == null) return false;
            _reactor = new UdpReactor(this, LogBandwidth, ReplicationClient, ReplicationClientPort, NullPort);
            _reactor.Open(new Uri("utn.service://0.0.0.0:" + Parser.Port).GetIPEndPoint(), 1024);
            return true;
        }

        public bool ServiceStop()
        {
            if (_reactor != null)
            {
                _reactor.Close();
                _reactor.Dispose();
            }
            return true;
        }

        #endregion
    }
}