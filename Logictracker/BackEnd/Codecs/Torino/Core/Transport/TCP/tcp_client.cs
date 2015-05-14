#region Usings

using System;
using System.Net;
using System.Net.Sockets;
using Urbetrack.DatabaseTracer.Core;

#endregion

namespace Urbetrack.Comm.Core.Transport.TCP
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
        protected tcp_client() 
            : this(1024) {}

        /// <summary>
        /// constructor completo. este constructor permite definir el tamanio del buffer 
        /// de recepcion.
        /// </summary>
        /// <param name="buffersize">tamanio del buffer de rececpion en bytes.</param>
        protected tcp_client(int buffersize)
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
        /// <param name="sync">indica si la conexion debe hacerse sincronica o asicnronicamente.</param>
        public bool connect(IPEndPoint ep, bool sync)
        {
            try
            {
                if (sync) {
                    _handler.Connect(ep);
                    if (_handler.Connected) {
                        _handler.BeginReceive(_read_buffer, 0, _buffersize, 0, _ReadCallback, _handler);
                        return true;
                    }
                    return false;
                }
                _handler.BeginConnect(ep, _ConnectedCallBack, _handler);
                return true;
            }
            catch (SocketException e)
            {
                STrace.Log(GetType().FullName, e, 0, LogTypes.Trace, null, e.Message);
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
                var client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                on_connect();
                _handler.BeginReceive(_read_buffer, 0, _buffersize, 0, _ReadCallback, _handler);
            }
            catch (SocketException e)
            {
                STrace.Exception(GetType().FullName, e);
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
                var read_bytes = _handler.EndReceive(ar);
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
                _handler.BeginReceive(_read_buffer, 0, _buffersize, 0, _ReadCallback, _handler);
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName, e);
            }
        }
    }
}