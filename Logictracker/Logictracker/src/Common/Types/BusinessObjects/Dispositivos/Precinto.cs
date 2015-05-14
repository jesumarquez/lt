using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Dispositivos
{
    [Serializable]
    public class Precinto : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Codigo { get; set; }
        public virtual bool Baja { get; set; }
    }
}