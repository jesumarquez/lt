using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Logictracker.Utils
{
    public static class XmlUtil
    {
        public static string ConvertToXml(object item)
        {
            var xmlser = new XmlSerializer(item.GetType());
            using (var ms = new MemoryStream())
            {
                xmlser.Serialize(ms, item);
                var textconverter = new UTF8Encoding();
                return textconverter.GetString(ms.ToArray());
            }
        }

        public static T FromXml<T>(string xml)
        {
            var xmlser = new XmlSerializer(typeof(T));
            using (var sr = new StringReader(xml))
            {
                return (T)xmlser.Deserialize(sr);
            }
        }

    }
}
