#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject.Positions;

#endregion

namespace Logictracker.Scheduler.Tasks.NoReport
{
    /// <summary>
    /// Tasks for warning about vehicles which las position was at least 48hs ago.
    /// </summary>
    public class Task : BaseTask
    {
        #region Protected Methods

        /// <summary>
        /// Performs task main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
			STrace.Trace(GetType().FullName, "Checking vehicles last position.");

            foreach (var position in GetPositionsToAnalize()) AnalizePosition(position);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gats al positions older than 48hs.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<LogUltimaPosicionVo> GetPositionsToAnalize()
        {
            var vehicles = DaoFactory.CocheDAO.FindAllActivos().Where(c => c.Dispositivo != null);
            
            var positions = DaoFactory.LogPosicionDAO.GetLastVehiclesPositions(vehicles);

            return positions.Values.Where(position => position != null && DateTime.UtcNow.Subtract(position.FechaMensaje).TotalHours >= 48).Select(position => position).ToList();
        }

        /// <summary>
        /// Analize if the givenn position requires a warning.
        /// </summary>
        /// <param name="position"></param>
        private void AnalizePosition(LogUltimaPosicionVo position)
        {
            var vehicle = DaoFactory.CocheDAO.FindById(position.IdCoche);
            var lastWarning = DaoFactory.LogMensajeDAO.GetLastMessageDate(vehicle, MessageCode.NoReport.GetMessageCode());

            if (lastWarning.HasValue && lastWarning.Value >= position.FechaMensaje) return;

            vehicle.DeleteLastMessageDate(MessageCode.NoReport.GetMessageCode());

            SaveNoReportEvent(vehicle, position);
        }

        /// <summary>
        /// Saves a no report warning event.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="position"></param>
        private void SaveNoReportEvent(Coche vehicle, LogUltimaPosicionVo position)
        {
            var lastPositionDate = GetLastPositionDate(vehicle, position);
            var text = String.Format(" - Vehiculo: {0} - Último Reporte: {1}", position.Coche, lastPositionDate);

            STrace.Trace(GetType().FullName, String.Format("The vehicle {0} needs to be checked (last position on: {1}).", position.Coche, lastPositionDate));

            MessageSaver.Save(MessageCode.NoReport.GetMessageCode(), vehicle, DateTime.UtcNow, null, text);
        }

        /// <summary>
        /// Gets the last position date converted to the vehicle`s associated culture.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static DateTime GetLastPositionDate(Coche vehicle, LogUltimaPosicionVo position)
        {
            var culture = GetCulture(vehicle);

            return culture != null ? position.FechaMensaje.AddHours(culture.BaseUtcOffset.TotalHours) : position.FechaMensaje;
        }

        /// <summary>
        /// Get the culture associated to the specified vehicle.
        /// </summary>
        /// <returns></returns>
        private static TimeZoneInfo GetCulture(Coche coche)
        {
			var timeZoneId = coche.Linea != null ? coche.Linea.TimeZoneId : coche.Empresa != null ? coche.Empresa.TimeZoneId : String.Empty;

			return String.IsNullOrEmpty(timeZoneId) ? null : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        #endregion
    }
}
