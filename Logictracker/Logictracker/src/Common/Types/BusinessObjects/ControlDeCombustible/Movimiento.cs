#region Usings

using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ControlDeCombustible
{
    [Serializable]
    public class Movimiento : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual DateTime Fecha { get; set; }
        public virtual DateTime FechaIngresoABase { get; set; }
        public virtual Double Volumen { get; set; }
        public virtual String Observacion { get; set; }
        public virtual int Estado { get; set; }
        public virtual Caudalimetro Caudalimetro { get; set; }
        public virtual Tanque Tanque { get; set; }
        public virtual TipoMovimiento TipoMovimiento { get; set; }
        public virtual MotivoConciliacion Motivo { get; set; }
        public virtual Coche Coche { get; set; }
        public virtual Double Caudal { get; set; }
        public virtual Double HsEnMarcha { get; set; }
        public virtual Double RPM { get; set; }
        public virtual Double Temperatura { get; set; }
        public virtual bool Procesado { get; set; }
        public virtual Empleado Empleado { get; set; }

        public override bool Equals(object obj)
        {
            var m = (Movimiento)obj;

            if (m == null) return false;

            return !(m.Caudal != Caudal || m.HsEnMarcha != HsEnMarcha || m.RPM != RPM || m.Temperatura != Temperatura || m.Volumen != Volumen);
        }

        public override int GetHashCode()
        {
            return Caudal.GetHashCode() + HsEnMarcha.GetHashCode() + RPM.GetHashCode() + Temperatura.GetHashCode() + Volumen.GetHashCode();
        }
    }
}
