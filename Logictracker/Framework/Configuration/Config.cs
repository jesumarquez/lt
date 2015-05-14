#region Usings

using System;
using System.Web;

#endregion

namespace Logictracker.Configuration
{
    /// <summary>
    /// Class for getting configuration values for the application.
    /// </summary>
    public static partial class Config
    {
        /// <summary>
        /// Application root.
        /// </summary>
        public static String ApplicationPath { get { return ConfigurationBase.ApplicationPath; } }

        /// <summary>
        /// Gets the application title.
        /// </summary>
        public static String ApplicationTitle { get { return ConfigurationBase.GetAsString("logictracker.title", "Logictracker"); } }

        /// <summary>
        /// Gets the current host name.
        /// </summary>
        public static String Host { get { return HttpContext.Current.Request.Url.Host; } }
        
        /// <summary>
        /// Gets the nhibernate mapping assembly.
        /// </summary>
        public static String NhibernateAssembly { get { return ConfigurationBase.GetAsString("logictracker.hibernate.assembly", "Logictracker.DAL"); } }

        /// <summary>
        /// Gets google analytics account name.
        /// </summary>
        /// <returns></returns>
        public static String AnalyticsAccount { get { return ConfigurationBase.GetAsString("logictracker.google.analytics", String.Empty); } }

        /// <summary>
        /// Logictracker configured resources groups.
        /// </summary>
        public static String LogictrackerResourcesGroups { get { return ConfigurationBase.GetAsString("logictracker.resources.groups", "Logictracker"); } }

        /// <summary>
        /// Available Cultures File
        /// </summary>
        public static String AvailableCulturesFile { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.controls.availablecultures", "Logictracker.Controls.AvailableCultures.config")); } }

		/// <summary>
		/// Max interval for pictures in seconds. Deafults 300 seg.
		/// </summary>
		public static int MaxPictureTime { get { return ConfigurationBase.GetAsInt32("logictracker.picture.max", 300); } }

		/// <summary>
		/// default timeToReceive an ack from device to messages sent by the gateway
		/// </summary>
		public static String DefaultTimeToReceive { get { return ConfigurationBase.GetAsString("logictracker.defaulttimetoreceive", "900"); } }

        /// <summary>
        /// Define si los Periodos se van a usar por Empresa(Distrito) o son globales para toda la aplicación 
        /// </summary>
        public static bool PeriodosGlobales { get { return ConfigurationBase.GetAsBoolean("logictracker.periodos.global", false); } }

        /// <summary>
        /// Si los períodos son globales define que día del mes se inician.
        /// </summary>
        public static int PeriodosInicio { get { return ConfigurationBase.GetAsInt32("logictracker.periodos.inicio", 1); } }
	}
}
