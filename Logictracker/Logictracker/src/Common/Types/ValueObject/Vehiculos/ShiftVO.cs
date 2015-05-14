#region Usings

using System;
using Logictracker.Types.BusinessObjects.Vehiculos;

#endregion

namespace Logictracker.Types.ValueObject.Vehiculos
{
    [Serializable]
    public class ShiftVO
    {
        public ShiftVO(Shift shift)
        {
            Codigo = shift.Codigo;
            Descripcion = shift.Descripcion;
            Dias = String.Concat(shift.Lunes ? "Lunes," : String.Empty, 
                                 shift.Martes ? "Martes," : String.Empty,
                                 shift.Miercoles ? "Miercoles," : String.Empty, 
                                 shift.Jueves ? "Jueves," : String.Empty, 
                                 shift.Viernes ? "Viernes," : String.Empty,
                                 shift.Sabado ? "Sabado," : String.Empty, 
                                 shift.Domingo ? "Domingo," : String.Empty).TrimEnd(',');
            Desde = new DateTime(TimeSpan.FromHours(shift.Inicio).Ticks).ToShortTimeString();
            Hasta = new DateTime(TimeSpan.FromHours(shift.Fin).Ticks).ToShortTimeString();
        }
        private DateTime DateDesde { get; set;}
        private DateTime DateHasta { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Dias { get; set; }
        public string Desde { get; set;}
        public string Hasta { get; set; }
    }
}
