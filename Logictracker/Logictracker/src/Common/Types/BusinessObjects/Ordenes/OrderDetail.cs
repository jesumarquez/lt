using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections;
using System.Collections.Generic;

namespace Logictracker.Types.BusinessObjects.Ordenes
{
    public class OrderDetail : IAuditable
    {
        public enum Estados
        { 
            Cancelado   = -1,
            Pendiente   = 0,
            Ruteado     = 1,
            NoRuteado   = 2
        }

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }
        public virtual Insumo Insumo { get; set; }
        public virtual decimal PrecioUnitario { get; set; }
        public virtual int Cantidad { get; set; }
        public virtual int Ajuste { get; set; }
        public virtual int Total { get; set; }
        public virtual Estados Estado { get; set; }
        public virtual decimal Descuento { get; set; }        
        public virtual Order Order { get; set; }
        public virtual int Cuaderna { get; set; }
        public virtual IList<OrderDetailContenedor> Contenedores { get; set; }
    }
}