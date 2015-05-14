#region Usings

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.XSynq.Storage;
using Logictracker.XSynq.Utilities;

#endregion

namespace Logictracker.XSynq.Behaviors
{
    /// <summary>
    /// Objeto satelite o suscriptor en la arquitectura de replicacion 
    /// para XLinq (XML).
    /// </summary>
    /// <remarks>
    /// Encargado de preparar y enviar como conjuntos, todas las novedades 
    /// originadas localmente, y tambien de recibir y procesar 
    /// (aplicarlas en el documento XML) los conjuntos de novedades 
    /// provenientes del Publisher, consolidando el documento "esclavo" que
    /// se utiliza localmente.
    /// </remarks>
    public class XSynqSubscriberContext : XSynqPublisherCallback
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly AutoResetEvent Syncro = new AutoResetEvent(true);

        //private const int TRACE = 10;
        private readonly XDocument Changelog;
        private readonly List<string> RulesList = new List<string>();
        private bool WhenSetIgnoreListed;
        private bool WhenLogIgnoreListed;
        private readonly XSynqSubscriber XSynqSubscriber;

        public XSynqSubscriberContext(XSynqSubscriber xSynqSubscriber)
        {
            XSynqSubscriber = xSynqSubscriber;
            Changelog = new XDocument(new XComment("Auto-Generated XDocumentSubscriber Log File, don't edit."),
                                      new XElement("changeset"));
            WhenSetIgnoreListed = true;
            WhenLogIgnoreListed = true;
            XSynqSubscriber.XDocument.Changed += XObjectChangedHandler;
            XSynqSubscriber.XDocument.Changing += XObjectChangingHandler;
        }

        private string logRefCache;
        private string LogRef
        {
            get
            {
                if (string.IsNullOrEmpty(logRefCache))
                    logRefCache = "XSynqSubscriberContext[" + XSynqSubscriber.Session.ClientName + "]";
                return logRefCache;
            }
        }

        public int Pendings
        {
            get
            {
                Debug.Assert(Changelog != null && Changelog.Root != null);
                return Changelog.Root.Elements().Count();
            }
        }

        public void AppendRules(IEnumerable<string> names, bool whenSetIgnoreListed, bool whenLogIgnoreListed)
        {
            WhenSetIgnoreListed = whenSetIgnoreListed;
            WhenLogIgnoreListed = whenLogIgnoreListed;
            RulesList.AddRange(names);
        }
        
        const int NormalProcessing = 1;
        const int WaitForAdd = 2;
        XElement newobject;
        int State = NormalProcessing;

        private void XObjectChangingHandler(object sender, XObjectChangeEventArgs e)
        {
            STrace.Debug(GetType().FullName, LogRef + ".XObjectChangingHandler");
            Syncro.WaitOne();
            try
            {
                Debug.Assert(Changelog != null);
                Debug.Assert(Changelog.Root != null);

                var xSource = sender as XObject;
                if (xSource == null) return; 
                if (e.ObjectChange == XObjectChange.Value ||
                    e.ObjectChange == XObjectChange.Name) return;
                var xAction = e.ObjectChange.ToString();

                XName xName = null;
                if (xSource as XAttribute != null) xName = (xSource as XAttribute).Name;
                if (xSource as XElement != null) xName = (xSource as XElement).Name;
                if (XSynqDocument.EvalRules(xName, RulesList, WhenLogIgnoreListed)) return;
                
                if (e.ObjectChange == XObjectChange.Add &&
                    xSource.NodeType == XmlNodeType.Element)
                {
                    xAction = "New";
                    newobject = new XElement(xAction);
                    newobject.SetAttributeValue("Type", sender.GetType().ToString());
                    newobject.SetAttributeValue("XType", xSource.NodeType.ToString());
                    newobject.SetAttributeValue("TrackerId", XSynqSubscriber.Session.Identifier);
                    newobject.Add(new XCData(xSource.ToString()));
                    State = WaitForAdd;
                    return; 
                }
                if (e.ObjectChange == XObjectChange.Add) return;
                if (!xSource.GetXPath().StartsWith(XSynqSubscriber.Session.SubscriberBranch)) return;
                var x = new XElement(xAction);
                x.SetAttributeValue("Type", sender.GetType().ToString());
                x.SetAttributeValue("XPath", xSource.GetXPath().Replace(XSynqSubscriber.Session.SubscriberBranch, XSynqSubscriber.Session.PublisherBranch));
                x.SetAttributeValue("XType", xSource.NodeType.ToString());
                x.SetAttributeValue("TrackerId", XSynqSubscriber.Session.Identifier);
                x.Add(XHelper.GetValueAsCData(xSource));
                AppendChangelog(x);
            }
            finally
            {
                Syncro.Set();    
            }
        }

