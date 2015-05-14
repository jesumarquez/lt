using Logictracker.Model.IAgent;

namespace Logictracker.Model
{
    /// <summary>
    /// Provee las funciones comunes a todos los niveles apilables. 
    /// </summary>
    public interface ILayer : IService
    {
        /// <summary>
        /// StackBind es convocada durante la inicializacion, con el unico proposito de 
        /// relacionar los niveles y oportunar (al ILayer) para darle referencia 
        /// a sus ILayer(s) adyacentes. 
        /// </summary>
        /// <param name="bottom">Nivel inmediatamente inferior en la pila, en el xml esta pro encima</param>
        /// <param name="top">Nivel inmediatamente superior en la pila, en el xml esta por debajo</param>
        /// <remarks>
        /// <para>
        /// Si bottom recibe una referencia nula (null) significa
        /// que dicho ILayer es la base de la pila.
        /// </para>
        /// <para>
        /// Por otro lado, si top recibe una referencia nula (null) significa
        /// que dicho ILayer es la cima de la pila.
        /// </para>
        /// <para> 
        /// Tener en cuenta que durante la carga de un ILayer no esta permitido 
        /// que bottom y top sean nulas. Si el framework detecta esta condicion, 
        /// producira una ArgumentNullException;
        /// </para>
        /// </remarks>
        /// <return>
        /// Si resulta true (verdadero) la vinculacion concreto con exito. 
        /// Caso contrario, se produjo algun error detallado en el trazador.
        /// </return>
        bool StackBind(ILayer bottom, ILayer top);
    }
}
