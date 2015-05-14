using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;

namespace Logictracker.Types.BusinessObjects.Positions
{
    /// <summary>
    /// Class that represents a position reported by any device.
    /// </summary>
    public class LogPosicion : LogPosicionBase
    {
        public LogPosicion()
        {
        }

        public LogPosicion(GPSPoint position, Coche coche)
            :base(position, coche)
        {

        }

        #region Public Methods

        /// <summary>
        /// Determines if the givenn objects is equivalent to the current position.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var castObj = obj as LogPosicion;

            return castObj != null && Id == castObj.Id && Id != 0;
        }

        /// <summary>
        /// Get the hash code associated to the position based on its id.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return 27 * 57 * Id.GetHashCode(); }

        #endregion
    }
}