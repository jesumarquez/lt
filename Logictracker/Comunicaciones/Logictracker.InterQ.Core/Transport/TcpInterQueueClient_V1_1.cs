using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Urbetrack.Messaging.Interfaces;
using Urbetrack.Messaging.Opaque;
using Urbetrack.Toolkit;

namespace Urbetrack.InterQ.Core.Transport
{
    public class TcpInterQueueClient_V1_1 : ISyncQueueDispatcher
    {
        public bool CanPush
        {
            get { return true; }
        }

        public IPEndPoint EndPoint { get; private set; }
        public String DestinationQueueName { get; private set; }
        public int FileTransferTraceLevel { get; set; }
        
        public TcpInterQueueClient_V1_1(IPEndPoint ep, String q)
        {
            EndPoint = ep;
            DestinationQueueName = q;
            FileTransferTraceLevel = 8;
        }

        public bool BeginPush(OpaqueMessage msg)
        {
            try
            {
                msg.DestinationQueueName = DestinationQueueName;
                T.TRACE(FileTransferTraceLevel,
                        "TcpInterQueueClient_V1_1[{0}]: Iniciando, cola_origen={1}, cola_destino={2}, tamaño={3}, label={4}",
                        msg.Id, msg.SourceQueueName, msg.DestinationQueueName, msg.Length, msg.Label);
                var socket = new TcpClient();
                socket.Connect(EndPoint);
                T.TRACE(FileTransferTraceLevel, "TcpInterQueueClient_V1_1[{0}]: conectado.", msg.Id);

                // calculando tamaño
                Stream strm = new MemoryStream();
                var formatter = new BinaryFormatter();
                formatter.Serialize(strm, msg, null);
                var datalength = Convert.ToInt16(strm.Length);

                // envia cabecera.
                socket.Client.Send(Encoding.ASCII.GetBytes("UIQP/1.1"));
                var chunk_size = new byte[2];
                var b = Convert.ToByte((byte) (datalength >> 8));
                chunk_size[0] = b;
                chunk_size[1] = Convert.ToByte(datalength & 0xFF);
                socket.Client.Send(chunk_size);
                T.TRACE(FileTransferTraceLevel, "TcpInterQueueClient_V1_1[{0}]: Cabeceras Enviadas. chunk_size={1}", msg.Id, datalength);

                // envia datos
                var databuffer = new byte[datalength];
                strm.Seek(0, 0);
                strm.Read(databuffer, 0, datalength);
                var bytes_sent = socket.Client.Send(databuffer, datalength, SocketFlags.None);
                T.TRACE(FileTransferTraceLevel, "TcpInterQueueClient_V1_1[{0}]: Mensaje Enviado, {1} bytes.", msg.Id, bytes_sent);
                var ack = new byte[1];
                try
                {
                    if (socket.Client.Receive(ack, 0, 1, SocketFlags.None) == 1)
                    {
                        try
                        {
                            socket.Client.Shutdown(SocketShutdown.Send);
                            socket.Client.Close();
                        }
// ReSharper disable EmptyGeneralCatchClause
                        catch
// ReSharper restore EmptyGeneralCatchClause
                        {
                        }
                        if (ack[0] == 'A')
                        {
                            T.TRACE(FileTransferTraceLevel, "TcpInterQueueClient_V1_1[{0}]: ACK Recibido", msg.Id);
                            return true;
                        }
                        T.TRACE(FileTransferTraceLevel, "TcpInterQueueClient_V1_1[{0}]: NACK Recibido", msg.Id);
                    }
                    return false;
                }
                catch (Exception e)
                {
                    T.EXCEPTION(e, "TcpInterQueueClient_V1_1.BeginPush WAIT FOR ACK/NACK PHASE");
                }
            } 
            catch (Exception e)
            {
                T.EXCEPTION(e, "TcpInterQueueClient_V1_1[" + msg.Id + "]");
            }
            return false;
        }

        public bool WaitCompleted(OpaqueMessage msg)
        {
            return true;
        }
    }
}