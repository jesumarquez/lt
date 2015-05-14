#region Usings

using System;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Tickets;

#endregion

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class TicketEvent : IComparable<TicketEvent>
    {
        #region Constructor

        public TicketEvent(LogMensaje msg)
        {
            Fecha = msg.Fecha; //Fecha en la que se disparo el mensaje.
            Description = (msg.Mensaje != null)?msg.Mensaje.Descripcion: string.Empty;
            Scheduled = msg.DetalleHorario.Programado;
            Automatic = null;
            Chofer = (msg.Chofer != null)? msg.Chofer.Entidad.Descripcion: string.Empty;
            Camion = (msg.Coche != null)? msg.Coche.ToString():string.Empty;
            Ticket = String.Empty;
        }

        public TicketEvent(DetalleTicket dtTicket)
        {
            Fecha = dtTicket.Manual; // Fecha en la que se disparo manualmente el estado logistico.
            Description = (dtTicket.EstadoLogistico != null)? dtTicket.EstadoLogistico.Descripcion:string.Empty;
            Scheduled =  dtTicket.Programado;
            Automatic =  dtTicket.Automatico;
            Chofer = (dtTicket.Ticket.Empleado != null) ? dtTicket.Ticket.Empleado.Entidad.Descripcion : string.Empty;
            Camion = (dtTicket.Ticket.Vehiculo != null)? dtTicket.Ticket.Vehiculo.ToString() : string.Empty;
            Ticket = dtTicket.Ticket.Codigo;
        }

        #endregion

        #region Public Properties

        public string Camion { get; set; }

        public string Ticket { get; set; }

        public string Chofer { get; set; }

        public DateTime? Scheduled { get; set; }

        public string Description { get; set; }

        public DateTime? Automatic { get; set; }

        public DateTime? Fecha { get; set; }
        
        #endregion

        #region Public Method

        public int CompareTo(TicketEvent tkR) { return Fecha.HasValue ? Fecha.Value.CompareTo(tkR.Fecha) : 0; }

        #endregion
    }
}

