using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobileDriversVo
    {
        public const int IndexLegajo = 0;
        public const int IndexNombre = 1;
        public const int IndexTarjeta = 2;
        public const int IndexInfracciones = 3;
        public const int IndexKilometros = 4;
        public const int IndexTiempoConduccion = 5;

        [GridMapping(Index = IndexLegajo, ResourceName = "Labels", VariableName = "LEGAJO", AllowGroup = false)]
        public string Legajo { get; set; }

        [GridMapping(Index = IndexNombre, ResourceName = "Labels", VariableName = "NAME", AllowGroup = false)]
        public string Nombre { get; set; }

        [GridMapping(Index = IndexTarjeta, ResourceName = "Entities", VariableName = "TARJETA", AllowGroup = false)]
        public string Tarjeta { get; set; }

        [GridMapping(Index = IndexInfracciones, ResourceName = "Labels", VariableName = "INFRACCIONES", AllowGroup = false)]
        public int Infracciones { get; set; }

        [GridMapping(Index = IndexKilometros, ResourceName = "Labels", VariableName = "KILOMETROS", DataFormatString = "{0:2} km", AllowGroup = false)]
        public double Kilometros { get; set; }

        [GridMapping(Index = IndexTiempoConduccion, ResourceName = "Labels", VariableName = "TIEMPO_CONDUCCION", AllowGroup = false)]
        public TimeSpan TiempoConduccion { get; set; }

        public MobileDriversVo(MobileDrivers mobileDrivers)
        {
            Legajo = mobileDrivers.Legajo;
            Nombre = mobileDrivers.Nombre;
            Tarjeta = mobileDrivers.Tarjeta;
            Infracciones = mobileDrivers.Infracciones;
            Kilometros = mobileDrivers.Kilometros;
            TiempoConduccion = mobileDrivers.DrivingTime;
        }
    }
}
