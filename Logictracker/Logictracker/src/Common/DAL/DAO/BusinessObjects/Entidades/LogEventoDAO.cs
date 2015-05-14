using System.Collections;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.Messages;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Entidades
{
    public class LogEventoDAO : GenericDAO<LogEvento>
    {
//        public LogEventoDAO(ISession session) : base(session) { }

        public static List<LogEvento> Popups;
        public static DateTime LastPopup = DateTime.MinValue;

        public List<LogEvento> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, IEnumerable<int> sensores, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades, IEnumerable<int> subentidades, IEnumerable<string> mensajesCodes, DateTime fechaDesde, DateTime fechaHasta)
        {
            var q = Query;

            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(lineas) || !QueryExtensions.IncludesAll(dispositivos))
                q = q.FilterDispositivo(Session, empresas, lineas, dispositivos);
            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(lineas) || !QueryExtensions.IncludesAll(dispositivos) || !QueryExtensions.IncludesAll(sensores))
                q = q.FilterSensor(Session, empresas, lineas, dispositivos, sensores);
            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(lineas) || !QueryExtensions.IncludesAll(tiposEntidad)
             || !QueryExtensions.IncludesAll(entidades) || !QueryExtensions.IncludesAll(subentidades))
                q = q.FilterSubEntidad(Session, empresas, lineas, tiposEntidad, entidades, subentidades);
            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(lineas) || !QueryExtensions.IncludesAll(mensajesCodes))
                q = q.FilterMensajeCodes(Session, empresas, lineas, mensajesCodes);
                        
            return q.Where(e => e.Fecha < fechaHasta && e.Fecha > fechaDesde)
                    .OrderBy(e => e.Fecha)
                    .ToList();
        }

        public IList GetAllMensajesConsola(IEnumerable<int> subentidades, IEnumerable<string> mensajes, int maxResults)
        {
            return Session.CreateCriteria(typeof(LogEvento))
                          .CreateAlias("Mensaje", "msg")
                          .Add(Restrictions.Gt("Expiracion", DateTime.UtcNow))
                          .Add(Restrictions.In("SubEntidad.Id", subentidades.ToArray()))
                          .Add(Restrictions.In("msg.Codigo", mensajes.ToArray()))
                          .AddOrder(Order.Desc("Fecha"))
                          .SetMaxResults(maxResults)
                          .List();
        }

        public IList GetLastMensajesConsola(IEnumerable<int> subentidades, IEnumerable<string> mensajes, int lastId)
        {
            return Session.CreateCriteria(typeof(LogEvento))
                          .CreateAlias("Mensaje", "msg")
                          .Add(Restrictions.Gt("Id", lastId))
                          .Add(Restrictions.Gt("Expiracion", DateTime.UtcNow))
                          .Add(Restrictions.In("SubEntidad.Id", subentidades.ToArray()))
                          .Add(Restrictions.In("msg.Codigo", mensajes.ToArray()))
                          .AddOrder(Order.Desc("Fecha"))
                          .List();
        }

        public IEnumerable<LogEvento> GetByEntitiesAndCodes(List<int> entidades, List<String> codes, DateTime desde, DateTime hasta)
        {
            var mensajes = Session.Query<Mensaje>()
                                  .Where(m => codes.Contains(m.Codigo)
                                           || codes.Contains("0"))
                                  .Select(m => m.Id)
                                  .ToList();
                                           
            return Session.Query<LogEvento>()
                          .Where(e => e.SubEntidad != null 
                                   && e.SubEntidad.Entidad != null 
                                   && entidades.Contains(e.SubEntidad.Entidad.Id) 
                                   && e.Fecha >= desde 
                                   && e.Fecha <= hasta
                                   && mensajes.Contains(e.Mensaje.Id))
                          .OrderBy(mes => mes.Fecha)
                          .ToList();
        }

        public IEnumerable<LogEvento> GetBySubEntitiesAndCodes(List<int> subentidades, List<String> codes, DateTime desde, DateTime hasta)
        {
            var mensajes = Session.Query<Mensaje>()
                                  .Where(m => codes.Contains(m.Codigo)
                                           || codes.Contains("0"))
                                  .Select(m => m.Id)
                                  .ToList();

            return Session.Query<LogEvento>()
                          .Where(e => e.SubEntidad != null 
                                   && subentidades.Contains(e.SubEntidad.Id)
                                   && e.Fecha >= desde
                                   && e.Fecha <= hasta
                                   && mensajes.Contains(e.Mensaje.Id))
                          .OrderBy(mes => mes.Fecha)
                          .ToList();
        }

        public IEnumerable<LogEvento> GetBySensoresAndCodes(IEnumerable<int> sensores, IEnumerable<string> codes, DateTime desde, DateTime hasta)
        {
            return Session.Query<LogEvento>().Where(mes => mes.Sensor != null
                                                        && sensores.Contains(mes.Sensor.Id)
                                                        && mes.Fecha >= desde
                                                        && mes.Fecha <= hasta
                                                        && (codes.Contains(mes.Mensaje.Codigo)
                                                           || codes.Contains("0")))
                                             .OrderBy(mes => mes.Fecha)
                                             .ToList();
        }

        public LogEvento GetLastBySensoresAndCodes(IEnumerable<int> sensores, IEnumerable<string> codes)
        {
            return Session.Query<LogEvento>().Where(ev => ev.Sensor != null
                                                         && sensores.Contains(ev.Sensor.Id)
                                                         && (codes.Contains(ev.Mensaje.Codigo)
                                                          || codes.Contains("0")))
                                             .OrderByDescending(ev => ev.Fecha)
                                             .OrderByDescending(ev => ev.Id)
                                             .FirstOrDefault();
        }

        public List<LogEvento> GetEventosDispositivosToPopUp(IEnumerable<int> dispositivos, int minutes)
        {
            //Cacheo la query una por minuto
            var now = DateTime.UtcNow;

            if (LastPopup.AddMinutes(1) < now)
            {
                Popups = Session.CreateQuery(
                        @"from LogEvento le 
                            where le.Estado = 0 
                            and le.Expiracion > :exp 
                            and le.Fecha > :date
                            and le.Accion.EsPopUp = 1")
                        .SetParameter("exp", now)
                        .SetParameter("date", now.Subtract(TimeSpan.FromMinutes(minutes)))
                        .SetCacheable(true)
                        .List().OfType<LogEvento>().ToList();
                LastPopup = now;
            }

            return Popups.OfType<LogEvento>().Where(m => dispositivos.Contains(m.Dispositivo.Id)).ToList();
        }

        public List<MedicionTour> GetMedicionTour(IEnumerable<int> medidores, DateTime inicio, DateTime fin)
        {
            var medicionDao = new MedicionDAO();
            var subEntidadDao = new SubEntidadDAO();
            var list = new List<MedicionTour>();

            foreach (var medidor in medidores)
            {
                var oMedidor = subEntidadDao.FindById(medidor);
                if (oMedidor != null && oMedidor.Sensor != null)
                {
                    var agg = medicionDao.GetAggregates(oMedidor.Sensor.Dispositivo.Id, oMedidor.Sensor.Codigo, inicio, fin);
                    var tour = new MedicionTour
                                   {
                                       Encendido = inicio,
                                       Apagado = fin,
                                       Inicio = agg.Min,
                                       Fin = agg.Max,
                                       Medidor = oMedidor
                                   };
                    list.Add(tour);
                }
            }

            return list;
        }

        [Serializable]
        public class MedicionTour
        {
            public DateTime Encendido { get; set; }
            public DateTime Apagado { get; set; }
            public TimeSpan Duracion { get { return Apagado.Subtract(Encendido); } }
            public SubEntidad Medidor { get; set; }
            public double Inicio { get; set; }
            public double Fin { get; set; }
            public double Total { get { return Fin - Inicio; } }
        }
    }
}
