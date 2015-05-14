using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace etao.marshall
{
    public class tcp_listener<T> where T:tcp_acceptor,new()
    {
        IPEndPoint _listen_address;
        Socket     _listener;
        int _a_data;

        public tcp_listener(IPEndPoint listen_address, int data)
            : this(listen_address, data, 100)
        {
            _a_data = data;
        }

        public tcp_listener(IPEndPoint listen_address, int data , int max_pendings)
        {
            _listen_address = listen_address;
            _listener = new Socket(AddressFamily.InterNetwork, 
                                    SocketType.Stream, ProtocolType.Tcp);
            
            _listener.Bind(_listen_address);
            _listener.Listen(max_pendings);
            _listener.BeginAccept(new AsyncCallback(this._AcceptCallback), _listener);
        }

        private void _AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            tcp_acceptor acceptor = new T();
            acceptor._data = _a_data;
            acceptor._handler = handler;
            acceptor.on_accept();

            _listener.BeginAccept(new AsyncCallback(this._AcceptCallback), _listener);
            handler.BeginReceive(acceptor._read_buffer, 0, acceptor._buffersize, 0, new AsyncCallback(this._ReadCallback), acceptor);
        }

        private void _ReadCallback(IAsyncResult ar)
        {
            try
            {

                tcp_acceptor acceptor = (tcp_acceptor)ar.AsyncState;
                int read_bytes = acceptor._handler.EndReceive(ar);
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
                acceptor._handler.BeginReceive(acceptor._read_buffer, 0, acceptor._buffersize, 0, new AsyncCallback(this._ReadCallback), acceptor);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("{0} TCP LISTENER EXCEPCION {1}", System.DateTime.Now, e.Message);
            }
        }
    }

}
