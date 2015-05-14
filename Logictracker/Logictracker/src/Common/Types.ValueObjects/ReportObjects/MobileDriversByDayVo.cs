using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobileDriversByDayVo
    {
        public const int IndexFecha = 0;
        public const int IndexLegajo = 1;
        public const int IndexNombre = 2;
        public const int IndexTarjeta = 3;
        public const int IndexInfracciones = 4;
        public const int IndexKilometros = 5;
        public const int IndexDrivingTime = 6;

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DIA", DataFormatString = "{0:00}", AllowGroup = false)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexLegajo, ResourceName = "Labels", VariableName = "LEGAJO", AllowGroup = false)]
        public string Legajo { get; set; }

        [GridMapping(Index = IndexNombre, ResourceName = "Labels", VariableName = "NAME", AllowGroup = false)]
        public string Nombre { get; set; }

        [GridMapping(Index = IndexTarjeta, ResourceName = "Entities", VariableName = "TARJETA", AllowGroup = false)]
        public string Tarjeta { get; set; }

        [GridMapping(Index = IndexInfracciones, ResourceName = "Labels", VariableName = "INFRACCIONES", AllowGroup = false)]
        public double Infracciones { get; set; }

        [GridMapping(Index = IndexKilometros, ResourceName = "Labels", VariableName = "KILOMETROS", DataFormatString = "{0:00} km", AllowGroup = false)]
        public double Kilometros { get; set; }

        [GridMapping(Index = IndexDrivingTime, ResourceName = "Labels", VariableName = "TIEMPO_CONDUCCION", AllowGroup = false)]
        public TimeSpan DrivingTime { get; set; }

        [GridMapping(HeaderText = "", IsDataKey = true, AllowGroup = false)]
        public int IdMovil { get; set; }

        public MobileDriversByDayVo(MobileDriversByDay mobileDriversByDay)
        {
            Fecha = mobileDriversByDay.Fecha;
            Legajo = mobileDriversByDay.Legajo;
            Nombre = mobileDriversByDay.Nombre;
            Tarjeta = mobileDriversByDay.Tarjeta;
            Infracciones = mobileDriversByDay.Infracciones;
            Kilometros = mobileDriversByDay.Kilometros;
            DrivingTime = mobileDriversByDay.DrivingTime;
            IdMovil = mobileDriversByDay.IdMovil;
        }
    }
}
