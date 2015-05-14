#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Sistema : IComparable, IAuditable
    {
        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion


        public virtual string Descripcion { get; set; }

        public virtual string Url { get; set; }

        public virtual short Orden { get; set; }

        public virtual short Enabled { get; set; }

        #endregion

        #region Public Methods

        public virtual int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var sistema = obj as Sistema;

            return sistema == null ? 1 : Orden.CompareTo(sistema.Orden);
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if ((obj == null) || (obj.GetType() != GetType())) return false;
            var castObj = (Sistema) obj;
            return (Id == castObj.Id) && (Id != 0);
        }

        public override int GetHashCode()
        {
            var hash = 57;
            hash = 27*hash*Id.GetHashCode();
            return hash;
        }

        #endregion
    }
}