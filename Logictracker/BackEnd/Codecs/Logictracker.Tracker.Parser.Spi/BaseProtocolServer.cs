using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net;
using Logictracker.Model;

namespace Logictracker.Tracker.Parser.Spi
{
    public abstract class BaseParserServer : IParserServer
    {
        public int Backlog { set; get; }
        public int Port { get; set; }
        public int BufferSize { get; set; }
        public ProtocolType ProtocolType { get; set; }
        public Socket Server { get; private set; }
        public Action<IMessage> Callback { get; set; }
        public abstract IParser Parser { get; set; }
        public abstract ITranslator Translator { get; set; }
        public abstract string Name { get; }

        protected const int DefaultBacklog = 10;
        protected const int DefaulBufferSize = 1024;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseParserServer));
        private readonly ManualResetEvent _allDone = new ManualResetEvent(false);
        private bool _isStopped;

        protected BaseParserServer()
        {
            Backlog = DefaultBacklog;
            BufferSize = DefaulBufferSize;
            ProtocolType = ProtocolType.Tcp;
        }
        
        public void Start()
        {
            if (Server != null)
                return;

            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType);
            var iep = new IPEndPoint(IPAddress.Any, Port);
            Server.Bind(iep);
            Server.Listen(Backlog);
            Logger.InfoFormat("Server Starting and Listening in Endpoint {0} With Backlog {1} ", iep, Backlog);
            new Thread(Run).Start();
        }

        public void Stop()
        {
            Logger.Warn("Server is Stopping...");

            _isStopped = true;
            _allDone.Set();
            if (Server != null)
                Server.Close();
        }

        private void Run()
        {
            Logger.Info("Server Started OK!!!");

            try
            {
                while (!_isStopped)
                {
                    _allDone.Reset();
                    Logger.Debug("AllDone.Reset");
                    Server.BeginAccept(AcceptConnection, this);
                    Logger.Debug("AllDone.WaitOne");
                    _allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Server Crashed : " + ex);
            }

            Logger.Warn("Server Stopped OK!!!");
        }

        public void AcceptConnection(IAsyncResult iAsyncResult)
        {
            Socket client = null;
            try
            {
                _allDone.Set();
                Logger.Debug("AllDone.Set");

                var portableProtocolServer = (BaseParserServer)iAsyncResult.AsyncState;
                client = portableProtocolServer.Server.EndAccept(iAsyncResult);

                Logger.DebugFormat("Accepted new connection from {0}", client.RemoteEndPoint);

                var clientData = new ClientData
                {
                    Client = client,
                    Data = new byte[BufferSize]
                };

                client.BeginReceive(clientData.Data, 0, BufferSize, SocketFlags.None, ReceiveData, clientData);
            }
            catch (Exception exception)
            {
                if (client != null)
                    Logger.Warn(
                        String.Format("Failed Accepting Connection from IpAddress {0} ", client.RemoteEndPoint),
                        exception);
            }
        }

        public void ReceiveData(IAsyncResult iAsyncResult)
        {
            string receivedData = string.Empty;
            int bytesReceived = 0;

            var clientData = (ClientData)iAsyncResult.AsyncState;

            //Logger.DebugFormat("ReceiveData called for {0}", clientData.Client.RemoteEndPoint);

            try
            {
                bytesReceived = clientData.Client.EndReceive(iAsyncResult);

                //Logger.DebugFormat("Received Data String was [{0}] from {1}", receivedData, clientData.Client.RemoteEndPoint);
            }
            catch (Exception exception)
            {
                Logger.ErrorFormat(
                    "Receiving data from socket failed from client with IpAddress {0}. The data [{1}] will be lost - {2}",
                    clientData.Client.RemoteEndPoint, receivedData, exception);
            }
            finally
            {
                try
                {
                    clientData.Client.BeginReceive(clientData.Data, 0, BufferSize, SocketFlags.None, ReceiveData,
                                                   clientData);
                }
                catch (Exception exception)
                {
                    //Server.BeginAccept(AcceptConnection,this);
                    Logger.ErrorFormat("Disconnecting client {0}", exception);
                    clientData.Client.Close();
                }
            }

            try
            {
                if (bytesReceived > 0)
                {
                    receivedData = Encoding.ASCII.GetString(clientData.Data, 0, bytesReceived);
                    Logger.DebugFormat(
                        "Received {0} bytes of data using a BufferSize of {1}  from {2}, receivedData {3}",
                        bytesReceived, BufferSize, clientData.Client.RemoteEndPoint, receivedData);

                    var frames = Parser.Parse(receivedData);
                    //Logger.DebugFormat("frames [{0}] from {1}", frames.Count(), receivedData);

                    var trackReports = Translator.Translate(frames);
                    //Logger.DebugFormat("trackreports [{0}] from {1}", trackReports.Count, receivedData);

                    foreach (IMessage trackReport in trackReports.Where(trackReport => trackReport != null))
                    {
                        //Logger.DebugFormat("trackreport  callback : {0}", trackReport.Imei);

                        Callback(trackReport);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Processing frames exception >> {0}", ex);
            }
        }

        /*
        public void ReceiveData(IAsyncResult iAsyncResult)
        {
            string receivedData = string.Empty;

            var clientData = (ClientData)iAsyncResult.AsyncState;

            Logger.DebugFormat("ReceiveData called for {0}", clientData.Client.RemoteEndPoint);

            try
            {
                var bytesReceived = clientData.Client.EndReceive(iAsyncResult);

                Logger.DebugFormat("Received {0} bytes of data using a BufferSize of {1}  from {2}", bytesReceived, BufferSize, clientData.Client.RemoteEndPoint);

                if (bytesReceived == 0)
                {
                    Logger.DebugFormat("Bytes received was 0 then closing connection with remote client {0}", clientData.Client.RemoteEndPoint);
                    //clientData.Client.Close();
                    return;
                }

                receivedData = Encoding.ASCII.GetString(clientData.Data, 0, bytesReceived);

                Logger.InfoFormat("Received Data String was [{0}] from {1}", receivedData, clientData.Client.RemoteEndPoint);

                var frames = Deserializer.Deserialize(receivedData);

                var trackReports = Translator.Translate(frames);

                foreach (var trackReport in trackReports.Where(trackReport => trackReport != null))
                {
                    Callback(trackReport);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(String.Format("Receiving data from socket failed from client with IpAddress {0}. The data [{1}] will be lost", clientData.Client.RemoteEndPoint, receivedData), exception);
            }
            finally
            {
                try
                {
                    clientData.Client.BeginReceive(clientData.Data, 0, BufferSize, SocketFlags.None, ReceiveData, clientData);
                }
                catch (Exception exception)
                {
                    //Server.BeginAccept(AcceptConnection,this);
                    Logger.ErrorFormat("Disconnecting client {0}", exception);
                    clientData.Client.Close();
                }
            }
        }
        */
    }
}

