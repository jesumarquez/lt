#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class DetallePeriodo : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual short Estado { get; set; }
        public virtual Periodo Periodo { get; set; }
        public virtual Transportista Transportista { get; set; }
    }
}
