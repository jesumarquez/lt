#region Usings

using Urbetrack.Types.BusinessObjects.BaseObjects;

#endregion

namespace Urbetrack.Types.BusinessObjects.Positions
{
    public class LogUltimaPosicion : LogPosicionBase
    {
        #region Public Methods

        /// <summary>
        /// Determines if the givenn objects is equivalent to the current position.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var castObj = obj as LogUltimaPosicion;

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