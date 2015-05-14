#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Cliente : IAuditable, ISecurable
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

        public virtual string DescripcionCorta { get; set; }

        public virtual string Telefono { get; set; }

        public virtual bool Baja { get; set; }

        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }

        public virtual bool Nomenclado { get; set; }

        public virtual string DireccionNomenclada { get; set; }

        public virtual string Comentario1 { get; set; }
        public virtual string Comentario2 { get; set; }
        public virtual string Comentario3 { get; set; }

        public Cliente()
        {
            Nomenclado = true;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var cliente = obj as Cliente;
            if (cliente == null) return false;
            return Id == cliente.Id;
        }

        public override int GetHashCode() { return Id; }

        public override string ToString() { return Descripcion; }
    }
}