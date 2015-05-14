#region Usings

using System;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class ConsistenciaStockPozoVo
    {
        public const int IndexFecha = 0;
        public const int IndexStockInicial = 1;
        public const int IndexIngresos = 2;
        public const int IndexIngresosPorConciliacion = 3;
        public const int IndexEgresos = 4;
        public const int IndexEgresosPorConciliacion = 5;
        public const int IndexStockFinal = 6;
        public const int IndexStockSonda = 7;
        public const int IndexDiferenciaDeStock = 8;

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", AllowGroup = false, InitialSortExpression = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexStockInicial, ResourceName = "Labels", VariableName = "STOCK_INICIAL", AllowGroup = false)]
        public double StockInicial { get; set; }

        [GridMapping(Index = IndexIngresos, ResourceName = "Labels", VariableName = "INGRESOS", AllowGroup = false)]
        public double Ingresos { get; set; }

        [GridMapping(Index = IndexIngresosPorConciliacion, ResourceName = "Labels", VariableName = "INGRESOS_CONCILIACION", AllowGroup = false)]
        public double IngresosPorConciliacion { get; set; }

        [GridMapping(Index = IndexEgresos, ResourceName = "Labels", VariableName = "EGRESOS", AllowGroup = false)]
        public double Egresos { get; set; }

        [GridMapping(Index = IndexEgresosPorConciliacion, ResourceName = "Labels", VariableName = "EGRESOS_CONCILIACION", AllowGroup = false)]
        public double EgresosPorConciliacion { get; set; }

        [GridMapping(Index = IndexStockFinal, ResourceName = "Labels", VariableName = "STOCK_FINAL", AllowGroup = false)]
        public double StockFinal { get; set; }

        [GridMapping(Index = IndexStockSonda, ResourceName = "Labels", VariableName = "STOCK_SONDA", AllowGroup = false)]
        public double StockSonda { get; set; }

        [GridMapping(Index = IndexDiferenciaDeStock, ResourceName = "Labels", VariableName = "DIFERENCIA", AllowGroup = false)]
        public double DiferenciaDeStock { get; set; }

        public ConsistenciaStockPozoVo(ConsistenciaStockPozo c)
        {
            Fecha = c.Fecha;
            StockInicial = c.StockInicial;
            Ingresos = c.Ingresos;
            IngresosPorConciliacion = c.IngresosPorConciliacion;
            Egresos = c.Egresos;
            EgresosPorConciliacion = c.EgresosPorConciliacion;
            StockFinal = c.StockFinal;
            StockSonda = c.StockSonda;
            DiferenciaDeStock = c.DiferenciaDeStock;
        }
    }
}
