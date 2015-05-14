#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Tarjeta : IAuditable, ISecurable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion

        public virtual string Numero { get; set; }
        public virtual string Pin { get; set; }
        public virtual string PinHexa { get; set; }
        public virtual int? CodigoAcceso { get; set; }

        public override bool Equals(object obj)
        {
            var castObj = obj as Tarjeta;

            return castObj != null && Id == castObj.Id && Id != 0;
        }

        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }
    }
}