using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Tickets
{
    [Serializable]
    public class DetalleTicket : IAuditable
    {
        Type IAuditable.TypeOf() { return GetType(); }

        public virtual int Id { get; set; }

        public virtual Ticket Ticket { get; set; }
        
        public virtual DateTime? Programado { get; set; }

        public virtual DateTime? Manual { get; set; }

        public virtual DateTime? Automatico { get; set; }

        public virtual Estado EstadoLogistico { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var detalleTicket = obj as DetalleTicket;
            if (detalleTicket == null) return false;
            return (Id == detalleTicket.Id && EstadoLogistico.Equals(detalleTicket.EstadoLogistico));
        }

        public override int GetHashCode() {return Id;}
    }
}