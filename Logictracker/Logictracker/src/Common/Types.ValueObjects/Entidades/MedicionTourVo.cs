using System;
using Logictracker.DAL.DAO.BusinessObjects.Entidades;
using Logictracker.Security;

namespace Logictracker.Types.ValueObjects.Entidades
{
    [Serializable]
    public class MedicionTourVo
    {
        public const int KeyIndexIdMovil = 0;

        public const int IndexMedidor = 0;
        public const int IndexSensor = 1;
        public const int IndexInicio = 2;
        public const int IndexFin = 3;
        public const int IndexDuracion = 4;
        public const int IndexInicial = 5;
        public const int IndexFinal = 6;
        public const int IndexTotal = 7;

        [GridMapping(Index = IndexMedidor, ResourceName = "Entities", VariableName = "PARENTI81", IsInitialGroup = true, InitialSortExpression = true)]
        public string Medidor { get; set; }

        [GridMapping(Index = IndexSensor, ResourceName = "Entities", VariableName = "PARENTI80", IsInitialGroup = true, InitialSortExpression = true)]
        public string Sensor { get; set; }

        [GridMapping(Index = IndexInicio, ResourceName = "Labels", VariableName = "INICIO", DataFormatString = "{0:dd/MM/yyyy HH:mm}", InitialSortExpression = true, AllowGroup = false)]
        public DateTime Inicio { get; set; }

        [GridMapping(Index = IndexFin, ResourceName = "Labels", VariableName = "FIN", DataFormatString = "{0:dd/MM/yyyy HH:mm}", AllowGroup = false)]
        public DateTime Fin { get; set; }

        [GridMapping(Index = IndexDuracion, ResourceName = "Labels", VariableName = "DURACION", DataFormatString = "{0:00:00:00}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:00:00:00}", AllowGroup = false)]
        public TimeSpan Duracion { get; set; }

        [GridMapping(Index = IndexInicial, ResourceName = "Labels", VariableName = "VALOR_INICIAL", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double ValorInicial { get; set; }

        [GridMapping(Index = IndexFinal, ResourceName = "Labels", VariableName = "VALOR_FINAL", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double ValorFinal { get; set; }

        [GridMapping(Index = IndexTotal, ResourceName = "Labels", VariableName = "TOTAL", DataFormatString = "{0:0.00}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00}", AllowGroup = false)]
        public double Total { get; set; }
        
        public MedicionTourVo(LogEventoDAO.MedicionTour mobileTour)
        {
            Medidor = mobileTour.Medidor.Descripcion;
            Sensor = mobileTour.Medidor.Sensor.Descripcion;
            Inicio = mobileTour.Encendido.ToDisplayDateTime();
            Fin = mobileTour.Apagado.ToDisplayDateTime();
            Duracion = mobileTour.Duracion;
            ValorInicial = mobileTour.Inicio;
            ValorFinal = mobileTour.Fin;
            Total = mobileTour.Total;
        }
    }
}
