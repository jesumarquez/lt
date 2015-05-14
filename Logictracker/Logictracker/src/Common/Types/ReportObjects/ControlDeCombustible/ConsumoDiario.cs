#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible
{
    [Serializable]
    public class ConsumoDiario
    {
        public DateTime Fecha { get; set; }
        public Double VolumenConsumido { get; set; }
        public Double Ingresos { get; set; }
        public Double IngresosPorConciliacion { get; set; }
        public Double EgresosPorConciliacion { get; set; }
    }
}
