using System;
using System.Net;
using System.Net.Sockets;
using Urbetrack.Mobile.Comm.Messages;
using Urbetrack.Mobile.Comm.Transaccional;
using Urbetrack.Mobile.Toolkit;

namespace Urbetrack.Mobile.Comm.Transport
{
    public class TransporteUDP : AbstractTransport
    {
        private int _buffersize;
        private IPEndPoint _local_address;
        private byte[] _read_buffer;
        private int _read_bytes;
        private EndPoint _remote_address = new IPEndPoint(IPAddress.Any, 0);
        private Socket _socket;
        public IPEndPoint ServerAddress { get; set; }

        public void AbrirTransporte(IPEndPoint local_address, int buffersize, string _role)
        {
            RequiereRetransmicion = true;
            _buffersize = buffersize;
            _read_buffer = new byte[_buffersize];
            _local_address = local_address;
            _socket = new Socket(AddressFamily.InterNetwork,
                                 SocketType.Dgram, ProtocolType.Udp);

            _socket.Bind(_local_address);
            _socket.BeginReceiveFrom(_read_buffer, 0, _buffersize, 0, ref _remote_address, _ReadCallback, _socket);
            T.TRACE(string.Format("UDP: reactor iniciado, address={0}", _local_address));
        }

        public void CerrarTransporte()
        {
            _socket.Close();
        }

        private int get_read_bytes()
        {
            return _read_bytes;
        }

        private byte[] get_buffer()
        {
            return _read_buffer;
        }

        private bool sendTo(byte[] buffer, int buffer_size, IPEndPoint remote_ep)
        {
            if (HACK_DisableSocketWrite)
            {
                T.INFO(string.Format("udp pdu hackeado al enviar, remote address={0}", remote_ep));
                return true;
            }
            try
            {
                var bytes = _socket.SendTo(buffer, buffer_size, SocketFlags.None, remote_ep);
                T.TRACE(string.Format("udp pdu enviado, remote address={0}", remote_ep));
                return bytes == buffer_size;
            }
            catch
            {
                return false;
            }
        }

        private void _ReadCallback(IAsyncResult ar)
        {
            try
            {
                _read_bytes = _socket.EndReceiveFrom(ar, ref _remote_address);
                if (HACK_DisableSocketRead)
                {
                    T.INFO(string.Format("udp reactor, hackeado, tamaño={0}", _read_bytes));
                    _socket.BeginReceiveFrom(_read_buffer, 0, _buffersize, 0, ref _remote_address, _ReadCallback,
                                             _socket);
                    return;
                }
                T.TRACE(string.Format("UDP: reactor, recibio datagrama de size={0}", _read_bytes));
                try
                {
                    T.TRACE("---RECEIVING---------------------------------------------------------------");

                    var ret = Decoder.DecodeErrors.NoError;
                    var instance_buffer = new byte[get_read_bytes()];
                    Array.Copy(get_buffer(), instance_buffer, get_read_bytes());
                    var pdu = Decoder.Decode(instance_buffer, ref ret);
                    pdu.Transport = this;
                    pdu.Destination = new Destination();
                    pdu.Destination.UDP = _remote_address as IPEndPoint;

                    T.TRACE(pdu.Trace(""));
                    Transaction t;
                    t = ObtenerTransaccion(pdu.TransactionId);
                    T.TRACE("UDP: trid=" + pdu.TransactionId);
                    if (t == null)
                    {
                        TransactionUser ut = null;
                        // Options 0 = mensaje de dispositivo.
                        if (pdu.Options == 0x00)
                        {
                            T.TRACE("UDP: transaccion de usuario. ");
                            ut = TransactionUser;
                        }
                            // Options 1 = mensaje de control de red.
                        else if (pdu.Options == 0x01)
                        {
                            T.TRACE("UDP: transaccion para modulo de managment ch=" + pdu.CH);
                            ut = Managment;
                        }

                        if (ut != null && pdu.CH < 0x80)
                        {
                            T.TRACE("UDP: transaccion nueva ch=" + pdu.CH);
                            var mrs = new MRS(pdu, this, ut);
                            NuevaTransaccion(mrs, pdu);
                            mrs.Start();
                        }
                        else
                        {
                            T.TRACE("UDP: pdu huerfana seq=" + pdu.Seq + " CH=" + pdu.CH + " CL=" + pdu.CL);
                        }
                    }
                    else
                    {
                        T.TRACE("UDP: transaccion existente recibe PDU");
                        t.ReceivedPDU(pdu);
                    }
                }
                catch (Exception e)
                {
                    T.TRACE(
                        string.Format("UDP: exception procesando PDU, local_address={1} remote address={0} texto={2}",
                                      _remote_address, _local_address, e));
                }
                finally
                {
                    T.TRACE("---------------------------------------------------------------------------");
                    _socket.BeginReceiveFrom(_read_buffer, 0, _buffersize, 0, ref _remote_address, _ReadCallback,
                                             _socket);
                }
            }
            catch (ObjectDisposedException)
            {
                // se ignora como excepcion, ya que se dispara por invocar Close del socket.
                T.TRACE(string.Format("excepcion, se asume que el reactor de udp esta detenido, address={0}",
                                      _local_address));
            }
            catch (SocketException)
            {
                // se ignora por problemas de red.
            }
        }

        public override bool Send(PDU pdu)
        {
            var size = 0;
            // todo: que el buffer se maneje automaticamente.
            var instance_buffer = new byte[1024];
            T.TRACE("---SENDING-----------------------------------------------------------------");
            T.TRACE(pdu.Trace(""));
            T.TRACE("UDP: trid=" + pdu.TransactionId);

            T.TRACE("---------------------------------------------------------------------------");
            Decoder.Encode(ref instance_buffer, ref size, pdu);
            return sendTo(instance_buffer, size, pdu.Destination.UDP);
        }
    }
}