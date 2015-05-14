#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class TipoEmpleado : IAuditable, ISecurable
    {
        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }
        public virtual DateTime? FechaBaja { get; set; }
        
        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var tipoEmpleado = obj as TipoEmpleado;

            return tipoEmpleado != null && Id.Equals(tipoEmpleado.Id);
        }

        public override int GetHashCode() { return Id.GetHashCode(); }

        public override string ToString() { return Descripcion; }

        #endregion
    }
}
