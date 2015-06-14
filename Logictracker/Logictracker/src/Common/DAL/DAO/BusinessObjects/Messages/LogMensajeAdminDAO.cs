using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public class LogMensajeAdminDAO : MaintenanceDAO<LogMensajeAdmin>
    {
        protected override string GetDeleteCommand() { return "delete top(:n) from opeeven12 where opeeven12_datetime <= :date ; select @@ROWCOUNT as count;"; }

        public int Count(int vehiculo, DateTime desde, DateTime hasta)
        {
            return Session.QueryOver<LogMensajeAdmin>()
                          .Where(m => m.Coche.Id == vehiculo)
                          .And(m => m.Fecha >= desde && m.Fecha <= hasta)
                          .Select(Projections.RowCount())
                          .SingleOrDefault<int>();
        }

        public IList<LogMensajeAdmin> GetMensajesConsola(IEnumerable<int> coches, IEnumerable<string> mensajes, int lastId, int maxResults)
        {
            var cochesList = coches.ToArray();
            var mensajesList = mensajes.ToArray();
            var expiresOn = DateTime.UtcNow;

            var list = GetEvents(maxResults, cochesList, mensajesList, new byte[] { }, null, null, expiresOn, null, null, null, Order.Desc("Fecha"), lastId);
            return list;
        }

        private IList<LogMensajeAdmin> GetEvents(Int32 top, Int32[] vehiculosId, string[] codigosMensaje, Byte[] estados, DateTime? from, DateTime? to, DateTime? expiresOn, int? maxMonths, Boolean? esPopup, Boolean? reqAtencion, Order order, int? lastId)
        {
            var dc = getDetachedEvents(0, codigosMensaje, estados, from, to, expiresOn, maxMonths, esPopup, reqAtencion, lastId)
                .FilterByVehicle(vehiculosId);

            return GetEvents(top, dc, order, true).List<LogMensajeAdmin>();
        }

        private ICriteria GetEvents(Int32 top, DetachedCriteria dc, Order order, bool existsWithDate)
        {
            return GetEvents(top, dc, order, null, existsWithDate);
        }

        private ICriteria GetEvents(Int32 top, DetachedCriteria dc, Order order, IProjection p, bool existsWithDate)
        {
            dc.Add(Restrictions.EqProperty("lm.Id", "dlm.Id"));

            if (existsWithDate)
                dc.Add(Restrictions.EqProperty("lm.Fecha", "dlm.Fecha"));

            dc.SetProjection(Projections.Property("dlm.Id"));

            return GetEvents(top, Subqueries.Exists(dc), order, p);
        }

        private ICriteria GetEvents(Int32 top, AbstractCriterion ac, Order order, IProjection p)
        {
            var eventosCriteria = Session.CreateCriteria<LogMensajeAdmin>("lm")
                     .Add(ac);

            if (order != null)
                eventosCriteria.AddOrder(order);

            if (p != null)
                eventosCriteria.SetProjection(p);

            if (top > 0)
                eventosCriteria.SetMaxResults(top);

            return eventosCriteria;
        }

        private DetachedCriteria getDetachedEvents(Int32 top, string[] codigosMensaje, Byte[] estados, DateTime? from, DateTime? to, DateTime? expiresOn, int? maxMonths, Boolean? esPopup, Boolean? reqAtencion, Int32? lastId)
        {
            var result = DetachedCriteria.For<LogMensajeAdmin>("dlm");

            if (codigosMensaje.Length > 0)
            {
                var mdc = DetachedCriteria.For<Mensaje>("dm")
                    .SetProjection(Projections.Property("Id"))
                    .Add(Restrictions.In(Projections.Property<Mensaje>(lm => lm.Codigo), codigosMensaje));

                result.Add(Subqueries.PropertyIn("Mensaje.Id", mdc));
            }

            if (expiresOn != null)
                result.Add(Restrictions.Gt(Projections.Property<LogMensajeAdmin>(lm => lm.Expiracion), expiresOn));

            if (esPopup != null)
            {
                result.CreateAlias("Accion", "a", JoinType.InnerJoin)
                      .Add(Restrictions.Eq("a.EsPopUp", esPopup.Value));
            }

            #region Por Id/Estado o RequiereAtencion

            if ((esPopup ?? false) && (reqAtencion ?? false))
            {
                var conj2 = Restrictions.Conjunction()
                    .Add(Restrictions.Eq("Estado", (byte)0))
                    .Add(Restrictions.Eq("a.RequiereAtencion", true));

                result.Add(conj2);
            }
            else
            {
                var conj1 = Restrictions.Conjunction();

                if (from != null || to != null || maxMonths != null)
                {
                    if (maxMonths != null)
                    {
                        var minDesde = DateTime.UtcNow.AddMonths(-maxMonths.Value);
                        from = (@from ?? DateTime.MinValue) > minDesde ? from : minDesde;
                    }
                    if (from != null && to != null)
                        conj1.Add(Restrictions.Between(Projections.Property<LogMensajeAdmin>(lm => lm.Fecha), from, to));
                    else
                        conj1.Add(Restrictions.Ge(Projections.Property<LogMensajeAdmin>(lm => lm.Fecha), from));
                }

                if (estados.Length > 0)
                    conj1.Add(Restrictions.In(Projections.Property<LogMensajeAdmin>(lm => lm.Estado), estados));

                if (lastId != null && lastId > 0)
                {
                    conj1.Add(Restrictions.Gt(Projections.Property<LogMensajeAdmin>(lm => lm.Id), lastId));
                }

                result.Add(conj1);
            }

            #endregion

            if (top > 0)
                result.SetMaxResults(top);

            result.SetProjection(Projections.Property<LogMensajeAdmin>(lm => lm.Id));

            return result;
        }
    }
}