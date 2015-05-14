using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Messaging;
using Logictracker.Types.ReportObjects;
using Logictracker.Utils;

namespace Logictracker.DAL.DAO.ReportObjects
{
    /// <summary>
    /// Multiple mobiles stopped events data access class.
    /// </summary>
    public class MobileStoppedEventDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileStoppedEventDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets locations with multiple stopped events using the specified filter values.
        /// </summary>
        /// <param name="vehiculos"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="duracion"></param>
        /// <param name="radio"></param>
        /// <param name="topN"></param>
        /// <param name="maxMonths"></param>
        /// <returns></returns>
        public List<MobileStoppedEvent> GetMultipleStoppedEvents(List<int> vehiculos, DateTime desde, DateTime hasta, int duracion, int radio, int topN, int maxMonths)
        {
            var diff = TimeSpan.FromMinutes(duracion);

            var events = DAOFactory.LogMensajeDAO.GetByVehiclesAndCode(vehiculos, MessageCode.StoppedEvent.GetMessageCode(), desde, hasta, maxMonths)
                .Where(e => e.FechaFin != null && e.Fecha != null)
                .Select(m => new ModifiedEvent { Id = m.Id, Latitud = m.Latitud, Longitud = m.Longitud, Fecha = m.Fecha, FechaFin = m.FechaFin })
                .ToList()
                .Where(e => e.FechaFin.GetValueOrDefault().Subtract(e.Fecha.GetValueOrDefault()) >= diff)
                .OrderBy(m => m.Latitud)
                .ToList();

            var results = new List<MobileStoppedEvent>();

            for (var i = 0; i < events.Count; i++)
            {
                var ev = events[i];
                var grupo = new List<ModifiedEvent> { ev };

                for (var j = i + 1; j < events.Count; j++)
                {
                    var ev2 = events[j];
                    var latDist = Distancias.Loxodromica(ev.Latitud, ev.Longitud, ev2.Latitud, ev.Longitud);
                    if (latDist > radio) break;

                    var dist = Distancias.Loxodromica(ev.Latitud, ev.Longitud, ev2.Latitud, ev2.Longitud);
                    if (dist <= radio)
                    {
                        grupo.Add(ev2);
                        events.RemoveAt(j);
                        j--;
                    }
                }

                results.Add(grupo.GroupBy(e => e.DummyProp).Select(e => new MobileStoppedEvent
                {
                    Latitude = e.Average(a => a.Latitud),
                    Longitude = e.Average(a => a.Longitud),
                    StoppedEvents = e.Count()
                }).FirstOrDefault());
            }

            return results.GroupBy(r => r.Corner).Select(r => new MobileStoppedEvent
            {
                Latitude = r.First().Latitude,
                Longitude = r.First().Longitude,
                StoppedEvents = r.Sum(res => res.StoppedEvents)
            }).OrderByDescending(r => r.StoppedEvents).Take(topN).ToList();
        }

        #endregion
    }

    internal class ModifiedEvent
    {
        public int DummyProp { get; set; }
        public int Id { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}