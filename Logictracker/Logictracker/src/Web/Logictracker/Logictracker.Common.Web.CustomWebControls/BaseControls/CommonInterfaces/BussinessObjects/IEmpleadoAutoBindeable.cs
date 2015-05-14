namespace Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects
{
    /// <summary>
    /// Interface for custom controls associated to employees.
    /// </summary>
    public interface IEmpleadoAutoBindeable : IAutoBindeable
    {
        #region Properties

        /// <summary>
        /// Determines wither to filter only employees in charge of other employees.
        /// </summary>
        bool SoloResponsables { get; }

        /// <summary>
        /// Determines if it will filter the results based only in district values.
        /// </summary>
        bool AllowOnlyDistrictBinding { get; }

        #endregion
    }
}