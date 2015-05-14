#region Usings

using System;
using Logictracker.Model.Utils;

#endregion

namespace Logictracker.Model
{
    /// <summary>
    /// Interface del Dispatcher Asincronico de IMessage
    /// </summary>
    public interface IAsyncDispatcherLayer : ILayer
    {
        /// <summary>
        /// Inicia una operacion asincronica para despachar un IMessage la cual recibe un
        /// timeout especifico y un objeto de estado especifico. Esta provee la información
        /// que identifica la vida de la operacion. Recibe un callback con la identidad del event handler
        /// al que se notifica el final de la operacion. La operacion 
        /// </summary>
        /// <param name="timeout">TimeSpan que especifica el intervalo de tiempo a esperar que se complete la operacion.</param>
        /// <param name="msg">IMessage que se pretende despachar.</param>
        /// <param name="state">object definido por la aplicacion para asociar informacion de estado.</param>
        /// <param name="callback">El AsyncCallback(IAsyncResult) que recibira la notificacion cuando la operacion se complete.</param>
        /// <returns>IAsyncResult que identifica la operacion asincronica que servira luego para obtener un resultado final de la operacion.</returns>
        IAsyncResult BeginDispatch(TimeSpan timeout, IMessage msg, object state, AsyncCallback callback);

        /// <summary>
        /// Completa la operacion asyncronica especificada.
        /// </summary>
        /// <param name="result">IAsyncResult que identifica la operacion asincronica y con la cual se obtiene un resultado final de la operacion.</param>
        /// <returns>Respuesta final de la operacion de despacho.</returns>
        BackwardReply EndDispatch(IAsyncResult result);
    }
}