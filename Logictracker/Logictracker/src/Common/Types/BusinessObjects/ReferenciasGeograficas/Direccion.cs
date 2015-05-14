#region Usings

using System;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ReferenciasGeograficas
{
    [Serializable]
    public class Direccion : IAuditable
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual short IdMapa { get; set; }
        public virtual int IdCalle { get; set; }
        public virtual int IdEsquina { get; set; }
        public virtual int IdEntrecalle { get; set; }
        public virtual int Altura { get; set; }
        public virtual double Latitud { get; set; }
        public virtual double Longitud { get; set; }
        public virtual string Calle { get; set; }
        public virtual string Partido { get; set; }
        public virtual string Provincia { get; set; }
        public virtual string Pais { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual Vigencia Vigencia { get; set; }

        public virtual void CloneData(Direccion dir)
        {
            IdMapa = dir.IdMapa;
            IdCalle = dir.IdCalle;
            IdEsquina = dir.IdEsquina;
            IdEntrecalle = dir.IdEntrecalle;
            Altura = dir.Altura;
            Latitud = dir.Latitud;
            Longitud = dir.Longitud;
            Calle = dir.Calle;
            Partido = dir.Partido;
            Provincia = dir.Provincia;
            Pais = dir.Pais;
            Descripcion = dir.Descripcion;
        }

        public virtual bool Equals(Direccion obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.IdMapa == IdMapa && obj.IdCalle == IdCalle && obj.IdEsquina == IdEsquina && obj.IdEntrecalle == IdEntrecalle && obj.Altura == Altura && obj.Latitud == Latitud && obj.Longitud == Longitud && Equals(obj.Calle, Calle) && Equals(obj.Partido, Partido) && Equals(obj.Provincia, Provincia) && Equals(obj.Pais, Pais) && Equals(obj.Descripcion, Descripcion);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Direccion)) return false;
            return Equals((Direccion) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = IdMapa.GetHashCode();
                result = (result*397) ^ IdCalle;
                result = (result*397) ^ IdEsquina;
                result = (result*397) ^ IdEntrecalle;
                result = (result*397) ^ Altura;
                result = (result*397) ^ Latitud.GetHashCode();
                result = (result*397) ^ Longitud.GetHashCode();
                result = (result*397) ^ Calle.GetHashCode();
                result = (result*397) ^ Partido.GetHashCode();
                result = (result*397) ^ Provincia.GetHashCode();
                result = (result*397) ^ Pais.GetHashCode();
                result = (result*397) ^ Descripcion.GetHashCode();
                return result;
            }
        }
    }
}
