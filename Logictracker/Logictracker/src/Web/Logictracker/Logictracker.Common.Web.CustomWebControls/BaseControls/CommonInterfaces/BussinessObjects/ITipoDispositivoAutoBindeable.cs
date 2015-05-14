#region Usings

using System;

#endregion

namespace Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects
{
    /// <summary>
    /// Interface for defining common functionality for device type auto bindeable controls.
    /// </summary>
    public interface ITipoDispositivoAutoBindeable : IAutoBindeable
    {
        #region Properties

        /// <summary>
        /// Determines wither to display or not the no assigment value.
        /// </summary>
        Boolean AddSinAsignar { get; set; }

        #endregion
    }
}
