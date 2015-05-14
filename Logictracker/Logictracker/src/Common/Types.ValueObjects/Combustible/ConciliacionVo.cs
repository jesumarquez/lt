using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class ConciliacionVo
    {
        public const int IndexFecha = 0;
        public const int IndexDescripcion = 1;
        public const int IndexTipoMov = 2;
        public const int IndexVolumen = 3;

        public int Id { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", DataFormatString = "{0:d}", IncludeInSearch = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "NRO_REMITO", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexTipoMov, ResourceName = "Entities", VariableName = "OPECOMB01", IncludeInSearch = true)]
        public string TipoMov { get; set; }

        [GridMapping(Index = IndexVolumen, ResourceName = "Labels", VariableName = "VOLUMEN", DataFormatString = "{0} lit", AllowGroup = false)]
        public double Volumen { get; set; }

        public ConciliacionVo(Movimiento mov)
        {
            Id = mov.Id;
            Fecha = mov.Fecha;
            Descripcion = mov.Observacion;
            TipoMov = mov.TipoMovimiento.Descripcion;
            Volumen = mov.Volumen;
        }
    }
}

