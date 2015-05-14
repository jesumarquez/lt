#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Dispositivos
{
    [Serializable]
    public class Firmware : IAuditable
    {
        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Nombre { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual DateTime? Fecha { get; set; }
        public virtual string Firma { get; set; }
        public virtual byte[] Binario { get; set; }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            var castObj = (Firmware) obj;

            return (castObj != null) && (Id == castObj.Id) && (Id != 0);
        }

        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }

        #endregion
    }
}
