#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Urbetrack.Compression;
using Urbetrack.InterQ.Core.Protocol;
using Urbetrack.Messaging.Opaque;
using Urbetrack.Net.TCP;
using Urbetrack.Toolkit;
using Stream=System.IO.Stream;
using Version=Urbetrack.InterQ.Core.Protocol.Version;

#endregion

namespace Urbetrack.InterQ.Core.Transport
{
    public class TcpInterQueueServer
    {
        #region Delegates

        public delegate bool MessageReceivedDelegate(
            int idDispositivo, byte operation, string filename, Stream file, bool status);

        #endregion

        private readonly TcpInterQueueAcceptor acceptor;
        private readonly Listener listener;

        public int FileTransferTraceLevel;

        public TcpInterQueueServer(IPEndPoint la, int trace_level, bool fsm_trace)
        {
            FileTransferTraceLevel = trace_level;
            acceptor = new TcpInterQueueAcceptor
                           {
                               FileTransferTraceLevel = FileTransferTraceLevel,
                               FileTransferFSMTrace = fsm_trace
                           };
            listener = new Listener("TcpRawServer", la, acceptor);
        }

        public void Close()
        {
            listener.Close();
        }

        public static event MessageReceivedDelegate MessageReceived;

        #region Nested type: TcpInterQueueAcceptor

        private class TcpInterQueueAcceptor : Acceptor
        {
            private readonly byte[] active_buffer = new byte[65535];
            private int buffer_position;
            private IPEndPoint source_endpoint;
            
            private Chunk active_chunk;
            private string version;

            public int FileTransferTraceLevel { get; set; }
            public bool FileTransferFSMTrace { get; set; }
            public Version ClientVersion { get; internal set; }
            private States state;
            public States State
            {
                get { return state; }
                internal set
                {
                    if (!FileTransferFSMTrace)
                    {
                        state = value;
                        return;
                    }
                    var title = "UIQS/TRANSITION[" + T.LOCALENDPOINT(os_socket) + "/" + T.REMOTEENDPOINT(os_socket) + "]";
                    var body = String.Format("       {0} ----> {1}", state, value);
                    T.FSM(Format.Boxed(body, title, 80));
                    state = value;
                }
            }

            public override void OnConnection()
            {
                State = States.WAITFOR_VERSION;
                source_endpoint = os_socket.RemoteEndPoint as IPEndPoint;
                if (source_endpoint == null)
                {
                    TRACE("Acceptor, no puede obtener el EP remoto.");
                    throw new Exception("no se puede obtener el host remoto.");
                }
                TRACE("Recibio una conexion de {0}", source_endpoint);
            }

            public override void OnDisconnect()
            {
                TRACE("Se ha desconectado el dispositivo de {0}", source_endpoint);
            }

            public override void TRACE(string format, object[] args)
            {
                if (T.ActiveContext.CurrentLevel < FileTransferTraceLevel) return;
                var nf = "UIQS[" + T.LOCALENDPOINT(os_socket) + "/" +
                         T.REMOTEENDPOINT(os_socket) + "]:" + format;
                T.TRACE(nf, args);
            }

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
                if (State == States.FAILURE)
                {
                    Disconnect();
                    return;
                }
                // agregarmos al buffer.
                TRACE("bloque de datos={0}", block.TotalBytes);
                Array.Copy(block.Data, 0, active_buffer, buffer_position, block.TotalBytes);
                buffer_position += block.TotalBytes;
                var continue_required = true;
                while (continue_required && buffer_position > 0)
                {
                    switch (State)
                    {
                        case States.WAITFOR_VERSION:
                            continue_required = OnReceiveWaitForVersion();
                            break;
                        case States.WAITFOR_CHUNK:
                            continue_required = OnReceiveWaitForChunk();
                            break;
                        case States.READING_CHUNK:
                            continue_required = OnReceiveReadingChunk();
                            break;
                        default:
                            Disconnect();
                            continue_required = false;
                            break;
                    }
                }
            }

            public bool SendOpaqueMessageAck(OpaqueMessage msg, OpaqueMessageAck.Responses Response)
            {
                var ack = new OpaqueMessageAck() {LookupId = msg.Id, Response = Response};
                // calculando tamaño
                var data = GZip.Serialize(ack);
                var datalength = data.GetLength(0);

                // enviando CHUNK
                var chunk_size = new byte[2];
                var b = Convert.ToByte((byte)(datalength >> 8));
                chunk_size[0] = b;
                chunk_size[1] = Convert.ToByte(datalength & 0xFF);
                Send(chunk_size, 2);

                // envia datos
                Send(data, datalength);

                return true;
            }

