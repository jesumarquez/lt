using System;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class OperatorStadisticsDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public OperatorStadisticsDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets mobile stadistics within the givenn interval.
        /// </summary>
        /// <param name="empleado"></param>
        /// <param name="iniDate"></param>
        /// <param name="finDate"></param>
        /// <returns></returns>
        public OperatorStadistics GetOperatorStadistics(Int32 empleado, DateTime iniDate, DateTime finDate)
        {
            var datamarts = DAOFactory.DatamartDAO.GetForEmployee(empleado, iniDate, finDate);

            var stats = datamarts
                .GroupBy(datamart => new { datamart.Employee.Legajo, Nombre = datamart.Employee.Entidad.Descripcion,
                    Tipo = datamart.Employee.TipoEmpleado != null ? datamart.Employee.TipoEmpleado.Descripcion : String.Empty})
                .Select(datamart => new OperatorStadistics
                                        {
                                            KilometrosTotales = datamart.Sum(data => data.Kilometers),
                                            VelocidadMaxima = datamart.Max(data => data.MaxSpeed),
                                            Infracciones = datamart.Sum(data => data.Infractions),
                                            HsMovimiento = datamart.Sum(data => data.MovementHours),
                                            HsDetenido = datamart.Sum(data => data.StoppedHours),
                                            HsSinReportar = datamart.Sum(data => data.NoReportHours),
                                            MinsInfraccion = datamart.Sum(data => data.InfractionMinutes),
                                            Legajo = datamart.Key.Legajo,
                                            Nombre = datamart.Key.Nombre,
                                            TipoEmpleado = datamart.Key.Tipo
                                        }).SingleOrDefault();

            if (stats == null) return stats;

            var dias = (finDate - iniDate).Days;

            var activos = datamarts.Where(datamart => datamart.MovementHours > 0).Select(datamart => datamart).ToList();

            stats.MovementEvents = activos.Count();

            stats.DiasActivo = (from activo in activos select activo.Begin.ToDisplayDateTime().DayOfYear).Distinct().Count();

            stats.Dias = stats.DiasActivo > dias ? stats.DiasActivo : dias;

            stats.KilometrosPromedio = stats.DiasActivo != 0 ? stats.KilometrosTotales/stats.DiasActivo : 0;

            stats.VelocidadPromedio = datamarts.Count(datamart => datamart.AverageSpeed > 0) > 0 ? datamarts.Where(datamart => datamart.AverageSpeed > 0).Average(data => data.AverageSpeed) : 0;

            stats.StoppedEvents = datamarts.Count(datamart => datamart.StoppedHours > 0);

            stats.NoReportEvents = datamarts.Count(datamart => datamart.NoReportHours > 0);

            stats.TipoEmpleado = stats.TipoEmpleado ?? "Sin Tipo de Empleado";

            return stats;
        }

        #endregion
    }
}
