using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Ordenes
{
    public class OrderDetail : IAuditable
    {
        public static class Estados
        {
            public const short Cancelado = -1;
            public const short Pendiente = 0;
            public const short Ruteado = 1;
            public const short NoRuteado = 2;
        }

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }
        public virtual Insumo Insumo { get; set; }
        public virtual decimal PrecioUnitario { get; set; }
        public virtual int Cantidad { get; set; }
        public virtual int Ajuste { get; set; }
        public virtual int Total { get; set; }
        public virtual short Estado { get; set; }
        public virtual decimal Descuento { get; set; }        
        public virtual Order Order { get; set; }
        public virtual int Cuaderna { get; set; }
    }
}