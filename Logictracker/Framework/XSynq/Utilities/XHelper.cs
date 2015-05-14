#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

#endregion

namespace Logictracker.XSynq.Utilities
{
    public static class XHelper
    {
        private static string GetQName(XElement xe)
        {
            var prefix = xe.GetPrefixOfNamespace(xe.Name.Namespace);
            if (xe.Name.Namespace == XNamespace.None || prefix == null) return xe.Name.LocalName;
            return string.Format("{0}:{1}", prefix, xe.Name.LocalName);
        }

        private static string GetQName(XAttribute xa)
        {
            if (xa.Parent == null) return "";
            var prefix = xa.Parent.GetPrefixOfNamespace(xa.Name.Namespace);

            if (xa.Name.Namespace == XNamespace.None || prefix == null)
                return xa.Name.ToString();

            return string.Format("{0}:{1}", prefix, xa.Name.LocalName);
        }

        private static string NameWithPredicate(XElement el)
        {
            if (el.Parent != null)
            {
                var idattr = el.Attribute("id");
            	var ix = idattr != null ? string.Format("@id = \"{0}\"", idattr.Value) : (el.ElementsBeforeSelf(el.Name).Count() + 1).ToString();
                return string.Format("{0}[{1}]", GetQName(el), ix);
            }
            return GetQName(el);
        }

        private static string StrCat<Type2>(this IEnumerable<Type2> source, string separator)
        {
            return source.Aggregate(new StringBuilder(), (sb, i) => sb.Append(i.ToString()).Append(separator), s => s.ToString());
        }

        public static string GetXPath(this XObject xobj)
        {
            if (xobj.Parent == null)
            {
                var doc = xobj as XDocument;
                if (doc != null) return ".";

                var el = xobj as XElement;
                if (el != null)
                    return "/" + NameWithPredicate(el);

                var xt = xobj as XText;
                if (xt != null)
                    return null;

                var com = xobj as XComment;
                if ((com != null) && (com.Document != null))
					return string.Format("/{0}", com.Document.Nodes().OfType<XComment>().Count() != 1 ? string.Format("comment()[{0}]", (com.NodesBeforeSelf().OfType<XComment>().Count() + 1)) : "comment()");

                var pi = xobj as XProcessingInstruction;

                if (pi != null && pi.Document != null)
                    return string.Format("/{0}", (pi.Document.Nodes().OfType<XProcessingInstruction>().Count() != 1 ? string.Format("processing-instruction()[{0}]", (pi.NodesBeforeSelf().OfType<XProcessingInstruction>().Count() + 1)) : "processing-instruction()"));

                return null;
            }
            else // Parent == null
            {
                var el = xobj as XElement;
                if (el != null)
                {
					return String.Format("/{0}{1}", el.Ancestors().InDocumentOrder().Select(e => NameWithPredicate(e)).StrCat("/"),NameWithPredicate(el));
				}

                var at = xobj as XAttribute;
                if (at != null && at.Parent != null)
					return String.Format("/{0}@{1}", at.Parent.AncestorsAndSelf().InDocumentOrder().Select(e => NameWithPredicate(e)).StrCat("/"), GetQName(at));

                var com = xobj as XComment;
                if (com != null && com.Parent != null)
					return String.Format("/{0}{1}", com.Parent.AncestorsAndSelf().InDocumentOrder().Select(e => NameWithPredicate(e)).StrCat("/"), com.Parent.Nodes().OfType<XComment>().Count() == 1 ? "comment()" : String.Format("comment()[{0}]", com.NodesBeforeSelf().OfType<XComment>().Count() + 1));

                var cd = xobj as XCData;
                if (cd != null && cd.Parent != null)
					return String.Format("/{0}{1}", cd.Parent.AncestorsAndSelf().InDocumentOrder().Select(e => NameWithPredicate(e)).StrCat("/"), cd.Parent.Nodes().OfType<XText>().Count() == 1 ? "text()" : String.Format("text()[{0}]", (cd.NodesBeforeSelf().OfType<XText>().Count() + 1)));

                var tx = xobj as XText;
                if (tx != null && tx.Parent != null)
                    return String.Format("/{0}{1}", tx.Parent.AncestorsAndSelf().InDocumentOrder().Select(e => NameWithPredicate(e)).StrCat("/"), tx.Parent.Nodes().OfType<XText>().Count() == 1 ? "text()" : String.Format("text()[{0}]", (tx.NodesBeforeSelf().OfType<XText>().Count() + 1)));

                var pi = xobj as XProcessingInstruction;
                if (pi != null && pi.Parent != null)
                    return String.Format("/{0}{1}", pi.Parent.AncestorsAndSelf().InDocumentOrder().Select(e => NameWithPredicate(e)).StrCat("/"), pi.Parent.Nodes().OfType<XProcessingInstruction>().Count() == 1 ? "processing-instruction()" : String.Format("processing-instruction()[{0}]", (pi.NodesBeforeSelf().OfType<XProcessingInstruction>().Count() + 1)));

                return null;
            }
        }

        public static XCData GetValueAsCData(XObject source)
        {
            var xa = source as XAttribute;
            if (xa != null)
            {
                return new XCData(xa.Value);
            }

            var xe = source as XElement;
            if (xe != null)
            {
                return new XCData(xe.Value);
            }

            var xt = source as XText;
            if (xt != null)
            {
                return new XCData(xt.Value);
            }

            var xc = source as XComment;
            if (xc != null)
            {
                return new XCData(xc.Value);
            }

            return new XCData("type error");
        }
    }
}