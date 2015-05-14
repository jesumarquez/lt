#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.XSynq.Storage
{
    public class XSynqDocument
    {
        private static readonly AutoResetEvent StaticSyncro = new AutoResetEvent(true);
        private static readonly Dictionary<string, XSynqDocument> Documents;

        /// <summary>
        /// Construye la parte estatica de XSynqDocument
        /// </summary>
        static XSynqDocument()
        {
            Documents = new Dictionary<string, XSynqDocument>();
        }

        /// <summary>
        /// Publica un documento.
        /// </summary>
        /// <param name="Name">Nombre que identificara al documento para obtenerlo.</param>
        /// <param name="FilePath">Path y Nombre de archivo del documento XML a publicar.</param>
        /// <returns>verdadero si se publico con exito, false sino.</returns>
        public static bool Publish(string Name, string FilePath)
        {
            StaticSyncro.WaitOne();
            try
            {
                var key = Name.ToLower();
                STrace.Debug(typeof(XSynqDocument).FullName, String.Format("XSynqDocument.Publish('{0}','{1}')", key, FilePath));
                var xSynqDocument = new XSynqDocument
                                        {
                                            FilePath = FilePath,
                                            FileName = Path.GetFileName(FilePath),
                                            Name = key,
                                            XDocument = XDocument.Load(FilePath)
                                        };
                Documents.Add(key, xSynqDocument);
                return true;
            }
            catch(Exception e)
            {
				STrace.Exception(typeof(XSynqDocument).FullName, e, "XSynqDocument.Publish");
                return false;
            }
            finally
            {
                StaticSyncro.Set();
            }
        }

        public static XSynqDocument GetSynqDocument(string Name)
        {
            StaticSyncro.WaitOne();
            try
            {
                Name = Name.ToLower();
                return !Documents.ContainsKey(Name) ? null : Documents[Name];
            }
            finally
            {
                StaticSyncro.Set();
            }
        }

        private readonly AutoResetEvent Syncro = new AutoResetEvent(true);
        public string FilePath;
        public string FileName;
        public string Name;
        public XDocument XDocument;

        /// <summary>
        /// Salva el documento y opcionalmente realiza una copia de seguridad
        /// del mismo. 
        /// </summary>
        /// <param name="backup">Si se indica verdadero, realiza una copia de seguridad
        /// en la misma carpeta del documento cambiando la extension del mismo a ".bak"</param>
        /// <returns>Verdadero si tubo exito, falso si hubo un error.</returns>
        public bool Save(bool backup)
        {
            Syncro.WaitOne();
            try
            {
                STrace.Debug(typeof(XSynqDocument).FullName, String.Format("XSynqDocument.Save('{0}','{1}')", FileName, FilePath));
                if (backup)
                {
                    var BackFilePath = Path.ChangeExtension(FilePath, ".bak");
                    File.Copy(FilePath, BackFilePath, true);
                }
                var TFilePath = Path.ChangeExtension(FilePath, ".tmp.xml");
                XDocument.Save(TFilePath);
                return true;
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(XSynqDocument).FullName, e, "XSynqDocument.Save");
                return false;
            }
            finally
            {
                Syncro.Set();
            }
        }

        public void ApplyChangeSet(XDocument ChangeSet)
        {
            Debug.Assert(ChangeSet.Root != null && ChangeSet.Root != null);
            Syncro.WaitOne();
            ApplyChangeSet("XSynqDocument", ChangeSet, XDocument, FileName, null, false);
            Syncro.Set();
            Save(true);
        }

        /// <summary>
        /// Aplica un ChangeSet a un XDocument filtrando novedades originadas
        /// en Identifier y filtrando elementos y propiedades segun las reglas
        /// especificadas.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="xChangeset">ChangeSet a aplicar.</param>
        /// <param name="xDocument">XDocument destino.</param>
        /// <param name="Identifier"></param>
        /// <param name="Rules"></param>
        /// <param name="IgnoreIfListed"></param>
        public static void ApplyChangeSet(string Source, XDocument xChangeset, XDocument xDocument, string Identifier, List<string> Rules, bool IgnoreIfListed)
        {
            var nav = xDocument.CreateNavigator();
            Debug.Assert(xChangeset.Root != null);
            var changes=0;
            var matches=0;
            var loopbacks=0;
            var rule=0;
            var comments=0;
            var missing=0;
            foreach (var xChange in xChangeset.Root.Elements())
            {
                changes++;
                // ReSharper disable PossibleNullReferenceException
                STrace.Debug(typeof(XSynqDocument).FullName, String.Format("CommitChangeset: src={0} op={1} xpath={2} value={3}",
                        xChange.Attribute("TrackerId").Value,
                        xChange.Name.LocalName, xChange.Attribute("XPath").Value,
                        xChange.Attribute("XType").Value == "Attribute" ? xChange.Value : "(...)"));
                // ReSharper restore PossibleNullReferenceException
                var xAction = xChange.Name.LocalName;
                var typeAttributte = xChange.Attribute("Type");
                var xPathAttributte = xChange.Attribute("XPath");
                var xTypeAttributte = xChange.Attribute("XType");
                var xSourceTracker = xChange.Attribute("TrackerId");
                var xValue = xChange.Value;
                if (xPathAttributte == null || xTypeAttributte == null
                    || typeAttributte == null || xSourceTracker == null ||
                    xSourceTracker.Value == Identifier) { loopbacks++;  continue; }

                var xPath = xPathAttributte.Value;
                var xType = xTypeAttributte.Value;
                if (xType == "Comment") { comments++;  continue; }
                var r = nav.Evaluate(xPath) as XPathNodeIterator;

                if (r == null)
                {
                    throw new Exception("Unexpected NULL Result calling XPathNavigator.Evaluate XPath=" + xPath);
                }
                var newSegment = "";
                if (!r.MoveNext())
                {
                    // si lo quire borrar y no esta, ignoro
                    if (xAction == "Remove") { missing++;  continue; }

                    var parentPath = XPathOfContainerNode(xPath, out newSegment);
                    // sacamos la arroba de los attrs..
                    newSegment = newSegment.TrimStart("@".ToCharArray());

                    r = nav.Evaluate(parentPath) as XPathNodeIterator;

                    if (r == null)
                    {
                        throw new Exception("Unexpected NULL Result calling XPathNavigator.Evaluate XPath=" + parentPath);
                    }

                    if (!r.MoveNext())
                    {
                        // Console.WriteLine("{0,-20}: Sec, No Matchea XPath={1}", xAction, parentPath);
                    }
                }


                var node = r.Current;
            	if (node != null)
            	{
            		var xattr = node.UnderlyingObject as XAttribute;
            		if (xattr != null)
            		{
            			if (EvalRules(xattr.Name, Rules, IgnoreIfListed)) { rule++;  continue; }

            			if (xAction != "Set")
            			{
            				if (xAction == "Remove") xattr.Remove();
            			}
            			else
            				xattr.SetValue(xValue);
            		}
            	}

            	if (node != null)
            	{
            		var xelem = node.UnderlyingObject as XElement;
            		if (xelem == null) { matches++;  continue; }
            		if (xType == "Attribute" && !string.IsNullOrEmpty(newSegment))
            		{
            			if (EvalRules(newSegment, Rules, IgnoreIfListed)) { rule++; continue; }

            			if (xAction != "Set")
            			{
            				if (xAction == "Remove")
            				{
            					var toremove = xelem.Attribute(newSegment);
            					if (toremove != null)
            						toremove.Remove();
            				}
            			}
            			else
            				xelem.SetAttributeValue(newSegment, xValue);
            		}
            		if (xAction != "New")
            		{
            			if (xAction == "Remove") xelem.Remove();
            		}
            		else
            		{
            			if (EvalRules(xelem.Name, Rules, IgnoreIfListed)) { rule++; continue; }

            			var addnew = XElement.Parse(xValue);
            			xelem.Add(addnew);
            		}
            	}

            	matches++;
            }

            STrace.Debug(typeof(XSynqDocument).FullName, String.Format("{0}: Changes: {1} Matches: {2} Rule: {3} Missing: {4} Loopbacks: {5} Comments:{6}", Source, changes, matches, rule, missing, loopbacks, comments));
        }

        private static string XPathOfContainerNode(string xPath, out string segment)
        {
            segment = "";
            var parentEndpos = xPath.LastIndexOf('/');
            if (parentEndpos == -1) return xPath;
            segment = xPath.Substring(parentEndpos + 1);
            return xPath.Substring(0, parentEndpos);
        }

        public static bool EvalRules(XName xName, ICollection<string> RulesList, bool IgnoreIfListed)
        {
            if (xName == null) return !IgnoreIfListed;
            if (RulesList == null) return false;
            bool result;
            var Listed = RulesList.Contains(xName.LocalName);
            if (Listed && IgnoreIfListed) result = true;
            else if (Listed) result = false;
            else if (IgnoreIfListed) result = false;
            else 
                result = true;

            STrace.Debug(typeof(XSynqDocument).FullName, String.Format("EvalRules: xName={0} IgnoreIfListed={1} Listed={2} Result={3}", xName, IgnoreIfListed, Listed, result));
            return result;
        }

    }
}