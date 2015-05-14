#region Usings

using System;
using System.Drawing;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ReferenciasGeograficas
{
    [Serializable]
    public class Punto : IAuditable
    {
        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Poligono Poligono { get; set; }
        public virtual double Latitud { get; set; }
        public virtual double Longitud { get; set; }
        public virtual int Orden { get; set; }

        #endregion

        #region Public Methods

        public virtual PointF ToPointF() { return new PointF((float)Longitud, (float)Latitud); }

        public virtual bool Equals(Punto obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.Latitud == Latitud && obj.Longitud == Longitud && obj.Orden == Orden;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == typeof (Punto) && Equals((Punto) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = Latitud.GetHashCode();
                result = (result*397) ^ Longitud.GetHashCode();
                result = (result*397) ^ Orden;

                return result;
            }
        }

        #endregion
    }
}