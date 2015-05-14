#region Usings

using System;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Transportista : IAuditable, ISecurable
    {
        private ISet _coches;
        private ISet _tarifas;

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion

        public virtual Double TarifaTramoCorto { get; set; }
        public virtual Double TarifaTramoLargo { get; set; }
        public virtual string Telefono { get; set; }
        public virtual string Mail { get; set; }
        public virtual string Contacto { get; set; }
        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }
        public virtual bool Baja { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool IdentificaChoferes { get; set; }

        public virtual ISet Coches { get { return _coches ?? (_coches = new ListSet()); } }

        public virtual ISet Tarifas { get { return _tarifas ?? (_tarifas = new ListSet()); } }

        public override bool Equals(object obj)
        {
            if (obj == this) return true;

            var objCast = obj as Transportista;

            return objCast != null && Id == objCast.Id && objCast.Id != 0;
        }

        public override int GetHashCode()
        {
            var hash = 57;
            hash = 27*hash* Id.GetHashCode();
            return hash;
        }
    }
}