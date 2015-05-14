using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Security;
using Microsoft.Win32;

namespace Logictracker.Scheduler.Tasks.ServiceStatus
{
    /// <summary>
    /// Tasks for checking the current status of the services.
    /// </summary>
    public class Task : BaseTask
    {
        #region Protected Methods

        /// <summary>
        /// Performs taks main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            using (var securityHelper = new WindowsSecurityHelper())
            {
				STrace.Trace(GetType().FullName, "Checking services status.");

                if (!securityHelper.ImpersonateValidUser()) throw new Exception("Can not impersonate admin user for restarting a service.");

                var crashedServices = GetCrasehdServices();

				if (!crashedServices.Any()) STrace.Trace(GetType().FullName, "All services are running normally.");
                else foreach (var winService in crashedServices) RestartService(winService);

                securityHelper.UndoImpersonation();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the currently crasehd services.
        /// </summary>
        /// <returns></returns>
		private static ServiceController[] GetCrasehdServices()
        {
	        return Config.Services.ServicesToMonitor.Select(service => new ServiceController(service)).Where(winService => winService.Status.Equals(ServiceControllerStatus.Stopped)).ToArray();
        }

        /// <summary>
        /// Restarts the specified service.
        /// </summary>
        /// <param name="service"></param>
        private void RestartService(ServiceController service)
        {
            STrace.Trace(GetType().FullName, String.Format("Service crash detected: {0}.", service.DisplayName));

            STrace.Trace(GetType().FullName, String.Format("Attempting to restart service: {0}.", service.DisplayName));

            try
            {
                if (!MustRestartService(service))
                {
                    STrace.Trace(GetType().FullName, String.Format("Ignoring service becouse it is disables: {0}", service.DisplayName));
                    return;
                }

                service.Start();

                service.WaitForStatus(ServiceControllerStatus.Running);

                STrace.Trace(GetType().FullName, String.Format("Service restarted ok: {0}.", service.DisplayName));

                SendMail(service, "Service restart ok!");
            }
            catch (Exception e)
            {
				STrace.Exception(GetType().FullName, e, String.Format("Service restart failed: {0}.", service.DisplayName));

                SendMail(service, "Service restart failed!");
            }
        }

        /// <summary>
        /// Determines if the current service should be restarted.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private Boolean MustRestartService(ServiceController service)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(String.Format(@"SYSTEM\CurrentControlSet\Services\{0}", service.ServiceName), true);

                if (key == null) return true;

                var value = key.GetValue("Start");

                return value == null || Convert.ToInt32(value) != 4;
            }
            catch (Exception e)
            {
				STrace.Exception(GetType().FullName, e);

                return true;
            }
        }

        /// <summary>
        /// Sends a email with info about the event.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="status"></param>
        private void SendMail(ServiceController service, String status)
        {
			STrace.Trace(GetType().FullName, "Sending email notification.");

            try
            {
                var configFile = Config.Mailing.SchedulerServiceStatusMailingConfiguration;

                if (String.IsNullOrEmpty(configFile)) throw new Exception("Imposible to load mailing configuration.");

                var sender = new MailSender(configFile);

                var parameters = new List<String> { service.DisplayName, DateTime.Now.ToString(CultureInfo.InvariantCulture), status };

                SendMailToAllDestinations(sender, parameters);

				STrace.Trace(GetType().FullName, "Notification send.");
            }
            catch (Exception e)
            {
				STrace.Exception(GetType().FullName, e, "Failed to send notification.");
            }
        }

        #endregion
    }
}
