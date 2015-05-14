#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Taller : IAuditable, ISecurable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion

        public virtual string Descripcion { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Telefono { get; set; }
        public virtual Empleado Responsable { get; set; }
        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }
        public virtual bool Baja { get; set; }
    }
}
