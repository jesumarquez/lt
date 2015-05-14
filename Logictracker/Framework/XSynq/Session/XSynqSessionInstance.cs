#region Usings

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using Logictracker.DatabaseTracer.Core;
using Logictracker.XSynq.Contracts;
using Logictracker.XSynq.Storage;
using Rnr.XSynq.Properties;

#endregion

namespace Logictracker.XSynq.Session
{
    public class XSynqSessionInstance
    {
        private readonly AutoResetEvent Syncro = new AutoResetEvent(true);
        private bool ResetDone;
        private readonly Timer Background;
        public XDocument PendingChangelog;
        public XSynqDocument XSynqDocument;
        public XSynqSession Session;
        public IXSynqPublisherCallback CallbackSubscriber;

    	public XSynqSessionInstance()
        {
            STrace.Debug(GetType().FullName,"XSynqSessionInstance()");
            Background = new Timer(PeridicUpdate, this, StaticConfiguration.SessionPeridicUpdate, Timeout.Infinite);
        }

        private void PeridicUpdate(object state)
        {
            STrace.Debug(GetType().FullName, "XSynqSessionInstance.PeriodicUpdate");
            if (!Syncro.WaitOne(0))
            {
                STrace.Debug(GetType().FullName, "XSynqSessionInstance.PeriodicUpdate.WaitHandleLocked");
                Background.Change(StaticConfiguration.SessionPeridicUpdate, Timeout.Infinite);
                return;
            }
            if (!ResetDone)
            {
                try
                {
                    STrace.Debug(GetType().FullName, "XSynqSessionInstance.PeriodicUpdate.Reseting");
                    var source = XDocument.Parse(XSynqDocument.XDocument.ToString());
                    var nav = source.CreateNavigator();
                    
                    var node = nav.Evaluate(Session.PublisherBranch) as XPathNodeIterator;
                    if (node == null || !node.MoveNext())
                        throw new Exception("Unexpected Result calling XPathNavigator.Evaluate XPath=" +
                                            Session.PublisherBranch);

                	if (node.Current != null)
                	{
                		var xRoot = node.Current.UnderlyingObject as XElement;
                		if (xRoot == null)
                			throw new Exception("Unexpected XPathNavigator.Current for XPath=" + Session.PublisherBranch);

                		CallbackSubscriber.Reset(xRoot.ToString());
                		ResetDone = true;
						Background.Change(StaticConfiguration.SessionPeridicUpdate, Timeout.Infinite);
                	}
                	else
                	{
						STrace.Debug(GetType().FullName,"node.Current == null");
                	}
                }
                finally
                {
                    Syncro.Set();
                }
                return;
            }
            XDocument pendings = null;
            try
            {
                STrace.Debug(GetType().FullName, "XSynqSessionInstance.PeriodicUpdate.HasPendings");
                if (HasPendings)
                {
                    if (PendingChangelog == null || PendingChangelog.Root == null) return;
                    STrace.Debug(GetType().FullName, String.Format("XSynqSessionInstance.PeriodicUpdate.DequeueAllPendings Pendings: {0}", PendingChangelog.Root.Elements().Count()));
                    pendings = DequeueAllPendings();
                }
            }
            catch(Exception e)
            {
                STrace.Exception(GetType().FullName,e,"XSynqSessionInstance.PeriodicUpdate");
            }
            finally
            {
                Syncro.Set();
                STrace.Debug(GetType().FullName, "XSynqSessionInstance.PeriodicUpdate.Finally");
                try
                {
                    if (pendings != null)
                    {
                        STrace.Debug(GetType().FullName, "XSynqSessionInstance.PeriodicUpdate.ApplyChangeSet");
                        CallbackSubscriber.ApplyChangeSet(pendings.ToString());
                    }
                } catch(Exception e)
                {
                    STrace.Exception(GetType().FullName,e, "XSynqSessionInstance.PeriodicUpdate.ApplyPendings");    
                }
                STrace.Debug(GetType().FullName, "XSynqSessionInstance.PeriodicUpdate.ChangeTimer");
                Background.Change(StaticConfiguration.SessionPeridicUpdate, Timeout.Infinite);
            }
        }

        private bool HasPendings
        {
            get
            {
                if (PendingChangelog == null || PendingChangelog.Root == null) return false;
                return PendingChangelog.Root.HasElements;
            }
        }

        private XDocument DequeueAllPendings()
        {
            if (!HasPendings) return null;
            var Result = PendingChangelog;
            PendingChangelog = new XDocument(new XComment("Auto-Generated ChangeSet Tracking File, don't edit."),
                                             new XElement("changeset"));
            return Result;
        }

        public void EnqueueChangeSet(string sourceSessionId, XDocument ChangeSet)
        {
            if (Session.Identifier == sourceSessionId) return;
            Debug.Assert(PendingChangelog.Root != null && ChangeSet.Root != null);
            if (!ChangeSet.Root.HasElements) return;

            var workingCopy = new XElement(ChangeSet.Root);
            var removes = 0;
            var misstrans = 0;
            var count = 0;
            foreach(var xElement in workingCopy.Elements())
            {
                var xPath = xElement.Attribute("XPath");
                Debug.Assert(xPath != null);
                var absoluteXPath = xPath.Value;
                if (!absoluteXPath.StartsWith(Session.PublisherBranch))
                {
                    xElement.Remove();
                    //STrace.Trace(GetType().FullName,"EnqueueChangeset[{0}] REMOVE XPath={1} LocalBranch={2} RemoteBranch={3}", Session.ClientName, absoluteXPath, Session.PublisherBranch, Session.SubscriberBranch);
                    removes ++;
                    continue;
                }
                var subscriberXPath = absoluteXPath.Replace(Session.PublisherBranch, Session.SubscriberBranch);
                if (subscriberXPath == absoluteXPath && Session.PublisherBranch != Session.SubscriberBranch)
                {
                    STrace.Debug(GetType().FullName, String.Format("EnqueueChangeset[{0}], fallo traslado de XPath={1} LocalBranch={2} RemoteBranch={3}", Session.ClientName, absoluteXPath, Session.PublisherBranch, Session.SubscriberBranch));
                    misstrans++;
                }

                //STrace.Trace(GetType().FullName,"EnqueueChangeset[{0}] ENQUEUE XPath={1} LocalBranch={2} RemoteBranch={3}", Session.ClientName, absoluteXPath, Session.PublisherBranch, Session.SubscriberBranch);
                
                count++;
                xElement.SetAttributeValue("XPath",subscriberXPath);
            }
            STrace.Debug(GetType().FullName, String.Format("EnqueueChangeset[{0}], Count={1} Remove={2} TranslationErrors={3}",Session.ClientName, count, removes, misstrans));
            if (!workingCopy.HasElements) return;
            Syncro.WaitOne();
            PendingChangelog.Root.Add(workingCopy.Elements());
            Syncro.Set();
            Background.Change(StaticConfiguration.SessionPeridicUpdate, Timeout.Infinite);
        }
    }
}