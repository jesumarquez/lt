#region Usings

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;

#endregion

namespace Urbetrack.Common.Utils
{
    /// <summary>
    /// Class for getting configuration values for the application.
    /// </summary>
    public static class ConfigurationHelper
    {
        #region Public Properties

        /// <summary>
        /// Application root.
        /// </summary>
        public static String ApplicationPath
        {
            get
            {
                if (HttpContext.Current != null) return VirtualPathUtility.ToAbsolute("~/");
                
                var path = Assembly.GetExecutingAssembly().Location;

                if (String.IsNullOrEmpty(path)) return String.Empty;

                path = path.Substring(0, path.LastIndexOf("\\"));

                return path;
            }
        }

        /// <summary>
        /// Images directory.
        /// </summary>
        public static String ImagesDir { get { return String.Concat(ApplicationPath, GetAsString("urbetrack.images", "images/")); } }

        /// <summary>
        /// Sounds directory.
        /// </summary>
        public static String SoundsDir { get { return String.Concat(ApplicationPath, GetAsString("urbetrack.sounds", "sonido/")); } }

        /// <summary>
        /// Icons directory.
        /// </summary>
        public static String IconDir { get { return String.Concat(ApplicationPath, GetAsString("urbetrack.icons", "iconos/")); } }

        /// <summary>
        /// Application temporary directory.
        /// </summary>
        public static String TmpDir { get { return String.Concat(ApplicationPath, GetAsString("urbetrack.tmp", "tmp/")); } }

        /// <summary>
        /// Gets the application title.
        /// </summary>
        public static String ApplicationTitle { get { return GetAsString("urbetrack.title", "Urbetrack"); } }

        /// <summary>
        /// Char definition file directory.
        /// </summary>
        public static String FusionChartDir { get { return String.Concat(ApplicationPath, GetAsString("urbetrack.fusioncharts.xml", "FusionCharts/")); } }

        /// <summary>
        /// Gets the queues user sid.
        /// </summary>
        public static String QueueUser { get { return GetAsString("urbetrack.queues.user", "S-1-5-32-545"); } }

        /// <summary>
        /// Compumap ISAPI directory.
        /// </summary>
        public static String CompumapIsapiDir
        {
            get
            {
                var isapidir = String.Concat(ApplicationPath, GetAsString("urbetrack.compumap.isapi", "CompumapISAPI/"));

                if (!isapidir.EndsWith("/")) isapidir += "/";

                return isapidir;
            }
        }

        /// <summary>
        /// Gets the current host name.
        /// </summary>
        public static String Host { get { return HttpContext.Current.Request.Url.Host; } }

        /// <summary>
        /// Gets the google maps script key according to the current host.
        /// </summary>
        public static String GoogleMapsKey
        {
            get
            {
                var domain = HttpContext.Current.Request.Url.Host;

                if (domain.StartsWith("www.")) domain = domain.Replace("www.", String.Empty);

                var key = String.Format("urbetrack.google.maps.{0}", domain);

                return ConfigurationManager.AppSettings[key];
            }
        }

        /// <summary>
        /// Gets the google maps script key according to the current host.
        /// </summary>
        public static String GoogleEarthKey
        {
            get
            {
                var domain = HttpContext.Current.Request.Url.Host;

                if (domain.StartsWith("www.")) domain = domain.Replace("www.", String.Empty);

                var key = String.Format("urbetrack.google.earth.{0}", domain);

                return ConfigurationManager.AppSettings[key];
            }
        }

        /// <summary>
        /// User for impersonating aspnet.
        /// </summary>
        public static String ImpersonateUser { get { return ConfigurationManager.AppSettings["urbetrack.services.user"]; } }

        /// <summary>
        /// The domain of the user to be impersonated.
        /// </summary>
        public static String ImpersonateDomain { get { return ConfigurationManager.AppSettings["urbetrack.services.domain"]; } }

        /// <summary>
        /// The password of the user to be impersonated.
        /// </summary>
        public static String ImpersonatePassword { get { return ConfigurationManager.AppSettings["urbetrack.services.password"]; } }

        /// <summary>
        /// Gets the Compumap layer tiles url.
        /// </summary>
        public static String CompumapTiles { get { return GetAsString("urbetrack.compumap.tiles", "http://200.42.96.150"); } }

        /// <summary>
        /// Gets the minimun log type to trace into database.
        /// </summary>
        public static String TracerMinLogType { get { return GetAsString("urbetrack.tracer.minlogtype", "Trace"); } }

        /// <summary>
        /// Gets the log trace retries.
        /// </summary>
        public static Int32 TracerMaxRetries { get { return GetAsInt32("urbetrack.tracer.maxretries", 5); } }

