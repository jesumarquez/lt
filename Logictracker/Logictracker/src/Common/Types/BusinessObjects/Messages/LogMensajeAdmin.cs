using System;
using Logictracker.Types.BusinessObjects.BaseObjects;

namespace Logictracker.Types.BusinessObjects.Messages
{
    [Serializable]
    public class LogMensajeAdmin : LogMensajeBase
    {
        public virtual Zona Zona { get; set; }

        public virtual int IdCoche { get; set; }
        public virtual string CodigoMensaje { get; set; }

        #region Public Methods

        /// <summary>
        /// Determines if the givenn objects equals the current logmensaje.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var castObj = obj as LogMensaje;

            return (castObj != null) && (Id == castObj.Id) && (Id != 0);
        }

        /// <summary>
        /// Gets the hash code for the current logmensaje.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }

        #endregion
    }
}