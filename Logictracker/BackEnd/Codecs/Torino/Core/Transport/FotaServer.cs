#region Usings

using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Transport.TCP;
using Urbetrack.DatabaseTracer.Core;

#endregion

namespace Urbetrack.Comm.Core.Transport
{
    public class FotaServer
    {
        readonly tcp_listener<acceptor> listener;
        private readonly IPEndPoint LocalEndPoint;
        public FotaServer(IPEndPoint LocalEndPoint)
        {
            this.LocalEndPoint = LocalEndPoint;
            listener = new tcp_listener<acceptor>(LocalEndPoint);
        }

        public void Start()
        {
            STrace.Debug(GetType().FullName, String.Format("FOTASERVER: Escuchando en direccion {0}", LocalEndPoint.Address));
        }

        public void Close()
        {
            listener.close();
        }

        class acceptor : tcp_acceptor
        {
            IPEndPoint ep;

            public override void on_accept()
            {
                ep = _handler.RemoteEndPoint as IPEndPoint;
                                
                if (ep == null)
                {
                    STrace.Debug(GetType().FullName,"acceptor, no puede obtener el EP remoto.");
                    throw new Exception("no se puede obtener el host remoto.");
                }

                STrace.Debug(GetType().FullName,String.Format("FotaServer: recibio una conexion de {0}", ep));
                
            }

            public override void on_data()
            {

                STrace.Debug(GetType().FullName, String.Format("FotaServer: datos recibidos={0}", get_read_bytes()));

                var fields = Encoding.ASCII.GetString(get_buffer(), 0, get_read_bytes()).Split("/".ToCharArray());

                if (fields.GetLength(0) < 4)
                {
                    send("X");
                    close();
                    return;
                }

                send("L4PNP");
                const string specs = "RNR-OSv1-Torino(R)-BL";
                const string path = @"C:\Documents and Settings\Administrator\Desktop\rnros\trunk\torino\bin";

                if (fields[3] == "REQ")
                {
                    var signature = IniFile.Get(path + @"\" + specs + ".ini", "install", "image.signature", "");
                    if (string.IsNullOrEmpty(signature))
                    {
                        send("E");
                        close();
                        return;
                    }
                    var order = IniFile.Get(specs + ".ini", "", "image.install.order", "0");

                    const string file = specs + ".bin";
                    SendFile(order, signature, file, path);
                }
                else 
                if (fields[3] == "E01")
                {
                    send("C");
                }
            }

            void SendFile(string order, string signature, string src_file, string path)
            {
                var full_file = String.Format(@"{0}\{1}", path, src_file);
                var fota_file = String.Format(@"{0}\{1}.l4pnp", path, src_file);

                if (!File.Exists(full_file))
                {
                    send("E");
                	return;
                }

                IO.PadFile(full_file, fota_file, 512, 0x00, true);

                using (var fs = File.OpenRead(fota_file))
                {
                    var filesize = (int)fs.Length;
                    if (filesize % 512 != 0)
                    {
                            send("E");
                    	return;
                    }
                    var filepages = filesize/512;
                    var md5Hasher = MD5.Create();
                    var b = md5Hasher.ComputeHash(fs);
                    if (b.GetLength(0) != 16) throw new Exception("el hash md5 no retorna 16 bytes.");
                    var buffer = new byte[256];
                    var pos = 0;
                    send("D");
                    UrbetrackCodec.EncodeByte(ref buffer, ref pos, (byte)(Convert.ToInt16(order) & 0xFF));
                    UrbetrackCodec.EncodeString(ref buffer, ref pos, signature);
                    UrbetrackCodec.EncodeInteger(ref buffer, ref pos, filepages);
                    UrbetrackCodec.EncodeBytes(ref buffer, ref pos, b);
                    send(buffer, pos);
                }

                using (var fs = File.OpenRead(fota_file))
                {
                    var b = new byte[512];
                    while (fs.Read(b, 0, 512) > 0)
                    {
                        send(b, 512);
                    }
                }

                var cpos = 0;
                var commit = new byte[3];
                UrbetrackCodec.EncodeByte(ref commit, ref cpos, (byte)'A');
                UrbetrackCodec.EncodeByte(ref commit, ref cpos, (byte)(Convert.ToInt16(order) & 0xFF));
                UrbetrackCodec.EncodeByte(ref commit, ref cpos, (byte)'R');
                send(commit, 3);
            }


            public override void on_close()
            {
                if (ep == null)
                {
                    throw new Exception("FOTASERVER: no puede eliminar el socket.");
                }
            }
        }
    }
}