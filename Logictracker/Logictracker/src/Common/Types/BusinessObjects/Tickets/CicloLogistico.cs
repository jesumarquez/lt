#region Usings

using System;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Tickets
{
    [Serializable]
    public class CicloLogistico : IAuditable, ISecurable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get { return Owner != null ? Owner.Empresa : null; } }
        public virtual Linea Linea { get { return Owner != null ? Owner.Linea : null; } }

        #endregion

        public virtual Owner Owner { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool EsCiclo { get; set; }
        public virtual bool EsEstado { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Custom1 { get; set; }
        public virtual string Custom2 { get; set; }
        public virtual string Custom3 { get; set; }
        public virtual bool Baja { get; set; }

        private ISet _detalles;

        public virtual ISet Detalles { get { return _detalles ?? (_detalles = new ListSet()); } }

        public CicloLogistico()
        {
            Owner = new Owner();
        }
    }
}
