namespace Logictracker.Model
{
    /// <summary>
    /// Datos de Flota para INode
    /// </summary>
	public interface IKeepAliveInfo : INode
    {
        /// <summary>
        /// Tiempo entre KeepAlives que envia el equipo.
        /// no se utiliza en ningun lado, marcado para _Deletion_ en la proxima version.
        /// </summary>
        int KeepAliveLapse { get; set; }
    }
}
