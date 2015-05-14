namespace Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects
{
    /// <summary>
    /// Extends the IAutoBindeable interface for adding device associated logic.
    /// </summary>
    public interface IDispositivoAutoBindeable : IAutoBindeable
    {
        #region Properties

        /// <summary>
        /// The id of the mobile associated to the device.
        /// </summary>
        int Coche { get; }

        string Padre { get; }

        bool HideAssigned { get; }

        #endregion
    }
}