namespace Logictracker.Model.EnumTypes
{
    /// <summary>
    /// Enumeracion de los resultados posibles de carga de una clase 
    /// que implemente ILoaderSettings.
    /// </summary>
    public enum LoadResults
    {
        /// Parametro cargado con exito.
        LoadOk,
        /// Parametro cargado con exito, pero requiere reiniciar el componente,
        /// para que los cambios surjan efecto.
        LoadRestartRequired,
        /// Parametro desconocido, se continua normalmente.
        SettingUnknown,
        /// El Parametro es valido, pero los datos no tienen sentido.
        /// se toma el valor por defecto.
        SettingError,
        /// El parametro es valido, y se ha seteado un error de contexto
        /// que descrive dicho error.
        AnnotatedError
    };
}
