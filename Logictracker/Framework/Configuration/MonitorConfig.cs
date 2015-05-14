#region Usings

using System;
using System.Web.UI;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Monitor
        {
            /// <summary>
            /// Gets the ext extenders css class plath.
            /// </summary>
            /// <param name="page"></param>
            /// <returns></returns>
            public static String GetExtExtendersCss(Page page) { return String.Concat(ApplicationPath, "App_Styles/extjs.css"); }
            //public static String GetExtExtendersCss(Page page) { return String.Concat(ApplicationPath, "App_Themes/", page.Theme.Replace(" ", "%20"), "/MapControl/xtheme-black.css"); }

            /// <summary>
            /// Gets the monitor control image folder path.
            /// </summary>
            /// <param name="page"></param>
            /// <returns></returns>
            public static String GetMonitorImagesFolder(Page page) { return String.Concat(ApplicationPath, "App_Styles/img/ol/"); }
            //public static String GetMonitorImagesFolder(Page page) { return String.Concat(ApplicationPath, "App_Themes/", page.Theme.Replace(" ", "%20"), "/MapControl/img/"); }

            /// <summary>
            /// Gets the logictracker historic monitor link configuration.
            /// </summary>
            public static String HistoricMonitorLink { get { return ConfigurationBase.GetAsString("logictracker.monitores.historico", String.Empty); } }
        }
    }
}
