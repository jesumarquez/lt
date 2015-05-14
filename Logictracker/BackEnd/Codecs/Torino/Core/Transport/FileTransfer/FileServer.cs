#region Usings

using System;
using System.IO;
using System.Net;
using System.Text;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.InetLayer.TCP;
using Stream = System.IO.Stream;

#endregion

namespace Urbetrack.Comm.Core.Transport.FileTransfer
{
    public class FileServer
    {
        readonly Listener listener;
        private readonly FileServerAcceptor acceptor;

		public FileServer(IPEndPoint listen_address)
        {
            acceptor = new FileServerAcceptor();
			listener = new Listener("FileServer", listen_address, acceptor);
        }

        public void Close()
        {
            listener.Close();
        }

        public delegate bool MessageReceivedDelegate(int idDispositivo, byte operation, string filename, Stream file, bool status);

        static public event MessageReceivedDelegate MessageReceived;

        class FileServerAcceptor : Acceptor
        {
            IPEndPoint ep;
            private readonly byte[] active_buffer = new byte[65535];
            private int buffer_position;
            private short deviceId;
            private int chunk_size;
            private string label;
            private byte op = 0xFF;
            private bool headers_readed;
            private int payload_start = 71;
            private Stream strm;

            public override void OnConnection()
            {
                ep = os_socket.RemoteEndPoint as IPEndPoint;
                if (ep == null)
                {
                    throw new Exception("no se puede obtener el host remoto.");
                }
				STrace.Debug(GetType().FullName, String.Format("Recibio una conexion de {0}", ep));
            }

	        protected override void OnDisconnect()
            {
				STrace.Debug(GetType().FullName, String.Format("Se ha desconectado el dispositivo de {0}", ep));
            }

	        protected override void OnReceive(StreamBlock block)
            {
                // agregarmos al buffer.
				STrace.Debug(GetType().FullName, deviceId, String.Format("FileServer: bloque de datos={0}", block.TotalBytes));
                Array.Copy(block.Data, 0, active_buffer, buffer_position, block.TotalBytes);
                buffer_position += block.TotalBytes;
                if (!headers_readed)
                {
                    if (buffer_position >= 71)
                    {
                        // leer la cabecera... podemos
                        // Short            IdDispositivo
                        // Byte             Destino (0x00 = File, 0x01 = Queue)
                        // Integer          Tamaño del Archivo (setea $FileSize)
                        // Bytes[64]        Nombre de Archivo o Base64MessageQueue segun corresponda (Rellenado con 0)
                        // Bytes[$FileSize] Datos del archivo.
                        var pos = 0;
                        deviceId = UrbetrackCodec.DecodeShort(active_buffer, ref pos);
                        op = UrbetrackCodec.DecodeByte(active_buffer, ref pos);
                        chunk_size = UrbetrackCodec.DecodeInteger(active_buffer, ref pos);
                        label = UrbetrackCodec.DecodeBytesAsString(active_buffer, ref pos, 64);
                        headers_readed = true;
                        strm = new MemoryStream();
						STrace.Debug(GetType().FullName, deviceId, String.Format("Iniciando Recepcion: queue={0} size={1}", label, chunk_size));
                    }
                    else return; // aun no leimos suficiente.
                }

                while (buffer_position > payload_start)
                {
					STrace.Debug(GetType().FullName, deviceId, String.Format("Recibiendo bloque: size={0}", buffer_position - payload_start));
                    var data_block = new byte[buffer_position - payload_start];
                    Array.Copy(active_buffer, payload_start, data_block, 0, buffer_position - payload_start);
                    strm.Write(data_block, 0, buffer_position - payload_start);
                    buffer_position = payload_start = 0;
                    if (strm.Length != chunk_size) continue;
                    bool local_result;
                    
                    try
                    {
                        local_result = MessageReceived(deviceId, op, label, strm, true);
                    }
                    catch (Exception e)
                    {
                        STrace.Exception(GetType().FullName, e);
                        local_result = false;
                    }

                    Send(Encoding.ASCII.GetBytes(local_result ? "A" : "N"), 1, deviceId, "MemoryStream");
					STrace.Debug(GetType().FullName, deviceId, String.Format("Respondo {0} y cierro.", local_result ? "A" : "N"));
                    Disconnect();
                }                
            }

            public override object Clone()
            {
                return new FileServerAcceptor();
            }
        }
    }
}