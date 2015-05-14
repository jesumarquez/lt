#region Usings

using System;
using System.Net;
using System.Net.Sockets;
using Urbetrack.DatabaseTracer.Core;

#endregion

namespace Urbetrack.Comm.Core.Transport.TCP
{
    internal class tcp_listener<T> where T : tcp_acceptor, new()
    {
        readonly IPEndPoint _listen_address;
        readonly Socket     _listener;
        readonly TransporteTCP _a_data;

        public tcp_listener(IPEndPoint listen_address, TransporteTCP data)
        {
            _a_data = data;
            _listen_address = listen_address;
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(_listen_address);
            _listener.Listen(100);
            _listener.BeginAccept(_AcceptCallback, _listener);
        }

        public tcp_listener(IPEndPoint listen_address)
        {
            _listen_address = listen_address;
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _listener.Bind(_listen_address);
            _listener.Listen(10);
            _listener.BeginAccept(_AcceptCallback, _listener);
        }

        public void close()
        {            
            _listener.Close();
        }

        private void _AcceptCallback(IAsyncResult ar)
        {
            try
            {
                var listener = (Socket)ar.AsyncState;
                var handler = listener.EndAccept(ar);

                tcp_acceptor acceptor = new T();
                acceptor._data = _a_data;
                acceptor._handler = handler;
                acceptor.on_accept();

                _listener.BeginAccept(_AcceptCallback, _listener);                
                handler.BeginReceive(acceptor._read_buffer, 0, acceptor._buffersize, 0, _ReadCallback, acceptor);
            }
            catch (ObjectDisposedException)
            {
                //Marshall.User("Excepcion ODE en cb de aceptar conexion: {0}", oe.Message);
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName, String.Format("Excepcion inesperada en cb de aceptar conexion: {0}", e.Message));
            }
        }

        private void _ReadCallback(IAsyncResult ar)
        {
            var acceptor = (tcp_acceptor)ar.AsyncState;
            try
            {
                var read_bytes = acceptor._handler.EndReceive(ar);
                STrace.Debug(GetType().FullName, String.Format("tcputils: se leyeron, {0} bytes.", read_bytes));
                if (read_bytes > 0)
                {
                    acceptor._read_bytes = read_bytes;
                    acceptor._total_read_bytes += read_bytes;
                    acceptor.on_data();
                }
                else
                {
                    if (tcp_base.is_connected(acceptor._handler))
                    {
                        acceptor.on_close();
                        return;
                    }
                }
                acceptor._handler.BeginReceive(acceptor._read_buffer, 0, acceptor._buffersize, 0, _ReadCallback, acceptor);
            }
            catch (IndexOutOfRangeException)
            {
                throw;
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName, String.Format("TCP: Excepcion en lectura: {0}", e.Message));
                acceptor.on_close();
            }
        }
    }
}