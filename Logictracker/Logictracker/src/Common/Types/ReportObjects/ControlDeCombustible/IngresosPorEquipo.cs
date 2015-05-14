#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible
{
    [Serializable]
    public class IngresosPorEquipo
    {
        public string NombreEquipo { get; set; }
        public double TotalCargado { get; set; }
        public int CantIngresos { get; set; }
        public int IDEquipo { get; set; }
        public DateTime UltiMedicion { get; set; }
    }
}
