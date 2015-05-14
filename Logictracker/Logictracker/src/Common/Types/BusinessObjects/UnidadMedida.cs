using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class UnidadMedida : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Descripcion { get; set; }
        public virtual string Simbolo { get; set; }
        public virtual string Codigo { get; set; }
    }
}