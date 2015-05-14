using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class VehicleUtilizationDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public VehicleUtilizationDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets mobile activitties report objects.
        /// </summary>
        /// <param name="coche"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public List<VehicleUtilization> GetMobileUtilizations(Int32 coche, DateTime desde, DateTime hasta)
        {
            var results = new List<VehicleUtilization>();

            var datamarts = DAOFactory.DatamartDAO.GetBetweenDates(coche, desde, hasta);

            desde = desde.ToDisplayDateTime();
            hasta = hasta.ToDisplayDateTime();

            foreach (var datamart in datamarts) datamart.Begin = datamart.Begin.ToDisplayDateTime();

            var movil = DAOFactory.CocheDAO.FindById(coche);

            var turnos = DAOFactory.ShiftDAO.GetVehicleShifts(movil);

            if (datamarts.Count <= 0 || !DAOFactory.ShiftDAO.HasShifts(movil)) return new List<VehicleUtilization>();

            var fechaActual = desde;

            while (fechaActual < hasta)
            {
                var result = new VehicleUtilization { Fecha = fechaActual };

                var turnosDia = turnos.Where(turno => turno.AppliesToDate(fechaActual)).OrderBy(turno => turno.Inicio).ToList();

                result.HsTurnos = turnosDia.Select(turno => turno.Fin - turno.Inicio).ToList();

                result.HsTurnos.Add(24 - result.HsTurnos.Sum());

                result.HsReales = turnosDia.Select(turno => datamarts.Where(data => data.Shift != null && data.Shift.Id == turno.Id
                        && data.Begin >= fechaActual && data.Begin < fechaActual.AddDays(1)).Sum(data => data.MovementHours)).ToList();

                result.HsReales.Add(datamarts.Where(data => data.Shift == null
                    && data.Begin >= fechaActual && data.Begin < fechaActual.AddDays(1)).Sum(data => data.MovementHours));

                results.Add(result);

                fechaActual = fechaActual.AddDays(1);
            }

            return results;
        }

        #endregion
    }
}
