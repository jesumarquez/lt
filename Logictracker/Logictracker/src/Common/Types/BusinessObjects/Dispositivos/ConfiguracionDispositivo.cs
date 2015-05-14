#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Dispositivos
{
    /// <summary>
    /// Class that represents a application device configuration.
    /// </summary>
    [Serializable]
    public class ConfiguracionDispositivo : IAuditable
    {
        #region Public Properties

        #region IAuditable

        public virtual Int32 Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual String Nombre { get; set; }
        public virtual String Descripcion { get; set; }
        public virtual String Configuracion { get; set; }
        public virtual Int32 Orden { get; set; }

        #endregion

        #region Public Methods

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var castObj = obj as ConfiguracionDispositivo;

            return castObj != null && Id.Equals(castObj.Id);
        }

        public override int GetHashCode() { return Id; }

        #endregion
    }
}
