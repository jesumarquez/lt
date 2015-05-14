#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class RemitoVo
    {
        public const int IndexTanque = 0;
        public const int IndexFecha = 1;
        public const int IndexDescriTipo = 2;
        public const int IndexNroRemito = 3;
        public const int IndexVolumen = 4;

        [GridMapping(Index = IndexTanque, ResourceName = "Entities", VariableName = "PARENTI36", AllowGroup = true, IsInitialGroup = true)]
        public string Tanque { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", DataFormatString = "{0:G}", AllowGroup = true, InitialSortExpression = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexDescriTipo, ResourceName = "Labels", VariableName = "TIPO_INGRESO", AllowGroup = true)]
        public string DescriTipo { get; set; }

        [GridMapping(Index = IndexNroRemito, ResourceName = "Labels", VariableName = "NRO_REMITO", AllowGroup = true)]
        public string NroRemito { get; set; }

        [GridMapping(Index = IndexVolumen, ResourceName = "Labels", VariableName = "VOLUMEN", DataFormatString = "{0:2} lit", AllowGroup = false, IsAggregate = true, AggregateTextFormat = "Total Despachado: {0} lit")]
        public double Volumen { get; set; }

        public RemitoVo(Movimiento m)
        {
            Fecha = m.Fecha;
            Volumen = m.Volumen;
            Tanque = m.Tanque.Descripcion;
            NroRemito = m.Observacion;
            DescriTipo = m.TipoMovimiento.Descripcion;
        }
    }
}
