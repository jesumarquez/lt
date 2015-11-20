using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public static class LogMensajeDAOX
    {
        public static DetachedCriteria FilterByVehicle(this DetachedCriteria dc, int[] ids)
        {
            if (ids.Length > 0)
                dc.Add(Restrictions.In("Coche.Id", ids));
            return dc;
        }

        public static DetachedCriteria FilterByEmpresa(this DetachedCriteria dc, int id)
        {
            return dc.Add(Restrictions.In("Coche.Empresa.Id", new[] { id }));            
        }
    }

    public partial class LogMensajeDAO : MaintenanceDAO<LogMensaje>
    {
//        public LogMensajeDAO(ISession session) : base(session) { }

        #region Private Methods

        private LogMensaje GetUniqueEvent(Int32[] vehiculosId, string[] codigosMensaje, Byte[] estados, DateTime? from, DateTime? to, DateTime? expiresOn, int? maxMonths, Boolean? esPopup, Order order)
        {
            var dc = getDetachedEvents(0, codigosMensaje, estados, from, to, expiresOn, maxMonths, esPopup, false, null)
                .FilterByVehicle(vehiculosId);

            return GetEvents(1, dc, order, true).UniqueResult<LogMensaje>();
        }

        private DateTime? GetUniqueEventDateTime(Int32[] vehiculosId, string[] codigosMensaje, Byte[] estados, DateTime? from, DateTime? to, DateTime? expiresOn, int? maxMonths, Boolean? esPopup, Order order)
        {
            var dc = getDetachedEvents(0, codigosMensaje, estados, from, to, expiresOn, maxMonths, esPopup, false, null)
                .FilterByVehicle(vehiculosId);

            var c = GetEvents(1, dc, order, Projections.Property("Fecha"), true);

            return c.UniqueResult<DateTime>();
        }

        private IList<LogMensaje> GetEvents(Int32 top, Int32[] vehiculosId, string[] codigosMensaje, Byte[] estados, DateTime? from, DateTime? to, DateTime? expiresOn, int? maxMonths, Boolean? esPopup, Boolean? reqAtencion, Order order)
        {
            return GetEvents(top, vehiculosId, codigosMensaje, estados, from, to, expiresOn, maxMonths, esPopup, reqAtencion, order, null);
        }

        private IList<LogMensaje> GetEvents(Int32 top, Int32[] vehiculosId, string[] codigosMensaje, Byte[] estados, DateTime? from, DateTime? to, DateTime? expiresOn, int? maxMonths, Boolean? esPopup, Boolean? reqAtencion, Order order, int page, int pageSize, ref int totalRows, bool reCount)
        {
            return GetEvents(top, vehiculosId, codigosMensaje, estados, from, to, expiresOn, maxMonths, esPopup, reqAtencion, order, null, page, pageSize, ref totalRows, reCount);
        }

        
        private IList<LogMensaje> GetEvents(Int32 top, Int32[] vehiculosId, string[] codigosMensaje, Byte[] estados, DateTime? from, DateTime? to, DateTime? expiresOn, int? maxMonths, Boolean? esPopup, Boolean? reqAtencion, Order order, int? lastId, int page, int pageSize, ref int totalRows, bool reCount)
        {
            var dc = getDetachedEvents(0, codigosMensaje, estados, from, to, expiresOn, maxMonths, esPopup, reqAtencion, lastId)
                .FilterByVehicle(vehiculosId);
            if (reCount)
            {
                var rowcount = GetEventsRowCount(top, dc, order, true).FutureValue<Int32>();
                totalRows = rowcount.Value;
            }
            return GetEvents(top, dc, order, true, page, pageSize).List<LogMensaje>();
        }

        private IList<LogMensaje> GetEvents(Int32 top, Int32[] vehiculosId, string[] codigosMensaje, Byte[] estados, DateTime? from, DateTime? to, DateTime? expiresOn, int? maxMonths, Boolean? esPopup, Boolean? reqAtencion, Order order, int? lastId)
        {
            var dc = getDetachedEvents(0, codigosMensaje, estados, from, to, expiresOn, maxMonths, esPopup, reqAtencion, lastId)
                .FilterByVehicle(vehiculosId);
            return GetEvents(top, dc, order, true).List<LogMensaje>();
        }

        private ICriteria GetEvents(Int32 top, DetachedCriteria dc, Order order, IProjection p)
        {
            return GetEvents(top, dc, order, p, true);
        }

        private ICriteria GetEventsRowCount(Int32 top, DetachedCriteria dc, Order order, IProjection p)
        {
            return GetEventsRowCount(top, dc, order, p, true);
        }        

        private ICriteria GetEvents(Int32 top, DetachedCriteria dc, Order order, IProjection p, bool existsWithDate)
        {
            dc.Add(Restrictions.EqProperty("lm.Id", "dlm.Id"));

            if (existsWithDate)
                dc.Add(Restrictions.EqProperty("lm.Fecha", "dlm.Fecha"));

            dc.SetProjection(Projections.Property("dlm.Id"));

            return GetEvents(top, Subqueries.Exists(dc), order, p);
        }

         private ICriteria GetEvents(Int32 top, DetachedCriteria dc, Order order, IProjection p, bool existsWithDate,  int page, int pageSize)
        {
            dc.Add(Restrictions.EqProperty("lm.Id", "dlm.Id"));

            if (existsWithDate)
                dc.Add(Restrictions.EqProperty("lm.Fecha", "dlm.Fecha"));

            dc.SetProjection(Projections.Property("dlm.Id"));

            return GetEvents(top, Subqueries.Exists(dc), order, p, page, pageSize);
        }

       

        private ICriteria GetEventsRowCount(Int32 top, DetachedCriteria dc, Order order, IProjection p, bool existsWithDate)
        {
            dc.Add(Restrictions.EqProperty("lm.Id", "dlm.Id"));

            if (existsWithDate)
                dc.Add(Restrictions.EqProperty("lm.Fecha", "dlm.Fecha"));

            dc.SetProjection(Projections.Property("dlm.Id"));

            return GetEventsRowCount(top, Subqueries.Exists(dc), order, p);
        }


        private ICriteria GetEvents(Int32 top, AbstractCriterion ac, Order order, IProjection p, int page, int pageSize)
        {
            var eventosCriteria = Session.CreateCriteria<LogMensaje>("lm")
                     .Add(ac);

            if (order != null)
                eventosCriteria.AddOrder(order);

            if (p != null)
                eventosCriteria.SetProjection(p);

            eventosCriteria.SetFirstResult(page * pageSize);
            eventosCriteria.SetMaxResults(pageSize);

            return eventosCriteria;
        }

        private ICriteria GetEvents(Int32 top, AbstractCriterion ac, Order order, IProjection p)
        {
            var eventosCriteria = Session.CreateCriteria<LogMensaje>("lm")
                     .Add(ac);

            if (order != null)
                eventosCriteria.AddOrder(order);

            if (p != null)
                eventosCriteria.SetProjection(p);

            if (top > 0)
                eventosCriteria.SetMaxResults(top);

            return eventosCriteria;
        }

        private ICriteria GetEventsRowCount(Int32 top, AbstractCriterion ac, Order order, IProjection p)
        {
            var eventosCriteria = Session.CreateCriteria<LogMensaje>("lm")
                .Add(ac)
                .SetProjection(Projections.RowCount());


            if (p != null)
                eventosCriteria.SetProjection(p);

            return eventosCriteria;
        }

        

        private ICriteria GetEventsRowCount(Int32 top, DetachedCriteria dc, Order order, bool existsWithDate)
        {
            return GetEventsRowCount(top, dc, order, null, existsWithDate);
        }

        private ICriteria GetEvents(Int32 top, DetachedCriteria dc, Order order, bool existsWithDate)
        {
            return GetEvents(top, dc, order, null, existsWithDate);
        }

        private ICriteria GetEvents(Int32 top, DetachedCriteria dc, Order order, bool existsWithDate, int page, int pageSize)
        {
            return GetEvents(top, dc, order, null, existsWithDate, page, pageSize);
        }

        private ICriteria GetEvents(Int32 top, DetachedCriteria dc, Order order)
        {
            return GetEvents(top, dc, order, null);
        }

        private DetachedCriteria getDetachedEvents(Int32 top, string[] codigosMensaje, Byte[] estados, DateTime? from, DateTime? to, DateTime? expiresOn, int? maxMonths, Boolean? esPopup, Boolean? reqAtencion, Int32? lastId)
        {
            var result = DetachedCriteria.For<LogMensaje>("dlm");

            if (codigosMensaje.Length > 0)
            {
                var mdc = DetachedCriteria.For<Mensaje>("dm")
                    .SetProjection(Projections.Property("Id"))
                    .Add(Restrictions.In(Projections.Property<Mensaje>(lm => lm.Codigo), codigosMensaje));

                result.Add(Subqueries.PropertyIn("Mensaje.Id", mdc));
            }

            if (expiresOn != null)
                result.Add(Restrictions.Gt(Projections.Property<LogMensaje>(lm => lm.Expiracion), expiresOn));

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
                        conj1.Add(Restrictions.Between(Projections.Property<LogMensaje>(lm => lm.Fecha), from, to));
                    else
                        conj1.Add(Restrictions.Ge(Projections.Property<LogMensaje>(lm => lm.Fecha), from));
                }

                if (estados.Length > 0)
                    conj1.Add(Restrictions.In(Projections.Property<LogMensaje>(lm => lm.Estado), estados));

                if (lastId != null && lastId > 0)
                {
                    conj1.Add(Restrictions.Gt(Projections.Property<LogMensaje>(lm => lm.Id), lastId));
                }

                result.Add(conj1);
            }

            #endregion

            if (top > 0)
                result.SetMaxResults(top);

            result.SetProjection(Projections.Property<LogMensaje>(lm => lm.Id));
            
            return result;
        }


        private void refreshMensajesToPopup(int empresa, Int32 minutes, int refreshTime)
        {
            var now = DateTime.UtcNow;
            lock (Popups)
            {
                if (LastPopup.AddSeconds(refreshTime) <
                    DateTime.UtcNow)
                {
                    var mensajesCodigo = new string[] {};
                    var estados = new byte[] {0};
                    var desde = now.Subtract(TimeSpan.FromMinutes(minutes));
                    DateTime? hasta = null;

                    var pop1DC =
                        getDetachedEvents(0, mensajesCodigo, estados, desde, hasta, now, null, true, false, null).
                            FilterByEmpresa(empresa);
                    var pop2DC = getDetachedEvents(0, mensajesCodigo, estados, desde, hasta, now, null, true, true, null)
                        .
                        FilterByEmpresa(empresa);

                    var pop1 = GetEvents(20, pop1DC, null).List<LogMensaje>();
                    var pop2 = GetEvents(0, pop2DC, null).List<LogMensaje>();
                    Popups = pop1.Union(pop2).OrderBy(lm => lm.Fecha).ToList();
                    LastPopup = DateTime.UtcNow;
                }
            }
        }

        #endregion Private Methods

        #region Public Methods

        public IList<LogMensaje> GetMensajesToPopup(int[] vehiclesIds, int minutes, int? lastId)
        {
            var now = DateTime.UtcNow;
            var desde = now.Subtract(TimeSpan.FromMinutes(minutes));
            //var mensajesCodigo = new string[] { };
            //var estados = new byte[] { 0 };

            var idsVehiculos = Ids2DataTable(vehiclesIds);

            //var pop1DC = getDetachedEvents(0, mensajesCodigo, estados, desde, null, now, null, true, false, lastId)
            //                              .FilterByVehicle(vehiclesIds);
            //var pop2DC = getDetachedEvents(0, mensajesCodigo, estados, null, null, now, null, true, true, lastId)
            //                              .FilterByVehicle(vehiclesIds);
            //var pop1 = GetEvents(20, pop1DC, null).List<LogMensaje>();
            //var pop2 = GetEvents(0, pop2DC, null).List<LogMensaje>();
            //return pop1.Union(pop2).ToList();

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_LogMensajeDAO_GetMensajesPopUpForVehiculos] @top = :top, @vehiculosTable = :ids, @desde = :desde, @lastId = :lastId;");
            sqlQ.SetInt32("top", 20);
            sqlQ.SetStructured("ids", idsVehiculos);
            sqlQ.SetDateTime("desde", desde);
            sqlQ.SetInt32("lastId", lastId.HasValue ? lastId.Value : 0);
            sqlQ.AddEntity(typeof(LogMensaje));

            //sqlQ.SetResultTransformer(Transformers.AliasToBean(typeof(LogMensaje)));
            var results = sqlQ.List<LogMensaje>();
            return results;
        }
        
        public ICriteria GetEventsCriteria(Int32 top, AbstractCriterion ac, Order order)
        {
            return GetEvents(top, ac, order, null);
        }

        public IList<LogMensaje> GetLastEvents(int vehiculo, DateTime from, int maxresults, int maxMonths)
        {
            var eventos = GetEvents(maxresults, new[] { vehiculo }, new String[] { }, new Byte[] { }, from, null, null, maxMonths, null, null, Order.Desc("Fecha"));

            return eventos;
        }

        public IList<LogMensaje> GetEventos(IEnumerable<int> vehiculos, string codigo1, string codigo2, DateTime desde, DateTime hasta)
        {
            var idsVehiculos = new DataTable();
            idsVehiculos.Columns.Add("id", typeof(int));
            foreach (var i in vehiculos)
            {
                var r = idsVehiculos.NewRow();
                r["id"] = i;
                idsVehiculos.Rows.Add(r);
            }
            
            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_LogMensajeDAO_GetEventos] :ids, :codigo1, :codigo2, :dateFrom, :dateTo;");
            sqlQ.SetStructured("ids", idsVehiculos);
            sqlQ.SetString("codigo1", codigo1);
            sqlQ.SetString("codigo2", codigo2);
            sqlQ.SetDateTime("dateFrom", desde);
            sqlQ.SetDateTime("dateTo", hasta);
            
            sqlQ.SetResultTransformer(Transformers.AliasToBean(typeof(LogMensaje)));
            var results = sqlQ.List<LogMensaje>();
            return results;
        }


        public IList<LogMensaje> GetEventos(IEnumerable<int> vehiculos, IEnumerable<string> codigosMensaje, DateTime desde, DateTime hasta, int maxMonths, bool existsWithDate, Order order)
        {
            var coches = vehiculos.ToArray();
            var mensajes = codigosMensaje.ToArray();

            var dc = getDetachedEvents(0, mensajes, new byte[]{}, desde, hasta, null, maxMonths, null, null, null)
                        .FilterByVehicle(coches);

            return GetEvents(0, dc, order, existsWithDate).List<LogMensaje>();
        }

        public IList<LogMensaje> GetEventos(IEnumerable<int> vehiculos, IEnumerable<string> codigosMensaje, DateTime desde, DateTime hasta, int maxMonths)
        {
            var coches = vehiculos.ToArray();
            var mensajes = codigosMensaje.ToArray();

            var eventos = GetEvents(0, coches, mensajes, new Byte[] { }, desde, hasta, null, maxMonths, null, null, null);

            return eventos;
        }

        public IList<LogMensaje> GetMensajesToPopUp(int empresa, IEnumerable<int> coches, int minutes, int refreshTime)
        {
            refreshMensajesToPopup(empresa, minutes, refreshTime*2); //Cacheo la query una cada 30 segundos
            return Popups.Where(m => coches.Contains(m.Coche.Id)).ToList();
        }

        public IList<LogMensaje> GetMensajesDispositivosToPopUp(int empresa, IEnumerable<int> dispositivos, int minutes, int refreshTime)
        {
            refreshMensajesToPopup(empresa, minutes, refreshTime); //Cacheo la query una cada 30 segundos
            return Popups.Where(m => dispositivos.Contains(m.Dispositivo.Id)).ToList();
        }

        public static IList<LogMensaje> Popups;
        public static DateTime LastPopup = DateTime.MinValue;

        public IList<LogMensaje> GetByMobilesAndTypes(IEnumerable<int> vehicleIds, IEnumerable<string> messages, DateTime from, DateTime to, int maxMonths)
        {
            var list = GetEvents(0, vehicleIds.ToArray(), messages.ToArray(), new Byte[] { }, from, to, null, maxMonths, null, null, null);

            return list;
        }

        public IList<LogMensaje> GetEvents(int vehicle, DateTime desde, DateTime hasta, int maxMonths)
        {
            var eventos = GetEvents(0, new[] { vehicle }, new string[] { }, new Byte[] { }, desde, hasta, null, maxMonths, null, null, Order.Asc("Fecha"));

            return eventos;
        }

        public IList<LogMensaje> GetQualityMessagesByMobilesAndTypes(int vehicleId, DateTime from, DateTime to, int maxMonths)
        {
            var list = GetEvents(0, new[] { vehicleId }, MessageCodeX.QualityMessages.ToArray(), new Byte[] { }, from, to, null, maxMonths, null, null, null);

            return list;
        }

        public IList<LogMensaje> GetMensajesConsola(IEnumerable<int> coches, IEnumerable<string> mensajes, int lastId, int maxResults)
        {
            var cochesList = coches.ToArray();
            var mensajesList = mensajes.ToArray();
            var expiresOn = DateTime.UtcNow;

            var list = GetEvents(maxResults, cochesList, mensajesList, new byte[] { }, null, null, expiresOn, null, null, null, Order.Desc("Fecha"), lastId);
            return list;
        }

        public IList<LogMensaje> GetMensajesConsolaEntregas(IEnumerable<int> coches, int lastId, int maxResults)
        {
            var mensajesList = new[]
                                   {
                                       MessageCode.CicloLogisticoCerrado.GetMessageCode(),
                                       MessageCode.CicloLogisticoIniciado.GetMessageCode(),
                                       MessageCode.EstadoLogisticoCumplido.GetMessageCode(),
                                       MessageCode.EstadoLogisticoCumplidoManual.GetMessageCode(),
                                       MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode(),
                                       MessageCode.EstadoLogisticoCumplidoManualNoRealizado.GetMessageCode(),
                                       MessageCode.EstadoLogisticoCumplidoEntrada.GetMessageCode(),
                                       MessageCode.EstadoLogisticoCumplidoSalida.GetMessageCode()
                                   };
            return GetMensajesConsola(coches, mensajesList, lastId, maxResults);
        }

        public DateTime? GetLastMessageDate(Coche coche, string codigo)
        {
            if (!coche.HasLastMessageDate(codigo))
            {
                var maxMonths = 2;

                var result = GetUniqueEventDateTime(new[] { coche.Id }, new[] { codigo }, new Byte[] { }, null, null, null, maxMonths, null, Order.Desc("Fecha"));

                coche.StoreLastMessageDate(codigo, result);
            }

            return coche.RetrieveLastMessageDate(codigo);
        }

        public LogMensaje GetLastMessageWithText(Coche coche, string codigo, string text)
        {
            var maxMonths = coche.Empresa != null ? coche.Empresa.MesesConsultaPosiciones : 3;

            var vehicleId = coche.Id;
            var dc = getDetachedEvents(0, new[] { codigo }, new Byte[] { }, null, null, null, maxMonths, null, null, null)
                .FilterByVehicle(new[] { vehicleId });

            dc.Add(Restrictions.Like("dlm.Texto", string.Format("%{0}%", text)));

            return GetEvents(1, dc, Order.Desc("lm.Fecha")).UniqueResult<LogMensaje>();
        }

        public LogMensaje GetLastMessageWithText(string codigo, string text, int maxMonths)
        {
            var dc = getDetachedEvents(0, new[] { codigo }, new Byte[] { }, null, null, null, maxMonths, null, null, null);
            dc.Add(Restrictions.Like("dlm.Texto", string.Format("%{0}%", text)));

            return GetEvents(1, dc, Order.Desc("lm.Fecha")).UniqueResult<LogMensaje>();
        }

        public LogMensaje GetLastVehicleRfidEvent(int coche, DateTime fecha)
        {
            var codigos = new[]
                              {
                                  MessageCode.RfidDriverLogin.GetMessageCode(),
                                  MessageCode.RfidDriverLogout.GetMessageCode()
                              };
            var fechaDesde = fecha.AddHours(-24);

            var mensaje = GetUniqueEvent(new[] { coche }, codigos, new Byte[] { }, fechaDesde, fecha, null, null,
                               null, Order.Desc("Fecha"));

            return mensaje;
        }

        public IList<LogMensaje> GetFirstByVehicleAndCode(int vehicleId, string code, DateTime desde, DateTime hasta, int maxMonths)
        {
            var eventos = GetEvents(1, new[] { vehicleId }, new[] { code }, new Byte[] { }, desde, hasta, null, maxMonths, null, null, Order.Asc("Fecha"));

            return eventos;
        }
        public LogMensaje GetLastByVehicleAndCode(int vehicleId, string code, DateTime desde, DateTime hasta, int maxMonths)
        {
            var eventos = GetEvents(1, new[] { vehicleId }, new[] { code }, new Byte[] { }, desde, hasta, null, maxMonths, null, null, Order.Desc("Fecha"));

            return eventos.LastOrDefault();
        }
        public LogMensaje GetLastByVehicleAndCodes(int vehicleId, string[] codes, DateTime desde, DateTime hasta, int maxMonths)
        {
            var eventos = GetEvents(1, new[] { vehicleId }, codes, new Byte[] { }, desde, hasta, null, maxMonths, null, null, Order.Desc("Fecha"));

            return eventos.LastOrDefault();
        }

        public IList<LogMensaje> GetByVehicleAndCode(int vehicleId, string code, DateTime desde, DateTime hasta, int maxMonths)
        {
            var eventos = GetEvents(0, new[] { vehicleId }, new[] { code }, new Byte[] { }, desde, hasta, null, maxMonths, null, null, Order.Asc("Fecha"));

            return eventos;
        }

        public IList<LogMensaje> GetByVehicleAndCode(int vehicleId, string code, DateTime desde, DateTime hasta, int maxMonths, int page, int pageSize, ref int totalRows, bool reCount)
        {
            var eventos = GetEvents(0, new[] { vehicleId }, new[] { code }, new Byte[] { }, desde, hasta, null, maxMonths, null, null, Order.Asc("Fecha"), page, pageSize, ref totalRows, reCount);

            return eventos;
        }

        public IList<LogMensaje> GetByVehicleAndCodeWithSession(int vehicleId, string code, DateTime desde, DateTime hasta, int maxMonths)
        {
            return GetByVehiclesAndCode(new[] { vehicleId }, code, desde, hasta, maxMonths);
        }

        public IList<LogMensaje> GetByVehiclesAndCode(List<int> vehicleIds, string code, DateTime desde, DateTime hasta, int maxMonths)
        {
            return GetByVehiclesAndCode(vehicleIds.ToArray(), code, desde, hasta, maxMonths);
        }

        public IList<LogMensaje> GetByVehiclesAndCode(Int32[] vehicleIds, string code, DateTime desde, DateTime hasta, int maxMonths)
        {
            var eventos = GetEvents(0, vehicleIds, new[] { code }, new Byte[] { }, desde, hasta, null, maxMonths, null, null, Order.Asc("Fecha"));

            return eventos;
        }

        public IList<LogMensaje> GetByVehiclesAndCodesLinq(List<int> vehicleIds, List<string> codes, DateTime desde, DateTime hasta, int maxMonths, int page, int pageSize, ref int totalRows, bool reCount)
        {
            return GetByVehiclesAndCodes(vehicleIds.ToArray(), codes.ToArray(), desde, hasta, maxMonths, page, pageSize, ref totalRows, reCount);
        }
        

        public IList<LogMensaje> GetByVehiclesAndCodes(List<int> vehicleIds, List<string> codes, DateTime desde, DateTime hasta, int maxMonths)
        {
            var tableVehiculos = Ids2DataTable(vehicleIds);
            var dao = new DAOFactory();
            var mensajesIds = dao.MensajeDAO.FindByCodes(codes).Select(m => m.Id);
            var tableMensajes = Ids2DataTable(mensajesIds);

            var minDate = DateTime.UtcNow.AddMonths(-maxMonths);

            if (desde < minDate) desde = minDate;
            if (hasta < minDate) hasta = minDate;

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_LogMensajeDAO_GetByVehiclesAndMessages] @vehiculosIds = :vehiculosIds, @mensajesIds = :mensajesIds, @desde = :desde, @hasta = :hasta;")
                              .AddEntity(typeof(LogMensaje))
                              .SetStructured("vehiculosIds", tableVehiculos)
                              .SetStructured("mensajesIds", tableMensajes)
                              .SetDateTime("desde", desde)
                              .SetDateTime("hasta", hasta);
            var results = sqlQ.List<LogMensaje>();
            return results;

            //return GetByVehiclesAndCodes(vehicleIds.ToArray(), codes.ToArray(), desde, hasta, maxMonths);
        }

        public IList<LogMensaje> GetByVehiclesAndCodes(Int32[] vehicleIds, string[] codes, DateTime desde, DateTime hasta, int maxMonths)
        {
            if (codes.Contains("0"))
                codes = new string[] { };

            var eventos = GetEvents(0, vehicleIds, codes, new Byte[] { }, desde, hasta, null, maxMonths, null, null, Order.Asc("Fecha"));


            return eventos;
        }

        public IList<LogMensaje> GetByVehiclesAndCodes(Int32[] vehicleIds, string[] codes, DateTime desde, DateTime hasta, int maxMonths, int page, int pageSize, ref int totalRows, bool reCount)
        {
            if (codes.Contains("0"))
                codes = new string[] { };

            var eventos = GetEvents(0, vehicleIds, codes, new Byte[] { }, desde, hasta, null, maxMonths, null, null, Order.Asc("Fecha"), page, pageSize, ref totalRows, reCount);


            return eventos;
        }

        public IList<LogMensaje> GetGeoRefferencesEvents(IEnumerable<int> vehicleIds, DateTime desde, DateTime hasta, List<int> geocercasIds, int maxMonths)
        {
            var codigos = new[] { MessageCode.InsideGeoRefference.GetMessageCode(), MessageCode.OutsideGeoRefference.GetMessageCode() };

            var dao = new DAOFactory();
            var mensajesIds = dao.MensajeDAO.FindByCodes(codigos).Select(m => m.Id);

            var tableVehiculos = Ids2DataTable(vehicleIds);
            var tableMensajes = Ids2DataTable(mensajesIds);
            var tableGeocercas = Ids2DataTable(geocercasIds);

            var minDate = DateTime.UtcNow.AddMonths(-maxMonths);

            if (desde < minDate) desde = minDate;
            if (hasta < minDate) hasta = minDate;

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_LogMensajeDAO_GetGeoRefferencesEvents] @vehiculosIds = :vehiculosIds, @mensajesIds = :mensajesIds, @geocercasIds = :geocercasIds, @desde = :desde, @hasta = :hasta;")
                              .AddEntity(typeof(LogMensaje))
                              .SetStructured("vehiculosIds", tableVehiculos)
                              .SetStructured("mensajesIds", tableMensajes)
                              .SetStructured("geocercasIds", tableGeocercas)
                              .SetDateTime("desde", desde)
                              .SetDateTime("hasta", hasta);
            var results = sqlQ.List<LogMensaje>();
            return results;

            //var dc = getDetachedEvents(0, codigos, new Byte[] { }, desde, hasta, null, maxMonths, null, null, null)
            //            .FilterByVehicle(vehicleIds.ToArray());

            //if (geocercas != null)
            //    dc.Add(Restrictions.In("IdPuntoDeInteres", geocercas));

            //var eventos = GetEvents(0, dc, Order.Asc("lm.Fecha"), false).List<LogMensaje>();

            //return eventos;
        }

        public DateTime? GetLastGeoRefferenceEventDate(Coche coche, string codigo, int georeferenceId)
        {
            var code = codigo + '-' + georeferenceId;

            if (!coche.HasLastMessageDate(code))
            {
                var maxMonths = coche.Empresa != null ? coche.Empresa.MesesConsultaPosiciones : 3;

                var result = GetUniqueEventDateTime(new[] { coche.Id }, new[] { codigo }, new Byte[] { }, null, null, null,
                                   maxMonths, null, Order.Desc("Fecha"));

                coche.StoreLastMessageDate(code, result);
            }

            return coche.RetrieveLastMessageDate(code);
        }

        #endregion

        #region Protected Methods

        protected override string GetDeleteCommand() { return "delete top(:n) from opeeven01 where opeeven01_datetime <= :date ; select @@ROWCOUNT as count;"; }

        #endregion

        public IList<LogMensaje> GetByVehiclesAndCodes(List<int> vehiclesId, List<string> codes, DateTime initialDate, DateTime finalDate, int maxMonths, int idEmpresa)
        {
            var tableVehiculos = Ids2DataTable(vehiclesId);
            var dao = new DAOFactory();
            var mensajesIds = dao.MensajeDAO.FindByCodes(codes).Select(m => m.Id);
            var tableMensajes = Ids2DataTable(mensajesIds);

            var minDate = DateTime.UtcNow.AddMonths(-maxMonths);

            if (initialDate < minDate) initialDate = minDate;
            if (finalDate < minDate) finalDate = minDate;

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_LogMensajeDAO_GetByVehiclesAndMessages] @vehiculosIds = :vehiculosIds, @mensajesIds = :mensajesIds, @desde = :desde, @hasta = :hasta;")
                              .AddEntity(typeof(LogMensaje))
                              .SetStructured("vehiculosIds", tableVehiculos)
                              .SetStructured("mensajesIds", tableMensajes)
                              .SetDateTime("desde", initialDate)
                              .SetDateTime("hasta", finalDate);
            var results = sqlQ.List<LogMensaje>();
            return results;
        }
    }
}