#region Usings

using System;

#endregion 

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class TransportActivity : MobileActivity
    {
        #region Public Properties

        public string Transport { get; set; }
        public int CantidadViajes { get; set; }
        public int IdVehiculo { get; set; }

        #endregion
    }
}
