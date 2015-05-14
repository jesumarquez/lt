#region Usings

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using Logictracker.DatabaseTracer.Core;
using Logictracker.XSynq.Session;
using Rnr.XSynq.Properties;

#endregion

namespace Logictracker.XSynq.Behaviors
{
    public class XSynqSubscriber
    {
        public enum ConnctionStates 
        {
            Init,           // Cuando se crea la clase.
            Trying,         // Intentando Suscribir
            WaitingReset,   // Esperando Reset (foto inicial)
            SynqActive,     // Conectado, Intercambiando novedades.
            TimeWait        // Conexion perdida.
        }

        private delegate void PendingsNotifyDelegate(int Count);
        public delegate void EventDelegate(XSynqSubscriber source);
        
        public event EventDelegate SubscriptionReady;
        public event EventDelegate SubscriptionLost;
        public event EventDelegate SubscriptionReset;

        private Timer PendingsNotifier;
        private InstanceContext InstanceContext;
        public XSynqPublisherClient Client;
        public XSynqSession Session;
        internal readonly XDocument XDocument;

        public XSynqSubscriberContext Context;
        public ConnctionStates ConnectionState { get; private set; }

        public XSynqSubscriber()
        {
            ConnectionState = ConnctionStates.Init;
        }

        public XSynqSubscriber(XDocument xDocument, XSynqSession session, Uri uri)
        {
            ConnectionState = ConnctionStates.Init;
            
            XDocument = xDocument;
            
            Provisione(session, uri);
            Subscribe();
        }

        public XSynqSubscriber(XDocument xDocument, XSynqSession session, Uri uri, int BufferSize)
        {
            ConnectionState = ConnctionStates.Init;
            XDocument = xDocument;

            var binding = new NetTcpBinding
                          	{
                          		Name = "NetTcpBinding_Client",
                          		SendTimeout = new TimeSpan(0, 5, 0),
                          		TransferMode = TransferMode.Buffered,
                          		MaxBufferPoolSize = BufferSize,
                          		MaxBufferSize = BufferSize,
                          		MaxReceivedMessageSize = BufferSize,
                          		ReaderQuotas = {MaxStringContentLength = BufferSize}
                          	};

        	Provisione(session, binding, uri);
            Subscribe();
        }
        
        private void Provisione(XSynqSession session, Uri uri)
        {
            PendingsNotifier = new Timer(XSynqSubscriberContextPendingsNotify, null, StaticConfiguration.DocumentPeridicUpdate, Timeout.Infinite);
            Session = session;
            Context = new XSynqSubscriberContext(this);
            Context.PropertyChanged += XSynqSubscriberContextPropertyChanged;
            InstanceContext = new InstanceContext(Context);
            Client = new XSynqPublisherClient(InstanceContext, "AgentEndPoint", uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped));
        }

