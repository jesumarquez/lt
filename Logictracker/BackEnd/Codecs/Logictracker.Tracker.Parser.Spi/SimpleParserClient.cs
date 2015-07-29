using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using log4net;
using Logictracker.Model;
using Timer = System.Timers.Timer;

namespace Logictracker.Tracker.Parser.Spi
{
    public abstract class SimpleParserClient : IParserServer
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseParserClient));

        public abstract string Name { get; }
        public Action<IMessage> Callback { get; set; }
        public abstract IParser Parser { get; set; }
        public abstract ITranslator Translator { get; set; }
        public int Port { get; set; }
        public string Ip { get; set; }
        public string Ip2 { get; set; }

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        // The response from the remote device.
        private static String response = String.Empty;
        Socket client;
        private StateObject state;
        IPEndPoint remoteEP;
        private IPAddress ipAddress;
        private Timer receptionTimer;
        private string currentIp;

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Error("Reception Timed out");

            Logger.Debug("timer-stop");
            Stop();
            Logger.Debug("timer-start");
            Run();
        }

        private int AssignTimeOut()
        {
            Logger.Debug("AssignTimeOut");

            var currentHour = DateTime.Now.Hour;

            if (currentHour > 20)
                return 10;

            if ((currentHour > 0)&&(currentHour<6))
                return 10;

            return 5;
        }

        public void Stop()
        {
            Logger.Warn("Client is Stopping...");

            // Release the socket.
            try
            {
                //client.EndDisconnect();
                Logger.Debug("Stop-Shutdown");
                client.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Error stoping ... {0}", ex.ToString());
            }
            finally
            {
                Logger.Debug("Stop-Close");                
                client.Close();                
            }

        }
        
        public void Start()
        {
            var minutes = AssignTimeOut();
            receptionTimer = new Timer(minutes * 60000);
            Logger.InfoFormat("Timed out set for {0} minutes", minutes);
            receptionTimer.Elapsed += _timer_Elapsed;
            
            // Connect to a remote device.
            try
            {
                new Thread(Run).Start();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("StartClient ex : {0}", e.ToString());
                Logger.Debug("Start-Stop");
                Stop();
            }
        }

        public void Run()
        {
            Logger.Info("Client Started OK!!!");

            try
            {
                    var currentIp = string.Empty;
                    currentIp = currentIp.Equals(Ip) ? Ip2 : Ip;

                    ipAddress = IPAddress.Parse(currentIp);
                    remoteEP = new IPEndPoint(ipAddress, Port);

                    Logger.Debug("Start-Creating Socket");
                    // Create a TCP/IP socket.
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // Connect to the remote endpoint.
                    Logger.Debug("Start-BeginConnect");
                    client.BeginConnect(remoteEP, ConnectCallback, client);
                    Logger.Debug("Start-ConnectDone WaitOne");
                    connectDone.WaitOne();        
            }
            catch (Exception ex)
            {
                Logger.Error("Server Crashed : " + ex);
                client.BeginConnect(remoteEP, ConnectCallback, client);
                //Reconnect();
            }

            Logger.Warn("Server Stopped OK!!!");
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            Logger.Debug("ConnectCallback");

            try
            {
                // Retrieve the socket from the state object.
                client = (Socket)ar.AsyncState;

                Logger.Debug("ConnectCallback-client.EndConnect");
                // Complete the connection.
                client.EndConnect(ar);

                Logger.InfoFormat("Socket connected to {0}", client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                Logger.Debug("ConnectCallback-connectDone.Set");
                connectDone.Set();
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Logger.ErrorFormat("ConnectCallback {0}", e.ToString());
                Logger.Debug("ConnectCallback-Calling BeginDisconnect");
                client.BeginConnect(remoteEP, ConnectCallback, client);
            }
        }

        private void Receive(Socket client)
        {
            Logger.Debug("Receive");

            try
            {
                Logger.Debug("Receive-new StateObject");
                // Create the state object.
                state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                Logger.Debug("Receive-BeginReceive");                
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Receive {0}", e.ToString());
                //Stop();
                client.BeginConnect(remoteEP, ConnectCallback, client);
                //Start();
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Logger.Debug("ReceiveCallback");

            // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                state = (StateObject)ar.AsyncState;
                client = state.workSocket;

                // Read data from the remote device.
                Logger.Debug("ReceiveCallback-client.EndReceive");

                var bytesReceived=0;

                try
                {
                    bytesReceived = client.EndReceive(ar);

                    //Logger.DebugFormat("Received Data String was [{0}] from {1}", receivedData, clientData.Client.RemoteEndPoint);
                }
                catch (Exception exception)
                {
                    Logger.ErrorFormat("Receiving data {0} from socket failed from server exception {1} ", response, exception);
                }
                finally
                {
                    try
                    {
                        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                    }
                    catch (Exception exception)
                    {
                        //Server.BeginAccept(AcceptConnection,this);
                        Logger.ErrorFormat("Disconnecting client {0}", exception);
                        client.Close();

                        Reconnect();
                    }
                }

            try
            {
                if (bytesReceived > 0)
                {
                    receptionTimer.Stop();
                    Logger.Debug("Timer stopped");

                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesReceived));
                    response = state.sb.ToString().Trim();

                    // All the data has arrived; put it in response.
                    if (response.Length > 1 && response.EndsWith("|"))
                    {
                        Logger.DebugFormat("Received {0} ", response);
                        state.sb = new StringBuilder();

                        Logger.Debug("ReceiveCallback-parser");
                        var frames = Parser.Parse(response);

                        Logger.Debug("ReceiveCallback-translate");
                        var messages = Translator.Translate(frames);

                        foreach (IMessage message in messages)
                        {
                            if (message != null)
                            {
                                Logger.DebugFormat("IMessage callback : {0} for ", message.DeviceId);

                                Callback(message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Processing frames exception >> {0}", ex);
            }
        }

        private void Reconnect()
        {
            Logger.Error("Disconnecting..");

            if (client.Connected)
            {
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            else
            {
                do
                {
                    if (client==null)
                        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    try
                    {
                        client.BeginConnect(remoteEP, ConnectCallback, client);
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat("Reconnecting falied.. {0}", ex);
                    }
                } while (true);
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            Logger.Debug("SendCallback");

            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Logger.InfoFormat("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("SendCallBack {0}", e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            Logger.Debug("Send");

            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        /*****************************************************************

        public void Stop()
        {
            edtcp.Disconnect();
            Logger.Info("Base parse client Stopped");
        }

        //Fired when the connection status changes in the TCP client - parsing these messages is up to the developer
        //I'm just adding the .ToString() of the state enum to a richtextbox here
        private void client_ConnectionStatusChanged(EventDrivenTcpClient sender,EventDrivenTcpClient.ConnectionStatus status)
        {
            Logger.InfoFormat("Connection: {0}", status);
        }



        //Fired when new data is received in the TCP client
        private void client_DataReceived(EventDrivenTcpClient sender, object data)
        {
            //Interpret the received data object as a string
            var receivedData = data as string;

            Logger.DebugFormat("Received {0} ", receivedData);

            IEnumerable<T> frames = Parser.Parse(receivedData);

            IEnumerable<IMessage> messages = Translator.Translate(frames);

            NHibernateHelper.CreateSession();
            Logger.Debug("Nhibernate session created");

            foreach (IMessage message in messages)
            {
                if (message != null)
                {
                    Logger.InfoFormat("IMessage callback : {0} for ", message.DeviceId);

                    Callback(message);
                }
            }
            NHibernateHelper.CloseSession();
            Logger.Debug("Nhibernate close session");

        }
         *******************************************************************/
    }
}
