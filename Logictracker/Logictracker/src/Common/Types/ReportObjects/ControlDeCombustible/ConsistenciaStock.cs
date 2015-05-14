#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible
{
    [Serializable]
    public class ConsistenciaStock
    {
        public virtual DateTime Fecha { get; set; }
        public virtual double StockInicial { get; set; }
        public virtual double Ingresos { get; set; }
        public virtual double Egresos { get; set; }
        public virtual double StockSonda { get; set; }
        public virtual double StockFinal { get; set; }
        public virtual double Diferencia { get; set; }
    }
}