        /// <summary>
        /// Gets the error sleep interval.
        /// </summary>
        public static TimeSpan TracerErrorInterval { get { return TimeSpan.FromMilliseconds(GetAsInt32("urbetrack.tracer.errorinterval", 100)); } }

        /// <summary>
        /// Gets the metrics error retries
        /// </summary>
        public static Int32 MetricsMaxRetries { get { return GetAsInt32("urbetrack.metrics.maxretries", 5); } }

        public static Int32 PuertoEscuchaAccess { get { return GetAsInt32("puertoEscucha", 2000); } }

        /// <summary>
        /// Gets the error sleep interval.
        /// </summary>
        public static TimeSpan MetricsErrorInterval { get { return TimeSpan.FromMilliseconds(GetAsInt32("urbetrack.metrics.errorinterval", 100)); } }

        /// <summary>
        /// Gets the interval in seconds to check if a metric should be generated.
        /// </summary>
        public static TimeSpan MetricsGenerationInterval { get { return TimeSpan.FromSeconds(GetAsInt32("urbetrack.metrics.generationinterval", 5)); } }

        /// <summary>
        /// Gets the error notifying mailing configuration.
        /// </summary>
        public static String SchedulerTaskSuccesMailingConfiguration { get { return GetLocalPath(GetAsString("urbetrack.scheduler.success.mailing", "Urbetrack.Common.Mailing.TaskSuccess.config")); } }

        /// <summary>
        /// Gets the error notifying mailing configuration.
        /// </summary>
        public static String SchedulerErrorMailingConfiguration { get { return GetLocalPath(GetAsString("urbetrack.scheduler.error.mailing", "Urbetrack.Common.Mailing.ErrorNotifying.config")); } }

        /// <summary>
        /// Gets the scheduler timers configuration file.
        /// </summary>
        public static String SchedulerTimersConfiguration { get { return GetLocalPath(GetAsString("urbetrack.scheduler.timers.configuration", "configuration.xml")); } }

        /// <summary>
        /// Gets the scheduler timers configuration file.
        /// </summary>
        public static String UrbetrackMetricsConfiguration { get { return GetLocalPath(GetAsString("urbetrack.metrics.configuration", "metrics.xml")); } }

        /// <summary>
        /// Gets the nhibernate mapping assembly.
        /// </summary>
        public static String NhibernateAssembly { get { return GetAsString("urbetrack.hibernate.assembly", "Urbetrack.DAL"); } }

        /// <summary>
        /// Gets the tracer nhibernate mapping assembly.
        /// </summary>
        public static String TracerNhibernateAssembly { get { return GetAsString("urbetrack.tracer.hibernate.assembly", "Urbetrack.DatabaseTracer"); } }

        /// <summary>
        /// Gets the tracer nhibernate mapping assembly.
        /// </summary>
        public static String MetricsNhibernateAssembly { get { return GetAsString("urbetrack.metrics.hibernate.assembly", "Urbetrack.Metrics"); } }

        /// <summary>
        /// Gets the urbetrack common sms mailing configuration.
        /// </summary>
        public static String MailingSmsConfiguration { get { return GetLocalPath(GetAsString("urbetrack.mailing.sms", "Urbetrack.Common.Mailing.SMS.config")); } }

        /// <summary>
        /// Gets the urbetrack common mailing configuration.
        /// </summary>
        public static String MailingConfiguration { get { return GetLocalPath(GetAsString("urbetrack.mailing", "Urbetrack.Common.Mailing.config")); } }

        /// <summary>
        /// Gets the urbetrack notifier mailing configuration.
        /// </summary>
        public static String SchedulerServiceStatusMailingConfiguration { get { return GetLocalPath(GetAsString("urbetrack.mailing.servicestatus", "Urbetrack.Common.Mailing.ServiceStatus.config")); } }

        public static String ReportSchedulerMailingConfiguration { get { return GetLocalPath(GetAsString("urbetrack.mailing.reportscheduler", "Urbetrack.Common.Mailing.ReportScheduler.config")); } }

        /// <summary>
        /// Gets the urbetrack common sms mailing configuration.
        /// </summary>
        public static String FuelMailingSmsConfiguration { get { return GetLocalPath(GetAsString("urbetrack.mailing.combustible.sms", "Urbetrack.Common.Mailing.SMS.Combustible.config")); } }

        /// <summary>
        /// Gets the urbetrack common mailing configuration.
        /// </summary>
        public static String FuelMailingConfiguration { get { return GetLocalPath(GetAsString("urbetrack.mailing.combustible", "Urbetrack.Common.Mailing.Combustible.config")); } }

        /// <summary>
        /// Gets the urbetrack historic monitor link configuration.
        /// </summary>
        public static String HistoricMonitorLink { get { return GetAsString("urbetrack.monitores.historico", String.Empty); } }

