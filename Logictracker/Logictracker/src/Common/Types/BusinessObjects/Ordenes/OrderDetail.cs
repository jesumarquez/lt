using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Ordenes
{
    public class OrderDetail : IAuditable
    {
        public virtual Insumo Insumo { get; set; }
        public virtual decimal PrecioUnitario { get; set; }
        public virtual int Cantidad { get; set; }
        public virtual decimal Descuento { get; set; }
        public virtual int Id { get; set; }
        public virtual Order Order { get; set; }

        public virtual Type TypeOf()
        {
            return GetType();
        }
    }
}