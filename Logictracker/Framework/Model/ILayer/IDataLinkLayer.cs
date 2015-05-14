using System.Net;

namespace Logictracker.Model
{
    /// <summary>
    /// Nivel de enlace entre nodos.
    /// </summary>
    public interface IDataLinkLayer: ILayer
    {
        /// <summary>
        /// Enviar una trama al vinculo indicado
        /// </summary>
		/// <param name="nodecode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        void SendMessage(int nodecode, IMessage message);

		/// <summary>
		/// Lanzado cuando UNL recibe un frame del TSP asociado o cuando la red se vuelve disponible.
		/// </summary>
		/// <remarks>
		/// Cuando el nivel de red subyacente <see>IUnderlayingNetworkLayer</see> acepta
		/// una solicitud de conexion (ej. TCP/SCTP) o recibe un datagrama de una fuente 
		/// desconocida (ej. UDP/XBEE), espera hasta recibir la primer trama por completo
		/// y dispara este evento
		/// </remarks>
	    bool OnFrameReceived(ILink link, IFrame frame, INode parser);

	    /// <summary>
	    /// Lanzado cuando la conectividad esta en falla.
	    /// </summary>
	    void OnNetworkSuspend(ILink link);

	    /// <summary>
	    /// Lanzado cuando UNL necesita obtener el ILink asociado
	    /// a una conexion.
	    /// </summary>
	    ILink OnLinkTranslation(IUnderlayingNetworkLayer unl, EndPoint ep, IFrame frame);
    }
}