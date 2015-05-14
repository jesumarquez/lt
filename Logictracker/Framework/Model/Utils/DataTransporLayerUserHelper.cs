#region Usings

using System.Collections.Generic;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Model
{
    /// <summary>
    /// Ayudante del nivel de transporte que implementa el 
    /// </summary>
    public class DataTransporLayerUserHelper
    {
        readonly Dictionary<int, IDataTransportLayerUser> UserPoints = new Dictionary<int, IDataTransportLayerUser>(100);

        #region Funciones de Registracion
        ///<summary>
        /// Registra un INode para ser notificado los eventos
        ///</summary>
        public void Register(INode node)
        {
			if (!(node is IDataTransportLayerUser)) return;
			if (UserPoints.ContainsKey(node.NodeCode))
			{
				UserPoints[node.NodeCode] = node as IDataTransportLayerUser;
			}
			else
			{
				UserPoints.Add(node.NodeCode, node as IDataTransportLayerUser);
			}
        }

        /// <summary>
        /// Desregistra un INode.
        /// </summary>
        /// <param name="node"></param>
        public void UnRegister(INode node)
        {
            if (!(node is IDataTransportLayerUser)) return;
            if (UserPoints.ContainsKey(node.NodeCode))
            {
                UserPoints.Remove(node.NodeCode);
            }
        }
        #endregion

        #region Despachadores de Eventos
        /// <summary>
        /// Event disparado por el nivel de transporte, antes de pasar el 
        /// mensaje al nivel de session. Permite tanto modificar el mensaje 
        /// como evitar que llegue a la sesion y responder directo al punto
        /// de red correspondiente una respuesta o ninguna. 
        /// </summary>
        /// <param name="node">INode que mando el mensaje</param>
        /// <param name="message">Mensaje que esta a punto de ser entregado a la session.</param>
        /// <param name="result">Referencia a la respuesta hacia atras.</param>
        /// <returns>Verdadero si se debe interrumpir, falso si continuar.</returns>
        public bool BeforeBorderSession(INode node, IMessage message, ref BackwardReply result)
        {
            if (!UserPoints.ContainsKey(node.NodeCode)) return false;
            result = UserPoints[node.NodeCode].BeforeBorderSession(message);
            if (BackwardReply.IsNothing(result))
            {
                STrace.Debug(GetType().FullName, "TransportHelper: El nodo no dio respuesta, Session Cancelada x null.");
                return true;
            }
            if (result.Action == ReplyAction.ReturnedResponseSilently)
            {
                //STrace.Trace(GetType().FullName,2, "TransportHelper: Respuesta al Movil, Session Cancelada x Rta Silenciosa.");
                result.Action = ReplyAction.ReturnedResponse;
                return true;
            }
            //STrace.Trace(GetType().FullName,2, "TransportHelper: Respuesta al Movil, Session Continua.");
            return false;
        }

    	/// <summary>
        /// Callback disparado por el nivel de transporte, despues de pasar el 
        /// mensaje al nivel de session. Incluye el mensaje de la session, y 
		/// permite cambiar su decisión (aunque no lo que haya hecho).
        /// </summary>
        /// <param name="node">INode que mando el mensaje</param>
        /// <param name="message">Mensaje entregado a la session.</param>
        /// <param name="result">Referencia a la respuesta hacia atras.</param>
        /// <returns>Verdadero si se debe interrumpir, falso si continuar.</returns>
        public bool AfterBorderSession(INode node, IMessage message, ref BackwardReply result)
        {
            return UserPoints.ContainsKey(node.NodeCode) && BackwardReply.IsNothing(UserPoints[node.NodeCode].AfterBorderSession(message, result));
        }

        /// <summary>
        /// Callback disparado cuando el punto esta disponible.
        /// </summary>
        public void BeforeResume(INode node, IMessage msg)
        {
            if (UserPoints.ContainsKey(node.NodeCode))
            {
                UserPoints[node.NodeCode].BeforeResume(msg);
            }
        }

        /// <summary>
        /// Callback disparado cuando el punto se vuelve inalcanzable.
        /// </summary>
        public void AfterSuspend(INode node, IMessage msg)
        {
            if (UserPoints.ContainsKey(node.NodeCode))
            {
                UserPoints[node.NodeCode].AfterSuspend(msg);
            }
        }
        #endregion

    }
}