        /// <summary>
        /// Gets google analytics account name.
        /// </summary>
        /// <returns></returns>
        public static String AnalyticsAccount { get { return GetAsString("urbetrack.google.analytics", String.Empty); } }

        /// <summary>
        /// Get the Folder Path of the Error Mailing Config Path.
        /// </summary>
        /// <returns></returns>
        public static String ErrorMailingConfig { get { return GetLocalPath(GetAsString("urbetrack.mailing.error", "Urbetrack.Common.Mailing.ErrorNotifying.config")); } }

        /// <summary>
        /// Get the Folder Path of the Support Mailing Config Path.
        /// </summary>
        /// <returns></returns>
        public static String SupportMailingConfig { get { return GetLocalPath(GetAsString("urbetrack.mailing.support", "Urbetrack.Common.Mailing.SupportTicket.config")); } }

        /// <summary>
        /// Gets the AssistCargo web service user
        /// </summary>
        public static String AssistCargoWebServiceUser { get { return GetAsString("urbetrack.webservice.assistcargo.user", "telef"); } }

        /// <summary>
        /// Gets the AssistCargo web service password
        /// </summary>
        public static String AssistCargoWebServicePassword { get { return GetAsString("urbetrack.webservice.assistcargo.pass", "Kurtz"); } }

        /// <summary>
        /// Gets the AssistCargo web service password
        /// </summary>
        public static String AssistCargoEventQueue { get { return GetAsString("urbetrack.queues.assistcargo.events", ".\\private$\\eventos_assistcargo"); } }

        public static String AssistCargoDisableFuelCode { get { return GetAsString("urbetrack.interfaces.assistcargo.eventcodes.disablefuel", "CC"); } }
        public static String AssistCargoDisableFuelInmediatelyCode { get { return GetAsString("urbetrack.interfaces.assistcargo.eventcodes.disablefuelinmediately", "CI"); } }
        public static String AssistCargoEnableFuelCode { get { return GetAsString("urbetrack.interfaces.assistcargo.eventcodes.enablefuel", "RC"); } }

        /// <summary>
        /// Gets the names of all services to be monitored.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> ServicesToMonitor
        {
            get
            {
                var services = new List<String>();
                var configServices = ConfigurationManager.AppSettings["urbetrack.services"];

                if (String.IsNullOrEmpty(configServices)) return services;

                var splitted = configServices.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                services.AddRange(splitted);

                return services;
            }
        }

        /// <summary>
        /// Gets the geocercas refresh interval.
        /// </summary>
        public static Int32 DispatcherGeocercasRefreshRate { get { return GetAsInt32("urbetrack.dispatcher.behaivours.geocercas.refresh", 15); } }

        /// <summary>
        /// Gets the max hdop value accepted for a position.
        /// </summary>
        public static Int32 DispatcherMaxHdop { get { return GetAsInt32("urbetrack.dispatcher.behaivours.positions.maxhdop", 10); } }

        /// <summary>
        /// Gets the radius to use for correcting stopped positions.
        /// </summary>
        public static Int32 DispatcherSttopedGeocercaRadius { get { return GetAsInt32("urbetrack.dispatcher.behaivours.detentions.radius", 50); } }

        /// <summary>
        /// Gets the min time to inform a stopped event.
        /// </summary>
        public static Int32 DispatcherSttopedGeocercaTime { get { return GetAsInt32("urbetrack.dispatcher.behaivours.detentions.time", 60); } }

        /// <summary>
        /// Gets the max device internal positions cache for the dispatcher.
        /// </summary>
        public static Int32 DispatcherDeviceQueueExpiration { get { return GetAsInt32("urbetrack.dispatcher.behaivours.positions.device.queue.expiration", 90); } }

        /// <summary>
        /// Gets the max time difference toleration to accept a future position as a valid one.
        /// </summary>
        public static Int32 DispatcherPositionsTimeTolerance { get { return GetAsInt32("urbetrack.dispatcher.behaivours.positions.timetolerance", 15); } }

        /// <summary>
        /// Gets the minimum duration of a speeding ticket to be considered a valid infraction..
        /// </summary>
        public static Int32 DispatcherMinimumSpeedTicketDuration { get { return GetAsInt32("urbetrack.dispatcher.behaivours.tickets.minimumduration", 0); } }

        /// <summary>
        /// Gets the max time difference toleration to accept a future position as a valid one.
        /// </summary>
        public static Int32 DispatcherDeviceParametersRefresh { get { return GetAsInt32("urbetrack.dispatcher.behaivours.device.parameters.refresh", 15); } }

        /// <summary>
        /// Urbetrack cache name.
        /// </summary>
        public static String UrbetrackCachePoolName { get { return GetAsString("urbetrack.cache.poolname", "urbetrack.cache"); } }

