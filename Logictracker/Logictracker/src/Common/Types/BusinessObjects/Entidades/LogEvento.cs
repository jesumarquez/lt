using System;
using Logictracker.Types.BusinessObjects.BaseObjects;

namespace Logictracker.Types.BusinessObjects.Entidades
{
    [Serializable]
    public class LogEvento : LogEventoBase
    {
        #region Public Methods

        public override bool Equals(object obj)
        {
            var castObj = obj as LogEvento;

            return (castObj != null) && (Id == castObj.Id) && (Id != 0);
        }

        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }

        #endregion
    }
}