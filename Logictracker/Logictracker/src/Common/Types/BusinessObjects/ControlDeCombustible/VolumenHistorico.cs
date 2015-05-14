#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ControlDeCombustible
{   
    [Serializable]
    public class VolumenHistorico : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual double Volumen { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual bool EsTeorico { get; set; }
        public virtual Tanque Tanque { get; set; }
        public virtual double VolumenAgua { get; set; }

        public override bool Equals(object obj)
        {
            var m = (VolumenHistorico)obj;

            if (m == null) return false;

            return !(m.Volumen != Volumen || m.EsTeorico != EsTeorico);
        }

        public override int GetHashCode() { return Volumen.GetHashCode() + EsTeorico.GetHashCode(); }
    }
}
