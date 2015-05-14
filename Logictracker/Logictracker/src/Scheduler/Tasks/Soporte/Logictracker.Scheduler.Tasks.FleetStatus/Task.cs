using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace Logictracker.Scheduler.Tasks.FleetStatus
{
    /// <summary>
    /// Task for periodically report the current fleet status.
    /// </summary>
    public class Task : BaseTask
    {
        #region Private Properties

        /// <summary>
        /// Auxiliar list for storing device status information.
        /// </summary>
        private readonly List<String> _fleetStatus = new List<String>();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs tasks main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
			STrace.Trace(GetType().FullName, "Retrieving device types.");

            var deviceTypes = DaoFactory.TipoDispositivoDAO.FindAll();

            foreach (var deviceType in deviceTypes) AddFleetStatus(deviceType);

            Notify();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds fleet status about the givenn device type.
        /// </summary>
        /// <param name="deviceType"></param>
        private void AddFleetStatus(TipoDispositivo deviceType)
        {
            STrace.Trace(GetType().FullName, String.Format("Retrieving devices of type: {0} - {1}.", deviceType.Modelo, deviceType.Fabricante));

            var now = DateTime.UtcNow;

            var devices = DaoFactory.DispositivoDAO.GetByTipo(deviceType);

            if (devices.Count.Equals(0)) return;

            var vehiculos = devices.Select(d => DaoFactory.CocheDAO.FindMobileByDevice(d.Id));

            if (vehiculos.Count().Equals(0)) return;

            var positions = DaoFactory.LogPosicionDAO.GetLastVehiclesPositions(vehiculos).Values.Any(position => position != null && now.Subtract(position.FechaMensaje).TotalMinutes <= 5);

            if (positions) return;

            var status = String.Format("None vehicle currently reporting - Device type: {0} - {1}.", deviceType.Modelo, deviceType.Fabricante);

			STrace.Trace(GetType().FullName, status);

            _fleetStatus.Add(status);
        }

        /// <summary>
        /// Sends mails to notify the current fleet status.
        /// </summary>
        private void Notify()
        {
            if (_fleetStatus.Count.Equals(0)) return;

			STrace.Trace(GetType().FullName, "Sending notification mails to all configured destinations.");

            var configFile = Config.Mailing.SchedulerFleetStatusMailingConfiguration;

			if (String.IsNullOrEmpty(configFile)) throw new Exception("Failed to load fleet status mailing configuration.");

            var sender = new MailSender(configFile);

            var statusBuilder = new StringBuilder();

            foreach (var status in _fleetStatus)
            {
                statusBuilder.Append(status);
                statusBuilder.Append("<br />");
            }

            var parameters = new List<String> { statusBuilder.ToString() };

            SendMailToAllDestinations(sender, parameters);
        }

        #endregion
    }
}
