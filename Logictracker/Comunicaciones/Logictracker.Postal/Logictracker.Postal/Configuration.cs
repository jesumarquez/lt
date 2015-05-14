using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Urbetrack.Postal
{
    public static class Configuration
    {
        public static bool PhotoNeedsGps = true;
        public static Size Resolution = new Size(1280, 1024);
        static Configuration()
        {
            try
            {
                var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                var configFile = Path.Combine(path, "Urbetrack.Postal.config");
                if(!File.Exists(configFile)) return;
                var sr = File.OpenText(configFile);
                while(!sr.EndOfStream)
                {
                    CheckParameter(sr.ReadLine().Trim());
                }
                sr.Close();
                
            }
            catch(Exception ex)
            {
                
            }
        }

        private static void CheckParameter(string line)
        {
            var s = line.Trim().Split('=');
            if(s.Length < 2) return;
            var parameter = s[0].ToLower();
            var value = string.Join("=", s, 1, s.Length - 1).Trim();

            switch(parameter)
            {
                case "photoneedsgps":
                    PhotoNeedsGps = value == "1" || value == "true";
                    break;
                case "resolution":
                    var res = value.Split('x');
                    int h, v;
                    if (TryParseInt32(res[0], out h) && TryParseInt32(res[1], out v))
                        Resolution = new Size(h, v);
                    break;
            }
        }

        private static bool TryParseInt32(string s, out int v)
        {
            try
            {
                v = int.Parse(s);
                return true;
            }
            catch
            {
                v = 0;
                return false;
            }
        }
    }
}
