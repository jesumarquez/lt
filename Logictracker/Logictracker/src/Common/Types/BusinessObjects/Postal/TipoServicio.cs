#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Postal
{
    [Serializable]
    public class TipoServicio : IAuditable
    {
        public virtual int Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string DescripcionCorta { get; set; }
        public virtual short? ConAcuse { get; set; }
        public virtual short? ConFoto { get; set; }
        public virtual short? ConLaterales { get; set; }
        public virtual short? ConReferencia { get; set; }
        public virtual short? ConGps { get; set; }
        public virtual DateTime FechaModificacion { get; set; }
        public virtual DateTime? FechaBaja { get; set; }

        public virtual Type TypeOf() { return GetType(); }

        public override Boolean Equals(Object obj)
        {
            var castObj = obj as TipoServicio;

            return castObj != null && castObj.Id.Equals(Id);
        }

        public override int GetHashCode() { return Id.GetHashCode(); }
    }
}
