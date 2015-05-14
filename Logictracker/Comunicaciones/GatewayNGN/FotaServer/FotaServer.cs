#region Usings

using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Toolkit;
using Urbetrack.Net.TCP;

#endregion

namespace Urbetrack.Comm.FotaServer
{
    public class FotaServer
    {
        readonly Listener listener;
        private FotaAcceptor acceptor;
        private readonly IPEndPoint LocalEndPoint;

        public FotaServer(IPEndPoint LocalEndPoint)
        {
            this.LocalEndPoint = LocalEndPoint;
            acceptor = new FotaAcceptor();  
            listener = new Listener("FotaServer", LocalEndPoint, acceptor);
        }

        public void Close()
        {
            listener.Close();
        }

        class FotaAcceptor : Acceptor
        {
            private readonly byte[] active_buffer = new byte[65535];
            private int buffer_position;
            private IPEndPoint source_endpoint;

            private string IMEI;

            bool SendFile(Device d, string name, string src_file, string path)
            {
                // var path = Process.GetApplicationFolder("temp");
                var full_file = String.Format(@"{0}\{1}", path, src_file);
                var fota_file = String.Format(@"{0}\{1}.l4pnp", path, src_file);

                if (!File.Exists(full_file))
                {
                    Send("E");
                    //T.ERROR("DEVICE[{0}]/FOTA: el archivo '{1}' no existe o no hay permisos.", d.LogId, full_file);
                    return false;
                }

                IO.PadFile(full_file, fota_file, 512, 0x00, true);

                using (var fs = File.OpenRead(fota_file))
                {
                    var filesize = (int)fs.Length;
                    if (filesize % 512 != 0)
                    {
                        //T.ERROR("DEVICE[{0}]/FOTA: el archivo '{1}' no existe o no hay permisos.", d.LogId, full_file);
                        Send("C");
                        return false;
                    }
                    var filepages = filesize/512;
                    var md5Hasher = MD5.Create();
                    var b = md5Hasher.ComputeHash(fs);
                    if (b.GetLength(0) != 16) throw new Exception("el hash md5 no retorna 16 bytes.");
                    var buffer = new byte[256];
                    var pos = 0;
                    Send("D");
                    UrbetrackCodec.EncodeString(ref buffer, ref pos, name);
                    UrbetrackCodec.EncodeInteger(ref buffer, ref pos, filepages);
                    UrbetrackCodec.EncodeBytes(ref buffer, ref pos, b);
                    Send(buffer, pos);
                }

                using (var fs = File.OpenRead(fota_file))
                {
                    var b = new byte[512];
                    while (fs.Read(b, 0, 512) > 0)
                    {
                        Send(b, 512);
                    }
                }

                return true;
            }

            public override void TRACE(string format, params object[] args)
            {
                T.TRACE(0, format, args);
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

            protected byte[] ActiveBufferPopTextLine()
            {
                /*if (buffer_position == 0) return null;
                var cr = Array.IndexOf(active_buffer, '\r', 0, buffer_position);
                var ln = Array.IndexOf(active_buffer, '\n', 0, buffer_position);
                if (cr == ln) return null;
                var eol = 0;
                
                eol = Math.Min(cr, ln);
                
                var return_buffer = new byte[count];
                Array.Copy(active_buffer, 0, return_buffer, 0, count);
                buffer_position -= count;
                if (buffer_position == 0) return return_buffer;
                Array.Copy(active_buffer, count, active_buffer, 0, buffer_position);
                return return_buffer;*/
                return null;
            }

            public override void OnReceive(StreamBlock block)
            {
                TRACE("bloque de datos={0}", block.TotalBytes);
                Array.Copy(block.Data, 0, active_buffer, buffer_position, block.TotalBytes);
                buffer_position += block.TotalBytes;

                var hdrs = ActiveBufferPop(27);
                var fields = Encoding.ASCII.GetString(hdrs).Split("/".ToCharArray());

                if (fields.GetLength(0) != 4)
                {
                    Send("TENGO UN PATO!");
                    Disconnect();
                    return;
                }

                IMEI = fields[2];

                var d = Devices.i().Find(IMEI);

                /*if (d == null)
                {
                    T.TRACE("FOTASERVER: DEVICE w/IMEI={0} no encontrado");
                    send("YOU ARE BANNED HERE!");
                    close();
                    return;
                }

                T.TRACE("FOTASERVER: DEVICE[{0}] conectado", d.LogId);
*/
                if (fields[3] != "REQ") return;
                //T.TRACE("FOTASERVER: DEVICE[{0}] solicita firmware", d.LogId);
                // var file = "rnr.bin";
                var file = "rnr.bin";
                //var path = @"C:\Documents and Settings\evecchio\Escritorio\rnros\trunk\torino\obj\";
                var path = @"C:\Documents and Settings\evecchio\Escritorio\rnros\trunk\torino\obj";
                Send("L4PNP");
                SendFile(d, "TEST", file, path);
                //close();
            }

            public override void OnDisconnect()
            {
                base.Disconnect();
            }

            public override void OnInternalError()
            {
                TRACE("Error al recibir");
                Disconnect();
            }

            public override void OnConnection()
            {
                source_endpoint = os_socket.RemoteEndPoint as IPEndPoint;
                if (source_endpoint == null)
                {
                    TRACE("Acceptor, no puede obtener el EP remoto.");
                    throw new Exception("no se puede obtener el host remoto.");
                }
                TRACE("Recibio una conexion de {0}", source_endpoint);
            }

            public override object Clone()
            {
                return new FotaAcceptor();
            }
        }
    }
}