#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible
{
    [Serializable]
    public class NivelesTanque
    {
        public Double Volumen { get; set; }
        public DateTime Fecha { get; set; }
        public int IdTanque { get; set; }
        public string TanqueDescri { get; set; }
        public double PorcentajeLlenado { private get; set; }

        /// <summary>
        /// El porcentaje de llenado convertido a string.
        /// </summary>
        public string PorcentajeLleno
        {
            get { return PorcentajeLlenado < 0? "Capacidad del tanque sin definir" : String.Concat(String.Format("{0:00.00}", PorcentajeLlenado), "%"); }
        }
    }
}
