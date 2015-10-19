using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class EstadoDistribucion: IAuditable, IHasViajeDistribucion
    {
        Type IAuditable.TypeOf() { return GetType(); }
        public virtual int Id { get; set; }
        public virtual ViajeDistribucion Viaje { get; set; }
        public virtual EstadoLogistico EstadoLogistico { get; set; }
        public virtual DateTime? Inicio { get; set; }
        public virtual DateTime? Fin { get; set; }
    }
}
