#region Usings

using System;
using Logictracker.Cache.Interfaces;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Linea : IDataIdentify, IAuditable, IHasEmpresa
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string DescripcionCorta { get; set; }
        public virtual bool Baja { get; set; }
        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }
        public virtual string Telefono { get; set; }
        public virtual string Mail { get; set; }
        public virtual string TimeZoneId { get; set; }
        public virtual bool Interfaceable { get; set; }
        public virtual bool IdentificaChoferes { get; set; }
        
        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var castObj = obj as Linea;

            return castObj!= null && (Id == castObj.Id) && (Id != 0);
        }

        public override int GetHashCode()
        {
            var hash = 57;

            hash = 27*hash*Id.GetHashCode();

            return hash;
        }

        public override string ToString() { return Descripcion; }
    }
}