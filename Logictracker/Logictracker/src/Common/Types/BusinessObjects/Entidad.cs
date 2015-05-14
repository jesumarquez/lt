#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Entidad : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual bool Baja { get; set; }
        public virtual string Cuil { get; set; }
        public virtual string Apellido { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string TipoDocumento { get; set; }
        public virtual string NroDocumento { get; set; }
        public virtual Direccion Direccion { get; set; }

        public virtual string Descripcion
        {
            get
            {
                if (Nombre == null) return Apellido.Trim();
                
                return Apellido.Trim() + ", " + Nombre.Trim();
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if ((obj == null) || (obj.GetType() != GetType())) return false;
            var castObj = obj as Entidad;
            return (castObj != null) && (Id == castObj.Id) && (Id != 0);
        }

        /// <summary>
        /// Local implementation of GetHashCode based on unique value members
        /// </summary>
        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }
    }
}