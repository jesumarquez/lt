#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Funcion : IComparable, IAuditable
    {
        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion


        public virtual Sistema Sistema { get; set; }

        public virtual string Descripcion { get; set; }

        public virtual string Modulo { get; set; }

        public virtual short Tipo { get; set; }

        public virtual string Url { get; set; }

        public virtual string Parametros { get; set; }

        public virtual string FechaBaja { get; set; }

        public virtual string Ref { get; set; }

        #endregion

        #region Public Methods

        public virtual int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var funcion = obj as Funcion;

            if (funcion == null) return 1;

            var order = Sistema.CompareTo(funcion.Sistema);

            return !order.Equals(0) ? order : Descripcion.CompareTo(funcion.Descripcion);
        }

        public override bool Equals(object obj)
        {
            var castObj = obj as Funcion;

            return castObj != null && Id == castObj.Id && Id != 0;
        }

        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }

        #endregion
    }
}