using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.ReportObjects.Datamart
{
    [Serializable]
    public class InformeViajeRecarga : IAuditable
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual string Interno { get; set; }
        public virtual string Patente { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Accion { get; set; }
        public virtual DateTime Inicio { get; set; }
        public virtual DateTime Fin { get; set; }
        public virtual double Duracion { get; set; }

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual Coche Vehiculo { get; set; }
    }
}
