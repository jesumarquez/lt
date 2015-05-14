#region Usings

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;

#endregion

namespace Logictracker.Services.Helpers.Types
{
    public class TrayectoMicro
    {
        public TrayectoMicro()
        {
            Micros = new List<Micro>();
        }

        public string Nombre { get; set; }
        public double Distancia { get; set; }
        public List<Micro> Micros {get; set;}

        public static TrayectoMicro FromXml(XmlNode node)
        {
            double tmp;
            var trayecto = new TrayectoMicro
            {
                Nombre = HttpUtility.UrlDecode(node.Attributes["name"].Value, Encoding.GetEncoding("ISO-8859-1")),
                Distancia = double.TryParse("0" + node.Attributes["dist"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp) ? tmp : 0
            };
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name != "MICRO") continue;
                trayecto.Micros.Add(Micro.FromXml(childNode));
            }
            return trayecto;
        }
    }
}
