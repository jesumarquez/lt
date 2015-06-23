using System;
using System.Globalization;
using System.Text;
using Logictracker.Cache;
using Logictracker.Cache.Interfaces;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Interfaces.OrbComm;
using Logictracker.Layers;
using Logictracker.Layers.MessageQueue;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Orbcomm;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;

namespace Logictracker.Scheduler.Tasks.OrbComm
{
    public class Task : BaseTask, IDataIdentify
    {
        public class TaskParser
        {
            public const short Orbcomm = 0;
            public const short Virloc = 1;
        }

        private const string OrbcommSessionIdCacheKey = "SessionId";
        private const string ComponentName = "Orbcomm Scheduler Task";
		private readonly FiltrarRepetidos _filtro = new FiltrarRepetidos();

        private short Parser
        {
            get
            {
                //return TaskParser.Virloc;
                var parser = GetString("parser");
                switch (parser)
                {
                    case "Virloc": return TaskParser.Virloc;
                    default: return TaskParser.Orbcomm;
                }
            }
        }
        private string User
        {
            get
            {
                //return "BRINKS2014"; 
                return GetString("user");
            }
        }
        private string Password
        {
            get
            {
                //return "2014cx234";
                return GetString("password");
            }
        }

        #region Overrides of BaseTask

        protected override void OnExecute(Timer timer)
        {
            try
            {
                var queue = GetDispatcherQueue();
                if (queue == null)
                {
                    STrace.Error(ComponentName, "Cola no encontrada: revisar configuracion");
                    return;
                }

                var service = new Service();

                string sessionId;
                if (!this.KeyExists(OrbcommSessionIdCacheKey))
                {
                    var login = Login(service);
                    if(login.Result != 1) return;
                    sessionId = login.SessionId;
                }
                else sessionId = this.Retrieve<string>(OrbcommSessionIdCacheKey);

                var messages = service.RetrieveMessages(sessionId, Service.MessageFlags.Unread, Service.SetMessageFlags.NoAction, Service.MessageStatusFlags.All, -1, true);
                
                if (messages.Result == -1)
                {
                    STrace.Debug(ComponentName, "No esta logueado: Logueando...");
                    var login = Login(service);
                    if (login.Result != 1)
                    {
                        STrace.Error(ComponentName, "Error No se pudo loguear");
                        return;
                    }
                    sessionId = login.SessionId;
                    messages = service.RetrieveMessages(sessionId, Service.MessageFlags.Unread, Service.SetMessageFlags.NoAction, Service.MessageStatusFlags.All, -1, true);
                }
                STrace.Debug(ComponentName, string.Format("Mensajes recibidos: {0}", messages.Messages.Count));

				messages.Messages.Reverse(); //NO SACAR! agregue este reverse para que los mensajes se procesen en orden cronologico sino se procesan primero los mas nuevos y despues los mas viejos
                foreach (var message in messages.Messages)
                {
                    var estado = "Decodificando cuerpo del mensaje: " + message.MessageEncoding;
                    try
                    {
                        var dispo = DaoFactory.DispositivoDAO.GetByCode(message.MessageFrom);

                        if(dispo == null)
                        {
                            STrace.Error(ComponentName, string.Format("Error Dispositivo no encontrado: {0} | MessageID: {1}", message.MessageFrom, message.MessageId));
                            continue;
                        }

                        estado = "Parseando mensaje";
                        
						var buff = new Frame(message.MessageEncoding == "ASCII" ? Encoding.ASCII.GetBytes(message.MessageBody) : Convert.FromBase64String(message.MessageBody), dispo.Id);

                        IMessage msg;
                        switch (Parser)
                        {
                            case TaskParser.Virloc:
                                var virlocParser = new Virloc.Parser {Id = dispo.Id};

                                if (buff.Payload.Length == 20)
                                {
                                    var payload = new byte[27];
                                    var id = dispo.Id.ToString("0000").ToCharArray();
                                    byte nroMensaje;
                                    try { nroMensaje = Convert.ToByte(message.MessageId); }
                                    catch { nroMensaje = Convert.ToByte(0); }

                                    buff.Payload.CopyTo(payload, 0);
                                    payload[20] = Convert.ToByte(id[0]);
                                    payload[21] = Convert.ToByte(id[1]);
                                    payload[22] = Convert.ToByte(id[2]);
                                    payload[23] = Convert.ToByte(id[3]);
                                    payload[24] = nroMensaje;

                                    var frame = new Frame( payload, dispo.Id);
                                    msg = virlocParser.Decode(frame);
                                    break;
                                }

                                msg = virlocParser.Decode(buff);
                                break;
                            default:
                                var orbCommParser = new Parser {Id = dispo.Id};
                                msg = orbCommParser.Decode(buff); 
                                break;
                        }

                        if (msg == null)
                        {
                            STrace.Error(ComponentName, dispo.Id, string.Format("Error Parseando: {0} | from {1}", message.MessageBody, message.MessageFrom));
                            continue;
                        }

                        estado = "Chequeando mensaje";
						if (!_filtro.IsRepetido(msg))
						{
							estado = "Encolando mensaje";
                            queue.Send(msg, string.Empty);
						}

                    	estado = "Marcando mensaje como leido";
                        Service.SetMessageFlag(sessionId, Service.SetMessageSelect.ByMessageId, message.MessageId.ToString(CultureInfo.InvariantCulture), Service.SetMessageFlags.Read);
                    }
                    catch (Exception e)
                    {
                        STrace.Error(ComponentName, string.Format("Error procesando mensaje: {0} | from {1} | Estado: {2} | {3}", message.MessageBody, message.MessageFrom, estado, e));
                    }
                }
            }
            catch (Exception e)
            {
                STrace.Exception(ComponentName, e);
            }
        }

    	private Authenticate Login(Service service)
        {
            var login = service.Authenticate(User, Password);
            if (login.Result == 1)
            {
                this.Store(OrbcommSessionIdCacheKey, login.SessionId);
                STrace.Debug(ComponentName, string.Format("SessionId: {0} | Expiration: {1} | TotalActiveSessions: {2}", login.SessionId, login.SessionExpiration, login.TotalActiveSessions));
            }
            else
            {
                STrace.Debug(ComponentName, string.Format("Authentication Error {0}: {1}", login.Result, login.ResultDescription));
            }
            return login;
        }

    	private IMessageQueue GetDispatcherQueue()
        {
            var queueName = GetString("queuename");
            var queueType = GetString("queuetype");
			if (string.IsNullOrEmpty(queueName)) return null;

			var umq = new IMessageQueue(queueName);
            if (queueType.ToLower() == "xml") umq.Formatter = "XmlMessageFormatter";

			return !umq.LoadResources() ? null : umq;
        }

        #endregion

        #region Implementation of IDataIdentify

        public int Id { get { return 0; } set { } }

        #endregion
    }
}
