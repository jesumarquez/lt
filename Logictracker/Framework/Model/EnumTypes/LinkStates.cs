namespace Logictracker.Model.EnumTypes
{
    /// <summary>
    /// Enumaracion de los Estados posibles de una sesion. 
    /// </summary>
    public enum LinkStates
    {
        /// Link Nuevo
        New,
        /// Link Activo
        Active,
        /// El Link se vencio por inactividad.
        Expired
    };
}