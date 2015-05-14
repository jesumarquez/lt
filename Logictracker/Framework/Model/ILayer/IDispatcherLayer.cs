using Logictracker.Model.Utils;

namespace Logictracker.Model
{
    /// <summary>
    /// El nivel de session, es el responsable de realizar todas la funciones comunes
    /// y del framework y de acuerdo al tipo de INode y las interfaces que implemente,
    /// la session, da la inteligencia general de la aplicacion para luego encaminar,
    /// los mensajes al nivel de aplicacion atravez de un enlace puntual.
    /// </summary>
    public interface IDispatcherLayer : ILayer
    {
        /// <summary>
        /// </summary>
        BackwardReply Dispatch(IMessage msg);
    }
}