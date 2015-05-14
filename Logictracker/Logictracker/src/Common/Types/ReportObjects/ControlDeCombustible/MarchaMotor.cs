#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible
{
    [Serializable]
    public class MarchaMotor
    {
        public int HsEnMarcha { get; set; }
        public DateTime Fecha { get; set; }
    }
}