using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ReportObjects.Datamart
{
    [Serializable]
    public class DatamartTramo : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual Coche Vehicle { get; set; }
        public virtual DateTime Inicio { get; set; }
        public virtual DateTime Fin { get; set; }
        public virtual double Kilometros { get; set; }
        public virtual double Horas { get; set; }
        public virtual double HorasMovimiento { get; set; }
        public virtual double HorasDetenido { get; set; }
        public virtual double HorasDetenidoDentro { get; set; }
        public virtual double HorasDetenidoFuera { get; set; }
        public virtual int DetencionesMenores { get; set; }
        public virtual int DetencionesMayores { get; set; }
        public virtual int VelocidadPromedio { get; set; }
        public virtual int GeocercasBase { get; set; }
        public virtual int GeocercasEntregas { get; set; }
        public virtual int GeocercasOtras { get; set; }
        public virtual bool MotorOn { get; set; }
    }
}
