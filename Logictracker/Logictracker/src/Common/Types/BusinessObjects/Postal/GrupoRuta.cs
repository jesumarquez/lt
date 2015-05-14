using System;

namespace Logictracker.Types.BusinessObjects.Postal
{
    public class GrupoRuta : IEquatable<GrupoRuta>
    {
        public Distribuidor Distribuidor { get; set;}
        public string NumeroRuta { get; set;}

        public string ShowName
        {
            get
            {
                return Distribuidor.Usuario + " - " + NumeroRuta;
            }
        }

        public bool Equals(GrupoRuta other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, Distribuidor)) return false;
            return Equals(other.Distribuidor.Id, Distribuidor.Id) && Equals(other.NumeroRuta, NumeroRuta);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(null, Distribuidor)) return false;
            if (obj.GetType() != typeof (GrupoRuta)) return false;
            return Equals((GrupoRuta) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Distribuidor != null ? Distribuidor.GetHashCode() : 0)*397) ^ (NumeroRuta != null ? NumeroRuta.GetHashCode() : 0);
            }
        }

        public static bool operator ==(GrupoRuta left, GrupoRuta right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GrupoRuta left, GrupoRuta right)
        {
            return !Equals(left, right);
        }
    }
}
