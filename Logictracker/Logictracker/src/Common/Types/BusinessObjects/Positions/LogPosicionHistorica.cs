using Logictracker.Types.BusinessObjects.BaseObjects;

namespace Logictracker.Types.BusinessObjects.Positions
{
    public class LogPosicionHistorica : LogPosicionBase
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
            var castObj = obj as LogPosicionHistorica;
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