            private bool CompleteChunkReceive(Version Version, Stream Stream, int Size)
            {
                try
                {
                    TRACE("Chunk Recibido: {0}, total={1}", source_endpoint, Size);
                    OpaqueMessage om;
                    if (Version == Version.UIQP_1_2)
                    {
                        var obj = GZip.DecompressAndDeserialize(Stream);
                        om = (OpaqueMessage)obj;
                    } else
                    {
                        var formatter = new BinaryFormatter();
                        var obj = formatter.Deserialize(Stream);    
                        om = (OpaqueMessage) obj;
                    }
                    if (om == null)
                    {
                        TRACE("recibi un mensaje y no puede convertir a opaco.");
                        SendOpaqueMessageAck(om, OpaqueMessageAck.Responses.DataError);
                        return false;
                    }
                    var queue = new OpaqueMessageQueue("") {Name = om.DestinationQueueName};
                    queue.Push(om);
                    SendOpaqueMessageAck(om, OpaqueMessageAck.Responses.Enqueued);
                    TRACE("Mensaje encolado en {0}", om.DestinationQueueName);
                    return true;
                } catch (Exception e)
                {
                    TRACE("recibi un mensaje y no puede convertir a opaco.");
                    T.EXCEPTION(e, "TcpInterQueueAcceptor.CompleteChunkReceive");
                    return false;
                }

            }

            private bool OnReceiveReadingChunk()
            {
                if (active_chunk == null) throw new Exception("Se requiere un Chunk Activo.");
                var chunk_buffer = ActiveBufferPop(active_chunk.GetActiveBufferSize());
                if (chunk_buffer == null)
                {
                    TRACE("CHUNK ActiveBuffer/Faltan Datos!");
                    return false;
                }
                if (active_chunk.ReceiveData(chunk_buffer, chunk_buffer.GetLength(0)))
                {
                    var local_result = CompleteChunkReceive(ClientVersion, active_chunk.GetStream(), active_chunk.Size);

                    if (ClientVersion == Version.UIQP_1_1)
                    {
                        Send(Encoding.ASCII.GetBytes(local_result ? "A" : "N"), 1);
                        TRACE("Respondo {0} y cierro.", local_result ? "A" : "N");
                        Disconnect();
                        return false;
                    }

                    // volvemos a waiting for chunk.
                    State = States.WAITFOR_CHUNK;
                    active_chunk = null;
                    return true;
                }
                TRACE("CHUNK ReceiveData/Faltan Datos!");
                return true;
            }

            private bool OnReceiveWaitForChunk()
            {
                if (active_chunk == null) active_chunk = new Chunk(ClientVersion);
                State = States.READING_CHUNK;
                return OnReceiveReadingChunk();
            }

            private bool OnReceiveWaitForVersion()
            {
                var version_buffer = ActiveBufferPop(8);
                if (version_buffer == null) return false;
                version = Encoding.ASCII.GetString(version_buffer);
                switch (version)
                {
                    case "UIQP/1.1":
                        ClientVersion = Version.UIQP_1_1;
                        break;
                    case "UIQP/1.2":
                        ClientVersion = Version.UIQP_1_2;
                        break;
                    default:
                        ClientVersion = Version.INCOMPATIBLE;
                        TRACE("Version de protocolo desconocida [{0}]", version);
                        Disconnect();
                        return false;
                }
                State = States.WAITFOR_CHUNK;
                return true;
            }

            public override void Disconnect()
            {
                base.Disconnect();
                State = States.TERMINATED;
            }

            public override void OnInternalError()
            {
                TRACE("Error al recibir");
                Disconnect();
            }

            public override object Clone()
            {
                return new TcpInterQueueAcceptor {
                                                     ClientVersion = Version.UNDEFINED,
                                                     State = States.WAITFOR_VERSION,
                                                     FileTransferFSMTrace = FileTransferFSMTrace,
                                                     FileTransferTraceLevel = FileTransferTraceLevel
                                                 };
            }
        }

        #endregion
    }
}