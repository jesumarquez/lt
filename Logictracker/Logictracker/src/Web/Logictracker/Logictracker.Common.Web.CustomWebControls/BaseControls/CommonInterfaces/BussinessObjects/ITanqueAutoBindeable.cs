namespace Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects
{
    /// <summary>
    /// Interface for tank auto bindeable custom controls.
    /// </summary>
    public interface ITanqueAutoBindeable : IAutoBindeable
    {
        #region Properties

        /// <summary>
        /// Determines wither to bind or not by equipment.
        /// </summary>
        bool AllowEquipmentBinding { get; }

        /// <summary>
        /// Determines wither to bind or not by Base.
        /// </summary>
        bool AllowBaseBinding { get; }
               
        #endregion
    }
}