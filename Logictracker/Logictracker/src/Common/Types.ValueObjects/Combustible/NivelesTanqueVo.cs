using System;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class NivelesTanqueVo
    {
        public const int IndexTanqueDescri = 0;
        public const int IndexFecha = 1;
        public const int IndexVolumen = 2;
        public const int IndexPorcentajeLleno = 3;

        [GridMapping(Index = IndexTanqueDescri, ResourceName = "Entities", VariableName = "PARENTI36", AllowGroup = true, IsInitialGroup = true)]
        public string TanqueDescri { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", DataFormatString = "{0:G}", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexVolumen, ResourceName = "Labels", VariableName = "VOLUMEN", DataFormatString = "{0:2} lit", AllowGroup = false)]
        public double Volumen { get; set; }

        [GridMapping(Index = IndexPorcentajeLleno, ResourceName = "Labels", VariableName = "PORCENTAJE_LLENO", AllowGroup = false)]
        public string PorcentajeLleno { get; set; }

        public NivelesTanqueVo(NivelesTanque v)
        {
            TanqueDescri = v.TanqueDescri;
            Fecha = v.Fecha;
            Volumen = v.Volumen;
            PorcentajeLleno = v.PorcentajeLleno;
        }
    }
}
