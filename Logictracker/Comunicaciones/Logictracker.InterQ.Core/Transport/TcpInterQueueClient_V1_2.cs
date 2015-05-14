using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Urbetrack.Compression;
using Urbetrack.Messaging.Interfaces;
using Urbetrack.Messaging.Opaque;
using Urbetrack.Net.TCP;
using Urbetrack.Toolkit;
using Stream=System.IO.Stream;

namespace Urbetrack.InterQ.Core.Transport
{
    public class TcpInterQueueClient_V1_2 : ISyncQueueDispatcher
    {
        public IPEndPoint EndPoint { get; private set; }
        public String DestinationQueueName { get; private set; }
        public int FileTransferTraceLevel { get; set; }
        public bool FileTransferFSMTrace { get; set; }
        private Client Stream { get; set; }
        private readonly ManualResetEvent syncro;

        public TcpInterQueueClient_V1_2(IPEndPoint EndPoint, String DestinationQueueName, int FileTransferTraceLevel, bool FileTransferFSMTrace)
        {
            syncro = new ManualResetEvent(false);
            this.EndPoint = EndPoint;
            this.DestinationQueueName = DestinationQueueName;
            this.FileTransferTraceLevel = FileTransferTraceLevel;
            this.FileTransferFSMTrace = FileTransferFSMTrace;
        }

        public TcpInterQueueClient_V1_2(IPEndPoint EndPoint, String DestinationQueueName)
            : this(EndPoint, DestinationQueueName, 8, false)
        {
                
        }

        public bool CanPush
        {
            get
            {
                if (Stream == null)
                {
                    Stream = new Client(this);
                    if (!Stream.Connect(EndPoint, true))
                    {
                        Stream.TRACE("UNABLE TO CONNECT");
                        return false;
                    }
                    Stream.Send("UIQP/1.2");
                    return true;
                }
                if (Stream.Broken)
                {
                    Stream = null;
                    return false;
                }
                return true;
            }
        }

        public bool BeginPush(OpaqueMessage msg)
        {
            msg.DestinationQueueName = DestinationQueueName;
            // calculando tamaño
            var data = GZip.SerializeAndCompress(msg);
            var datalength = data.GetLength(0);

            // enviando CHUNK
            var chunk_size = new byte[2];
            var b = Convert.ToByte((byte)(datalength >> 8));
            chunk_size[0] = b;
            chunk_size[1] = Convert.ToByte(datalength & 0xFF);
            Stream.Send(chunk_size, 2);

            // envia datos
            Stream.Send(data, datalength);

            return true;    
        }

        public bool WaitCompleted(OpaqueMessage msg)
        {
            Stream.TRACE("SYNCRO.WAIT");
            syncro.WaitOne();
            Stream.TRACE("SYNCRO.RESET");
            syncro.Reset();
            return LastResult;
        }

        public bool LastResult { get; set; }

        internal class Client : Net.TCP.Client {
            private TcpInterQueueClient_V1_2 Parent;

            internal Client(TcpInterQueueClient_V1_2 Parent)
            {
                this.Parent = Parent;
            }

            public override void TRACE(string format, object[] args)
            {
                if (T.ActiveContext.CurrentLevel < Parent.FileTransferTraceLevel) return;
                var nf = "UIQC[" + T.LOCALENDPOINT(os_socket) + "/" +
                         T.REMOTEENDPOINT(os_socket) + "]:" + format;
                T.TRACE(nf, args);
            }

            private readonly byte[] active_buffer = new byte[65535];
            private int buffer_position;
            protected byte[] ActiveBufferPop(int count)
            {
                if (buffer_position == 0 || count == 0 || buffer_position < count) return null;
                var return_buffer = new byte[count];
                Array.Copy(active_buffer, 0, return_buffer, 0, count);
                buffer_position -= count;
                if (buffer_position == 0) return return_buffer;
                Array.Copy(active_buffer, count, active_buffer, 0, buffer_position);
                return return_buffer;
            }
            public override void OnReceive(StreamBlock block)
            {
                // agregarmos al buffer.
                TRACE("bloque de datos={0}", block.TotalBytes);
                Array.Copy(block.Data, 0, active_buffer, buffer_position, block.TotalBytes);
                buffer_position += block.TotalBytes;
                var ack_buffer = ActiveBufferPop(1);
                if (ack_buffer == null) return;
                Parent.LastResult = ack_buffer[0] == 'A';
                TRACE("SYNCRO.SET");
                Parent.syncro.Set();
            }

            public override void OnDisconnect()
            {
                TRACE("OnDisconnect");
            }

            public override void OnInternalError()
            {
                TRACE("OnReceive");
            }

            public override void OnConnect()
            {
                TRACE("OnConnect");
            }

            public override void OnRejected()
            {
                TRACE("OnRejected");
            }

            public override void OnTimedOut()
            {
                TRACE("OnTimedOut");
            }
        }
    }
}