#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible
{
    [Serializable]
    public class ConsumosMotor
    {
        public string CentroDeCostos { get; set; }
        public string DescripcionMotor { get; set; }
        public Double DifVolumen { get; set; }
        public Double CaudalMinimo { get; set; }
        public Double CaudalMaximo { get; set; }
        public Double HsEnMarcha { get; set; }
        public int CantidadConsumos { get; set; }
        public int IDMotor { get; set; }
        public DateTime UltiMedicion { get; set; }
    }
}
