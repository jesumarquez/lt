using System;
using System.IO;
using System.Xml.Serialization;

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Camera
        {
            private static String CameraConfigFile { get { return ConfigurationBase.GetLocalPath(ConfigurationBase.GetAsString("logictracker.monitores.camaras", "Logictracker.Cameras.config")); } }

            private static DateTime _lastRefresh = DateTime.MinValue;
            private static Cameras.Configuration _config;
            public static Cameras.Configuration Config
            {
                get
                {
                    if (_config == null || _lastRefresh.AddMinutes(15) < DateTime.UtcNow)
                    {
                        _lastRefresh = DateTime.UtcNow;
                        using (var fs = new FileStream(CameraConfigFile, FileMode.Open, FileAccess.Read))
                        {
                            var xml = new XmlSerializer(typeof (Cameras.Configuration));
                            _config = (Cameras.Configuration) xml.Deserialize(fs);
                            fs.Close();
                        }
                    }
                    return _config;
                }
            }
        }
    }
}