        private void Provisione(XSynqSession session, System.ServiceModel.Channels.Binding Binding, Uri uri)
        {
            PendingsNotifier = new Timer(XSynqSubscriberContextPendingsNotify, null, StaticConfiguration.DocumentPeridicUpdate, Timeout.Infinite);
            Session = session;
            Context = new XSynqSubscriberContext(this);
            Context.PropertyChanged += XSynqSubscriberContextPropertyChanged;
            InstanceContext = new InstanceContext(Context);
            Client = new XSynqPublisherClient(InstanceContext, Binding, new EndpointAddress(uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped)));
        }

        private delegate void SubscribeDelegate();
        
        private void Subscribe()
        {
            if (ConnectionState != ConnctionStates.Init) 
                throw new InvalidOperationException("Estado incorrecto, no puede suscribir.");
            ConnectionState = ConnctionStates.Trying;
            var d = new SubscribeDelegate(AsyncSubscribe);
            d.BeginInvoke(AsyncSubscribeComplete, d);
        }

        public void Abort()
        {
            Client.Abort();
        }

        private void AsyncSubscribeComplete(IAsyncResult ar)
        {
            try
            {
                var d = ar.AsyncState as SubscribeDelegate;
                if (d == null) throw new ApplicationException("No se recibio el delagado asincronico al recibir el callback.");
                d.EndInvoke(ar);
                DoSubscriptionReady();
            } catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e,"AsyncSubscribeComplete");
                DoSubscriptionLost();
            }
        }

        private void AsyncSubscribe()
        {
            try
            {
                Session.Identifier = Client.Subscribe(Session.DocumentName, Session.ClientName, Session.PublisherBranch,
                                                  Session.SubscriberBranch, Session.BranchAuthority);
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e, "AsyncSubscribe");
                DoSubscriptionLost();
            }
        }


        private void XSynqSubscriberContextPendingsNotify(object sender)
        {
            try
            {
                STrace.Debug(GetType().FullName, "XSynqSubscriber.XSynqSubscriberContextPendingsNotify");
                if (Context.Pendings == 0)
                {
                    PendingsNotifier.Change(StaticConfiguration.DocumentPeridicUpdate, Timeout.Infinite);
                    return;
                }
                STrace.Debug(GetType().FullName, "void XSynqSubscriberContextPendingsNotify(..)");
                var caller = new PendingsNotifyDelegate(Client.PendigsNotify);
                caller.BeginInvoke(Context.Pendings, PendingsNotifyComplete, caller);
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e, "XSynqSubscriberContextPendingsNotify");
                DoSubscriptionLost();
            }
        }

        private void XSynqSubscriberContextPropertyChanged(object sender, PropertyChangedEventArgs evt)
        {
            try
            {
                STrace.Debug(GetType().FullName, "XSynqSubscriber.XSynqSubscriberContextPropertyChanged");
                if (!evt.PropertyName.Equals("Pendings")) return;
                STrace.Debug(GetType().FullName, "void XSynqSubscriberContextPropertyChanged(..)");
                PendingsNotifier.Change(StaticConfiguration.DocumentPeridicUpdate, Timeout.Infinite);
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e, "XSynqSubscriberContextPropertyChanged");
                DoSubscriptionLost();
            }
        }

        private void PendingsNotifyComplete(IAsyncResult ar)
        {
            try
            {
                var caller = ar.AsyncState as PendingsNotifyDelegate;
                Debug.Assert(caller != null);
                caller.EndInvoke(ar);
                PendingsNotifier.Change(StaticConfiguration.DocumentPeridicUpdate, Timeout.Infinite);
            } catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e, "XSynqSubscriber.PendingsNotifyComplete");
                DoSubscriptionLost();
            }
        }

        public XElement Branch
        {
            get
            {
                try
                {
                    STrace.Debug(GetType().FullName, "XSynqSubscriber.Branch.Get");
                    var nav = XDocument.CreateNavigator();
                    var node = nav.Evaluate(Session.SubscriberBranch) as XPathNodeIterator;
                    if (node == null)
                    {
                        throw new Exception("Unexpected NULL Result calling XPathNavigator.Evaluate XPath=" +
                                            Session.SubscriberBranch);
                    }
                    if (!node.MoveNext()) return null;
                    var current = node.Current;
					if (current != null)
					{
						STrace.Debug(GetType().FullName,"XSynqSubscriber.GetBranch -> current == null");
						return current.UnderlyingObject as XElement;
					}
					return null;
                }
                catch (Exception e)
                {
                    STrace.Exception(GetType().FullName,e, "XSynqSubscriber.GetBranch");
                    return null;
                }
            }
        }

        public void DoSubscriptionReady()
        {
            try
            {
                if (ConnectionState != ConnctionStates.Trying) return;
                ConnectionState = ConnctionStates.WaitingReset;
                if (SubscriptionReady != null)
                    SubscriptionReady(this);
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e, "DoSubscriptionReady.Event");
            }
        }

        public void DoSubscriptionLost()
        {
            if (ConnectionState == ConnctionStates.TimeWait) return;
            ConnectionState = ConnctionStates.TimeWait;
            try
            {
                if (Client != null) Client.Abort();
            } catch(Exception e)
            {
                STrace.Exception(GetType().FullName,e, "DoSubscriptionLost.Client.Abort()");
            }
            try
            {
                PendingsNotifier.Change(Timeout.Infinite, Timeout.Infinite);
                PendingsNotifier = null;

                if (SubscriptionLost != null)
                    SubscriptionLost(this);
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e, "DoSubscriptionLost.Event");
            }
        }

        public void DoSubscriptionReset()
        {
            try
            {
                ConnectionState = ConnctionStates.SynqActive;
                if (SubscriptionReset != null)
                    SubscriptionReset(this);
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e, "DoSubscriptionReset.Event");
            }
        }
    }
}
