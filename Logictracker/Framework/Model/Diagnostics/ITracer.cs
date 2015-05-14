namespace Urbetrack.Model
{
    ///<summary>
    /// Trazador de flujo.
    ///</summary>
    public interface ITracer
    {
        /// <summary>
        /// Agrega una linea al trazado.
        /// </summary>
        /// <param name="txt">texto de la linea.</param>
        /// <returns></returns>
        ITracer Append(string txt);

        /// <summary>
        /// Agrega varias lineas consecutivamente al trazado
        /// </summary>
        /// <param name="lines">lineas a gregar</param>
        /// <returns></returns>
        ITracer Append(string[] lines);
    }
}
