using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Messaging;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;
using Logictracker.Utils;

namespace Logictracker.DAL.DAO.ReportObjects
{
    /// <summary>
    /// Class that implements all the necessari logic for the mobiles event data access.
    /// </summary>
    public class MobileEventDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileEventDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a list of mobiles events filtered by the givenn search criteria.
        /// </summary>
        /// <param name="mobilesIds">A list of mobiles ids.</param>
        /// <param name="messagesIds">A list of message ids.</param>
        /// <param name="driversIds">A list of drivers ids.</param>
        /// <param name="initialDate">The initial date.</param>
        /// <param name="finalDate">The final date.</param>
        /// <returns>A list of events.</returns>
        public List<MobileEvent> GetMobilesEvents(List<int> mobilesIds, IEnumerable<int> messagesIds, List<int> driversIds, DateTime initialDate, DateTime finalDate, int maxMonths)
        {
            var codigos = (from codigo in messagesIds select codigo.ToString()).ToList();

            var results = DAOFactory.LogMensajeDAO.GetByVehiclesAndCodes(mobilesIds, codigos, initialDate, finalDate, maxMonths);

            results = results.Where(log => (driversIds.Contains(0) || (log.Chofer != null && driversIds.Contains(log.Chofer.Id)))).ToList();

            if (!results.Any()) return new List<MobileEvent>();

            return results.Select(log => new MobileEvent
                                      {
                                          Intern = log.Coche.Interno,
                                          MobileType = log.Coche.TipoCoche != null ? log.Coche.TipoCoche.Descripcion : "",
                                          Driver = log.Chofer != null ? log.Chofer.Entidad.Descripcion : "",
                                          EventTime = log.Fecha.ToDisplayDateTime(),
                                          Reception = log.FechaAlta.HasValue ? log.FechaAlta.Value.ToDisplayDateTime() : (DateTime?)null,
                                          Message = log.Texto,
                                          Latitude = log.Latitud,
                                          Longitude = log.Longitud,
                                          IconUrl = log.GetIconUrl(),
                                          EventEndTime = log.FechaFin != null ? log.FechaFin.Value.ToDisplayDateTime() : log.FechaFin,
                                          FinalLatitude = log.LatitudFin,
                                          FinalLongitude = log.LongitudFin,
                                          Id = log.Id,
                                          Responsable = log.Coche.Chofer != null ? log.Coche.Chofer.Entidad.Descripcion : "",
                                          TieneFoto = log.TieneFoto,
                                          IdPuntoInteres = log.IdPuntoDeInteres,
                                          Atendido = log.Estado > 0,
                                          Usuario = log.Usuario,
                                          AtencionEvento = log.Estado > 0 ? DAOFactory.AtencionEventoDAO.GetByEvento(log.Id) : null
                                      }).ToList();
        }
               
        public List<MobileEvent> GetDetenciones(List<int> mobilesIds, DateTime initialDate, DateTime finalDate, int duracion, double radio, double lat, double lon, int maxMonths)
        {
            var results = DAOFactory.LogMensajeDAO.GetByVehiclesAndCode(mobilesIds, MessageCode.StoppedEvent.GetMessageCode(), initialDate, finalDate, maxMonths);

            if (!results.Any()) return new List<MobileEvent>();

            return results.Where(log => Distancias.Loxodromica(lat, lon, log.Latitud, log.Longitud) <= radio && log.FechaFin.Value.Subtract(log.Fecha).TotalMinutes >= duracion)
                .Select(log => new MobileEvent
                                   {
                                       Intern = log.Coche.Interno,
                                       MobileType = log.Coche.TipoCoche != null ? log.Coche.TipoCoche.Descripcion : "",
                                       Driver = log.Chofer != null ? log.Chofer.Entidad.Descripcion : "",
                                       EventTime = log.Fecha.ToDisplayDateTime(),
                                       Message = log.Texto,
                                       Latitude = log.Latitud,
                                       Longitude = log.Longitud,
                                       IconUrl = log.GetIconUrl(),
                                       EventEndTime = log.FechaFin != null ? log.FechaFin.Value.ToDisplayDateTime() : log.FechaFin,
                                       FinalLatitude = log.LatitudFin,
                                       FinalLongitude = log.LongitudFin,
                                       Id = log.Id,
                                       Responsable = log.Coche.Chofer != null ? log.Coche.Chofer.Entidad.Descripcion : "",
                                       TieneFoto = log.TieneFoto,
                                       IdPuntoInteres = log.IdPuntoDeInteres
                                   }).ToList();
        }

        #endregion


    }
}
