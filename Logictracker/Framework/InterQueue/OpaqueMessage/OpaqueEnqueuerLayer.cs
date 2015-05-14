#region Usings

using System.Messaging;
using System.Xml.Linq;
using Logictracker.Configuration;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Model.IAgent;

#endregion

namespace Logictracker.InterQueue.OpaqueMessage
{
    public class OpaqueEnqueuerHandler : ILoaderSettings, IMessageHandler<OpaqueMessage>
    {
        private MessageQueue Queue { get; set; }

        /// <summary>
        /// Procesa un mensaje.
        /// </summary>
        /// <param name="message"></param>
        public HandleResults HandleMessage(OpaqueMessage message)
        {
            Queue.Send(message, MessageQueueTransactionType.Single);
            return HandleResults.Success;
        }

    	/// <summary>
    	/// Configura un parametro al ILayer.
    	/// </summary>
    	/// <param name="xElement">Instancia de <c>Setting</c> con la informacion del parametro.</param>
    	/// <param name="object"></param>
        /// <returns>Retorna el resultado de la carga segun la enumeracion <c>Logictracker.LoadResults</c></returns>
    	public LoadResults LoadSetting(XElement xElement, object @object)
        {
			if (xElement.Name.LocalName == "queue")
            {
                Queue = MessageQueue.Exists(xElement.Value) ? new MessageQueue(xElement.Value) : MessageQueue.Create(xElement.Value, true);
                if (!Queue.Transactional)
                {
                    return Annotate(xElement, "error", "La cola {0} debe ser una cola transaccional.", xElement.Value);
                }
                Queue.SetPermissions(Config.Queue.QueueUser, MessageQueueAccessRights.FullControl);
                Queue.Formatter = new OpaqueMessageFormatter("");
                Queue.DefaultPropertiesToSend.Recoverable = true;
                return LoadResults.LoadOk;
            }
            return LoadResults.SettingUnknown;
        }

		public static LoadResults Annotate(XElement xElement, string type, string a, params object[] args)
		{
			return LoadResults.AnnotatedError;
		}
    }
}