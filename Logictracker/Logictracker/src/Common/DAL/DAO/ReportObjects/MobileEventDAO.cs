using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Messaging;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;
using Logictracker.Utils;
using NHibernate.Criterion;
using NHibernate.Transform;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.BusinessObjects;

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

        public IEnumerable<MobileEvent> GetMobilesEventsByDistritoBase(int distritoId, int baseId)
        {
            ViajeDistribucion viaje = null;
            LogMensaje log = null;
            Coche coche = null;
            TipoCoche tCoche = null;
            Empleado chofer = null;
            Mensaje mensaje = null;

            var q = NHibernate.SessionHelper.Current
                .QueryOver(() => log)
                .Inner.JoinAlias(() => log.Viaje, () => viaje)
                .Inner.JoinAlias(() => log.Coche, () => coche)
                .Inner.JoinAlias(() => coche, () => tCoche)
                .Inner.JoinAlias(() => log.Chofer, () => chofer)
                .Inner.JoinAlias(() => log.Mensaje, () => mensaje)
                .Where(() => log.Estado > 0)
                .Select(Projections.ProjectionList()
                    .Add(Projections.Property(() => coche.Interno).As("Intern"))
                    .Add(Projections.Property(() => tCoche.Descripcion).As("MobileType"))
                    .Add(Projections.Property(() => chofer.Entidad.Descripcion).As("Driver"))
                    //.Add(Projections.Property(() => log.Fecha.ToDisplayDateTime()).As("EventTime"))
                    //.Add(Projections.Property(() => log.FechaAlta.HasValue ? log.FechaAlta.Value.ToDisplayDateTime() : (DateTime?)null).As("Reception"))
                    .Add(Projections.Property(() => log.Texto).As("Message"))
                    .Add(Projections.Property(() => mensaje.Id).As("IdMensaje"))
                    .Add(Projections.Property(() => log.Latitud).As("Latitude"))
                    .Add(Projections.Property(() => log.Longitud).As("Longitude"))
                    //.Add(Projections.Property(() => log.GetIconUrl()).As("IconUrl"))
                    //.Add(Projections.Property(() => log.FechaFin != null ? log.FechaFin.Value.ToDisplayDateTime() : log.FechaFin).As("EventEndTime"))
                    .Add(Projections.Property(() => log.LatitudFin).As("FinalLatitude"))
                    .Add(Projections.Property(() => log.LongitudFin).As("FinalLongitude"))
                    .Add(Projections.Property(() => log.Id).As("Id"))
                    .Add(Projections.Property(() => coche.Chofer.Entidad.Descripcion).As("Responsable"))
                    .Add(Projections.Property(() => log.TieneFoto).As("TieneFoto"))
                    .Add(Projections.Property(() => log.IdPuntoDeInteres).As("IdPuntoInteres"))
                    .Add(Projections.Property(() => log.Estado).As("Atendido"))
                    .Add(Projections.Property(() => log.Usuario).As("Usuario"))
                    //.Add(Projections.Property(() => log.Estado > 0 ? DAOFactory.AtencionEventoDAO.GetByEvento(log.Id) : null).As("AtencionEvento"))
                );

            if (distritoId != -1)
                q = q.Where(m => viaje.Empresa.Id == distritoId);

            if (baseId != -1)
                q = q.Where(m => viaje.Linea.Id == baseId);

            q = q.TransformUsing(Transformers.AliasToBean<MobileEvent>());

            return q.Future<MobileEvent>();
        }

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
                                          IdMensaje = log.Mensaje.Id,
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

        /// <summary>
        /// Gets a list of mobiles events filtered by the givenn search criteria.
        /// </summary>
        /// <param name="mobilesIds">A list of mobiles ids.</param>
        /// <param name="messagesIds">A list of message ids.</param>
        /// <param name="driversIds">A list of drivers ids.</param>
        /// <param name="initialDate">The initial date.</param>
        /// <param name="finalDate">The final date.</param>
        /// <returns>A list of events.</returns>
        public List<MobileEvent> GetMobilesEventsLinq(List<int> mobilesIds, IEnumerable<int> messagesIds, List<int> driversIds, DateTime initialDate, DateTime finalDate, int maxMonths, int page, int pageSize, ref int totalRows, bool reCount)
        {
            var codigos = (from codigo in messagesIds select codigo.ToString()).ToList();

            var results = DAOFactory.LogMensajeDAO.GetByVehiclesAndCodesLinq(mobilesIds, codigos, initialDate, finalDate, maxMonths, page, pageSize, ref totalRows, reCount);

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
                IdMensaje = log.Mensaje.Id,
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
                                       IdMensaje = log.Mensaje.Id,
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
