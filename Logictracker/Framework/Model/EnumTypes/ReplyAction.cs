namespace Logictracker.Model.EnumTypes
{
    /// <summary>
    /// Enumeracion de acciones utilizadas entre niveles para señalizar
    /// hacia atras.
    /// </summary>
    public enum ReplyAction
    {
        /// No tiene implicancias, se debe seguir normalmente.
        None,
        /// Implica que la solicitud, contiene la respuesta que debe
        /// enviarse al origen.
        ReturnedResponse,
        /// Implica que la solicitud, contiene la respuesta que debe
        /// enviarse al origen, pero sin
        /// disparar hacia adelante los eventos involucrados.
        ReturnedResponseSilently,
        /// Implica liberar todos los recursos asociados, disparando hacia
        /// adelante los eventos involucrados.
        Release,
        /// Implica liberar todos los recursos asociados, pero sin
        /// disparar hacia adelante los eventos involucrados.
        ReleaseSilently,
        /// Implica que la solicitud no es sportada por la implementacion, 
        /// pero NO implica un error.
        NotSupported,
    };
}