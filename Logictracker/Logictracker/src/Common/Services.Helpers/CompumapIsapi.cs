#region Usings

using System.Net;
using System.Text;
using System.Xml;
using Logictracker.Services.Helpers.Types;

#endregion

namespace Logictracker.Services.Helpers
{
    public class CompumapIsapi
    {
        public Recorrido CalcularRecorrido(Recorrido recorrido, string url)
        {
            var urlFinal = string.Format(
                "{0}?mi={1}&pi={2}&ai={3}&ei={4}&mf={5}&pf={6}&af={7}&ef={8}&t={9}&av={10}&au={11}",
                url,
                recorrido.DireccionInicial.IdMapa,
                recorrido.DireccionInicial.IdPoligonal,
                recorrido.DireccionInicial.Altura,
                recorrido.DireccionInicial.IdEsquina,
                recorrido.DireccionFinal.IdMapa,
                recorrido.DireccionFinal.IdPoligonal,
                recorrido.DireccionFinal.Altura,
                recorrido.DireccionFinal.IdEsquina,
                recorrido.TipoRecorrido,
                recorrido.PesoAvenida,
                recorrido.PesoAutopista);

            try
            {
                var xml = GetXml(urlFinal);

                var node = xml.GetElementsByTagName("RECORRIDO")[0];
                recorrido.FromXml(node);
            }
            catch
            {
            }
            return recorrido;
        }
        public RecorridoMicro CalcularRecorridoMicro(RecorridoMicro recorrido, string url)
        {
            var urlFinal = string.Format(
                "{0}?mi={1}&pi={2}&ai={3}&ei={4}&mf={5}&pf={6}&af={7}&ef={8}&e={9}",
                url,
                recorrido.DireccionInicial.IdMapa,
                recorrido.DireccionInicial.IdPoligonal,
                recorrido.DireccionInicial.Altura,
                recorrido.DireccionInicial.IdEsquina,
                recorrido.DireccionFinal.IdMapa,
                recorrido.DireccionFinal.IdPoligonal,
                recorrido.DireccionFinal.Altura,
                recorrido.DireccionFinal.IdEsquina,
                recorrido.Radio);

            try
            {
                var xml = GetXml(urlFinal);

                var node = xml.GetElementsByTagName("RECORRIDO")[0];
                recorrido.FromXml(node);
            }
            catch
            {
            }
            return recorrido;
        }
        private XmlDocument GetXml(string url)
        {
            var request = WebRequest.Create(url);
            var response = request.GetResponse();

            var stream = response.GetResponseStream();

            var buffer = new byte[response.ContentLength];

            for (long i = 0; i < response.ContentLength; i++)
                buffer[i] = (byte)stream.ReadByte();

            var content = Encoding.GetEncoding("ISO-8859-1").GetString(buffer);

            var xml = new XmlDocument();
            xml.LoadXml(content);
            return xml;
        }
    }
}
