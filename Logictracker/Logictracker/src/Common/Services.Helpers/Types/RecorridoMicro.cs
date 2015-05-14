#region Usings

using System.Collections.Generic;
using System.Xml;
using Logictracker.Configuration;

#endregion

namespace Logictracker.Services.Helpers.Types
{
    public class RecorridoMicro: RecorridoBase
    {
        private const string IsapiFile = "micros.srf";

        public RecorridoMicro()
        {
            Radio = 500;
            Trayectos = new List<TrayectoMicro>();
        }

        public int Radio { get; set; }
        public List<TrayectoMicro> Trayectos { get; set; }

        internal override void FromXml(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name != "TRAYECTO") continue;
                Trayectos.Add(TrayectoMicro.FromXml(childNode));
            }
        }
        public override void Calcular()
        {
            var url = string.Concat("http://", Config.Host, Config.Map.CompumapIsapiDir + IsapiFile); 
            var isapi = new CompumapIsapi();
            isapi.CalcularRecorridoMicro(this, url);
        }
    }
}
