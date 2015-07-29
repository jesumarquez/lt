using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net;
using Logictracker.Model;

namespace Logictracker.Tracker.Parser.Spi
{
    public abstract class BaseParserClient : IParserServer
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseParserClient));

        public abstract string Name { get; }
        public Action<IMessage> Callback { get; set; }
        public abstract IParser Parser { get; set; }
        public abstract ITranslator Translator { get; set; }
        public int Port { get; set; }
        public static int PortStatic { get; set; }
        public string Ip { get; set; }
        public string Ip2 { get; set; }
        public static bool Reconnecting = false;

        public static string IpStatic { get; set; }
        public static string Ip2Static { get; set; }

        public AsyncCallback m_pfnCallBack;
        IAsyncResult m_result;

        // ManualResetEvent instances signal completion.
    /*    private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private readonly ManualResetEvent _allDone = new ManualResetEvent(false);*/
        private bool _isStopped;
        public static AsyncCallback ConnectCallbackStatic;
        public static Thread RunStatic;

        // The response from the remote device.
        private static String response = String.Empty;
        public Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly object syncLock = new object();
        public static Socket clientStatic;
        private StateObject state;
        IPEndPoint remoteEP;
        public static IPEndPoint remoteEPStatic;
        private IPAddress ipAddress;
        private static IPAddress ipAddressStatic;

     //   private Timer receptionTimer;
        private string currentIp = "";
        private static string currentIPStatic = "";

     /*   void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Error("Reception Timed out");

            Logger.Debug("timer-stop");
            Stop();
            Logger.Debug("timer-start");
            Start();
        }
        */
        private int AssignTimeOut()
        {
            Logger.Debug("AssignTimeOut");

            var currentHour = DateTime.Now.Hour;

            if (currentHour > 20)
                return 10;

            if ((currentHour > 0) && (currentHour < 6))
                return 10;

            return 5;
        }

        public void Stop()
        {
            Logger.Warn("Client is Stopping...");

            // Release the socket.
            //try
            //{
            //    //client.EndDisconnect();
            //    Logger.Debug("Stop-Shutdown");
            //    client.Shutdown(SocketShutdown.Both);
            //    Logger.Debug("Stop-Close");                
            //    client.Close();
            //}
            //catch (Exception ex)
            //{
            //    Logger.ErrorFormat("Error stoping ... {0}", ex.ToString());
            //}
            //finally
            //{
            //    Logger.Debug("Stop-Dipose");                
            //    client.Dispose();                
            //}

            _isStopped = true;
            // _allDone.Set();
            // Request the lock, and block until it is obtained.
          //  Monitor.Enter(client);
        //    try
        //    {              
                if (client != null)
                    client.Close();
                Logger.Warn("Server Stopped OK!!!");
      //      }
      //      finally
      //      {
                // Ensure that the lock is released.
          //      Monitor.Exit(client);
      //      }
        }

        //private void Stop(IAsyncResult ar)
        //{
        //    Logger.Debug("Stop-ar");
        //    client = (Socket)ar.AsyncState;
        //    client.EndDisconnect(ar);
        //    Logger.Debug("Stop.ar-EndDisconnect and Reset");
        //    connectDone.Reset();
        //    //client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);  
        //    Logger.Warn("Client Stopped");
        //    Thread.Sleep(5000);
        //    Logger.Debug("Stop.ar-Calling Start");
        //    Connect();
        //}

        public void Start()
        {
            IpStatic = Ip;
            Ip2Static = Ip2;
            var minutes = AssignTimeOut();
       //     receptionTimer = new Timer(minutes * 60000);
       //     Logger.InfoFormat("Timed out set for {0} minutes", minutes);
       //     receptionTimer.Elapsed += _timer_Elapsed;
        //    receptionTimer.Enabled = true;
        //    receptionTimer.AutoReset = false;

            // var currentIp = string.Empty;
            currentIp = currentIp.Equals(Ip) ? Ip2 : Ip;
            currentIPStatic = currentIp;
            // Connect to a remote device.

         
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com".
                //IPHostEntry ipHostInfo = Dns.Resolve("host.contoso.com");
                ipAddress = IPAddress.Parse(currentIp);
                ipAddressStatic = IPAddress.Parse(currentIp);
                PortStatic = Port;
                remoteEP = new IPEndPoint(ipAddress, Port);
                remoteEPStatic = new IPEndPoint(ipAddress, Port);

                Logger.Debug("Start-Creating Socket");

                Logger.Debug("Start-BeginConnect");
                
                
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
               // client.BeginConnect(remoteEP, ConnectCallback, client);

                client.Connect(remoteEP);
                if (client.Connected)
                {
                    //Wait for data asynchronously 
                    WaitForData();
                }

               // ConnectCallbackStatic = ConnectCallback;
                Logger.Debug("Start-ConnectDone WaitOne");
                
                clientStatic = client;

              }
            catch (Exception e)
            {
                Logger.ErrorFormat("StartClient ex : {0}", e.ToString());
                Logger.Debug("Start-Stop");
                Stop();
            }
        }

        public class SocketPacket
        {
            public System.Net.Sockets.Socket thisSocket;
            public byte[] dataBuffer = new byte[65535];
            public StringBuilder sb = new StringBuilder();
        }


        private void WaitForData()
        {
            try
			{
				if  ( m_pfnCallBack == null ) 
				{
					m_pfnCallBack = new AsyncCallback (OnDataReceived);
				}
				SocketPacket theSocPkt = new SocketPacket ();
				theSocPkt.thisSocket = client;
				// Start listening to the data asynchronously
				m_result = client.BeginReceive (theSocPkt.dataBuffer,
				                                        0, theSocPkt.dataBuffer.Length,
				                                        SocketFlags.None, 
				                                        m_pfnCallBack, 
				                                        theSocPkt);
			}
			catch(SocketException e)
			{
                Logger.ErrorFormat("ConnectCallback {0}", e.ToString());
                Logger.Debug("ConnectCallback-Calling BeginDisconnect");
                currentIp = currentIp.Equals(Ip) ? Ip2 : Ip;
                currentIPStatic = currentIp;
                CheckReconnectionError(e, ipAddress, currentIp, Port, remoteEP, client, ConnectCallback, RunStatic);
			}
        }


        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                SocketPacket theSockId = (SocketPacket)asyn.AsyncState;

                client = theSockId.thisSocket;

                int iRx = theSockId.thisSocket.EndReceive(asyn);
                char[] chars = new char[iRx + 1];
                System.Text.Decoder d = System.Text.Encoding.ASCII.GetDecoder();
                int charLen = d.GetChars(theSockId.dataBuffer, 0, iRx, chars, 0);
                System.String szData = new System.String(chars);
                // There might be more data, so store the data received so far.
               // state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesReceived));
                response = szData.ToString().Trim().Replace("\0", "");

                // All the data has arrived; put it in response.
                if (response.Length > 1 && response.EndsWith("|"))
                {
                    Logger.DebugFormat("Received {0} ", response);
                    theSockId.sb = new StringBuilder();
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
                WaitForData();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("ConnectCallback {0}", e.ToString());
                Logger.Debug("ConnectCallback-Calling BeginDisconnect");
                currentIp = currentIp.Equals(Ip) ? Ip2 : Ip;
                currentIPStatic = currentIp;
                CheckReconnectionError(e, ipAddress, currentIp, Port, remoteEP, client, ConnectCallback, RunStatic);
            }
        }

     /*   public void Run()
        {
            // Connect to a remote device.
            //try
            //{
            //   Logger.Debug("Start-Calling Receive");
            //    // Receive the response from the remote device.
            //    Receive(client);
            //    Logger.Debug("Start-ReceiveDone WaitOne");
            //    receiveDone.WaitOne();

            //    // Write the response to the console.
            //    Logger.InfoFormat("Response received : {0}", response);
            //}
            //catch (Exception e)
            //{
            //    Logger.ErrorFormat("StartClient ex : {0}", e.ToString());
            //    if (client.Connected)
            //    {
            //        Logger.Debug("Start-BeginDisConnect");
            //        client.BeginDisconnect(true, new AsyncCallback(Stop), client);
            //    }
            //    else
            //    {
            //        Logger.Debug("Start-Stop");
            //        Stop();
            //    }
            //}

            Logger.Info("Client Started OK!!!");

            try
            {
               // while (!_isStopped)
               // {
                    //    _allDone.Reset();
               //     Logger.Debug("AllDone.Reset");
                   // if (client.Connected)
                     //   Receive(client);
                    //  receiveDone.WaitOne();
                //    Logger.Debug("AllDone.WaitOne");
                    //    _allDone.WaitOne();

              //  }
            }
            catch (Exception e)
            {
                Logger.Error("Server Crashed : " + e);
                // var currentIp = string.Empty;
                currentIp = currentIp.Equals(Ip) ? Ip2 : Ip;
                currentIPStatic = currentIp;
                CheckReconnectionError(e, ipAddress, currentIp, Port, remoteEP, client, ConnectCallback, RunStatic);
                if (!Reconnecting && e.GetType().ToString().Equals("System.Net.Sockets.SocketException"))
                {
                    ThreadStart t = new ThreadStart(Run);
                    RunStatic = new Thread(t);
                    RunStatic.Start();
                    return;
                }
                // client.BeginConnect(remoteEP, ConnectCallback, client);
            }

           // Logger.Warn("Server Stopped OK!!!");
        }
        */
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
             //   connectDone.Set();

                Receive(client);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("ConnectCallback {0}", e.ToString());
                Logger.Debug("ConnectCallback-Calling BeginDisconnect");
                currentIp = currentIp.Equals(Ip) ? Ip2 : Ip;
                currentIPStatic = currentIp;
                CheckReconnectionError(e, ipAddress, currentIp, Port, remoteEP, client, ConnectCallback, RunStatic);
              /*  if (!Reconnecting && e.GetType().ToString().Equals("System.Net.Sockets.SocketException"))
                {
                    ThreadStart t = new ThreadStart(Run);
                    RunStatic = new Thread(t);
                    RunStatic.Start();
                    return;
                }*/

                // client.BeginConnect(remoteEP, ConnectCallback, client);
                //Start();
                //Stop();
                //Start();
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
              //   Monitor.Enter(client);
              //   try
              //   {
              //       client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
              //   }
              //  catch(Exception e)
              //  { 
              //      throw e; 
              //  }
              //  finally
              //   {
              //       Monitor.Exit(client);
              //   }
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Receive {0}", e.ToString());
                //Stop();
                currentIp = currentIp.Equals(Ip) ? Ip2 : Ip;
                currentIPStatic = currentIp;
                CheckReconnectionError(e, ipAddress, currentIp, Port, remoteEP, client, ConnectCallback, RunStatic);
               /* if (!Reconnecting && e.GetType().ToString().Equals("System.Net.Sockets.SocketException"))
                {
                    ThreadStart t = new ThreadStart(Run);
                    RunStatic = new Thread(t);
                    RunStatic.Start();
                    return;
                }
*/                //  client.BeginConnect(remoteEP, ConnectCallback, client);
                //Start();
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            bool error = false;
            Logger.Debug("ReceiveCallback");

            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            state = (StateObject)ar.AsyncState;
            client = state.workSocket;

            // Read data from the remote device.
            Logger.Debug("ReceiveCallback-client.EndReceive");

            var bytesReceived = 0;

            try
            {
   //             Monitor.Enter(client);
        //        try
        //        {
                    bytesReceived = client.EndReceive(ar);
        //        }
         //       catch (Exception e)
         //       {
         //           throw e;
         //       }
         //       finally
         //       {
         //           Monitor.Exit(client);
         //       }
                //Logger.DebugFormat("Received Data String was [{0}] from {1}", receivedData, clientData.Client.RemoteEndPoint);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Receiving data {0} from socket failed from server exception {1} ", response, e);
                currentIp = currentIp.Equals(Ip) ? Ip2 : Ip;
                currentIPStatic = currentIp;
                CheckReconnectionError(e, ipAddress, currentIp, Port, remoteEP, client, ConnectCallback, RunStatic);
                /*if (!Reconnecting && e.GetType().ToString().Equals("System.Net.Sockets.SocketException"))
                {
                    error = true;
                    bytesReceived = 0;
                    ThreadStart t = new ThreadStart(Run);
                    RunStatic = new Thread(t);
                    RunStatic.Start();
                    return;
                }*/
            }

            try
            {
                if (bytesReceived > 0)
                {
                   // receptionTimer.Stop();
                   // Logger.Debug("Timer stopped");

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
            catch (Exception e)
            {
                Logger.ErrorFormat("Processing frames exception >> {0}", e);
                currentIp = currentIp.Equals(Ip) ? Ip2 : Ip;
                currentIPStatic = currentIp;
                CheckReconnectionError(e, ipAddress, currentIp, Port, remoteEP, client, ConnectCallback, RunStatic);
                /*if (!Reconnecting && e.GetType().ToString().Equals("System.Net.Sockets.SocketException"))
                {
                    ThreadStart t = new ThreadStart(Run);
                    RunStatic = new Thread(t);
                    RunStatic.Start();
                    return;
                }*/
            }
            finally
            {
                Receive(client);
            }
        }

        public void CheckReconnectionError(
            Exception exception,
            IPAddress ipAddress,
            String currentIp,
            int Port,
            IPEndPoint remoteEP,
            Socket client,
            AsyncCallback ConnectCallback,
            Thread Run)
        {
            switch (exception.GetType().ToString())
            {
                case "System.Net.Sockets.SocketException":
                    {
                        if (!Reconnecting)
                        {
                            Reconnecting = true;
                            Logger.ErrorFormat("SocketException error code {0} ", ((SocketException)exception).SocketErrorCode.ToString());
                            switch (((SocketException)exception).SocketErrorCode)
                            {
                                case SocketError.AccessDenied:
                                    break;
                                case SocketError.AddressAlreadyInUse:
                                    break;
                                case SocketError.AddressFamilyNotSupported:
                                    break;
                                case SocketError.AddressNotAvailable:
                                    break;
                                case SocketError.AlreadyInProgress:
                                    break;
                                case SocketError.ConnectionAborted:
                                    break;
                                case SocketError.ConnectionRefused:
                                    break;
                                case SocketError.ConnectionReset:
                                    break;
                                case SocketError.DestinationAddressRequired:
                                    break;
                                case SocketError.Disconnecting:
                                    break;
                                case SocketError.Fault:
                                    break;
                                case SocketError.HostDown:
                                    break;
                                case SocketError.HostNotFound:
                                    break;
                                case SocketError.HostUnreachable:
                                    break;
                                case SocketError.IOPending:
                                    break;
                                case SocketError.InProgress:
                                    break;
                                case SocketError.Interrupted:
                                    break;
                                case SocketError.InvalidArgument:
                                    break;
                                case SocketError.IsConnected:
                                    break;
                                case SocketError.MessageSize:
                                    break;
                                case SocketError.NetworkDown:
                                    break;
                                case SocketError.NetworkReset:
                                    break;
                                case SocketError.NetworkUnreachable:
                                    break;
                                case SocketError.NoBufferSpaceAvailable:
                                    break;
                                case SocketError.NoData:
                                    break;
                                case SocketError.NoRecovery:
                                    break;
                                case SocketError.NotConnected:
                                    break;
                                case SocketError.NotInitialized:
                                    break;
                                case SocketError.NotSocket:
                                    break;
                                case SocketError.OperationAborted:
                                    break;
                                case SocketError.OperationNotSupported:
                                    break;
                                case SocketError.ProcessLimit:
                                    break;
                                case SocketError.ProtocolFamilyNotSupported:
                                    break;
                                case SocketError.ProtocolNotSupported:
                                    break;
                                case SocketError.ProtocolOption:
                                    break;
                                case SocketError.ProtocolType:
                                    break;
                                case SocketError.Shutdown:
                                    break;
                                case SocketError.SocketError:
                                    break;
                                case SocketError.SocketNotSupported:
                                    break;
                                case SocketError.Success:
                                    break;
                                case SocketError.SystemNotReady:
                                    break;
                                case SocketError.TimedOut:
                                    break;
                                case SocketError.TooManyOpenSockets:
                                    break;
                                case SocketError.TryAgain:
                                    break;
                                case SocketError.TypeNotFound:
                                    break;
                                case SocketError.VersionNotSupported:
                                    break;
                                case SocketError.WouldBlock:
                                    break;
                                default:
                                    break;
                            }
                            Thread.Sleep(60000); //esperamos un minuto
                            bool isConnected = false;
                            while (!isConnected)
                            {
                                try
                                {
                                    // Establish the remote endpoint for the socket.
                                    // The name of the 
                                    // remote device is "host.contoso.com".
                                    //IPHostEntry ipHostInfo = Dns.Resolve("host.contoso.com");
                                    ipAddress = IPAddress.Parse(currentIp);
                                    remoteEP = new IPEndPoint(ipAddress, Port);
                                    Logger.Debug("Start-Creating Socket");
                                    // Create a TCP/IP socket.
                                   // Monitor.Enter(client);
                                     // Connect to the remote endpoint.
                                    Logger.Debug("Start-BeginConnect");
                                 //   try
                                  //  {
                                     //   client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                     //   client.BeginConnect(remoteEP, ConnectCallback, client);
                                  //  }
                                  //  catch (Exception e)
                                  //  {
                                  //      throw e;
                                  //  }
                                  //  finally
                                  //  {
                                  //      Monitor.Exit(client);
                                  //  }

                                    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                    // client.BeginConnect(remoteEP, ConnectCallback, client);

                                    client.Connect(remoteEP);
                                    if (client.Connected)
                                    {
                                        //Wait for data asynchronously 
                                        WaitForData();
                                    }
                                    Logger.Debug("Start-ConnectDone WaitOne");
                                 //   connectDone.WaitOne();
                                    isConnected = true;
                                    Reconnecting = false;
                                }
                                catch (Exception ex)
                                {
                                    Logger.ErrorFormat("SocketException error code {0} ", ((SocketException)ex).SocketErrorCode.ToString());
                                    Thread.Sleep(60000);
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private  void SendCallback(IAsyncResult ar)
        {
            Logger.Debug("SendCallback");
            Socket clientLocal = null;
            try
            {
                // Retrieve the socket from the state object.
                clientLocal = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = clientLocal.EndSend(ar);
                Logger.InfoFormat("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
            //    sendDone.Set();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("SendCallBack {0}", e.ToString());
                currentIPStatic = currentIPStatic.Equals(IpStatic) ? Ip2Static : IpStatic;
                CheckReconnectionError(e, ipAddressStatic, currentIPStatic, PortStatic, remoteEPStatic, clientLocal, ConnectCallbackStatic, RunStatic);
            }
        }

        private void Send(Socket clientLocal, String data)
        {
            Logger.Debug("Send");

            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            //Monitor.Enter(clientLocal);
           // try
           // {
                clientLocal.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), clientLocal);
          //  }
          //  finally
          //  {
          //      Monitor.Exit(clientLocal);
          //  }            
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
