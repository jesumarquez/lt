namespace Logictracker.Model
{
    /// <summary>
    /// Mensajeria corta 
    /// </summary>
	public interface IShortMessage : INode
    {
        /// <summary>
        /// Agrega el mensaje dado a la tabla de mensajes predefinidos del dispositivo.
        /// </summary>
		/// <param name="messageId"></param>
        /// <param name="code">codigo del mensaje.</param>
        /// <param name="message">texto del mensaje.</param>
        /// <param name="revision">numero de revision de la tabla luego de agregado/actualizado el mensaje</param>
        /// <c>false</c> si no soporta mensajes predefinidos.
        /// en caso contrario retorna <c>true</c>
		bool SetCannedMessage(ulong messageId, int code, string message, int revision);

        /// <summary>
		/// Agrega el mensaje dado a la tabla de respuestas predefinidas del dispositivo.
        /// </summary>
		/// <param name="messageId"></param>
        /// <param name="code">codigo del mensaje.</param>
        /// <param name="response">texto del mensaje.</param>
        /// <param name="revision">numero de revision de la tabla luego de agregada/actualizada el mensaje</param>
        /// <c>false</c> si no soporta mensajes predefinidos.
        /// en caso contrario retorna <c>true</c>
		bool SetCannedResponse(ulong messageId, int code, string response, int revision);

        /// <summary>
		/// Elimina el mensaje dado de la tabla de mensajes/respuestas predefinidos del dispositivo.
        /// </summary>
        /// <remarks>
        /// Se debe tener en cuenta que estableciendo <c>code</c> en 0 (cero)
        /// implica borrar todos los mensajes, restaurando la tabla a su estado
        /// original.
        /// </remarks>
		/// <param name="messageId"></param>
        /// <param name="code">codigo del mensaje a eliminar</param>
        /// <param name="revision">numero de revision de la tabla luego de borrado el mensaje/respuesta</param>
        /// <returns>
		/// <c>false</c> si no soporta mensajes predefinidos.
        /// en caso contrario retorna <c>true</c>
        /// </returns>
		bool DeleteCannedMessage(ulong messageId, int code, int revision);

        /// <summary>
		/// Envia la orden al dispositivo de mostrar en pantalla el mensaje 
        /// predefinido dado en <c>code</c>
        /// </summary>
		/// <param name="messageId"></param>
        /// <param name="code">codigo del mensaje a mostrar</param>
        /// <param name="replies">lista con los codigos de mensajes predefinidos de las respuestas posibles.</param>
		/// <c>false</c> si no soporta mensajes predefinidos.
        /// en caso contrario retorna <c>true</c>
		bool SubmitCannedMessage(ulong messageId, int code, int[] replies);

        /// <summary>
		/// Envia la orden al dispositivo de mostrar en pantalla el mensaje 
        /// dado en <c>textMessage</c>
        /// </summary>
		/// <param name="messageId"></param>
        /// <param name="textMessage">texto del mensaje a enviar</param>
        /// <param name="replies">lista con los codigos de mensajes predefinidos de las respuestas posibles.</param>
		/// <c>false</c> si no soporta mensajes predefinidos.
        /// en caso contrario retorna <c>true</c>
		bool SubmitTextMessage(ulong messageId, string textMessage, int[] replies);

        bool SubmitLongTextMessage(ulong messageId, string textMessage);

        /// <summary>
        /// Envia la orden al dispositivo de mostrar en pantalla el mensaje 
        /// dado en <c>textMessage</c>
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="textMessageId">Id con el cual se guarda el mensaje en el dispositivo de destino.</param>
        /// <param name="textMessage">texto del mensaje a enviar</param>
        /// <param name="replies">lista con los codigos de mensajes predefinidos de las respuestas posibles.</param>
        /// <c>false</c> si no soporta mensajes predefinidos.
        /// en caso contrario retorna <c>true</c>
        /// <param name="ackEvent"></param>
        bool SubmitTextMessage(ulong messageId, uint textMessageId, string textMessage, uint[] replies, int ackEvent);
    }
}