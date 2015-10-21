using System;
using System.Collections.Generic;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Ordenes
{
    [Serializable]
    public class Order : IAuditable, IHasEmpresa, IHasLinea
    {
        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual PuntoEntrega PuntoEntrega { get; set; }
        public virtual string CodigoPedido { get; set; }
        public virtual DateTime FechaAlta { get; set; }
        public virtual DateTime FechaPedido { get; set; }
        public virtual DateTime FechaEntrega { get; set; }
        public virtual string InicioVentana { get; set; }
        public virtual string FinVentana { get; set; }
        public virtual Linea Linea { get; set; }

        public virtual IList<OrderDetail> OrderDetails { get; set; }

        public virtual Type TypeOf()
        {
            return GetType();
        }

        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }

    }
}
