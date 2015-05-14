#region Usings

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace Logictracker.Web.Helpers.ExtensionMethods
{
    public static partial class WebExtentions
    {
        public static Control CreateWebControl(string id, string metadata, string value)
        {
            var md = ParseMetadata(metadata);
            if (!md.ContainsKey("Control") || md["Control"] == "hidden") return null;
            Control ctrl = null;
            var setValue = !string.IsNullOrEmpty(value) ? value : (md.ContainsKey("default") ? md["default"] : "");
            switch (md["Control"])
            {
                case "textbox":
                    ctrl = new TextBox { ID= id, Text = setValue };
                    break;
                default:
                    return null;
            }
             
            return ctrl;
        }

        public static bool Matchs(this IDictionary<string, string> dict, string param, string value)
        {
            if (string.IsNullOrEmpty(param) || !dict.ContainsKey(param)) return false;
            return dict[param] == value;
        }

        public static Dictionary<string, string> ParseMetadata(string metadata)
        {
            if (string.IsNullOrEmpty(metadata)) throw new ArgumentNullException("metadata", "Debe especificar el tipo de control."); 

            var md = new Dictionary<string, string>();
            foreach (var p in metadata.Split(";".ToCharArray()))
            {
                var x = p.Split("=".ToCharArray(), 2);
                if (x.GetLength(0) != 2) continue;
                md.Add(x[0], x[1]);
            }
            return md;
        }

    }
}
