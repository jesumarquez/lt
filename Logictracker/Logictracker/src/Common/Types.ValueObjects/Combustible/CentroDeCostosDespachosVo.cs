#region Usings

using System;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class CentroDeCostosDespachosVo
    {
        public const int IndexCentroDeCostos = 0;
        public const int IndexTotalDespachado = 1;
        public const int IndexCantVehiculos = 2;
        public const int IndexCantDespachos = 3;

        [GridMapping(Index = IndexCentroDeCostos, ResourceName = "Labels", VariableName = "CENTRO_CARGA", AllowGroup = false, InitialSortExpression = true)]
        public string CentroDeCostos { get; set; }

        [GridMapping(Index = IndexTotalDespachado, ResourceName = "Labels", VariableName = "TOTAL_DESPACHADO", DataFormatString = "{0:0.00} lit", AllowGroup = false)]
        public double TotalDespachado { get; set; }

        [GridMapping(Index = IndexCantVehiculos, ResourceName = "Labels", VariableName = "CANTIDAD_VEHICULOS", AllowGroup = false)]
        public int CantVehiculos { get; set; }

        [GridMapping(Index = IndexCantDespachos, ResourceName = "Labels", VariableName = "DESPACHOS_REALIZADOS", AllowGroup = false)]
        public int CantDespachos { get; set; }

        public CentroDeCostosDespachosVo(CentroDeCostosDespachos m)
        {
            CentroDeCostos = m.CentroDeCostos;
            TotalDespachado = m.TotalDespachado;
            CantVehiculos = m.CantVehiculos;
            CantDespachos = m.CantDespachos;
        }
    }
}
