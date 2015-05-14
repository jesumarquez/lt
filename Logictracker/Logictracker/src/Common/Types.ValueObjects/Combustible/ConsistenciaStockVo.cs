using System;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class ConsistenciaStockVo
    {
        public const int IndexFecha = 0;
        public const int IndexStockInicial = 1;
        public const int IndexIngresos = 2;
        public const int IndexEgresos = 3;
        public const int IndexStockFinal = 4;
        public const int IndexStockSonda = 5;
        public const int IndexDiferencia = 6;

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", AllowGroup = false, InitialSortExpression = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexStockInicial, ResourceName = "Labels", VariableName = "STOCK_INICIAL", AllowGroup = false)]
        public double StockInicial { get; set; }

        [GridMapping(Index = IndexIngresos, ResourceName = "Labels", VariableName = "INGRESOS", AllowGroup = false)]
        public double Ingresos { get; set; }

        [GridMapping(Index = IndexEgresos, ResourceName = "Labels", VariableName = "EGRESOS", AllowGroup = false)]
        public double Egresos { get; set; }

        [GridMapping(Index = IndexStockFinal, ResourceName = "Labels", VariableName = "STOCK_FINAL", AllowGroup = false)]
        public double StockFinal { get; set; }

        [GridMapping(Index = IndexStockSonda, ResourceName = "Labels", VariableName = "STOCK_SONDA", AllowGroup = false)]
        public double StockSonda { get; set; }

        [GridMapping(Index = IndexDiferencia, ResourceName = "Labels", VariableName = "DIFERENCIA", AllowGroup = false)]
        public double Diferencia { get; set; }

        public ConsistenciaStockVo(ConsistenciaStock c)
        {
            Fecha = c.Fecha;
            StockInicial = c.StockInicial;
            Ingresos = c.Ingresos;
            Egresos = c.Egresos;
            StockFinal = c.StockFinal;
            StockSonda = c.StockSonda;
            Diferencia = c.Diferencia;
        }
    }
}
