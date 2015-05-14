#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ControlDeCombustible
{
    [Serializable]
    public class Tanque : IComparable, IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Descripcion { get; set; }
        public virtual double VolReal { get; set; }
        public virtual double VolTeorico { get; set; }
        public virtual Equipo Equipo { get; set; }
        public virtual string Codigo { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual double Capacidad { get; set; }
        public virtual Double? StockReposicion { get; set; }
        public virtual Double? StockCritico { get; set; }
        public virtual Double? AguaMin { get; set; }
        public virtual Double? AguaMax { get; set; }
        public virtual Int32? TiempoSinReportar { get; set; }
        public virtual Int32? PorcentajeRealVsTeorico { get; set; }

        public virtual int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var tanque = obj as Tanque;

            return tanque == null ? 1 : Descripcion.CompareTo(tanque.Descripcion);
        }
    }
}
