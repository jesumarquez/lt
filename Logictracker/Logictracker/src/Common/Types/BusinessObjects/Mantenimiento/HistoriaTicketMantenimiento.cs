using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Mantenimiento
{
    [Serializable]
    public class HistoriaTicketMantenimiento : IAuditable, IHasEmpresa, IHasVehiculo, IHasEmpleado, IHasTaller
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual Empresa Empresa { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Taller Taller { get; set; }
        public virtual TicketMantenimiento TicketMantenimiento { get; set; }
        public virtual Usuario Usuario { get; set; }

        public virtual DateTime Fecha { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Codigo { get; set; }

        public virtual string PrimerPresupuesto { get; set; }
        public virtual string Presupuesto { get; set; }
        public virtual double Monto { get; set; }
        public virtual DateTime FechaSolicitud { get; set; }
        public virtual DateTime? FechaTurno { get; set; }
        public virtual DateTime? FechaRecepcion { get; set; }
        public virtual DateTime? FechaPresupuestoOriginal { get; set; }
        public virtual DateTime? FechaPresupuestada { get; set; }
        public virtual DateTime? FechaRecotizacion { get; set; }
        public virtual DateTime? FechaAprobacion { get; set; }
        public virtual DateTime? FechaVerificacion { get; set; }
        public virtual DateTime? FechaEntrega { get; set; }
        public virtual DateTime? FechaTrabajoTerminado { get; set; }
        public virtual DateTime? FechaTrabajoAceptado { get; set; }
    
        public virtual short Estado { get; set; }
        public virtual short EstadoPresupuesto { get; set; }
        public virtual short NivelComplejidad { get; set; }
    }
}