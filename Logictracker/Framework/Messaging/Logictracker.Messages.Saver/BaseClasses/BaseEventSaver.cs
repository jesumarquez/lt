#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Cache;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.ExpressionEvaluator;
using Logictracker.ExpressionEvaluator.Contexts;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.Messages;

#endregion

namespace Logictracker.Messages.Saver.BaseClasses
{
    /// <summary>
    /// Event saver base class.
    /// </summary>
    public abstract class BaseEventSaver
    {
        #region Properties

        protected object lockDispose = new object();

        private DAOFactory _daof;
        protected DAOFactory DaoFactory { get { return _daof ?? (_daof = new DAOFactory()); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Get all actions for the specified message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected List<Accion> GetActions(Mensaje message)
        {
            var actions = DaoFactory.AccionDAO.FindByMensaje(message);

            return actions != null ? actions.OfType<Accion>().ToList() : new List<Accion>();
        }

        protected static Boolean ApplyAction(LogMensajeBase log, Accion accion)
        {
            if (String.IsNullOrEmpty(accion.Condicion)) return true;

            var context = new EventContext
                              {
                                  Dispositivo = log.Dispositivo.Codigo,
                                  Duracion = log.Duracion,
                                  Exceso = log.Exceso,
                                  Interno = log.Coche.Interno,
                                  Legajo = log.Chofer != null ? log.Chofer.Legajo : string.Empty,
                                  Texto = log.Texto,
                                  TieneTicket = log.Horario != null,
                                  VelocidadAlcanzada = log.VelocidadAlcanzada.HasValue ? log.VelocidadAlcanzada.Value : -1,
                                  VelocidadPermitida = log.VelocidadPermitida.HasValue ? log.VelocidadPermitida.Value : -1,
                                  Fecha = log.Fecha,
                                  FechaFin = log.FechaFin
                              };
            return ApplyAction(context, accion);
        }

        /// <summary>
        /// Determines if the givenn action applies for the specified message.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="accion"></param>
        /// <returns></returns>
        protected static Boolean ApplyAction(Object context, Accion accion)
        {
            if (String.IsNullOrEmpty(accion.Condicion)) return true;

            try
            {
                var expression = ExpressionContext.CreateExpression(accion.Condicion, context);

                var cachedResult = LogicCache.Retrieve<Object>(typeof (Boolean), expression);

                if (cachedResult != null) return Convert.ToBoolean(cachedResult);

                var result = Logictracker.ExpressionEvaluator.ExpressionEvaluator.Evaluate<bool>(expression);

                LogicCache.Store(typeof (Boolean), expression, result);

                return result;
            }
            catch(Exception e)
            {
				STrace.Exception(typeof(BaseEventSaver).FullName, e, String.Format("Error procesando condicion: {0} | Accion: {1}", accion.Condicion, accion.Descripcion));
                    
                return false;   
            }
        }

        /// <summary>
        /// Dispose all assigned resources.
        /// </summary>
        protected void DisposeResources()
        {   
            lock (lockDispose)
            {
				DisposeSessions();
            }
        }

        #endregion

		public void DisposeSessions()
		{
            _daof.Dispose();
            _daof = null;
		}
    }
}