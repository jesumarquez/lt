namespace Urbetrack.Comm.Core.Transport.TCP
{
    /// <summary>
    /// tcp_acceptor es una envoltura que permite realizar conexiones asincronicas TCP
    /// de forma rapida. Se debe implementar una clase que herede de tcp_acceptor e
    /// implementar los metodos abstractos que esta clase posee. on_accept, on_data,
    /// on_close. estos eventos seran llamados por el tcp_listener cuando se disparen
    /// los eventos asociados. 
    /// el metodo de uso se ve en el siguiente ejemplo:
    /// 
    /// using etao.marshall;
    /// 
    /// public class example : tcp_acceptor
    /// {
    ///         public override void on_accept() {
    ///             System.Console.WriteLine("{0} accepto conexion", System.DateTime.Now);
    ///         }
    ///         public override void on_data() {
    ///             System.Console.WriteLine("{0} recibio datos, {1}[{2}]", System.DateTime.Now, this.get_read_bytes(), this.get_buffer());
    ///         }
    ///         public override void on_close() {
    ///             System.Console.WriteLine("{0} perdio conexion", System.DateTime.Now);
    ///         }
    ///         public void Main() {
    ///             IPEndPoint ep = new IPEndPoint(IPAddress.Any, 9999);
    ///             tcp_listener<example> _appl = new tcp_listener</example>(ep);
    ///             System.Threading.Thread.Sleep(600000);
    ///         }
    /// }
    /// 
    /// en el ejemplo que se describe, tcp_listener instancia un example cada vez
    /// que recibe una conexion nueva, inmediatamente despues llama a on_accept
    /// luego cuando se reciben datos, llama a on_data y en caso de que se desconecte
    /// la conexion desde el lado opuesto llama a on_close.
    /// </summary>
    internal abstract class tcp_acceptor : tcp_base
    {
        public TransporteTCP _data;
        /// <summary>
        /// constructor por defecto, este constructor define un buffer de recepcion de
        /// 1024 bytes que se utilizara para la lectura.
        /// </summary>
        protected tcp_acceptor()
        {
            _buffersize = 1024;
            _read_buffer = new byte[_buffersize];
        }

        /// <summary>
        /// callback, este metodo es llamado por tcp_listener cuando se instancia una 
        /// clase.
        /// </summary>
        public abstract void on_accept();

        /// <summary>
        /// callback, este metodo es llamado por tcp_listener cada vez que se reciben
        /// datos por el socket.
        /// </summary>
        public abstract void on_data();
       
        /// <summary>
        /// callback, este metodo es llamado por tcp_listener cuando el socket se desconecta
        /// desde el lado opuesto de la conexion.
        /// </summary>
        public abstract void on_close();
    }
}