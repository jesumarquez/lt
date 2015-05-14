#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class MovMenu : IAuditable
    {
        #region Public Properties

        public virtual int Id { get; set; }

        public virtual Perfil Perfil { get; set; }

        public virtual Funcion Funcion { get; set; }

        public virtual short Orden { get; set; }

        public virtual bool Alta { get; set; }

        public virtual bool Modificacion { get; set; }

        public virtual bool Baja { get; set; }

        public virtual bool Consulta { get; set; }

        public virtual bool Reporte { get; set; }

        public virtual bool VerMapa { get; set; }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if ((obj == null) || (obj.GetType() != GetType())) return false;
            var castObj = (MovMenu) obj;
            return (Id == castObj.Id) && (Id != 0);
        }

        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }

        public virtual bool IsActive() { return Alta || Baja || Consulta || Modificacion || Reporte || VerMapa; }

        public virtual Type TypeOf() { return GetType(); }

        #endregion
    }
}