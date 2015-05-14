#region Usings

using System;
using System.Configuration;

#endregion

namespace Urbetrack.Postal.Sync.Helpers
{
    /// <summary>
    /// Class for handling all configuration properties.
    /// </summary>
    public static class Configuration
    {
        #region Public Properties

        /// <summary>
        /// Pda configuration ini file path.
        /// </summary>
        public static String PdaIniFilePath { get { return GetConfigurationString("urbetrack.postal.pda.ini.path", String.Empty); } }

        /// <summary>
        /// Pda configuration ini file name.
        /// </summary>
        public static String PdaIniFileName { get { return GetConfigurationString("urbetrack.postal.pda.ini.name", "postal.ini"); } }

        /// <summary>
        /// Pda configuration database file path.
        /// </summary>
        public static String PdaDatabaseFilePath { get { return GetConfigurationString("urbetrack.postal.pda.database.path", String.Empty); } }

        /// <summary>
        /// Pda configuration database file name.
        /// </summary>
        public static String PdaDatabaseFileName { get { return GetConfigurationString("urbetrack.postal.pda.database.name", "urbetrack_postal.sqlite"); } }

        /// <summary>
        /// Local temporary directory path.
        /// </summary>
        public static String TemporaryPath { get { return GetConfigurationString("urbetrack.tmp.path", "tmp"); } }

        /// <summary>
        /// Local temporary export directory path.
        /// </summary>
        public static String TemporaryExportPath { get { return GetConfigurationString("urbetrack.tmp.path", "tmp\\export"); } }

        /// <summary>
        /// Local temporary import directory path.
        /// </summary>
        public static String TemporaryImportPath { get { return GetConfigurationString("urbetrack.tmp.path", "tmp\\import"); } }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets as a string the configuration value associated to the givenn key or the default value if the key is not present.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static String GetConfigurationString(String key, String defaultValue) { return (GetConfigurationObject(key) as String) ?? defaultValue; }

        /// <summary>
        /// Gets the object associated to the givenn key or null if it does not exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static Object GetConfigurationObject(String key) { return ConfigurationManager.AppSettings[key]; }

        #endregion
    }
}