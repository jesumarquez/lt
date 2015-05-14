using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class OperatorMobilesByDayVo
    {
        public const int KeyIndexIdMovil = 0;

        public const int IndexFecha = 0;
        public const int IndexMovil = 1;
        public const int IndexTipoVehiculo = 2;
        public const int IndexMarca = 3;
        public const int IndexResponsable = 4;
        public const int IndexInfracciones = 5;
        public const int IndexRecorrido = 6;
        public const int IndexVelocidadMaxima = 7;
        public const int IndexVelocidadPromedio = 8;
        public const int IndexHorasActivo = 9;

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DIA", AllowGroup = false)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexMovil, ResourceName = "Labels", VariableName = "INTERNO", AllowGroup = false)]
        public string Movil { get; set; }

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = false)]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexMarca, ResourceName = "Labels", VariableName = "MARCA", AllowGroup = false)]
        public string Marca { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE", AllowGroup = false)]
        public string Responsable { get; set; }

        [GridMapping(Index = IndexInfracciones, ResourceName = "Labels", VariableName = "INFRACCIONES", AllowGroup = false)]
        public int Infracciones { get; set; }

        [GridMapping(Index = IndexRecorrido, ResourceName = "Labels", VariableName = "RECORRIDO", DataFormatString = "{0:2} km", AllowGroup = false)]
        public double Recorrido { get; set; }

        [GridMapping(Index = IndexVelocidadMaxima, ResourceName = "Labels", VariableName = "VELOCIDAD_MAXIMA", AllowGroup = false)]
        public int VelocidadMaxima { get; set; }

        [GridMapping(Index = IndexVelocidadPromedio, ResourceName = "Labels", VariableName = "VELOCIDAD_PROMEDIO", AllowGroup = false)]
        public int VelocidadPromedio { get; set; }

        [GridMapping(Index = IndexHorasActivo, ResourceName = "Labels", VariableName = "TIEMPO_CONDUCCION", AllowGroup = false)]
        public TimeSpan HorasActivo { get; set; }

        [GridMapping(HeaderText = "", IsDataKey = true, AllowGroup = false )]
        public int IdMovil { get; set; }

        public OperatorMobilesByDayVo(OperatorMobilesByDay operatorMobilesByDay)
        {
            Fecha = operatorMobilesByDay.Fecha;
            Movil = operatorMobilesByDay.Movil;
            TipoVehiculo = operatorMobilesByDay.TipoVehiculo;
            Marca = operatorMobilesByDay.Marca;
            Responsable = operatorMobilesByDay.Responsable;
            Infracciones = operatorMobilesByDay.Infracciones;
            Recorrido = operatorMobilesByDay.Recorrido;
            VelocidadMaxima = operatorMobilesByDay.VelocidadMaxima;
            VelocidadPromedio = operatorMobilesByDay.VelocidadPromedio;
            HorasActivo = operatorMobilesByDay.HorasActivo;
            IdMovil = operatorMobilesByDay.IdMovil;
        }
    }
}
