#region Usings

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Core.Transaction;
using Urbetrack.Comm.Core.Transport.TCP;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Hacking;

#endregion

namespace Urbetrack.Comm.Core.Transport
{
    public class TransporteTCP : Transporte
    {
        private readonly Dictionary<IPEndPoint, acceptor> conexiones = new Dictionary<IPEndPoint, acceptor>();
        private IPEndPoint udp_fwd_host;
        private UdpClient udp_fwd; 
        private tcp_listener<acceptor> tcp_server;

        class acceptor : tcp_acceptor
        {
            IPEndPoint ep;
            public override void on_accept()
            {
                ep = _handler.RemoteEndPoint as IPEndPoint;
                STrace.Debug(GetType().FullName, String.Format("TCP: Se ha conectado el dispositivo de {0}", ep));
                if (ep == null)
                {
                    throw new Exception("no se puede obtener el host remoto.");
                }
                _data.conexiones.Add(ep, this);
            }

            private readonly byte[] active_buffer = new byte[65535];
            private int  buffer_position;
            private int  reset_position;

            public override void on_data()
            {
                // agregarmos al buffer.
                //Marshall.Debug(string.Format("tcp reactor, datos en buffer={0} recibidos={1}", buffer_position, get_read_bytes()));
                Array.Copy(get_buffer(), 0, active_buffer, buffer_position, get_read_bytes());
                buffer_position += get_read_bytes();
                while (buffer_position > 5)
                {
                    reset_position = 0;
                    // tengo el tamaño del pdu y options.
                    var size = UrbetrackCodec.DecodeShort(active_buffer, ref reset_position);
                    //byte options = UnetelCodec.DecodeByte(active_buffer, ref reset_position);
                    //Marshall.Debug(string.Format("tcp reactor, datos en buffer={0} recibidos={1} size={2}", buffer_position, get_read_bytes(), size));

                    // valido si esta completo en el buffer.
                    if ((size + 2) < (buffer_position)) return; // faltan datos
                    var pdu_buffer = new byte[size + 2];
                    
                    // copiamos la pdu.
                    Array.Copy(pdu_buffer, 0, active_buffer, 0, size + 2);
                    try
                    {
                        var ret = Codes.DecodeErrors.NoError;
                        var instance_buffer = new byte[get_read_bytes()];
                        Array.Copy(get_buffer(), instance_buffer, get_read_bytes());
                        if (_data.udp_fwd != null)
                        {
                            if (_data.udp_fwd_host != null)
                            {
                                STrace.Debug(GetType().FullName,string.Format("**** UDP FORWARD TO: {0}", _data.udp_fwd_host));
                                _data.udp_fwd.Send(instance_buffer, get_read_bytes(), _data.udp_fwd_host);
                            }
                        }
                        var pdu = _data.CODEC.Decode(instance_buffer, ref ret);
                        pdu.Transporte = _data;
                        pdu.Destino = new Destino {TCP = ep};

                        var d = Devices.I().FindById(pdu.IdDispositivo);

                        if (d != null)
                        {
                            d.Touch(pdu.Destino);
                        }
                        else
                        {
                            if ((Codes.HighCommand)pdu.CH != Codes.HighCommand.LoginRequest)
                            {
                                close();
                                return;
                            }
                        }

                        var t = _data.ObtenerTransaccion(pdu.IdTransaccion);
                        if (t == null)
                        {
                            if (pdu.CH < 0x80)
                            {
                                var mrs = new MRS(pdu, _data, _data.TransactionUser);
                                _data.NuevaTransaccion(mrs, pdu);
                                mrs.Start();
                            }
                        }
                        else
                        {
                            t.RecibePDU(pdu);
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        STrace.Exception(GetType().FullName,e);
                        throw;
                    }
                    catch (Exception e)
                    {
                        STrace.Exception(GetType().FullName,e);
                    }

                    // Rotamos el Array
                    Array.Copy(active_buffer, 0, active_buffer, size + 2, (buffer_position - (size + 2)));
                    buffer_position -= size + 2;
                }
            }

            public override void on_close()
            {
                if (ep == null)
                {
                    STrace.Debug(GetType().FullName,"acceptor, no puede eliminar el socket.");
                    throw new Exception("acceptor, no puede eliminar el socket.");
                }
                STrace.Debug(GetType().FullName, String.Format("TCP: Se ha desconectado el dispositivo de {0}", ep));
                _data.conexiones.Remove(ep);
            }
        }

        public override void Close(Destino dst)
        {
            if (conexiones.ContainsKey(dst.TCP))
            {
                conexiones[dst.TCP].close();
            }
        }

        public override bool Send(PDU pdu)
        {
            var size = 0;
            // to do: que el buffer se maneje automaticamente.
            var d = Devices.I().FindById(pdu.IdDispositivo);
            if (d != null) d.Touch(pdu.Destino);
            var instance_buffer = new byte[1024];
            //Marshall.Debug("---TCP SENDING-------------------------------------------------------------");
            //Marshall.Debug(pdu.Trace(""));
            //Marshall.Debug("---------------------------------------------------------------------------");
            CODEC.Encode(ref instance_buffer, ref size, pdu);
            if (!conexiones.ContainsKey(pdu.Destino.TCP))
            {
                STrace.Debug(GetType().FullName,"No existe la conexion TCP especificada.");
                return false;
            }
            conexiones[pdu.Destino.TCP].send(instance_buffer, size);
            return true;
        }

        public void AbrirTransporte(IPEndPoint local_address)
        {
            if (tcp_server != null) return;
            ConnectionMode = Hacker.TCP.Retransmit ? ConnectionModes.DatagramOriented : ConnectionModes.ConnectionOriented;
            tcp_server = new tcp_listener<acceptor>(local_address, this);
            STrace.Debug(GetType().FullName,string.Format("reactor tcp iniciado, address={0}", local_address));
        }

        public void CerrarTransporte()
        {
            tcp_server = null;
        }

        public void RenvioUDP(IPEndPoint dst)
        {
            udp_fwd_host = dst;
            udp_fwd = new UdpClient();
        }
    }
}