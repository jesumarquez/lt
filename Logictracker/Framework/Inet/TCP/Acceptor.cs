#region Usings

using System;

#endregion

namespace Logictracker.InetLayer.TCP
{
    /// <summary>
    /// ACCEPTOR es una envoltura que permite realizar conexiones asincronicas TCP
    /// de forma rapida. Se debe implementar una clase que herede de ACCEPTOR e
    /// implementar los metodos abstractos que esta clase posee. OnConnection, OnReceive,
    /// OnDisconnection. estos eventos seran llamados por el Listener cuando se disparen
    /// los eventos asociados. 
    /// el metodo de uso se ve en el siguiente ejemplo:
    /// 
    /// using etao.marshall;
    /// 
    /// public class example : ACCEPTOR
    /// {
    ///         public override void OnConnection() {
    ///             System.Console.WriteLine("{0} accepto conexion", System.DateTime.Now);
    ///         }
    ///         public override void OnReceive() {
    ///             System.Console.WriteLine("{0} recibio datos, {1}[{2}]", System.DateTime.Now, this.get_read_bytes(), this.get_buffer());
    ///         }
    ///         public override void OnDisconnection() {
    ///             System.Console.WriteLine("{0} perdio conexion", System.DateTime.Now);
    ///         }
    ///         public void Main() {
    ///             IPEndPoint ep = new IPEndPoint(IPAddress.Any, 9999);
    ///             Listener _appl = new Listener(ep);
    ///             System.Threading.Thread.Sleep(600000);
    ///         }
    /// }
    /// 
    /// en el ejemplo que se describe, Listener instancia un example cada vez
    /// que recibe una conexion nueva, inmediatamente despues llama a OnConnection
    /// luego cuando se reciben datos, llama a OnReceive y en caso de que se desconecte
    /// la conexion desde el lado opuesto llama a OnDisconnection.
    /// </summary>
    public abstract class Acceptor : Stream, ICloneable
    {
        /// <summary>
        /// callback, este metodo es llamado por Listener cuando se instancia una 
        /// clase.
        /// </summary>
        public abstract void OnConnection();

        /// <summary>
        /// Implentacion de la interface ICloneable, utilizado por el 
        /// listener para instanciar un nuevo Acceptor.
        /// </summary>
        public abstract object Clone();
    }
}