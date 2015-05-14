namespace Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects
{
    /// <summary>
    /// Interface for custom controls associated to messages.
    /// </summary>
    public interface IMensajeAutoBindeable : IAutoBindeable
    {
        #region Properties

        /// <summary>
        /// Determines wether to show only maintenance messages.
        /// </summary>
        bool SoloMantenimiento { get; }

        /// <summary>
        /// Determines wether to show only combustible messages.
        /// </summary>
        bool SoloCombustible { get; }

        /// <summary>
        /// Determines wither to add the non message option.
        /// </summary>
        bool AddSinMensaje { get; }

        bool SoloAtencion { get; }

        bool BindIds { get; }


        #endregion
    }
}