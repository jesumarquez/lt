#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Sonido : IAuditable
    {
        private string _descri;
        private string _url;

        public Sonido()
        {
            Id = 0;
            _descri = null;
            _url = null;
        }

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion


        public virtual string Descripcion
        {
            get { return _descri; }
            set
            {
                if (value != null)
                    if (value.Length > 50)
                        throw new ArgumentOutOfRangeException("Valor invalido para Sonido.Descripcion", value,
                                                              value);

                _descri = value;
            }
        }
        public virtual string URL
        {
            get { return _url; }
            set
            {
                if (value != null)
                    if (value.Length > 128)
                        throw new ArgumentOutOfRangeException("Valor invalido para Sonido.URL", value,
                                                              value);

                _url = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if ((obj == null) || (obj.GetType() != GetType())) return false;
            var castObj = (Icono)obj;
            return (Id == castObj.Id) &&
                   (Id != 0);
        }

        public override int GetHashCode()
        {
            var hash = 57;
            hash = 27 * hash * Id.GetHashCode();
            return hash;
        }
    }
}