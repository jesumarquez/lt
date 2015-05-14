using System.Linq;
using System.Xml;
using System.Globalization;
using Geocoder.Core.VO;

namespace Logictracker.Services.Helpers
{
    public static class GoogleGeocoder
    {
        private const string ReverseGeocodingUrl = "http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false&language=es";
        public static DireccionVO ReverseGeocoding(double latitud, double longitud)
        {
            try
            {
                var fullUrl = string.Format(ReverseGeocodingUrl, latitud.ToString(CultureInfo.InvariantCulture),
                                            longitud.ToString(CultureInfo.InvariantCulture));
                var xml = new XmlDocument();
                xml.Load(fullUrl);
                var status = xml.GetElementsByTagName("status").Cast<XmlNode>().FirstOrDefault();
                if(status == null || status.InnerText != "OK")
                {
                    return null;
                }
                var result = xml.GetElementsByTagName("result").Cast<XmlNode>().FirstOrDefault();
                if (result == null)
                {
                    return null;
                }

                var addressComponent = result.ChildNodes.Cast<XmlNode>().FirstOrDefault(t => t.Name == "address_component");
                var longName = addressComponent != null ? addressComponent.ChildNodes.Cast<XmlNode>().FirstOrDefault(t => t.Name == "long_name").InnerText : string.Empty;

                var partidoNode = xml.GetElementsByTagName("result").Cast<XmlNode>().FirstOrDefault(t => t.ChildNodes.Cast<XmlNode>().FirstOrDefault(y => y.Name == "type").InnerText == "administrative_area_level_2");
                var addressComponentPartido = partidoNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(t => t.Name == "address_component");
                var partido = addressComponentPartido != null ? addressComponentPartido.ChildNodes.Cast<XmlNode>().FirstOrDefault(t => t.Name == "long_name").InnerText : string.Empty;

                var provinciaNode = xml.GetElementsByTagName("result").Cast<XmlNode>().FirstOrDefault(t => t.ChildNodes.Cast<XmlNode>().FirstOrDefault(y => y.Name == "type").InnerText == "administrative_area_level_1");
                var addressComponentProvincia = provinciaNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(t => t.Name == "address_component");
                var provincia = addressComponentProvincia != null ? addressComponentProvincia.ChildNodes.Cast<XmlNode>().FirstOrDefault(t => t.Name == "long_name").InnerText : string.Empty;

                var direccion = new DireccionVO
                                    {
                                        Direccion =
                                            result.ChildNodes.Cast<XmlNode>().First(t => t.Name == "formatted_address").
                                            InnerText,
                                        Altura = -1,
                                        Calle = longName,
                                        IdEsquina = -1,
                                        IdMapaUrbano = -1,
                                        IdPoligonal = -1,
                                        IdProvincia = -1,
                                        Latitud = latitud,
                                        Longitud = longitud,
                                        Partido = partido,
                                        Provincia = provincia
                                    };

                return direccion;
            }
            catch
            {
                return null;
            }
        }
    }
}
