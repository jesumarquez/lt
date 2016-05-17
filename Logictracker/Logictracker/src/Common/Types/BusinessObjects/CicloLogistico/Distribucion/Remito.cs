using System;
using System.Collections.Generic;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class Remito : IAuditable
    {
        Type IAuditable.TypeOf() { return GetType(); }
        public virtual int Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual PuntoEntrega PuntoEntrega { get; set; }

        private IList<DetalleRemito> _detalles;
        public virtual IList<DetalleRemito> Detalles
        {
            get { return _detalles ?? (_detalles = new List<DetalleRemito>()); }
            set { _detalles = value; }
        }
    }
}
