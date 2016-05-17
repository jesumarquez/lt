using System;

namespace Logictracker.Types.BusinessObjects.Rechazos
{
    public class RechazoMov
    {
        public virtual int Id { get; set; }
        public virtual TicketRechazo Ticket { get; set; }
        public virtual DateTime Ingreso { get; set; }
        public virtual DateTime Egreso { get; set; }
        public virtual Empleado EmpledoEgreso { get; set; }
        public virtual TicketRechazo.Estado EstadoEgreso { get; set; }
        public virtual int Lapso { get; set; }
    }
}
