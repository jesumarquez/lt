using System;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class MobileStadisticsDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileStadisticsDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets mobile stadistics within the givenn interval.
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="iniDate"></param>
        /// <param name="finDate"></param>
        /// <returns></returns>
        public MobileStadistics GetMobileStadistics(Int32 mobile, DateTime iniDate, DateTime finDate)
        {
            var datamarts = DAOFactory.DatamartDAO.GetBetweenDates(mobile, iniDate, finDate);

            if (!datamarts.Any()) return new MobileStadistics();

            var stats = datamarts
                .GroupBy(datamart => new
                                         {
                                             datamart.Vehicle.Interno,
                                             datamart.Vehicle.Patente,
                                             datamart.Vehicle.TipoCoche.Descripcion,
                                             datamart.Vehicle.Dispositivo
                                         })
                .Select(datamart => new MobileStadistics
                                        {
                                            KilometrosTotales = datamart.Sum(data => data.Kilometers),
                                            VelocidadMaxima = datamart.Max(data => data.MaxSpeed),
                                            VelocidadPromedio = (int) datamart.Average(data => data.AverageSpeed),
                                            Infracciones = datamart.Sum(data => data.Infractions),
                                            HsMovimiento = datamart.Sum(data => data.MovementHours),
                                            HsDetenido = datamart.Sum(data => data.StoppedHours),
                                            HsSinReportar = datamart.Sum(data => data.NoReportHours),
                                            HsInfraccion = datamart.Sum(data => data.InfractionMinutes),
                                            Interno = datamart.Key.Interno,
                                            Patente = datamart.Key.Patente,
                                            TipoVehiculo = datamart.Key.Descripcion,
                                            Dispositivo = datamart.Key.Dispositivo != null ? datamart.Key.Dispositivo.Codigo : string.Empty
                                        })
                .SingleOrDefault();

            if (stats == null) return stats;

            var dias = (int)Math.Ceiling((finDate - iniDate).TotalDays);

            var activos = datamarts.Where(datamart => datamart.MovementHours > 0).Select(datamart => datamart).ToList();

            stats.MovementEvents = activos.Count();

            stats.DiasActivo = (from activo in activos select activo.Begin.ToDisplayDateTime().DayOfYear).Distinct().Count();

            stats.Dias = stats.DiasActivo > dias ? stats.DiasActivo : dias;

            stats.KilometrosPromedio = stats.DiasActivo != 0 ? stats.KilometrosTotales/stats.DiasActivo : 0;

            stats.VelocidadPromedio = datamarts.Any(datamart => datamart.AverageSpeed > 0) ? (int)datamarts.Where(datamart => datamart.AverageSpeed > 0).Average(datamart => datamart.AverageSpeed) : 0;

            stats.StoppedEvents = datamarts.Count(datamart => datamart.StoppedHours > 0);

            stats.NoReportEvents = datamarts.Count(datamart => datamart.NoReportHours > 0);

            stats.TipoVehiculo = stats.TipoVehiculo ?? "Sin Tipo de Vehiculo";

            return stats;
        }

        #endregion
    }
}
