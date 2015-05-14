using System;
using System.Linq;
using System.Xml;
using System.Globalization;

namespace Logictracker.Interfaces.OrbComm
{
    public static class OrbcommExtenders
    {
		public static String GetValue(this XmlNode node, String nodeName)
        {
            var child = node.ChildNodes.OfType<XmlNode>().FirstOrDefault(n => n.Name.ToUpper() == nodeName.ToUpper());
			return child != null ? child.InnerText : String.Empty;    
        }
		public static int GetInt32(this XmlNode node, String nodeName)
        {
            var value = GetValue(node, nodeName);
			if (String.IsNullOrEmpty(value)) return 0;
            int i;
            int.TryParse(value, out i);
            return i;
        }
		public static DateTime GetDateTime(this XmlNode node, String nodeName)
        {
            var value = GetValue(node, nodeName);
			if (String.IsNullOrEmpty(value)) return DateTime.MinValue;
            DateTime dt;
            DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dt);
            return dt;
        }
    }
}
