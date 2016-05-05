using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class EstadoDiario : IAuditable, IHasEmpresa, IHasVehiculo
    {
        public virtual Type TypeOf() { return this.GetType(); }
        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual double HorasTaller { get; set; }
        public virtual double HorasBase { get; set; }
        public virtual double HorasMovimiento { get; set; }
        public virtual double HorasDetenido { get; set; }
        public virtual double HorasEnMarcha { get; set; }
        public virtual double HorasSinReportar { get; set; }
        public virtual double HorasDetenidoEnGeocerca { get; set; }
        public virtual double HorasDetenidoSinGeocerca { get; set; }
        public virtual double Kilometros { get; set; }
        public virtual DateTime Fecha { get; set; }        
    }
}
