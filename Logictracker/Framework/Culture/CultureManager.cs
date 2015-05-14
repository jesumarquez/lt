#region Usings

using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using Logictracker.Security;

#endregion

namespace Logictracker.Culture
{
    public static class CultureManager
    {
        #region Public Constant Resources Declaration

        private const string Controls = "Controls";
        private const string Entities = "Entities";
        private const string Errors = "Errors";
        private const string Labels = "Labels";
        private const string Menu = "Menu";
        private const string ImagesMenu = "ImagesMenu";
        private const string SystemMessages = "SystemMessages";
        private const string User = "User";
        private const string Securables = "Securables";

        #endregion

        #region Private Properties

        private static string[] _manifestResourceNames;

        private static IEnumerable<string> ManifestResourceNames
        {
            get { return _manifestResourceNames ?? (_manifestResourceNames = ResourceAssembly.GetManifestResourceNames().Select(r=>r.Replace(".resources", "")).ToArray()); }
        }
        /// <summary>
        /// The application name.
        /// </summary>
        private static string AppName
        {
            get
            {
                var user = WebSecurity.AuthenticatedUser;
                var appName = user != null ? user.Client : string.Empty;
                return string.IsNullOrEmpty(appName) ? string.Empty : appName;
            }
        }

        private static Assembly _resourceAssembly;

        private static Assembly ResourceAssembly
        {
            get { return _resourceAssembly ?? (_resourceAssembly = typeof(CultureManager).Assembly); }
        }

        private static string ResourceBaseName
        {
            get { return "Logictracker.Culture.Resources"; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the indicated resource using the current thread culture info.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetString(string resource, string variable) { return GetString(resource, variable, Thread.CurrentThread.CurrentCulture); }

        /// <summary>
        /// Gets the indicated resource using the provided culture info.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="variable"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static string GetString(string resource, string variable, CultureInfo cultureInfo)
        {
            var res = string.Empty;

            if (!string.IsNullOrEmpty(AppName)) res = GetResource(string.Concat(ResourceBaseName, '.', AppName, '.', resource), variable, cultureInfo);

            if (!string.IsNullOrEmpty(res)) return res;

            res = GetResource(string.Concat(ResourceBaseName, '.', resource), variable, cultureInfo);

            return string.IsNullOrEmpty(res) ? variable/*"UNDEFINED"*/ : res;
        }

        /// <summary>
        /// Gets the indicated resource using the provided culture info.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static object GetObject(string resource, string variable) { return GetObject(resource, variable, Thread.CurrentThread.CurrentCulture); }

        /// <summary>
        /// Gets the indicated resource using the provided culture info.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="variable"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static object GetObject(string resource, string variable, CultureInfo cultureInfo)
        {
            object res = null;

            if (!string.IsNullOrEmpty(AppName)) res = GetObjectResource(string.Format("{0}.{1}.{2}", ResourceBaseName, AppName, resource), variable, cultureInfo);

            if (res != null) return res;

            res = GetObjectResource(string.Format("{0}.{1}", ResourceBaseName, resource), variable, cultureInfo);

            return res;
        }

        /// <summary>
        /// Gets all the strings associated to the givenn resource and current culture.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllStrings(string resource) { return GetAllStrings(resource, Thread.CurrentThread.CurrentCulture); }

        /// <summary>
        /// Gets all the strings associated to the givenn resource and culture.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllStrings(string resource, CultureInfo cultureInfo)
        {
            var resources = new Dictionary<string, string>();

            var resourceSet = GetResourceSet(string.Format("{0}.{1}", ResourceBaseName, resource), cultureInfo);

            if (resourceSet == null) return resources;

            var enumerator = resourceSet.GetEnumerator();

            while (enumerator.MoveNext()) resources.Add((string)enumerator.Key, GetString(resource, (string)enumerator.Key));

            return resources;
        }

        /// <summary>
        /// Gets the error message associated to the givenn variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetError(string variable) { return GetString(Errors, variable); }

        /// <summary>
        /// Gets the system message associated to the givenn variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetSystemMessage(string variable) { return GetString(SystemMessages, variable); }

        /// <summary>
        /// Gets the menu item description associated to the givenn variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetMenu(string variable) { return GetString(Menu, variable); }

        /// <summary>
        /// Gets the user defined message associated to the givenn variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetUser(string variable) { return GetString(User, variable); }

        /// <summary>
        /// Gets the entity description associated to the givenn variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetEntity(string variable) { return GetString(Entities, variable); }

        /// <summary>
        /// Gets the control description associated to the givenn variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetControl(string variable) { return GetString(Controls, variable); }

        /// <summary>
        /// Gets the label associated to the givenn variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetLabel(string variable) { return GetString(Labels, variable); }

        /// <summary>
        /// Gets the securable name for the given variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetSecurable(string variable) { return GetString(Securables, variable); }

        public static Bitmap GetMenuImage(string variable) { return GetObject(ImagesMenu, variable) as Bitmap; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Looks for the specified variable using the givenn resorce path and culture.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <param name="variable"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        private static string GetResource(string resourcePath, string variable, CultureInfo culture)
        {
            try
            {
                if (!ManifestResourceNames.Any(r => r == resourcePath)) return null;

                var resourceManager = new ResourceManager(resourcePath, ResourceAssembly) { IgnoreCase = true };

                return resourceManager.GetString(variable, culture);
            }
            catch { return null; }
        }

        /// <summary>
        /// Looks for the specified variable using the givenn resorce path and culture.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <param name="variable"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        private static object GetObjectResource(string resourcePath, string variable, CultureInfo culture)
        {
            try
            {
                var resourceManager = new ResourceManager(resourcePath, ResourceAssembly) { IgnoreCase = true };

                return resourceManager.GetObject(variable, culture);
            }
            catch { return null; }
        }

        /// <summary>
        /// Gets the resource set for the givenn resource path and culture.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        private static ResourceSet GetResourceSet(string resourcePath, CultureInfo culture)
        {
            try
            {
                var resourceManager = new ResourceManager(resourcePath, ResourceAssembly) { IgnoreCase = true };

                return resourceManager.GetResourceSet(culture, true, true);
            }
            catch { return null; }
        }
        #endregion
    }
}
