using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace etao.marshall
{
    /// <summary>
    /// esta clase es la base de las clases tcp_acceptor y tcp_client. ambas representan
    /// una conexion unica TCP. la primera es instanciada automaticamnete por el tcp_listener
    /// lo otra es instanciada por el usuario.
    /// </summary>
    public class tcp_base
    {
        internal Socket _handler;
        internal int _buffersize;
        internal byte[] _read_buffer;
        internal int _read_bytes;
        public int _total_read_bytes;

        /// <summary>
        /// Este metodo retorna el tamanio del buffer de recepcion que se definio
        /// en el constructor.
        /// </summary>
        /// <returns>tamanio del buffer de recepcion de datos en bytes.</returns>
        public int get_buffer_size() { return _buffersize; }

        /// <summary>
        /// cuando el tcp_listener llama a on_data, este metodo puede ser utilizado
        /// y retorna la cantidad de bytes disponibles en el buffer de lectura.       
        /// el metodo Read es llamado internamente por el tcp_listener y almacena en el
        /// buffer todos los bytes leidos del stream. 
        /// </summary>
        /// <returns>cantidad de bytes disponibles en el buffer.</returns>
        public int get_read_bytes() { return _read_bytes; }

        /// <returns>cantidad de bytes disponibles en el buffer.</returns>
        public int get_total_read_bytes() { return _total_read_bytes; }

        /// <summary>
        /// este metodo es utilizado para obtener el buffer de lectura del socket.
        /// el tamanio fisico de este buffer es determinado por el metodo get_buffer_size
        /// de todos modos, la cantidad real de datos disponibles en dicho buffer esta
        /// determinada por el metodo get_read_bytes.
        /// </summary>
        /// <returns>retorna una referencia al buffer de lectura del socket.</returns>
        public byte[] get_buffer() { return _read_buffer; }

        /// <summary>
        /// envia datos al peer, se debe indicar la cantidad de bytes que es necesario
        /// enviar del buffer provisto.
        /// </summary>
        /// <param name="buffer">buffer que contiene los datos a ser enviados.</param>
        /// <param name="buffer_size">cantidad de bytes que se requiere enviar de dicho buffer.</param>
        public void send(byte[] buffer, int buffer_size)
        {
            _handler.Send(buffer, buffer_size, SocketFlags.None);
        }
        public void send(string s)
        {
            _handler.Send(Encoding.ASCII.GetBytes(s.ToCharArray(), 0, s.Length));
        }
        /// <summary>
        /// este metodo cierra ordenadamente la conexion. luego de ejecutado este metodo
        /// intentar ejecutar send lanzara una excepcion.
        /// </summary>
        public void close()
        {            
            _handler.Shutdown(SocketShutdown.Both);
            _handler.Close();
        }

        /// <summary>
        /// este metodo verifica de forma segura si un socket aun esta conectado
        /// al peer remotor.
        /// </summary>
        /// <param name="s">socket sobre el cual se quiere hacer la verificacion.</param>
        /// <returns>verdadero si el socket aun esta conectado.</returns>            
        internal static bool is_connected(Socket s)
        {
            bool blockingState = s.Blocking;
            bool return_value = false;
            try
            {
                byte[] tmp = new byte[1];

                s.Blocking = false;
                s.Send(tmp, 0, 0);
                return_value = true; // conectado
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                return_value = e.NativeErrorCode.Equals(10035);
            }
            finally
            {
                s.Blocking = blockingState;
            }
            return return_value;
        }
    }
}
