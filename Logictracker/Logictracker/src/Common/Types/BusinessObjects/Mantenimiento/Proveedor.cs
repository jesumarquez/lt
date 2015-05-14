using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Mantenimiento
{
    [Serializable]
    public class Proveedor : IAuditable, ISecurable, IHasTipoProveedor
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual TipoProveedor TipoProveedor { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Codigo { get; set; }

        public override string ToString()
        {
            return Descripcion;
        }
    }
}