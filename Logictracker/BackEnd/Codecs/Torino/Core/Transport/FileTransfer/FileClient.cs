#region Usings

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Torino;

#endregion

namespace Urbetrack.Comm.Core.Transport.FileTransfer
{
    public class FileClient
    {

        private readonly Device device;
        private readonly int filesize;
        private readonly string filename;
        private readonly byte op = 0xFF;
        private readonly byte[] mensaje;
    	private Thread writer;
        private TcpClient socket;
        private readonly IPEndPoint endPoint;

        public delegate void SendMessageResultDelegate(int deviceId, string remote_queue, bool success);

        public event SendMessageResultDelegate SendMessageResult;

        static public int BlockDataTransferDeviceServerPort { get; set; }

        public FileClient(IPEndPoint ep, Device d, byte operation, byte[] msg, string remotequeue)
        {
            endPoint = ep;
            op = operation;
            mensaje = msg;
            device = d;
            filesize = mensaje.GetLength(0);
            filename = remotequeue;
            StartWriter();
        }

        void StartWriter()
        {
            writer = new Thread(WriterProc);
            writer.Start();
        }

        void WriterProc()
        {
            var remote_result = false;
            try
            {
                STrace.Debug(typeof(FileClient).FullName, String.Format("FileClient[{0}]: Thread de escritura iniciada.", device.LogId));
                socket = new TcpClient();
                socket.Connect(endPoint);
                STrace.Debug(typeof(FileClient).FullName, String.Format("FileClient[{0}]: Thread de escritura conectada.", device.LogId));
                var buff = new byte[71];
                // ponemos a cero.
                for (var i = 0; i < 71; ++i)
                {
                    buff[i] = 0x00;
                }
                var pos = 0;
                UrbetrackCodec.EncodeShort(ref buff, ref pos, device.Id_short);
                UrbetrackCodec.EncodeByte(ref buff, ref pos, op);
                UrbetrackCodec.EncodeInteger(ref buff, ref pos, filesize);
                var b = Encoding.ASCII.GetBytes(filename);
                Array.Copy(b, 0, buff, pos, b.GetLength(0));
                socket.Client.Send(buff);
                STrace.Debug(typeof(FileClient).FullName, String.Format("FileClient[{0}]: Cabeceras Enviadas.", device.LogId));
                var total = 0;
                var r = socket.Client.Send(mensaje, mensaje.GetLength(0), SocketFlags.None);
                total += r;
                STrace.Debug(typeof(FileClient).FullName, String.Format("FileClient[{2}]: Termino el envio de archivo de {0} bytes. total={1}", total, total + 71, device.LogId));
                var ack = new byte[1];
                ack[0] = Convert.ToByte('N');
                try
                {
                    if (socket.Client.Receive(ack, 0, 1, SocketFlags.None) == 1)
                    {
                        try
                        {
                            socket.Client.Shutdown(SocketShutdown.Send);
                            socket.Client.Close();
                        }
						catch(Exception)
                        {
                        }
                        if (ack[0] == 'A')
                        {
                            STrace.Debug(typeof(FileClient).FullName,String.Format("FileClient[{0}]: ACK Recibido", endPoint));
                            remote_result = true;
                        }
                        else
                        {
                            STrace.Debug(typeof(FileClient).FullName,String.Format("FileClient[{0}]: NACK Recibido", endPoint));
                        }
                    } 
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(FileClient).FullName, e, "FileClient.BeginPush WAIT FOR ACK/NACK PHASE");
                }
                //socket.Client.Shutdown(SocketShutdown.Both);
                //socket.Client.Close();
            } catch (Exception e)
            {
                STrace.Exception(typeof(FileClient).FullName, e, String.Format("FileClient[{0}]", device.LogId));
            }
            finally
            {
                if (SendMessageResult != null)
                {
                    SendMessageResult(device.Id_short, filename, remote_result);
                }
                else
                {
                    throw new ApplicationException(String.Format("FileClient[{0}]: Finalizo operacion desconocida.", device.LogId));
                }
            }
        }

        public static bool SendMessage(IPEndPoint endPoint, short idDispositivo, byte op, byte[] mensaje, string filename)
        {
            var filesize = mensaje.GetLength(0);
            
            try
            {
                STrace.Debug(typeof(FileClient).FullName, String.Format("FileClient[{0}]: === inciando cliente ===", endPoint));
                var socket = new TcpClient();
                socket.Connect(endPoint);
                STrace.Debug(typeof(FileClient).FullName, String.Format("FileClient[{0}]: Se ha conectado el socket TCP/IP", endPoint));
                var buff = new byte[71];
                // ponemos a cero.
                for (var i = 0; i < 71; ++i)
                {
                    buff[i] = 0x00;
                }
                var pos = 0;
                UrbetrackCodec.EncodeShort(ref buff, ref pos, idDispositivo);
                UrbetrackCodec.EncodeByte(ref buff, ref pos, op);
                UrbetrackCodec.EncodeInteger(ref buff, ref pos, filesize);
                var b = Encoding.ASCII.GetBytes(filename);
                Array.Copy(b, 0, buff, pos, b.GetLength(0));
                socket.Client.Send(buff);
                STrace.Debug(typeof(FileClient).FullName, String.Format("FileClient[{0}]: Cabeceras enviadas", endPoint));
                var total = 0;
                var r = socket.Client.Send(mensaje, mensaje.GetLength(0), SocketFlags.None);
                total += r;
                STrace.Debug(typeof(FileClient).FullName, String.Format("FileClient[{0}]: Cabeceras enviadas", endPoint));
                STrace.Debug(typeof(FileClient).FullName, String.Format("FileClient[{0}]: {1} byte enviados.", endPoint, total));
                var ack = new byte[1];
                ack[0] = Convert.ToByte('N');
                try
                {
                    if (socket.Client.Receive(ack, 0, 1, SocketFlags.None) == 1)
                    {
                        try
                        {
                            socket.Client.Shutdown(SocketShutdown.Send);
                            socket.Client.Close();
                        }
						catch(Exception)
                        {
                        }

                        if (ack[0] == 'A')
                        {
                            STrace.Debug(typeof(FileClient).FullName,String.Format("FileClient[{0}]: ACK Recibido", endPoint));
                            return true;
                        }
                        STrace.Debug(typeof(FileClient).FullName, String.Format("FileClient[{0}]: NACK Recibido", endPoint));
                    }
                    return false;
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(FileClient).FullName, e, "FileClient.BeginPush WAIT FOR ACK/NACK PHASE");
                }
            }
			catch (Exception e)
            {
                STrace.Debug(typeof(FileClient).FullName, String.Format("Exception en FileClient: txt={0}", e.Message));
            }
            return false;
        }
    }
}