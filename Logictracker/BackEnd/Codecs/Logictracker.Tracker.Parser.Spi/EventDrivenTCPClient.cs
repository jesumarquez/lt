using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Logictracker.Tracker.Parser.Spi
{
    internal class EventDrivenTcpClient : IDisposable
    {
        #region Consts/Default values

        private const int DEFAULTTIMEOUT = 5000; //Default to 5 seconds on all timeouts
        private const int RECONNECTINTERVAL = 2000; //Default to 2 seconds reconnect attempt rate

        #endregion

        #region Components, Events, Delegates, and CTOR

        //Timer used to detect receive timeouts
        public delegate void delConnectionStatusChanged(EventDrivenTcpClient sender, ConnectionStatus status);

        public delegate void delDataReceived(EventDrivenTcpClient sender, object data);

        public enum ConnectionStatus
        {
            NeverConnected,
            Connecting,
            Connected,
            AutoReconnecting,
            DisconnectedByUser,
            DisconnectedByHost,
            ConnectFail_Timeout,
            ReceiveFail_Timeout,
            SendFail_Timeout,
            SendFail_NotConnected,
            Error
        }

        private readonly Timer tmrConnectTimeout = new Timer();

        private readonly Timer tmrReceiveTimeout = new Timer();
        private readonly Timer tmrSendTimeout = new Timer();

        public EventDrivenTcpClient(IPAddress ip, int port, bool autoreconnect = true)
        {
            _IP = ip;
            _Port = port;
            AutoReconnect = autoreconnect;
            _client = new TcpClient(AddressFamily.InterNetwork);
            _client.NoDelay = true; //Disable the nagel algorithm for simplicity

            ReceiveTimeout = DEFAULTTIMEOUT;
            SendTimeout = DEFAULTTIMEOUT;
            ConnectTimeout = DEFAULTTIMEOUT;
            ReconnectInterval = RECONNECTINTERVAL;

            tmrReceiveTimeout.AutoReset = false;
            tmrReceiveTimeout.Elapsed += tmrReceiveTimeout_Elapsed;

            tmrConnectTimeout.AutoReset = false;
            tmrConnectTimeout.Elapsed += tmrConnectTimeout_Elapsed;

            tmrSendTimeout.AutoReset = false;
            tmrSendTimeout.Elapsed += tmrSendTimeout_Elapsed;

            ConnectionState = ConnectionStatus.NeverConnected;
        }

        public event delDataReceived DataReceived;

        public event delConnectionStatusChanged ConnectionStatusChanged;

        #endregion

        #region Private methods/Event Handlers

        private void tmrSendTimeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            ConnectionState = ConnectionStatus.SendFail_Timeout;
            DisconnectByHost();
        }

        private void tmrReceiveTimeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            ConnectionState = ConnectionStatus.ReceiveFail_Timeout;
            DisconnectByHost();
        }

        private void tmrConnectTimeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            ConnectionState = ConnectionStatus.ConnectFail_Timeout;
            DisconnectByHost();
        }

        private void DisconnectByHost()
        {
            ConnectionState = ConnectionStatus.DisconnectedByHost;
            tmrReceiveTimeout.Stop();
            if (AutoReconnect)
                Reconnect();
        }

        private void Reconnect()
        {
            if (ConnectionState == ConnectionStatus.Connected)
                return;

            ConnectionState = ConnectionStatus.AutoReconnecting;
            try
            {
                _client.Client.BeginDisconnect(true, cbDisconnectByHostComplete, _client.Client);
            }
            catch
            {
            }
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            _client.Close();
            _client.Client.Dispose();
        }

        /// <summary>
        ///         Try connecting to the remote host
        /// </summary>
        public void Connect()
        {
            if (ConnectionState == ConnectionStatus.Connected)
                return;

            ConnectionState = ConnectionStatus.Connecting;

            tmrConnectTimeout.Start();
            _client.BeginConnect(_IP, _Port, cbConnect, _client.Client);
        }

        /// <summary>
        ///         Try disconnecting from the remote host
        /// </summary>
        public void Disconnect()
        {
            if (ConnectionState != ConnectionStatus.Connected)
                return;

            _client.Client.BeginDisconnect(true, cbDisconnectComplete, _client.Client);
        }

        /// <summary>
        ///         Try sending a string to the remote host
        /// </summary>
        /// <param name="data">The data to send</param>
        public void Send(string data)
        {
            if (ConnectionState != ConnectionStatus.Connected)
            {
                ConnectionState = ConnectionStatus.SendFail_NotConnected;
                return;
            }

            byte[] bytes = _encode.GetBytes(data);
            var err = new SocketError();
            tmrSendTimeout.Start();
            _client.Client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, out err, cbSendComplete, _client.Client);
            if (err != SocketError.Success)
            {
                Action doDCHost = DisconnectByHost;
                doDCHost.Invoke();
            }
        }

        /// <summary>
        ///         Try sending byte data to the remote host
        /// </summary>
        /// <param name="data">The data to send</param>
        public void Send(byte[] data)
        {
            if (ConnectionState != ConnectionStatus.Connected)
                throw new InvalidOperationException("Cannot send data, socket is not connected");

            var err = new SocketError();
            _client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, out err, cbSendComplete, _client.Client);
            if (err != SocketError.Success)
            {
                Action doDCHost = DisconnectByHost;
                doDCHost.Invoke();
            }
        }

        #endregion

        #region Callbacks

        private void cbConnectComplete()
        {
            if (_client.Connected)
            {
                tmrConnectTimeout.Stop();
                ConnectionState = ConnectionStatus.Connected;
                _client.Client.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, cbDataReceived,
                    _client.Client);
            }
            else
            {
                ConnectionState = ConnectionStatus.Error;
            }
        }

        private void cbDisconnectByHostComplete(IAsyncResult result)
        {
            var r = result.AsyncState as Socket;
            if (r == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");

            r.EndDisconnect(result);
            if (AutoReconnect)
            {
                Action doConnect = Connect;
                doConnect.Invoke();
            }
        }

        private void cbDisconnectComplete(IAsyncResult result)
        {
            var r = result.AsyncState as Socket;
            if (r == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");

            r.EndDisconnect(result);
            ConnectionState = ConnectionStatus.DisconnectedByUser;
        }

        private void cbConnect(IAsyncResult result)
        {
            var sock = result.AsyncState as Socket;
            if (result == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");

            if (!sock.Connected)
            {
                if (AutoReconnect)
                {
                    Thread.Sleep(ReconnectInterval);
                    Action reconnect = Connect;
                    reconnect.Invoke();
                    return;
                }
                return;
            }

            sock.EndConnect(result);

            var callBack = new Action(cbConnectComplete);
            callBack.Invoke();
        }

        private void cbSendComplete(IAsyncResult result)
        {
            var r = result.AsyncState as Socket;
            if (r == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");

            var err = new SocketError();
            r.EndSend(result, out err);
            if (err != SocketError.Success)
            {
                Action doDCHost = DisconnectByHost;
                doDCHost.Invoke();
            }
            else
            {
                lock (SyncLock)
                {
                    tmrSendTimeout.Stop();
                }
            }
        }

        private void cbChangeConnectionStateComplete(IAsyncResult result)
        {
            var r = result.AsyncState as EventDrivenTcpClient;
            if (r == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a EDTC object");

            r.ConnectionStatusChanged.EndInvoke(result);
        }

        private void cbDataReceived(IAsyncResult result)
        {
            var sock = result.AsyncState as Socket;

            if (sock == null)
                throw new InvalidOperationException("Invalid IASyncResult - Could not interpret as a socket");
            var err = new SocketError();
            int bytes = sock.EndReceive(result, out err);
            if (bytes == 0 ||
                err != SocketError.Success)
            {
                lock (SyncLock)
                {
                    tmrReceiveTimeout.Start();
                    return;
                }
            }
            lock (SyncLock)
            {
                tmrReceiveTimeout.Stop();
            }
            if (DataReceived != null)
                DataReceived.BeginInvoke(this, _encode.GetString(dataBuffer, 0, bytes), cbDataRecievedCallbackComplete,
                    this);
        }

        private void cbDataRecievedCallbackComplete(IAsyncResult result)
        {
            var r = result.AsyncState as EventDrivenTcpClient;
            if (r == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as EDTC object");

            r.DataReceived.EndInvoke(result);
            var err = new SocketError();
            _client.Client.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, out err, cbDataReceived,
                _client.Client);
            if (err != SocketError.Success)
            {
                Action doDCHost = DisconnectByHost;
                doDCHost.Invoke();
            }
        }

        #endregion

        #region Properties and members

        private readonly IPAddress _IP = IPAddress.None;
        private readonly int _Port;
        private readonly object _SyncLock = new object();
        private readonly TcpClient _client;
        private readonly byte[] dataBuffer = new byte[5000];
        private ConnectionStatus _ConStat;
        private Encoding _encode = Encoding.Default;

        /// <summary>
        ///         Syncronizing object for asyncronous operations
        /// </summary>
        public object SyncLock
        {
            get { return _SyncLock; }
        }

        /// <summary>
        ///         Encoding to use for sending and receiving
        /// </summary>
        public Encoding DataEncoding
        {
            get { return _encode; }
            set { _encode = value; }
        }

        /// <summary>
        ///         Current state that the connection is in
        /// </summary>
        public ConnectionStatus ConnectionState
        {
            get { return _ConStat; }
            private set
            {
                bool raiseEvent = value != _ConStat;
                _ConStat = value;
                if (ConnectionStatusChanged != null && raiseEvent)
                    ConnectionStatusChanged.BeginInvoke(this, _ConStat, cbChangeConnectionStateComplete, this);
            }
        }

        /// <summary>
        ///         True to autoreconnect at the given reconnection interval after a remote host closes the connection
        /// </summary>
        public bool AutoReconnect { get; set; }

        public int ReconnectInterval { get; set; }

        /// <summary>
        ///         IP of the remote host
        /// </summary>
        public IPAddress IP
        {
            get { return _IP; }
        }

        /// <summary>
        ///         Port to connect to on the remote host
        /// </summary>
        public int Port
        {
            get { return _Port; }
        }

        /// <summary>
        ///         Time to wait after a receive operation is attempted before a timeout event occurs
        /// </summary>
        public int ReceiveTimeout
        {
            get { return (int) tmrReceiveTimeout.Interval; }
            set { tmrReceiveTimeout.Interval = value; }
        }

        /// <summary>
        ///         Time to wait after a send operation is attempted before a timeout event occurs
        /// </summary>
        public int SendTimeout
        {
            get { return (int) tmrSendTimeout.Interval; }
            set { tmrSendTimeout.Interval = value; }
        }

        /// <summary>
        ///         Time to wait after a connection is attempted before a timeout event occurs
        /// </summary>
        public int ConnectTimeout
        {
            get { return (int) tmrConnectTimeout.Interval; }
            set { tmrConnectTimeout.Interval = value; }
        }

        #endregion

    }
}
