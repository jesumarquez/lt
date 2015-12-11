using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Rechazos
{
    public class DetalleTicketRechazo : IAuditable
    {
        public virtual int Id { get; set; }

        public virtual TicketRechazo Ticket { get; set; }
 
        public virtual DateTime FechaHora { get; set; }

        public virtual TicketRechazo.Estado Estado { get; set; }

        public virtual string Observacion { get; set; }

        public virtual Type TypeOf() { return GetType(); }

        public virtual Empleado Empleado { get; set; }

    }
}