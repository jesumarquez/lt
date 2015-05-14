#region Usings

using System;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects.RankingDeOperadores;

#endregion

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class OperatorRankingVo
    {
        public const int KeyIndexIdOperador = 0;

        public const int IndexOperador = 0;
        public const int IndexLegajo = 1;
        public const int IndexCentroCosto = 2;
        public const int IndexDepartamento = 3;
        public const int IndexKilometros = 4;
        public const int IndexHorasMovimiento = 5;
        public const int IndexPuntaje = 6;
        public const int IndexInfraccionesLeves = 7;
        public const int IndexInfraccionesMedias = 8;
        public const int IndexInfraccionesGraves = 9;
        public const int IndexInfraccionesTotales = 10;
        
        [GridMapping(Index = IndexOperador, ResourceName = "Labels", VariableName = "CHOFER", AllowGroup = false)]
        public string Operador { get; set; }

        [GridMapping(Index = IndexLegajo, ResourceName = "Labels", VariableName = "LEGAJO", AllowGroup = false)]
        public string Legajo { get; set; }

        [GridMapping(Index = IndexCentroCosto, ResourceName = "Entities", VariableName = "PARENTI37", AllowGroup = true)]
        public string CentroCosto { get; set; }

        [GridMapping(Index = IndexDepartamento, ResourceName = "Entities", VariableName = "PARENTI04", AllowGroup = true)]
        public string Departamento { get; set; }

        [GridMapping(Index = IndexKilometros, ResourceName = "Labels", VariableName = "KILOMETROS", DataFormatString = "{0:0.00} km", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00} km")]
        public double Kilometros { get; set; }

        [GridMapping(Index = IndexHorasMovimiento, ResourceName = "Labels", VariableName = "MOVIMIENTO", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum)]
        public TimeSpan HorasMovimiento { get; set; }

        [GridMapping(Index = IndexPuntaje, ResourceName = "Labels", VariableName = "PUNTAJE", DataFormatString = "{0}", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Descending, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0}")]
        public double Puntaje { get; set; }

        [GridMapping(Index = IndexInfraccionesLeves, ResourceName = "Labels", VariableName = "LEVES", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum)]
        public int InfraccionesLeves { get; set; }

        [GridMapping(Index = IndexInfraccionesMedias, ResourceName = "Labels", VariableName = "MEDIAS", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum)]
        public int InfraccionesMedias { get; set; }

        [GridMapping(Index = IndexInfraccionesGraves, ResourceName = "Labels", VariableName = "GRAVES", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum)]
        public int InfraccionesGraves { get; set; }

        [GridMapping(Index = IndexInfraccionesTotales, ResourceName = "Labels", VariableName = "TOTALES", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum)]
        public int InfraccionesTotales { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdOperador { get; set; }

        public OperatorRankingVo(OperatorRanking infractionDetail)
        {
            IdOperador = infractionDetail.IdOperador;
            Operador = infractionDetail.Operador;

            var dao = new DAOFactory();
            var chofer = dao.EmpleadoDAO.FindById(IdOperador);
            CentroCosto = chofer != null && chofer.CentroDeCostos != null
                              ? chofer.CentroDeCostos.Descripcion
                              : string.Empty;
            Departamento = chofer != null && chofer.Departamento != null
                               ? chofer.Departamento.Descripcion
                               : string.Empty;
            Legajo = infractionDetail.Legajo;
            Kilometros = infractionDetail.Kilometros;
            HorasMovimiento = infractionDetail.HorasMovimiento;
            Puntaje = infractionDetail.Puntaje;
            InfraccionesLeves = infractionDetail.InfraccionesLeves;
            InfraccionesMedias = infractionDetail.InfraccionesMedias;
            InfraccionesGraves = infractionDetail.InfraccionesGraves;
            InfraccionesTotales = infractionDetail.InfraccionesTotales;
        }
    }
}
