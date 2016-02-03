using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class EntregaProgramada : IAuditable
    {
        Type IAuditable.TypeOf() { return GetType(); }
        public virtual int Id { get; set; }
        public virtual ViajeProgramado ViajeProgramado { get; set; }
        public virtual PuntoEntrega PuntoEntrega { get; set; }
        public virtual int Orden { get; set; }        
    }
}