        private void XObjectChangedHandler(object sender, XObjectChangeEventArgs e)
        {
            STrace.Debug(GetType().FullName, LogRef + ".XObjectChangedHandler");
            Syncro.WaitOne();
            try
            {
                Debug.Assert(Changelog != null);
                Debug.Assert(Changelog.Root != null);

                var xSource = sender as XObject;
                if (xSource == null) return;

                XName xName = null;
                if (xSource as XAttribute != null) xName = (xSource as XAttribute).Name;
                if (xSource as XElement != null) xName = (xSource as XElement).Name;
                if (XSynqDocument.EvalRules(xName, RulesList, WhenLogIgnoreListed)) return;
       
                if (e.ObjectChange == XObjectChange.Add &&
                    State == WaitForAdd
                    && newobject != null)
                {
                    if (!xSource.GetXPath().StartsWith(XSynqSubscriber.Session.SubscriberBranch))
                    {
                        State = NormalProcessing;
                        newobject = null;
                        return;
                    }
                    newobject.SetAttributeValue("XPath", xSource.GetXPath().Replace(XSynqSubscriber.Session.SubscriberBranch, XSynqSubscriber.Session.PublisherBranch));
                    AppendChangelog(newobject);
                    State = NormalProcessing;
                    newobject = null;
                    return;
                }
                if (e.ObjectChange == XObjectChange.Remove ||
                    e.ObjectChange == XObjectChange.Name) return;
                var xType = xSource.NodeType.ToString();
                var xAction = e.ObjectChange.ToString();
                if (!xSource.GetXPath().StartsWith(XSynqSubscriber.Session.SubscriberBranch)) return;
                if (xType == "Attribute") xAction = "Set";
                var x = new XElement(xAction);
                x.SetAttributeValue("Type", sender.GetType().ToString());
                x.SetAttributeValue("XPath", xSource.GetXPath().Replace(XSynqSubscriber.Session.SubscriberBranch, XSynqSubscriber.Session.PublisherBranch));
                x.SetAttributeValue("XType", xType);
                x.SetAttributeValue("TrackerId", XSynqSubscriber.Session.Identifier);
                x.Add(XHelper.GetValueAsCData(xSource));
                AppendChangelog(x);
            }
            finally
            {
                Syncro.Set();    
            }
        }

        private void AppendChangelog(XElement xChange)
        {
            // ReSharper disable PossibleNullReferenceException
            //STrace.Trace(GetType().FullName,TRACE, LogRef + ".AppendChangeLog: src={0} op={1} xpath={2} value={3}", xChange.Attribute("TrackerId").Value,
            //        xChange.Name.LocalName, xChange.Attribute("XPath").Value, xChange.Attribute("XType").Value == "Attribute" ? xChange.Value : "(...)");
            // ReSharper restore PossibleNullReferenceException
            if (Changelog.Root != null) Changelog.Root.Add(xChange);
            PropertyChanged(this, new PropertyChangedEventArgs("Pendings"));
        }

        /// <summary>
        /// Fuerza al suscriptor a descartar su copia local y las novedades pentientes
        /// que tenga. 
        /// </summary>
        /// <remarks>
        /// Este metodo es llamado por el publisher al iniciar la suscripcion para 
        /// establecer la copia local del suscriptor.
        /// </remarks>
        public void Reset(string xDocument)
        {
            STrace.Debug(GetType().FullName, LogRef + ".XSynqSubscriberContext.Reset");
            Syncro.WaitOne();

            XSynqSubscriber.XDocument.Changed -= XObjectChangedHandler;
            XSynqSubscriber.XDocument.Changing -= XObjectChangingHandler;

            var xDoc = XDocument.Parse(xDocument);
            Debug.Assert(xDoc != null && xDoc.Root != null);
            foreach(var a in xDoc.Root.Attributes())
            {
                if (a.Name == "id") continue;
                XSynqSubscriber.Branch.SetAttributeValue(a.Name, a.Value);
            }
            XSynqSubscriber.Branch.ReplaceNodes(xDoc.Root.Elements());
 
            XSynqSubscriber.XDocument.Changed += XObjectChangedHandler;
            XSynqSubscriber.XDocument.Changing += XObjectChangingHandler;

            Syncro.Set();

            XSynqSubscriber.DoSubscriptionReset();
        }

        /// <summary>
        /// Obtiene el conjunto de novedades pendientes hacia arriba.
        /// </summary>
        /// <returns>un ChangeSet con todas las novedades.</returns>
        public string GetChangeSet()
        {
            Debug.Assert(Changelog != null && Changelog.Root != null);
            Syncro.WaitOne();
            var cs = Changelog.ToString();
        	if (Changelog.Root != null) Changelog.Root.RemoveAll();
        	Syncro.Set();
            return cs;
        }

        /// <summary>
        /// Consolida el ChangeSet dado en el documento local.
        /// </summary>
        /// <param name="ChangeSet">ChangeSet a aplicarse sobre le documento local.</param>
        public void ApplyChangeSet(string ChangeSet)
        {
            Syncro.WaitOne();

            XSynqSubscriber.XDocument.Changed -= XObjectChangedHandler;
            XSynqSubscriber.XDocument.Changing -= XObjectChangingHandler;

            XSynqDocument.ApplyChangeSet(LogRef, XDocument.Parse(ChangeSet), XSynqSubscriber.XDocument, XSynqSubscriber.Session.Identifier, RulesList, WhenSetIgnoreListed);

            XSynqSubscriber.XDocument.Changed += XObjectChangedHandler;
            XSynqSubscriber.XDocument.Changing += XObjectChangingHandler;

            Syncro.Set();
        }

        
    }
}