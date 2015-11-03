using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Rechazos
{
    public class DetalleTicketRechazo : IAuditable
    {
        public virtual int Id { get; set; }

        public virtual TicketRechazo Ticket { get; set; }

        public virtual Usuario Usuario { get; set; }
 
        public virtual DateTime FechaHora { get; set; }

        public virtual TicketRechazo.Estado Estado { get; set; }

        public virtual string Observacion { get; set; }

        public Type TypeOf() { return GetType(); }

    }
}