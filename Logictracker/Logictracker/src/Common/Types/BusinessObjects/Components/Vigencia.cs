#region Usings

using System;

#endregion

namespace Logictracker.Types.BusinessObjects.Components
{
    [Serializable]
    public class Vigencia
    {
        public virtual DateTime? Inicio { get; set; }
        public virtual DateTime? Fin { get; set; }

        public virtual bool Vigente(DateTime now)
        {
            if (Inicio.HasValue && now < Inicio.Value) return false;
            if (Fin.HasValue && now > Fin.Value) return false;
            return true;
        }

        public virtual bool Vigente(DateTime desde, DateTime hasta)
        {
            if (Fin.HasValue && desde > Fin.Value) return false;
            if (Inicio.HasValue && hasta < Inicio.Value) return false;
            return true;
        }
    }
}