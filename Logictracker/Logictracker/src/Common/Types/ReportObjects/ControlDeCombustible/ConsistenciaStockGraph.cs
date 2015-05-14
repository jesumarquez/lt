#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible
{
    [Serializable]
    public class ConsistenciaStockGraph
    {
        public DateTime Fecha { get; set; }
        public double StockReal { get; set; }
        public double StockTeorico { get; set; }
    }
}
