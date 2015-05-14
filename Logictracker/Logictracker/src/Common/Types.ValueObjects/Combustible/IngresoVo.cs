using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class IngresoVo
    {
        public const int IndexTanque = 0;
        public const int IndexFecha = 1;
        public const int IndexDescriTipo = 2;
        public const int IndexVolumen = 3;

        [GridMapping(Index = IndexTanque, ResourceName = "Entities", VariableName = "PARENTI36", AllowGroup = true, IsInitialGroup = true)]
        public string Tanque { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", DataFormatString = "{0:G}", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexDescriTipo, ResourceName = "Labels", VariableName = "TIPO_MOVIMIENTO", AllowGroup = false)]
        public string DescriTipo { get; set; }

        [GridMapping(Index = IndexVolumen, ResourceName = "Labels", VariableName = "VOLUMEN", DataFormatString = "{0:2} lit", AllowGroup = false, IsAggregate = true, AggregateTextFormat = "Volumen Utilizado: {0} lit")]
        public double Volumen { get; set; }

        public IngresoVo(Movimiento m)
        {
            Tanque = m.Tanque != null ? m.Tanque.Descripcion : String.Empty;
            Fecha = m.Fecha;
            DescriTipo = m.TipoMovimiento.Descripcion;
            Volumen = m.Volumen;
        }
    }
}
