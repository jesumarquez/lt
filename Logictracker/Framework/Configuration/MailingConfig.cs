namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Mailing
        {
            /// <summary>
            /// Gets the logictracker common sms mailing configuration.
            /// </summary>
            public static string MailingSmsConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.sms", "Logictracker.Mailing.SMS.config")); } }

            public static string MailingArriboConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.prediccionArribo", "Logictracker.Mailing.PrediccionArribo.config")); } }

            /// <summary>
            /// Gets the logictracker common mailing configuration.
            /// </summary>
            public static string MailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing", "Logictracker.Mailing.config")); } }

            public static string GeneracionTareasMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.generacionTareas", "Logictracker.Mailing.GeneracionTareas.config")); } }

            public static string LogiclinkErrorMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.logiclink.error", "Logictracker.Mailing.LogiclinkError.config")); } }

            public static string CambioEstadoMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.cambio.estado", "Logictracker.Mailing.CambioEstado.config")); } }

            public static string MessageCountMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.message.count", "Logictracker.Mailing.MessageCount.config")); } }

            public static string WatchDogMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.watchdog", "Logictracker.Mailing.WatchDog.config")); } }

            public static string DatamartErrorMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.datamart.error", "Logictracker.Mailing.DatamartError.config")); } }

            public static string ReportConfirmation { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.reportConfirmation", "Logictracker.Mailing.ReportConfirmation.config")); } }

            public static string DatamartSuccessMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.datamart.success", "Logictracker.Mailing.DatamartSuccess.config")); } }
            
            public static string MonitoreoGarminMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.monitoreo.garmin", "Logictracker.Mailing.MonitoreoGarmin.config")); } }

            /// <summary>
            /// Gets the logictracker notifier mailing configuration.
            /// </summary>
            public static string SchedulerServiceStatusMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.servicestatus", "Logictracker.Mailing.ServiceStatus.config")); } }

            /// <summary>
            /// Gets the logictracker common sms mailing configuration.
            /// </summary>
            public static string FuelMailingSmsConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.combustible.sms", "Logictracker.Mailing.SMS.Combustible.config")); } }

            /// <summary>
            /// Gets the logictracker common mailing configuration.
            /// </summary>
            public static string FuelMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.combustible", "Logictracker.Mailing.Combustible.config")); } }

            /// <summary>
            /// Get the Folder Path of the Error Mailing Config Path.
            /// </summary>
            /// <returns></returns>
            public static string ErrorMailingConfig { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.error", "Logictracker.Mailing.ErrorNotifying.config")); } }

            /// <summary>
            /// Get the Folder Path of the Support Mailing Config Path.
            /// </summary>
            /// <returns></returns>
            public static string SupportMailingConfig { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.support", "Logictracker.Mailing.SupportTicket.config")); } }

            /// <summary>
            /// Gets the queue status notifying mailing configuration.
            /// </summary>
            public static string SchedulerQueueStatusMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.scheduler.queue.mailing", "Logictracker.Mailing.QueueStatus.config")); } }

            /// <summary>
            /// Gets the fleet status notifying mailing configuration.
            /// </summary>
            public static string SchedulerFleetStatusMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.scheduler.fleet.mailing", "Logictracker.Mailing.FleetStatus.config")); } }

            /// <summary>
            /// Gets the error notifying mailing configuration.
            /// </summary>
            public static string SchedulerTaskSuccesMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.scheduler.success.mailing", "Logictracker.Mailing.TaskSuccess.config")); } }

            /// <summary>
            /// Gets the error notifying mailing configuration.
            /// </summary>
            public static string SchedulerErrorMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.scheduler.error.mailing", "Logictracker.Mailing.ErrorNotifying.config")); } }

            public static string ReportSchedulerMailingConfiguration { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.mailing.reportscheduler", "Logictracker.Mailing.ReportScheduler.config")); } }

        }
    }
}
