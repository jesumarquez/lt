using System;

namespace Urbetrack.Postal.Enums
{
    public enum TipoValidacion { Nunca = 0, ConEntrega = 1, SinEntrega = 2, Siempre = 3 }

    public static class TipoValidacionExtenssions
    {
        public static TipoValidacion GetType(Int32 value) { return (TipoValidacion)Enum.Parse(typeof(TipoValidacion), value.ToString(), true); }

        public static Boolean Validate(this TipoValidacion tipoValidacion, Boolean esEntrega)
        {
            if (tipoValidacion.Equals(TipoValidacion.Siempre)) return true;

            if (esEntrega)
            {
                if (tipoValidacion.Equals(TipoValidacion.ConEntrega)) return true;
            }
            else
            {
                if (tipoValidacion.Equals(TipoValidacion.SinEntrega)) return true;
            }

            return false;
        }
    }
}