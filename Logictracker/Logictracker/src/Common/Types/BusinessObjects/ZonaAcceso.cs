using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class ZonaAcceso : IAuditable, ISecurable, IHasTipoZonaAcceso
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion

        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }
        public virtual TipoZonaAcceso TipoZonaAcceso { get; set; }
    }
}