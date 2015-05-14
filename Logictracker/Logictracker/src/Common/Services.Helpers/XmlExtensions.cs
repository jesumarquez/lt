using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Logictracker.Services.Helpers
{
    internal static class XmlExtensions
    {
        public static string GetInnerText(this XmlNode node, string nodeName)
        {
            if (node == null) return null;
            var child = node.ChildNodes.Cast<XmlNode>().FirstOrDefault(t => t.Name == nodeName);
            return child != null ? child.InnerText : null;
        }

        public static XmlNode GetXmlNode(this XmlDocument xml, string nodeName)
        {
            return xml.GetElementsByTagName(nodeName).Cast<XmlNode>().FirstOrDefault();
        }
        public static XmlNode GetChild(this XmlNode xml, string nodeName)
        {
            return GetChilds(xml, nodeName).FirstOrDefault();
        }
        public static IEnumerable<XmlNode> GetChilds(this XmlNode xml, string nodeName)
        {
            return xml.ChildNodes.Cast<XmlNode>().Where(t => t.Name == nodeName);
        }
    }
}
