using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections.Specialized;
using Urbetrack.Mobile.Toolkit;

namespace Urbetrack.Mobile.Gateway
{
    public static class ConfigurationManager
    {

        #region Private Members

        private static readonly NameValueCollection appSettings = new NameValueCollection();
        private static readonly string configFile;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets configuration settings in the appSettings section.
        /// </summary>

        public static NameValueCollection AppSettings
        {
            get
            {
                return appSettings;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>

        static ConfigurationManager()
        {
            // Determine the location of the config file
            configFile = String.Format("{0}.config", Assembly.GetExecutingAssembly().GetName().CodeBase);
            T.TRACE(0, "Configuration File: {0}", configFile);
            // Ensure configuration file exists
            if (!File.Exists(configFile))
            {
                throw new Exception(String.Format("Configuration file ({0}) could not be found.", configFile));
            }
            // Load config file as an XmlDocument
            var myXmlDocument = new XmlDocument();
            myXmlDocument.Load(configFile);
            // Add keys and values to the AppSettings NameValueCollection
            var nodes = myXmlDocument.SelectNodes("/configuration/appSettings/add");
            if (nodes == null) return;
            foreach (XmlNode appSettingNode in nodes)
            {
                AppSettings.Add(appSettingNode.Attributes["key"].Value, appSettingNode.Attributes["value"].Value);
            }
        }

        #endregion

        #region Public Methods

        public static string GetAppSettingsValue(string key, string default_value)
        {
            try
            {
                var value = AppSettings[key];
                if (!String.IsNullOrEmpty(value)) return value;
                
            } catch (Exception e)
            {
                T.EXCEPTION(e, "GetAppSettingsValue[" + key + "]");
            }
            return default_value;
        }

        /// <summary>
        /// Saves changes made to the configuration settings.
        /// </summary>
        public static void Save()
        {
            // Load config file as an XmlDocument
            var myXmlDocument = new XmlDocument();
            myXmlDocument.Load(configFile);

            // Get the appSettings node
            var appSettingsNode = myXmlDocument.SelectSingleNode("/configuration/appSettings");

            if (appSettingsNode != null)
            {
                // Remove all previous appSetting nodes
                appSettingsNode.RemoveAll();

                foreach (var key in AppSettings.AllKeys)
                {

                    // Create a new appSetting node
                    var appSettingNode = myXmlDocument.CreateElement("add");
                    // Create the key attribute and assign its value
                    var keyAttribute = myXmlDocument.CreateAttribute("key");
                    keyAttribute.Value = key;
                    // Create the value attribute and assign its value
                    var valueAttribute = myXmlDocument.CreateAttribute("value");
                    valueAttribute.Value = AppSettings[key];
                    // Append the key and value attribute to the appSetting node
                    appSettingNode.Attributes.Append(keyAttribute);
                    appSettingNode.Attributes.Append(valueAttribute);
                    // Append the appSetting node to the appSettings node
                    appSettingsNode.AppendChild(appSettingNode);
                }
            }

            // Save config file
            myXmlDocument.Save(configFile);
        }
        #endregion

    }
}