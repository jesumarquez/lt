using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.TimeTracking
{
    [Serializable]
    public class EventoViaje : IAuditable, IHasVehiculo, IHasEmpleado
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual Coche Vehiculo { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual bool EsInicio { get; set; }
        public virtual bool EsIntermedio { get; set; }
        public virtual bool EsFin { get; set; }
        public virtual bool EsEntrada { get; set; }
    }
}
