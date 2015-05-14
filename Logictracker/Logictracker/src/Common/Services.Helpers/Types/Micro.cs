#region Usings

using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

#endregion

namespace Logictracker.Services.Helpers.Types
{
    public class Micro
    {
        public Micro()
        {
            Segmentos = new List<Segmento>();
        }

        public string Nombre { get; set; }
        public string EsquinaInicial { get; set; }
        public string EsquinaFinal { get; set; }
        public List<Segmento> Segmentos { get; set; }

        public static Micro FromXml(XmlNode node)
        {
            var micro = new Micro
            {
                Nombre = HttpUtility.UrlDecode(node.Attributes["name"].Value, Encoding.GetEncoding("ISO-8859-1")),
                EsquinaInicial = HttpUtility.UrlDecode(node.Attributes["esqI"].Value, Encoding.GetEncoding("ISO-8859-1")),
                EsquinaFinal = HttpUtility.UrlDecode(node.Attributes["esqF"].Value, Encoding.GetEncoding("ISO-8859-1"))
            };
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name != "SEGMENTO") continue;
                micro.Segmentos.Add(Segmento.FromXml(childNode));
            }
            return micro;
        }
    }
}
