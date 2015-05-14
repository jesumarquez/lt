using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.Messages;
using NHibernate;
using NHibernate.Criterion;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    /// <summary>
    /// LogMensajeDescartado data access class.
    /// </summary>
    public class LogMensajeDescartadoDAO : MaintenanceDAO<LogMensajeDescartado>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public LogMensajeDescartadoDAO(ISession session) : base(session) { }

        #endregion

        #region Private Const Properties

        /// <summary>
        /// RFID events codes.
        /// </summary>
        private const string RfidLogin = "93";
        private const string RfidLogout = "94";

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the message deletion sql command.
        /// </summary>
        /// <returns></returns>
        protected override String GetDeleteCommand() { return "delete top(:n) from opeeven04 where opeeven04_datetime <= :date ; select @@ROWCOUNT as count;"; }

        #endregion

        #region Public Methods

        public IList<LogMensajeDescartado> GetLastEvents(int vehiculo, int maxresults)
        {
            return Session.QueryOver<LogMensajeDescartado>()
                .And(l => l.Coche.Id == vehiculo)
                .OrderBy(l => l.Fecha).Desc
                .Take(maxresults)
                .List<LogMensajeDescartado>();
        }

        public List<LogMensajeDescartado> GetQualityMessagesByMobilesAndTypes(int mobile, DateTime from, DateTime to)
        {
            var list = Session.CreateCriteria(typeof(LogMensajeDescartado))
                .CreateAlias("Mensaje", "msg")
                .Add(Restrictions.Eq("Coche.Id", mobile))
                .Add(Restrictions.In("msg.Codigo", new List<string> { "91", "92", RfidLogin, RfidLogout, "111", "112", "113", "114", "115", "116", "117", "118" }))
                .Add(Restrictions.Ge("Fecha", from))
                .Add(Restrictions.Le("Fecha", to))
                .SetCacheable(true)
                .List<LogMensajeDescartado>();

            return ApplyUserGmt(list.ToList());
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Applyes the user configured gmt modifier to the database datetimes.
        /// </summary>
        /// <param name="mensajes"></param>
        /// <returns></returns>
        private static List<LogMensajeDescartado> ApplyUserGmt(List<LogMensajeDescartado> mensajes)
        {
            if (mensajes == null) return mensajes;

            foreach (var mensaje in mensajes) ApplyUserGmt(mensaje);

            return mensajes;
        }

        /// <summary>
        /// Applyes the user configured gmt to the message.
        /// </summary>
        /// <param name="mensaje"></param>
        private static void ApplyUserGmt(LogMensajeBase mensaje)
        {
            if (mensaje.Expiracion.HasValue) mensaje.Expiracion = mensaje.Expiracion.Value.ToDisplayDateTime();

            mensaje.Fecha = mensaje.Fecha.ToDisplayDateTime();

            if (mensaje.FechaFin.HasValue) mensaje.FechaFin = mensaje.FechaFin.Value.ToDisplayDateTime();
        }

        #endregion
    }
}