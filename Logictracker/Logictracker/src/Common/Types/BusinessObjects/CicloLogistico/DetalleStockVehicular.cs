using System;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    public class DetalleStockVehicular : IAuditable
    {
        public virtual Type TypeOf() { return GetType(); }

        public virtual int Id { get; set; }
        public virtual StockVehicular StockVehicular { get; set; }
        public virtual Coche Vehiculo { get; set; }
    }
}
