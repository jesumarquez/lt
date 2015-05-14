#region Usings

using System;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class CentroDeCargaRemitosVo
    {
        public const int IndexCentroDeCostos = 0;
        public const int IndexTotalCargado = 1;
        public const int IndexCantRemitos = 2;
        public const int IndexCantAjustes = 3;

        [GridMapping(Index = IndexCentroDeCostos, ResourceName = "Labels", VariableName = "CENTRO_CARGA", AllowGroup = false, InitialSortExpression = true)]
        public string CentroDeCostos { get; set; }

        [GridMapping(Index = IndexTotalCargado, ResourceName = "Labels", VariableName = "TOTAL_CARGADO", DataFormatString = "{0:2} lit", AllowGroup = false)]
        public double TotalCargado { get; set; }

        [GridMapping(Index = IndexCantRemitos, ResourceName = "Labels", VariableName = "CANTIDAD_REMITOS", AllowGroup = false)]
        public int CantRemitos { get; set; }

        [GridMapping(Index = IndexCantAjustes, ResourceName = "Labels", VariableName = "CANTIDAD_AJUSTES", AllowGroup = false)]
        public int CantAjustes { get; set; }

        public CentroDeCargaRemitosVo(CentroDeCargaRemitos m)
        {
            CentroDeCostos = m.CentroDeCostos;
            TotalCargado = m.TotalCargado;
            CantAjustes = m.CantAjustes;
            CantRemitos = m.CantRemitos;
        }
    }
}
