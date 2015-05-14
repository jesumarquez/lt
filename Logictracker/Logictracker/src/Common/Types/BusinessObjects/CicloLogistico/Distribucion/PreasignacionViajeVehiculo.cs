using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class PreasignacionViajeVehiculo : IAuditable, ISecurable, IHasVehiculo, IHasTransportista
    {
        Type IAuditable.TypeOf() { return GetType(); }

        public virtual int Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual Transportista Transportista { get; set; }
    }
}
