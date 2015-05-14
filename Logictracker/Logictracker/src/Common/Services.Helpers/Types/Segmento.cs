#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;

#endregion

namespace Logictracker.Services.Helpers.Types
{
    public class Segmento
    {
        public Segmento()
        {
            Vertices = new List<Vertice>();
        }

        public string Nombre { get; set; }
        public string NombreMapa { get; set; }
        public double Distancia { get; set; }
        public short IdMapa { get; set; }
        public int IdPoligonal { get; set; }
        public int IdEsquina { get; set; }
        public int Altura { get; set; }

        public List<Vertice> Vertices { get; set; }

        public static Segmento FromXml(XmlNode node)
        {
            double tmp;
            var segmento = new Segmento
                       {
                           Nombre = HttpUtility.UrlDecode(node.Attributes["nombreLargo"].Value, Encoding.GetEncoding("ISO-8859-1")),
                           NombreMapa = HttpUtility.UrlDecode(node.Attributes["nombreMapa"].Value, Encoding.GetEncoding("ISO-8859-1")),
                           Distancia = double.TryParse("0"+node.Attributes["distancia"].Value,NumberStyles.Any, CultureInfo.InvariantCulture, out tmp)?tmp: 0,
                           IdMapa = Convert.ToInt16(node.Attributes["idMapa"].Value),
                           IdPoligonal = Convert.ToInt32(node.Attributes["idPoligonal"].Value),
                           IdEsquina = Convert.ToInt32(node.Attributes["idEsquina"].Value),
                           Altura = Convert.ToInt32(node.Attributes["altura"].Value)
                       };
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name != "VERTICE") continue;
                segmento.Vertices.Add(Vertice.FromXml(childNode));
            }
            
            return segmento;
        }
    }
}
