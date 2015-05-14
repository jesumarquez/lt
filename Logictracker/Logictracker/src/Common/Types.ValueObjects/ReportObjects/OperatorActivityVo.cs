using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class OperatorActivityVo
    {
        public const int IndexOperator = 0;
        public const int IndexRecorrido = 1;
        public const int IndexHorasActivo = 2;
        public const int IndexHorasDetenido = 3;
        public const int IndexInfracciones = 4;
        public const int IndexHorasInfraccion = 5;
        public const int IndexVelocidadPromedio = 6;
        public const int IndexVelocidadMaxima = 7;

        [GridMapping(Index = IndexOperator, ResourceName = "Labels", VariableName = "CHOFER", AllowGroup = false, InitialSortExpression = true)]
        public string Operator { get; set; }

        [GridMapping(Index = IndexRecorrido, ResourceName = "Labels", VariableName = "KILOMETROS", DataFormatString = "{0:2} km", AllowGroup = false)]
        public double Recorrido { get; set; }

        [GridMapping(Index = IndexHorasActivo, ResourceName = "Labels", VariableName = "MOVIMIENTO", AllowGroup = false)]
        public string HorasActivo { get; set; }

        [GridMapping(Index = IndexHorasDetenido, ResourceName = "Labels", VariableName = "DETENCION", AllowGroup = false)]
        public string HorasDetenido { get; set; }

        [GridMapping(Index = IndexInfracciones, ResourceName = "Labels", VariableName = "INFRACCIONES", AllowGroup = false)]
        public int Infracciones { get; set; }

        [GridMapping(Index = IndexHorasInfraccion, ResourceName = "Labels", VariableName = "TIEMPO_INFRACCION", AllowGroup = false)]
        public string HorasInfraccion { get; set; }

        [GridMapping(Index = IndexVelocidadPromedio, ResourceName = "Labels", VariableName = "VELOCIDAD_PROMEDIO", AllowGroup = false)]
        public double VelocidadPromedio { get; set; }

        [GridMapping(Index = IndexVelocidadMaxima, ResourceName = "Labels", VariableName = "VELOCIDAD_MAXIMA", AllowGroup = false)]
        public double VelocidadMaxima { get; set; }

        public OperatorActivityVo(OperatorActivity t)
        {
            Operator = t.Operator;
            VelocidadMaxima = t.MaxSpeed;
            VelocidadPromedio = t.AverageSpeed;
            Infracciones = t.Infractions;
            Recorrido = t.Kilometers;
            HorasActivo = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.Movement.Days, t.Movement.Hours, t.Movement.Minutes, t.Movement.Seconds);
            HorasDetenido = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.Stopped.Days, t.Stopped.Hours, t.Stopped.Minutes, t.Stopped.Seconds);
            HorasInfraccion = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.InfractionsTime.Days, t.InfractionsTime.Hours, t.InfractionsTime.Minutes, t.InfractionsTime.Seconds);
        }
    }
}
