#region Usings

using System;
using System.Globalization;
using System.Xml;

#endregion

namespace Logictracker.Services.Helpers.Types
{
    public class Vertice
    {
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public int Altura { get; set; }

        public static Vertice FromXml(XmlNode node)
        {
            double tmp;
            return new Vertice
            {
                Latitud = double.TryParse(node.Attributes["latitud"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)?tmp:0,
                Longitud = double.TryParse(node.Attributes["longitud"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp) ? tmp : 0,
                Altura = Convert.ToInt32(node.Attributes["altura"].Value)
            };
        }
    }
}
