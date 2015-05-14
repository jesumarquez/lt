using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class MobileMessageDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileMessageDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets mobile messages report objects.
        /// </summary>
        /// <param name="mobileId"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        public List<MobileMessage> GetMobileMessages(int mobileId, DateTime fechaInicio, DateTime fechaFin)
        {
            var coche = DAOFactory.CocheDAO.FindById(mobileId);
            var maxMonths = coche != null && coche.Empresa != null ? coche.Empresa.MesesConsultaPosiciones : 3;

            var messages = DAOFactory.LogMensajeDAO.GetEvents(mobileId, fechaInicio, fechaFin, maxMonths);

            var results = messages.Select(m => new MobileMessage
                              {
                                MovilId = m.Coche.Id,
                                Velocidad = null,
                                Indice = m.Id,
                                Mensaje = m.Texto ?? " ",
                                Patente = m.Coche.Patente,
                                Interno = m.Coche.Interno,
                                Chofer = m.Chofer != null ? m.Chofer.Entidad.Descripcion : String.Empty,
                                Responsable = m.Coche.Chofer != null ? m.Coche.Chofer.Entidad.Descripcion : String.Empty,
                                FechaYHora = m.Fecha.ToDisplayDateTime()
                              }).ToList();

            results.AddRange(GetPositions(mobileId, fechaInicio, fechaFin));

            return results.OrderBy(data => data.FechaYHora).ToList();
        }

        public List<MobileMessage> GetMobileVelocities(int mobileId, DateTime fechaInicio, DateTime fechaFin)
        {
            return GetPositions(mobileId, fechaInicio, fechaFin).OrderBy(data => data.FechaYHora).ToList();
        }

        public List<MobileMessage> GetPositions(int mobileId, DateTime fechaInicio, DateTime fechaFin)
        {
            var vehicle = DAOFactory.CocheDAO.FindById(mobileId);
            var maxMonths = vehicle != null && vehicle.Empresa != null ? vehicle.Empresa.MesesConsultaPosiciones : 3;
            var positions = DAOFactory.LogPosicionDAO.GetPositionsBetweenDates(mobileId, fechaInicio, fechaFin, maxMonths);

            return positions.Select(p => new MobileMessage
                                             {
                                                 MovilId = p.Coche.Id,
                                                 Velocidad = p.Velocidad,
                                                 Indice = p.Id,
                                                 Mensaje = "Toma de Posición",
                                                 Patente = p.Coche.Patente,
                                                 Interno = p.Coche.Interno,
                                                 Chofer = String.Empty,
                                                 Responsable = p.Coche.Chofer != null ? p.Coche.Chofer.Entidad.Descripcion : String.Empty,
                                                 FechaYHora = p.FechaMensaje.ToDisplayDateTime(),
                                             }).ToList();
        }

        #endregion
    }
}
