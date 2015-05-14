#region Usings

using System;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Directory
        {
            /// <summary>
            /// Images directory.
            /// </summary>
            public static String ImagesDir { get { return String.Concat(ApplicationPath, ConfigurationBase.GetAsString("logictracker.images", "images/")); } }

            /// <summary>
            /// Sounds directory.
            /// </summary>
            public static String SoundsDir { get { return String.Concat(ApplicationPath, ConfigurationBase.GetAsString("logictracker.sounds", "sonido/")); } }

            /// <summary>
            /// Icons directory.
            /// </summary>
            public static String IconDir { get { return String.Concat(ApplicationPath, ConfigurationBase.GetAsString("logictracker.icons", "iconos/")); } }

            /// <summary>
            /// Attach directory.
            /// </summary>
            public static String AttachDir { get { return String.Concat(ApplicationPath, ConfigurationBase.GetAsString("logictracker.attach", "Parametrizacion/Attach/")); } }

            /// <summary>
            /// Application temporary directory.
            /// </summary>
            public static String TmpDir { get { return String.Concat(ApplicationPath, ConfigurationBase.GetAsString("logictracker.tmp", "tmp/")); } }

            /// <summary>
            /// Char definition file directory.
            /// </summary>
            public static String FusionChartDir { get { return String.Concat(ApplicationPath, ConfigurationBase.GetAsString("logictracker.fusioncharts.xml", "FusionCharts/")); } }

            /// <summary>
            /// Logictracker qtree directory.
            /// </summary>
            public static String PicturesDirectory { get { return ConfigurationBase.GetAsString("logictracker.pictures.directory", ConfigurationBase.GetLocalPath(@"Pictures\")); } }

            /// <summary>
            /// Logictracker fota directory.
            /// </summary>
            public static String FotaDirectory { get { return ConfigurationBase.GetAsString("logictracker.fota.path", ConfigurationBase.GetLocalPath(@"")); } }
            
        }
    }
}
