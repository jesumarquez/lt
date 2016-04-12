using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.ReportObjects.Datamart
{
    public class DatamartDistribucion : IAuditable, IHasEmpresa, IHasLinea, IHasVehiculo, IHasCentroDeCosto, IHasViajeDistribucion, IHasPuntoEntrega
    {
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual CentroDeCostos CentroDeCostos { get; set; }
        public virtual ViajeDistribucion Viaje { get; set; }
        public virtual EntregaDistribucion Detalle { get; set; }
        public virtual PuntoEntrega PuntoEntrega { get; set; }
        
        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Ruta { get; set; }
        public virtual int Orden { get; set; }
        public virtual string Entrega { get; set; }
        public virtual string Estado { get; set; }
        public virtual int IdEstado { get; set; }
        public virtual double Km { get; set; }
        public virtual double Recorrido { get; set; }
        public virtual double TiempoEntrega { get; set; }
        public virtual DateTime? Entrada { get; set; }
        public virtual DateTime? Salida { get; set; }
        public virtual DateTime? Manual { get; set; }
        public virtual DateTime Programado { get; set; }
        public virtual double Desvio { get; set; }
        public virtual float Importe { get; set; }
        public virtual string Cliente { get; set; }
        public virtual string Confirmacion { get; set; }
        public virtual double? Distancia { get; set; }
        public virtual Type TypeOf() { return GetType(); }
    }
}
