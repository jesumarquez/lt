#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible
{
    [Serializable]
    public class CombustibleEvent
    {
        public DateTime Fecha { get; set; }
        public string MotorDescri { get; set; }
        public string Mensaje { get; set; }
        public int IDAccion { get; set; }
    }
}