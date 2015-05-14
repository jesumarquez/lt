using System;
using System.Drawing;

namespace Urbetrack.Postal.Enums
{
    public enum EstadoServicio { Pendiente = 0, EnCurso = 1, Terminado = 2 }

    public static class EstadoServicioExtenssions
    {
        public static EstadoServicio GetState(Int32 value) { return (EstadoServicio)Enum.Parse(typeof(EstadoServicio), value.ToString(), true); }
    }
}