#region Usings

using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;

#endregion

namespace Logictracker.Configuration
{
    internal static class ConfigurationBase
    {
        /// <summary>
        /// Application root.
        /// </summary>
        internal static String ApplicationPath
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
        /// Gets the specified key as a Boolean.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value associated.</param>
        /// <returns></returns>
		internal static Boolean GetAsBoolean(String key, Boolean defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];

			return String.IsNullOrEmpty(value) ? defaultValue : Convert.ToBoolean(value);
        }

        /// <summary>
        /// Gets the specified key as a String.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value associated.</param>
        /// <returns></returns>
        internal static String GetAsString(String key, String defaultValue)
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
        internal static Int32 GetAsInt32(String key, Int32 defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];

            return String.IsNullOrEmpty(value) ? defaultValue : Convert.ToInt32(value);
        }

        /// <summary>
        /// Resolves the givenn path into its absolute local path.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        internal static String GetLocalPath(String file)
        {
            var path = Path.Combine(ApplicationPath, file);

            return HttpContext.Current != null ? HttpContext.Current.Server.MapPath(path) : path;
        }
    }
}
