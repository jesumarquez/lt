#region Usings

using System;
using System.Configuration;
using System.Web;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Map
        {
            /// <summary>
            /// Gets the google maps script key according to the current host.
            /// </summary>
            public static String GoogleMapsKey
            {
                get
                {
                    var domain = HttpContext.Current.Request.Url.Host;

                    if (domain.StartsWith("www.")) domain = domain.Replace("www.", String.Empty);

                    var key = String.Format("logictracker.google.maps.{0}", domain);

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

                    var key = String.Format("logictracker.google.earth.{0}", domain);

                    return ConfigurationManager.AppSettings[key];
                }
            }

            /// <summary>
            /// Compumap ISAPI directory.
            /// </summary>
            public static String CompumapIsapiDir
            {
                get
                {
                    var isapidir = String.Concat(ApplicationPath, ConfigurationBase.GetAsString("logictracker.compumap.isapi", "CompumapISAPI/"));

                    if (!isapidir.EndsWith("/")) isapidir += "/";

                    return isapidir;
                }
            }

            /// <summary>
            /// Logictracker Geocoder nhibernate session factory configuration.
            /// </summary>
            public static String GeocoderSessionFactory { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.geocoder.sessionfactory", "Geocoder.NHibernate.config")); } }

            /// <summary>
            /// Compumap files directory path.
            /// </summary>
            public static String MapFilesDirectory { get { return ConfigurationBase.GetAsString("logictracker.compumap.mapfiles", String.Empty); } }

            /// <summary>
            /// Gets the Compumap layer tiles url.
            /// </summary>
            public static String CompumapTiles { get { return ConfigurationBase.GetAsString("logictracker.compumap.tiles", "http://200.26.26.78/tiles"); } }
        }
    }
}
