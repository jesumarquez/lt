using System;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model.IAgent;

namespace Logictracker.MobileApps.v1
{
    [FrameworkElement(XName = "MobileAppsFileServer", IsContainer = false)]
    public class FileServer : FrameworkElement, IService
    {
        [ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2007)]
        public int Port { get; set; }

        private Socket _listener;

        // Objeto de estado.
        public class StateObject
        {
            public enum States
            {
                SKIPING_HTTP,
                NEW,
                READING_FILESIZE,
                READING_FILENAME,
                READING_DATA,
                WRITING_ACKNOWLEDGE,
                DONE
            };

            public States State = States.SKIPING_HTTP;
            // Socket
            public Socket WorkSocket;
            // Buffer size
            public const int BufferSize = 1024;
            // Buffer
            public byte[] Buffer = new byte[BufferSize];
            // Nombre del Archivo
            public String Filename;
            // File Handler
            public FileStream File;
            public BinaryWriter Writer;

            public int Filesize = 0;
            public int Totalbytes = Int32.MaxValue;

            public bool Write(byte[] data, int size)
            {
                const string http = "POST / HTTP/1.0\r\n\r\n";
                //Console.WriteLine("RECIBI: " + size);
                var ix = 0;
                // HACK para superar el 
                if (State == States.SKIPING_HTTP)
                {
                    if (size >= http.Length)
                    {
                        if (data[0] == 'P' && data[1] == 'O' && data[2] == 'S' && data[3] == 'T')
                        {
                            Console.WriteLine("Saltie POST");
                            ix += http.Length;
                            size -= http.Length;
                        }
                    }
                    State = States.NEW;
                }
                if (State == States.NEW && size > 0)
                {
                    if (size < 4) throw new ApplicationException("No hay suficientes datos para leer el tamano de archivo");                    
                    Filesize = (data[ix++] << 24 | data[ix++] << 16 | data[ix++] << 8 | data[ix++]);
                    size -= 4;
                    State = States.READING_FILENAME;
                    Console.WriteLine("File Size: " + Filesize);
                }
                if (State == States.READING_FILENAME && size > 0)
                {
                    var filenamesize = data[ix++];
                    if (size < filenamesize) throw new ApplicationException("No hay suficientes datos para leer el nombre de archivo");
                    Filename = Encoding.ASCII.GetString(data, ix, filenamesize);
                    ix += filenamesize;
                    size -= filenamesize + 1; // el uno es del char
                    State = States.READING_DATA;
                    var imei = Filename.Split('-')[0];
                    
                    Console.WriteLine("FILENAME: " + Filename);
                        
                    var device = new DAOFactory().DispositivoDAO.GetByIMEI(imei);
                    if (device == null) throw new ApplicationException("No existe el dispositivo IMEI=" + imei);
                    Filename = device.Id.ToString("D4") + "-" + Filename.Split('-')[1];
                    var basePath = Path.Combine(Config.Directory.PicturesDirectory, device.Id.ToString("D4"));
                    Filename = Path.Combine(basePath, Filename);
				    if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
                    File = new FileStream(Filename, FileMode.Create);
                    Writer = new BinaryWriter(File);
                    Totalbytes = 0;
                    Console.WriteLine("File Size: " + Filesize);
                }
                if (State == States.READING_DATA && size > 0)
                {
                    Totalbytes += size;
                    //Console.WriteLine("Bytes Readed: " + totalbytes);
                    Writer.Write(data, ix, size);
                }
                if (State == States.READING_DATA && Totalbytes >= Filesize)
                {
                    Console.WriteLine("Bytes Readed: " + Totalbytes + " FileSize: " + Filesize);
                    State = States.DONE;
                    Writer.Close();
                    return false;
                }
                return true;
            }
        };

        public FileServer()
        {
        }

        public bool ServiceStart() {
            // Data buffer for incoming data.
            var localEndPoint = new IPEndPoint(IPAddress.Any, Port);

            // Create a TCP/IP socket.

            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            _listener.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            //listener.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, true);
            // Bind the socket to the local endpoint and listen for incoming connections.
            try 
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(100);
                LaunchBeginAccept();
            } 
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("MobileApps FileServer Startup Failure.");
                return false;
            }

            Console.WriteLine("MobileApps FileServer Started.");
            return true;
        }

        public bool ServiceStop()
        {
            return true;
        }

        public void LaunchBeginAccept()
        {
            // Start an asynchronous socket to listen for connections.
            Console.WriteLine("Waiting for a connection: port=" + Port);
            _listener.BeginAccept(AcceptCallback, _listener);
        }

        public void AcceptCallback(IAsyncResult ar) {
            // Obtengo el listener
            var listener = (Socket) ar.AsyncState;
            var handler = listener.EndAccept(ar);
            Console.WriteLine("Connection Accepted: host=" + handler.RemoteEndPoint);
            // Creo el estado.
            var state = new StateObject();
            state.WorkSocket = handler;
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
            // Lanzamos nuevamente el BeginAccept.
            LaunchBeginAccept();
        }

        public static void ReadCallback(IAsyncResult ar) 
        {
            // Casteo el estado
            var state = (StateObject) ar.AsyncState;
            var handler = state.WorkSocket;

            // Leemos el socket
            var bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                try
                {
                    if (state.Write(state.Buffer, bytesRead))
                    {
                        // Faltan datos.
                        handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
                    }
                    else
                    {
                        Send(handler, "ok");
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    catch { }

                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);	
                }
            }
            else
            {
                throw new ApplicationException("Error, se produjo una lectura de valor cero");
            }
        }
        
        private static void Send(Socket handler, String data) 
        {
            // Convert the string data to byte data using ASCII encoding.
            var byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, handler);
        }

        private static void SendCallback(IAsyncResult ar) 
        {
            try 
            {
                // Retrieve the socket from the state object.
                var handler = (Socket) ar.AsyncState;

                // Complete sending the data to the remote device.
                var bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            } 
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}

