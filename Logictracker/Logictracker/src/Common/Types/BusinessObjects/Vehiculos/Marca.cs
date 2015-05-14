using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Vehiculos
{
    [Serializable]
    public class Marca : IAuditable, ISecurable, IEquatable<Marca>
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }

        public virtual bool Equals(Marca other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Marca)) return false;
            return Equals((Marca) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(Marca left, Marca right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Marca left, Marca right)
        {
            return !Equals(left, right);
        }
    }
}