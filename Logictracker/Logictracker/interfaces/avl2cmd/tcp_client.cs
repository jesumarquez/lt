using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace etao.marshall
{
    /// <summary>
    /// tcp_client representa una conexion saliente TCP.
    /// </summary>
    public abstract class tcp_client : tcp_base
    {
        /// <summary>
        /// constructor por defecto. este constructor define un buffer de 1024 bytes para
        /// lectura.
        /// </summary>
        public tcp_client() 
        : this(1024) {}

        /// <summary>
        /// constructor completo. este constructor permite definir el tamanio del buffer 
        /// de recepcion.
        /// </summary>
        /// <param name="buffersize">tamanio del buffer de rececpion en bytes.</param>
        public tcp_client(int buffersize)
        {
            _buffersize = buffersize;
            _read_buffer = new byte[_buffersize];
            _handler = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
        }
        
        /// <summary>
        /// callback, este metodo es llamado por el framework cuando se completa la 
        /// conexion asincronica.
        /// </summary>
        public abstract void on_connect();

        /// <summary>
        /// callback, este metodo es llamado por el framework cuando se completa con falla la 
        /// conexion asincronica.
        /// </summary>
        public abstract void on_rejected();

        /// <summary>
        /// callback, este metodo es llamado cuando se reciben datos por el socket.
        /// </summary>
        public abstract void on_data();

        /// <summary>
        /// callback, este metodo es llamado por el framework cuando se desconecta
        /// el socket desde el otro extremo de la conexion.
        /// </summary>
        public abstract void on_close();

        /// <summary>
        /// este metodo inicial la conexion asincronica al destino indicado.
        /// </summary>
        /// <param name="ep">endpoint a donde realizar la conexion.</param>
        public bool connect(IPEndPoint ep, bool sync)
        {
            try
            {
                if (sync) {
                    _handler.Connect(ep);
                    if (_handler.Connected) {
                        _handler.BeginReceive(this._read_buffer, 0, this._buffersize, 0, new AsyncCallback(this._ReadCallback), _handler);
                        return true;
                    }
                    return false;
                } else {
                    _handler.BeginConnect(ep, new AsyncCallback(this._ConnectedCallBack), _handler);
                    return true;
                }
            }
            catch (SocketException e)
            {
                System.Console.WriteLine("{0} {1}", System.DateTime.Now, e.ToString());
                return false;
            }            
        }

        /// <summary>
        /// callback interno, este metodo es utilizado internamente para recibir 
        /// el evento de conexion establecida.
        /// </summary>
        /// <param name="ar">parametro de conexion asincronica provisto por .NET FW</param>
        private void _ConnectedCallBack(IAsyncResult ar)
        {
            try
            {                
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                on_connect();
                _handler.BeginReceive(this._read_buffer, 0, this._buffersize, 0, new AsyncCallback(this._ReadCallback), _handler);
            }
            catch (SocketException e)
            {                
                System.Console.WriteLine("{0} {1}", System.DateTime.Now, e.ToString());
                on_rejected();
            }
        }

        /// <summary>
        /// callback interno, este metodo es utilizado internamente para recibir 
        /// el evento de recepcion de datos.
        /// </summary>
        /// <param name="ar">parametro de conexion asincronica provisto por .NET FW</param>
        private void _ReadCallback(IAsyncResult ar)
        {
            try
            {
                int read_bytes = _handler.EndReceive(ar);
                if (read_bytes > 0)
                {
                    _read_bytes = read_bytes;
                    _total_read_bytes += read_bytes;
                    on_data();
                }
                else
                {
                    if (is_connected(_handler))
                    {
                        on_close();
                        return;
                    }
                }
                _handler.BeginReceive(_read_buffer, 0, _buffersize, 0, new AsyncCallback(this._ReadCallback), _handler);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("{0} TCP CLIENT EXCEPCION {1}", System.DateTime.Now, e.Message);
            }
        }
    }
}
