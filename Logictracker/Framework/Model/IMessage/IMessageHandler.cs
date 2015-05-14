using Logictracker.Model.EnumTypes;

namespace Logictracker.Model
{
    /// <summary>
    /// Proporciona un mecanismo generico para procesar mensajes. 
	/// Para manejar el tipo de mensaje MSGTYPE, se debe implementar esta interface siendo el argumento TYPE igual a MSGTYPE o una interface que este tipo implemente
    /// </summary>
    public interface IMessageHandler<TYPE>
    {
        /// <summary>
        /// Procesa un mensaje.
        /// </summary>
        /// <param name="message"></param>
        HandleResults HandleMessage(TYPE message);
    }
}