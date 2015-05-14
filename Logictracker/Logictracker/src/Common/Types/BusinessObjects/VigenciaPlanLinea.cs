using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class VigenciaPlanLinea : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual DateTime Inicio { get; set; }
        public virtual DateTime? Fin { get; set; }
        public virtual Plan Plan { get; set; }
        public virtual LineaTelefonica LineaTelefonica { get; set; }
    }
}