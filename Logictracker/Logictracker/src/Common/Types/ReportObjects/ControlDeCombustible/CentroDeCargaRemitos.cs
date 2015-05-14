#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible
{
    [Serializable]
    public class CentroDeCargaRemitos
    {
        public virtual string CentroDeCostos { get; set; }
        public virtual double TotalCargado { get; set; }
        public virtual int CantRemitos { get; set; }
        public virtual int CantAjustes { get; set; }
    }
}
