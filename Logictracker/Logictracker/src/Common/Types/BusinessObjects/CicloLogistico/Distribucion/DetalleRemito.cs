using System;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.BusinessObjects.Ordenes;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class DetalleRemito: IAuditable
    {
        Type IAuditable.TypeOf() { return GetType(); }
        public virtual int Id { get; set; }
        public virtual Remito Remito { get; set; }
        public virtual Insumo Insumo { get; set; }
        public virtual int Cantidad { get; set; }

        private ISet<OrderDetail> _ordenes;
        public virtual ISet<OrderDetail> Ordenes { get { return _ordenes ?? (_ordenes = new HashSet<OrderDetail>()); } }
    }
}
