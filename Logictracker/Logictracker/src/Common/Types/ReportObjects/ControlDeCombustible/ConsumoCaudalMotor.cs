#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible
{
    [Serializable]
    public class ConsumoCaudalMotor
    {
        public DateTime Fecha { get; set; }
        public Double Consumo { get; set; }
        public Double Caudal { get; set; }
    }
}
