using System;
using System.Text;
using System.IO;
using System.Net;
using Urbetrack.Mobile.Net.TCP;
using Urbetrack.Mobile.Toolkit;
using Decoder=Urbetrack.Mobile.Comm.Messages.Decoder;
using Stream =System.IO.Stream;

namespace Urbetrack.Mobile.Comm.Transport.FileTransfer
{
    public class FileServer
    {
        Listener listener;
        private readonly FileServerAcceptor acceptor;

        public static int AcceptedConnections { get; set; }
        public static int ClosedConnections { get; set; }
        public static int SuccesfulReceivedMessages { get; set; }
        public static int AbortsReceivingMessages { get; set; }

        public FileServer(IPEndPoint la)
        {
            acceptor = new FileServerAcceptor();
            listener = new Listener("FileServer",la, acceptor);
        }

        public void close()
        {
            listener.Close();
            listener = null;
        }

        public delegate bool ObjectReceivedHandler(object sender, int idDispositivo, string filename, Stream file, bool status);

        static public event ObjectReceivedHandler MessageReceived;

        class FileServerAcceptor : Acceptor
        {
            IPEndPoint ep;
            
            public override void OnConnection()
            {
                ep = os_socket.RemoteEndPoint as IPEndPoint;
                AcceptedConnections++;

                if (ep == null)
                {
                    T.ERROR("acceptor, no puede obtener el EP remoto.");
                    throw new Exception("no se puede obtener el host remoto.");
                }
                
                TRACE("UIQ: recibio una conexion de {0}", ep);
                
            }

            protected override void TRACE(string format, object[] args)
            {
                T.TRACE(StreamTraceLevel, format, args);
            }

            private readonly byte[] active_buffer = new byte[65535];
            private int buffer_position;
            private short idDispositivo;
            private int chunk_size;
            private byte[] bfilename;
            private string filename;
            private byte op = 0xFF;
            private bool headers_readed;
            private int payload_start = 71;
            private int payload_size;
            private Stream strm;

            public override void OnReceive(StreamBlock block)
            {
                // agregarmos al buffer.
                TRACE("datos en buffer={0} recibidos={1}", buffer_position, block.TotalBytes);
                Array.Copy(block.Data, 0, active_buffer, buffer_position, block.TotalBytes);
                buffer_position += block.TotalBytes;
                if (!headers_readed)
                {
                    if (buffer_position >= 71)
                    {
                        // leer la cabecera... podemos
                        // Short            DeviceId
                        // Byte             destination (0x00 = File, 0x01 = Queue)
                        // Integer          Tamaño del Archivo (setea $FileSize)
                        // Bytes[64]        Name de Archivo o Base64MessageQueue segun corresponda (Rellenado con 0)
                        // Bytes[$FileSize] Datos del archivo.
                        var pos = 0;
                        idDispositivo = Decoder.DecodeShort(active_buffer, ref pos);
                        op = Decoder.DecodeByte(active_buffer, ref pos);
                        chunk_size = Decoder.DecodeInteger(active_buffer, ref pos);
                        bfilename = Decoder.DecodeBytes(active_buffer, ref pos, 64);
                        int zpos;
                        for (zpos = 0; zpos < 64; ++zpos)
                            if (bfilename[zpos] != 0) continue;
                        filename = Encoding.ASCII.GetString(bfilename,0,zpos);
                        headers_readed = true;

                        switch (op)
                        {
                            case 0x01:
                                strm = new MemoryStream();
                                break;
                            default:
                                AbortsReceivingMessages++;
                                throw new Exception("UIQS: Tipo de operacion desconocida.");
                        }
                        TRACE("Recibiedo {0}{1} size={2} dev={3}", (op == 0x01 ? "Mensaje para cola=" : "Archivo nombre="), filename, chunk_size, idDispositivo);
                    }
                    else return; // aun no leimos suficiente.
                }
                while (buffer_position > payload_start)
                {
                    T.TRACE(String.Format("UIQS: Recibiedo bloque: size={0}", buffer_position - payload_start));
                    payload_size += buffer_position - payload_start;
                    var data_block = new byte[buffer_position - payload_start];
                    Array.Copy(active_buffer, payload_start, data_block, 0, buffer_position - payload_start);
                    strm.Write(data_block, 0, buffer_position - payload_start);
                    buffer_position = payload_start = 0;
                    if (strm.Length != chunk_size) continue;
                    bool local_result;
                    try
                    {
                        local_result = complete_receive();
                    }
                    catch (Exception e)
                    {
                        T.EXCEPTION(e, "RECEIVE DATA");
                        local_result = false;
                    }

                    Send(Encoding.ASCII.GetBytes(local_result ? "A" : "N"), 1);
                    TRACE("Respondo {0} y cierro.", local_result ? "A" : "N");
                    Disconnect();
                }
            }

            private bool complete_receive()
            {
                TRACE("Se ha desconectado el dispositivo de {0}", ep);
                return MessageReceived != null && MessageReceived(this, idDispositivo, filename, strm, (chunk_size != 0 && payload_size == chunk_size));
            }

            public override void OnDisconnect()
            {
                ClosedConnections++;
            }

            public override void OnInternalError()
            {
                TRACE("Error al recibir");
                Disconnect();    
            }

            public override object Clone()
            {
                return new FileServerAcceptor();
            }
        }

    }
}