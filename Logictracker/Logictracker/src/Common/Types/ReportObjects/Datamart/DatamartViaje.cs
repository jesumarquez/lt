using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.ReportObjects.Datamart
{
    public class DatamartViaje : IAuditable, IHasEmpresa, IHasLinea, IHasViajeDistribucion, IHasVehiculo
    {
        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual ViajeDistribucion Viaje { get; set; }
        public virtual DateTime Inicio { get; set; }
        public virtual DateTime Fin { get; set; }
        public virtual double Duracion { get; set; }

        public virtual double KmTotales { get; set; }
        public virtual double KmProductivos { get; set; }
        public virtual double KmImproductivos { get; set; }
        public virtual double KmProgramados { get; set; }

        public virtual int EntregasTotales { get; set; }
        public virtual int EntregasCompletadas { get; set; }
        public virtual int EntregasNoCompletadas { get; set; }
        public virtual int EntregasVisitadas { get; set; }
        public virtual int EntregasNoVisitadas { get; set; }
        public virtual int EntregasEnSitio { get; set; }
        public virtual int EntregasEnZona { get; set; }
        
        public virtual double EntregaMaxima { get; set; }
        public virtual double EntregaMinima { get; set; }
        public virtual double EntregaPromedio { get; set; }

        public virtual double HorasDetenido { get; set; }
        public virtual double HorasEnEntrega { get; set; }

        public virtual double VelocidadMaxima { get; set; }
        public virtual double VelocidadPromedio { get; set; }

        public virtual double Costo { get; set; }

        public virtual Type TypeOf() { return GetType(); }
    }
}
