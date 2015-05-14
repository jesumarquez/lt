using System;
using System.Linq;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Icono : IAuditable, ISecurable
    {
        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea {get; set;}
        public virtual string PathIcono {get; set;}
        public virtual string Descripcion { get; set; }
        public virtual short Width { get; set; }
        public virtual short Height { get; set; }
        public virtual short OffsetX { get; set; }
        public virtual short OffsetY { get; set; }

        #endregion

        #region Public Methods

        public virtual bool Equals(Icono obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Icono)) return false;
            return Equals((Icono) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public virtual bool HasKeywords(string keywords)
        {
            var kw = keywords.ToLower().Split(' ');

            return kw.All(k => Descripcion.ToLower().Contains(k));
        }

        #endregion
    }
}