using System;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class RankingVehiculosVo
    {
        public const int KeyIndexIdOperador = 0;

        public const int IndexVehiculo = 0;
        public const int IndexPatente = 1;
        public const int IndexTipoVehiculo = 2;
        public const int IndexTransportista = 3;
        public const int IndexCentroCosto = 4;
        public const int IndexKilometros = 5;
        public const int IndexHorasMovimiento = 6;
        public const int IndexPuntaje = 7;
        public const int IndexInfraccionesLeves = 8;
        public const int IndexInfraccionesMedias = 9;
        public const int IndexInfraccionesGraves = 10;
        public const int IndexInfraccionesTotales = 11;
        
        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexPatente, ResourceName = "Labels", VariableName = "PATENTE", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Count, AggregateTextFormat = "{0:0}")]
        public string Patente { get; set; }

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = true)]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexTransportista, ResourceName = "Entities", VariableName = "PARENTI07", AllowGroup = true)]
        public string Transportista { get; set; }

        [GridMapping(Index = IndexCentroCosto, ResourceName = "Entities", VariableName = "PARENTI37", AllowGroup = true)]
        public string CentroCosto { get; set; }

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
        public int IdVehiculo { get; set; }

        public RankingVehiculosVo(RankingVehiculos ranking)
        {
            IdVehiculo = ranking.IdVehiculo;
            Vehiculo = ranking.Vehiculo;

            var dao = new DAOFactory();
            var coche = dao.CocheDAO.FindById(IdVehiculo);
            if (coche != null)
            {
                TipoVehiculo = coche.TipoCoche != null ? coche.TipoCoche.Descripcion : string.Empty;
                Transportista = coche.Transportista != null ? coche.Transportista.Descripcion : string.Empty;
                CentroCosto = coche.CentroDeCostos != null ? coche.CentroDeCostos.Descripcion : string.Empty;
            }
            Patente = ranking.Patente;
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