        /// <summary>
        /// Urbetrack cache server.
        /// </summary>
        public static String[] UrbetrackCacheServers { get { return GetAsString("urbetrack.cache.servers", "127.0.0.1:11212").Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).ToArray(); } }

        /// <summary>
        /// Urbetracks configured resources groups.
        /// </summary>
        public static String UrbetrackResourcesGroups { get { return GetAsString("urbetrack.resources.groups", "Urbetrack"); } }

		/// <summary>
		/// Urbetrack qtree directory.
		/// </summary>
		public static String PicturesDirectory { get { return GetAsString("urbetrack.pictures.directory", GetLocalPath(@"Pictures\")); } }

		/// <summary>
		/// Urbetrack qtree directory.
		/// </summary>
		public static String QtreeDirectory { get { return GetAsString("urbetrack.qtree.directory", GetLocalPath(@"Qtree\")); } }

		/// <summary>
        /// Urbetrack gte qtree directory.
        /// </summary>
        public static String QtreeGteDirectory { get { return Path.Combine(QtreeDirectory, "Gte"); } }

        /// <summary>
        /// Urbetrack torino qtree directory.
        /// </summary>
        public static String QtreeTorinoDirectory { get { return Path.Combine(QtreeDirectory, "Torino"); } }

        /// <summary>
        /// Urbetrack addresser nhibernate session factory configuration.
        /// </summary>
        public static String AddresserSessionFactory { get { return GetLocalPath(GetAsString("urbetrack.addresser.sessionfactory", "Addresser.NHibernate.config")); } }

        /// <summary>
        /// Compumap files directory path.
        /// </summary>
        public static String MapFilesDirectory { get { return GetAsString("urbetrack.compumap.mapfiles", String.Empty); } }

        /// <summary>
        /// Gets a list of the system queues currently beeing monitored.
        /// </summary>
        public static IEnumerable<string> UrbetrackQueues
        {
            get
            {
                var queues = GetAsString("urbetrack.queues", String.Empty);

                return queues.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        /// <summary>
        /// Gets the queue status notifying mailing configuration.
        /// </summary>
        public static String SchedulerQueueStatusMailingConfiguration { get { return GetLocalPath(GetAsString("urbetrack.scheduler.queue.mailing", "Urbetrack.Common.Mailing.QueueStatus.config")); } }

        /// <summary>
        /// Gets the fleet status notifying mailing configuration.
        /// </summary>
        public static String SchedulerFleetStatusMailingConfiguration { get { return GetLocalPath(GetAsString("urbetrack.scheduler.fleet.mailing", "Urbetrack.Common.Mailing.FleetStatus.config")); } }

        /// <summary>
        /// Available Cultures File
        /// </summary>
        public static String AvailableCulturesFile { get { return GetLocalPath(GetAsString("urbetrack.controls.availablecultures", "Urbetrack.Controls.AvailableCultures.config")); } }

        /// <summary>
        /// Max interval for pictures in seconds. Deafults 300 seg.
        /// </summary>
        public static int MaxPictureTime { get { return GetAsInt32("urbetrack.picture.max", 300); } }


        #region Audit Configurations

        /// <summary>
        /// Gets the Audit configuration.
        /// </summary>
        public static String AuditConfiguration { get { return GetLocalPath(GetAsString("urbetrack.web.auditfile", "Urbetrack.Common.Audit.config")); } }

        /// <summary>
        /// Gets the audit fie refresh interval.
        /// </summary>
        public static Int32 AuditFileRefreshRate { get { return GetAsInt32("urbetrack.web.audit.refresh", 300); } }

        #endregion
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the ext extenders css class plath.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static String GetExtExtendersCss(Page page) { return String.Concat(ApplicationPath, "App_Themes/", page.Theme.Replace(" ", "%20"), "/MapControl/xtheme-black.css"); }

        /// <summary>
        /// Gets the monitor control image folder path.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static String GetMonitorImagesFolder(Page page) { return String.Concat(ApplicationPath, "App_Themes/", page.Theme.Replace(" ", "%20"), "/MapControl/img/"); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the specified key as a String.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value associated.</param>
        /// <returns></returns>
        private static String GetAsString(String key, String defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];

            return String.IsNullOrEmpty(value) ? defaultValue : value;
        }

        /// <summary>
        /// Gets the specified key as a Int32.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value associated.</param>
        /// <returns></returns>
        private static Int32 GetAsInt32(String key, Int32 defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];

            return String.IsNullOrEmpty(value) ? defaultValue : Convert.ToInt32(value);
        }

        /// <summary>
        /// Resolves the givenn path into its absolute local path.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static String GetLocalPath(String file)
        {
            var path = Path.Combine(ApplicationPath, file);

            return HttpContext.Current != null ? HttpContext.Current.Server.MapPath(path) : path;
        }

        #endregion
    }
}