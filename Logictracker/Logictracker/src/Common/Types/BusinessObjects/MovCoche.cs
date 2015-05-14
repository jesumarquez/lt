#region Usings

using System;
using Logictracker.Types.BusinessObjects.Vehiculos;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class MovCoche
    {
        private Coche _coche;
        private int _id;
        private Usuario _usuario;

        public MovCoche()
        {
            _id = 0;
            _usuario = null;
            _coche = null;
        }

        public virtual int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual Usuario Usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }

        public virtual Coche Coche
        {
            get { return _coche; }
            set { _coche = value; }
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if ((obj == null) || (obj.GetType() != GetType())) return false;
            var castObj = (MovCoche) obj;
            return (_id == castObj.Id) &&
                   (_id != 0);
        }

        public override int GetHashCode()
        {
            var hash = 57;
            hash = 27*hash*_id.GetHashCode();
            return hash;
        }
    }
}