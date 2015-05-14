#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Messages
{
    /// <summary>
    /// Class used to represent the assigned value to the speed excess.
    /// </summary>
    public class PuntajeExcesoVelocidad : IAuditable
    {
        #region Public Properties

        public virtual int Id { get; set; }
        public virtual int Porcentaje { get; set; }
        public virtual int Puntaje { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the type of the current object.
        /// </summary>
        /// <returns></returns>
        public virtual Type TypeOf() { return GetType(); }

        #endregion
    }
}
