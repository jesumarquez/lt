#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.ControlDeCombustible
{
    [Serializable]
    public class CentroDeCostosDespachos
    {
        public virtual string CentroDeCostos { get; set; }
        public virtual double TotalDespachado { get; set; }
        public virtual int CantVehiculos { get; set; }
        public virtual int CantDespachos { get; set; }
    }
}
