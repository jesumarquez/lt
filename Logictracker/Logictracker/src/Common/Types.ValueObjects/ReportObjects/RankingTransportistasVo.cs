using System;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class RankingTransportistasVo
    {
        public const int KeyIndexIdOperador = 0;

        public const int IndexTransportista = 0;
        public const int IndexVehiculos = 1;
        public const int IndexKilometros = 2;
        public const int IndexHorasMovimiento = 3;
        public const int IndexPuntaje = 4;
        public const int IndexInfraccionesLeves = 5;
        public const int IndexInfraccionesMedias = 6;
        public const int IndexInfraccionesGraves = 7;
        public const int IndexInfraccionesTotales = 8;
        
        [GridMapping(Index = IndexTransportista, ResourceName = "Entities", VariableName = "PARENTI07", AllowGroup = true)]
        public string Transportista { get; set; }

        [GridMapping(Index = IndexVehiculos, ResourceName = "Menu", VariableName = "PAR_VEHICULOS", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0}")]
        public double Vehiculos { get; set; }

        [GridMapping(Index = IndexKilometros, ResourceName = "Labels", VariableName = "KILOMETROS", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00}")]
        public double Kilometros { get; set; }

        [GridMapping(Index = IndexHorasMovimiento, ResourceName = "Labels", VariableName = "MOVIMIENTO", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum)]
        public TimeSpan HorasMovimiento { get; set; }

        [GridMapping(Index = IndexPuntaje, ResourceName = "Labels", VariableName = "PUNTAJE", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Descending, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0}")]
        public double Puntaje { get; set; }

        [GridMapping(Index = IndexInfraccionesLeves, ResourceName = "Labels", VariableName = "LEVES", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0}")]
        public int InfraccionesLeves { get; set; }

        [GridMapping(Index = IndexInfraccionesMedias, ResourceName = "Labels", VariableName = "MEDIAS", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0}")]
        public int InfraccionesMedias { get; set; }

        [GridMapping(Index = IndexInfraccionesGraves, ResourceName = "Labels", VariableName = "GRAVES", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0}")]
        public int InfraccionesGraves { get; set; }

        [GridMapping(Index = IndexInfraccionesTotales, ResourceName = "Labels", VariableName = "TOTALES", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0}")]
        public int InfraccionesTotales { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdTransportista { get; set; }

        public RankingTransportistasVo(RankingTransportistas ranking)
        {
            IdTransportista = ranking.IdTransportista;
            Transportista = ranking.Transportista;
            Vehiculos = ranking.Vehiculos;
            Kilometros = ranking.Kilometros;
            HorasMovimiento = ranking.HorasMovimiento;
            Puntaje = ranking.Puntaje;
            InfraccionesLeves = ranking.InfraccionesLeves;
            InfraccionesMedias = ranking.InfraccionesMedias;
            InfraccionesGraves = ranking.InfraccionesGraves;
            InfraccionesTotales = ranking.InfraccionesTotales;
        }
    }
}
