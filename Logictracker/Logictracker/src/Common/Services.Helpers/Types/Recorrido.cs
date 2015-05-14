#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Logictracker.Configuration;

#endregion

namespace Logictracker.Services.Helpers.Types
{
    public class Recorrido: RecorridoBase
    {
        public const char Corto = 'C';
        private const string IsapiFile = "recorridos.srf";
        public const char Pie = 'P';
        public const char Rapido = 'R';

        public Recorrido()
            :this(new Direccion(), new Direccion())
        {
        }
        public Recorrido(Direccion inicio, Direccion fin)
        {
            Segmentos = new List<Segmento>();
            DireccionInicial = inicio;
            DireccionFinal = fin;
            TipoRecorrido = Rapido;
            PesoAvenida = 850;
            PesoAutopista = 700;
        }

        public List<Segmento> Segmentos { get; set; }
        public char TipoRecorrido { get; set; }
        public int PesoAvenida { get; set; }
        public int PesoAutopista { get; set; }

        public double Distancia
        {
            get
            {
                return Segmentos.Sum(segmento => segmento.Distancia);
            }
        }
        internal override void FromXml(XmlNode node)
        {
            if (node == null) return;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name != "SEGMENTO") continue;
                Segmentos.Add(Segmento.FromXml(childNode));
            }
        }
        public override void Calcular()
        {
            try
            {
                var url = string.Concat("http://", Config.Host, Config.Map.CompumapIsapiDir + IsapiFile);
                var isapi = new CompumapIsapi();

                isapi.CalcularRecorrido(this, url);
            }
            catch
            { }
        }
    }
}
