namespace Logictracker.Model.EnumTypes
{
    /// <summary>
    /// Resulatados del procesamiento de mensajes
    /// </summary>
    public enum HandleResults
    {
        /// Exito, seguir procesando handlers
        Success,
        /// Exito, no seguir procesando handlers.
        BreakSuccess,
        /// Descartado silenciosamente, seguir procesando handlers.
        SilentlyDiscarded,
        /// Error generico, seguir procesando handlers.
        UnspecifiedFailure
    };
}