#region Usings

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Xml.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.XSynq.Contracts;
using Logictracker.XSynq.Session;
using Logictracker.XSynq.Storage;
using Logictracker.XSynq.Utilities;
using Rnr.XSynq.Properties;

#endregion

namespace Logictracker.XSynq.Behaviors
{

    [ServiceBehavior(
        IncludeExceptionDetailInFaults = BuildConfig.IncludeExceptionDetailInFaults,
        InstanceContextMode = InstanceContextMode.PerSession,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class XSynqPublisher : IXSynqPublisher
    {
    	private static readonly AutoResetEvent Syncro = new AutoResetEvent(true);
        private static readonly Dictionary<string, XSynqSessionInstance> Subscritions = new Dictionary<string, XSynqSessionInstance>();

        private static XSynqSessionInstance CurrentSession()
        {
            try{
                Syncro.WaitOne();
                if (!Subscritions.ContainsKey(OperationContext.Current.SessionId))
                    throw new Exception("No existe la session " + OperationContext.Current.SessionId);
                return Subscritions[OperationContext.Current.SessionId];
            } finally
            {
                Syncro.Set();
            }
        }

        public XSynqPublisher()
        {
            STrace.Debug(GetType().FullName, String.Format("XSynqPublisher: SessionId={0}", OperationContext.Current.SessionId));
        }

        
        /// <summary>
        /// Crea una Suscripcion a la publicacion, y opcionalmente limita la 
        /// suscripcion a una rama puntual del arbol. 
        /// </summary> 
        /// <returns>Identificador de la instancia del Subscriber.</returns>
        public string Subscribe(string DocumentName, string ClientName, string PublisherBranch, string SubscriberBranch, bool BranchOwner)
        {

            var Session = new XSynqSession
                              {
                                  Identifier = OperationContext.Current.SessionId,
                                  DocumentName = DocumentName,
                                  ClientName = ClientName,
                                  PublisherBranch = PublisherBranch,
                                  SubscriberBranch = SubscriberBranch
                              };
            try
            {
                Syncro.WaitOne();
                if (Subscritions.ContainsKey(Session.Identifier))
                {
                    Subscritions[Session.Identifier].Session = Session;
                    return Session.Identifier;
                }
            }
            finally
            {
                Syncro.Set();
            }

            var xSynqDocument = XSynqDocument.GetSynqDocument(Session.DocumentName);

            if (xSynqDocument == null)
                throw new XSynqFaultException(XSynqFaultCode.DocumentNotFound);

            var si = new XSynqSessionInstance
                         {
                             Session = Session,
                             XSynqDocument = xSynqDocument,
                             PendingChangelog = new XDocument(new XElement("changeset")),
                             CallbackSubscriber =
                                 OperationContext.Current.GetCallbackChannel<IXSynqPublisherCallback>()
                         };

            Syncro.WaitOne();
            Subscritions.Add(Session.Identifier, si);
            Syncro.Set();

            STrace.Debug(GetType().FullName, String.Format("Subscribe: client={0} SessionId={1}", CurrentSession().Session.ClientName, Session.Identifier));

            return Session.Identifier;

        }


        /// <summary>
        /// Termina la suscripcion a la publicacion. 
        /// </summary>
        /// <remarks>
        /// El publisher, sincrinizara las novedades antes de retornar y
        /// finalizar la session.
        /// </remarks>
        public void Unsubscribe()
        {
            STrace.Debug(GetType().FullName, String.Format("Unsubscribe: client={0}", CurrentSession().Session.ClientName));
        }

        /// <summary>
        /// Notifica al Publisher que hay cambios locales pendientes de 
        /// ser sincronizados hacia arriba.
        /// </summary>
        /// <param name="Count">Cantidad de Novedades Pendientes.</param>
        /// <remarks>
        /// La cantidad de novedades pendientes puede ser o no indicada e
        /// incluso puede ser aproximada. Se define para dar al publisher 
        /// la posibilidad de determinar un costo y/o programar la sincrinización.
        /// </remarks>
        public void PendigsNotify(int Count)
        {
            STrace.Debug(GetType().FullName, String.Format("PendigsNotify: Client={0} Count={1}", CurrentSession().Session.ClientName, Count));
            var cs = XDocument.Parse(CurrentSession().CallbackSubscriber.GetChangeSet());
            
            CurrentSession().XSynqDocument.ApplyChangeSet(cs);
            var id = CurrentSession().Session.Identifier;
            
            Syncro.WaitOne();
            foreach (var s in Subscritions.Values)
            {
                s.EnqueueChangeSet(id, cs);
            }
            Syncro.Set();
        }

        /// <summary>
        /// Solicita el bloqueo de una rama del arbol.
        /// </summary>
        /// <param name="BranchXPath">Camino XPath a bloquearse</param>
        /// <remarks>
        /// <para>
        /// El bloqueo se completa de forma asincronica cuando el publisher 
        /// establece el atributo "lockedby" en elemento referenciado. (Tenga 
        /// en cuenta que esta tecnica implica que no se pueden bloquear atributos)
        /// </para>
        /// <para>
        /// La confirmacion se determina si "lockedby" tiene el mismo valor que
        /// la propiedad "Identifier" del objeto <see cref="XSynqSession"/>
        /// asociado a la suscripcion. Si el bloqueo no se puede completar, 
        /// ya que habia un bloqueo previo que aun no se habia propagado al 
        /// suscriptor solicitante, "lockedby" tendra un valor que no 
        /// diferente a "Identifier" y por lo tanto el Usuario del Suscriptor
        /// debera considerar que el bloqueo fallo. 
        /// </para>
        /// <para>
        /// Durante el bloqueo la rama bloqueada no podra ser modificada por 
        /// otros suscriptores, excepto por el suscriptor propietario.
        /// El Publisher ignorara cualquier novedad recibida para una rama
        /// bloqueada salvo que provenga del propietario o del suscriptor
        /// bloqueante.
        /// </para>
        /// <para>
        /// El bloqueo es automaticamente anulado si se rompe la conexion
        /// entre el suscriptor y publisher.
        /// </para>
        /// </remarks>
        public void LockRequest(string BranchXPath)
        {
            STrace.Debug(GetType().FullName, String.Format("LockRequest: client={0} BranchXPath={1}", CurrentSession().Session.ClientName, BranchXPath));
        }

        /// <summary>
        /// Solicita el desbloqueo de una rama del arbol.
        /// </summary>
        /// <param name="BranchXPath">Camino XPath a bloquearse</param>
        /// <remarks>
        /// El desbloqueo completa de forma asincronica cuando el publisher llama
        /// por callBack al suscriptor y confirma el desbloqueo.
        /// </remarks>
        public void UnlockRequest(string BranchXPath)
        {
            STrace.Debug(GetType().FullName, String.Format("UnlockRequest: client={0} BranchXPath={1}", CurrentSession().Session.ClientName, BranchXPath));
        }

        /// <summary>
        /// Indicates that the channel has faulted.
        /// </summary>
        /// <param name="channel">The <see cref="T:System.ServiceModel.IDuplexContextChannel"/> that has faulted.</param>
        public void ChannelFaulted(IDuplexContextChannel channel)
        {
            STrace.Debug(GetType().FullName, String.Format("ChannelFaulted: state={0} client={1}", channel.State.ToString(), CurrentSession().Session.ClientName));
        }

        /// <summary>
        /// Indicates when the client channel is done receiving messages as part of the duplex message exchange.
        /// </summary>
        /// <param name="channel">The <see cref="T:System.ServiceModel.Channels.IDuplexSessionChannel"/> that is done receiving.</param>
        public void DoneReceiving(IDuplexContextChannel channel)
        {
            STrace.Debug(GetType().FullName, String.Format("DoneReceiving: state={0} client={1}", channel.State.ToString(), CurrentSession().Session.ClientName));
        }


        /*

        private readonly Uri ServiceUri;
        private readonly ServiceHost ServiceHost;
        private readonly string EndPointName;

        public XSynqPublisher(Uri ListenUri, string EndPoint, XDocumentPublisher publisher)
        {
            EndPointName = EndPoint;
            ServiceUri = ListenUri;
            
            // Step 2 of the hosting procedure: Create ServiceHost
            ServiceHost = new ServiceHost(publisher, ServiceUri);

            try
            {
                // Step 3 of the hosting procedure: Add a service endpoint.
                ServiceHost.AddServiceEndpoint(
                    typeof(IXDocumentPublisher),
                    new WSHttpBinding(),
                    EndPointName);


                // Step 4 of the hosting procedure: Enable metadata exchange.
                var smb = new ServiceMetadataBehavior {HttpGetEnabled = true};
                ServiceHost.Description.Behaviors.Add(smb);

            }
            catch (Exception e)
            {
                T.VSEXCEPTION(e, "XSynqPublisher - Initialize");
                ServiceHost.Abort();
            }
        }

        public bool ServiceStart()
        {
            try {
                // Step 5 of the hosting procedure: Start (and then stop) the service.
                ServiceHost.Open();
                return true;
            }
            catch (Exception e)
            {
                T.VSEXCEPTION(e,"XSynqPublisher - Start");
                ServiceHost.Abort();
                return false;
            }
        }

        public bool ServiceStop()
        {
            try
            {
                // Close the ServiceHostBase to shutdown the service.
                ServiceHost.Close(); 
                return true;
            }
            catch (Exception e)
            {
                T.VSEXCEPTION(e, "XSynqPublisher - Stop");
                ServiceHost.Abort();
                return false;
            }
        }
        */

    }
}