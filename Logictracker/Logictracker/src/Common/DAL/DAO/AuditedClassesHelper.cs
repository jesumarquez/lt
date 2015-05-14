using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Logictracker.Configuration;

namespace Logictracker.DAL.DAO
{
    public static class AuditedClassesHelper
    {
        #region Private Properties

        /// <summary>
        /// Last update of the Audit Log File.
        /// </summary>
        private static DateTime _lastAuditFileUpdateDate;

        private static readonly object AuditFileLock = new Object();

        /// <summary>
        /// Contains all the classes going to be audited.
        /// </summary>
        private static readonly List<string> AuditedClasses = new List<string>();

        /// <summary>
        /// Configured TimeSpan for Audit Configuration file Refresh.
        /// </summary>
        private static readonly TimeSpan AuditUpdateInterval = TimeSpan.FromMinutes(Config.Dispatcher.DispatcherGeocercasRefreshRate);

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the Types of all classes configured as auditable.
        /// </summary>
        /// <returns></returns>
        public static List<String> GetAuditedClasses()
        {
            if (DateTime.Now.Subtract(_lastAuditFileUpdateDate) >= AuditUpdateInterval)
            {
                lock (AuditFileLock)
                {
                    _lastAuditFileUpdateDate = DateTime.Now;
                    ReloadAuditFile();
                }
            }
            return AuditedClasses;
        }

        public static List<String> GetAuditedEntityNames()
        {
            return new List<String>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reloads the Audit Config XML.
        /// </summary>
        private static void ReloadAuditFile()
        {
            var xmlDoc = new XmlDocument();

            var configFile = Config.Audit.AuditConfiguration;

            if (string.IsNullOrEmpty(configFile))
            {
                AuditedClasses.Clear();
                return;
            }

            xmlDoc.Load(configFile);

            var xmlNodesList = xmlDoc.GetElementsByTagName("audit");
            var classes = (from XmlNode node in xmlNodesList select node.Attributes["className"].Value).ToList();

            AuditedClasses.Clear();
            AuditedClasses.AddRange(classes);
        }
    }

    #endregion
}
