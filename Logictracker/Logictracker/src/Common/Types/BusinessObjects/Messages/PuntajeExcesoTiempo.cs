#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Messages
{
    /// <summary>
    /// Class for representing the assigned value to the infraction duration.
    /// </summary>
    public class PuntajeExcesoTiempo : IAuditable
    {
        #region Public Properties

        public virtual int Id { get; set; }
        public virtual int Segundos { get; set; }
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