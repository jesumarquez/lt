#region Usings

using Logictracker.Utils;

#endregion

namespace Logictracker.Model
{
    /// <summary>
    /// Agrega la caracteristica de ubicar un evento en el espacio
    /// y tiempo, tomando 3 condiciones. Inicio, Desicion y Fin.
    /// </summary>
	public interface ITicket : IMessage
    {
        /// <summary>
        /// Punto donde se detecta la condicion.
        /// </summary>
        GPSPoint StartPoint { get; set; }

        /// <summary>
        /// Punto donde se decide generar el Ticket.
        /// </summary>
        GPSPoint TicketPoint { get; set; }
        
        /// <summary>
        /// Punto donde se normaliza la condicion.
        /// </summary>
        GPSPoint EndPoint { get; set; }
    }
}