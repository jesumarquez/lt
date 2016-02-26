using System;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.BusinessObjects.Vehiculos;
using System.Collections.Generic;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    public class StockVehicular : IAuditable, IHasEmpresa, IHasTipoVehiculo, IHasZona
    {
        public virtual Type TypeOf() { return GetType(); }
        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Zona Zona { get; set; }
        public virtual TipoCoche TipoCoche { get; set; }

        private IList<DetalleStockVehicular> _detalles;
        public virtual IList<DetalleStockVehicular> Detalles
        {
            get { return _detalles ?? (_detalles = new List<DetalleStockVehicular>()); }
            set { _detalles = value; }
        }
    }
}
