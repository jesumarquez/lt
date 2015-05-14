#region Usings

using System.ServiceModel;
using Logictracker.XSynq.Session;
using Logictracker.XSynq.Utilities;

#endregion

namespace Logictracker.XSynq.Contracts
{
    [ServiceContract(Name = "XSynqPublisher",
        Namespace = "http://walks.ath.cx/xsynq",
        CallbackContract = typeof(IXSynqPublisherCallback),
        SessionMode = SessionMode.Required)]
    public interface IXSynqPublisher
    {
        /// <summary>
        /// Crea una Suscripcion a la publicacion, y opcionalmente limita la 
        /// suscripcion a una rama puntual del arbol. 
        /// </summary>
        /// <returns>Identificador de la instancia del Subscriber.</returns>
        [OperationContract(IsInitiating = true)]
        [FaultContract(typeof(XSynqFaultCode))]
        string Subscribe(string DocumentName, string ClientName, string PublisherBranch, string SubscriberBranch, bool BranchOwner);

        /// <summary>
        /// Termina la suscripcion a la publicacion. 
        /// </summary>
        /// <remarks>
        /// El publisher, sincrinizara las novedades antes de retornar y
        /// finalizar la session.
        /// </remarks>
        [OperationContract(IsOneWay = false, IsTerminating = true)]
        [FaultContract(typeof(XSynqFaultCode))]
        void Unsubscribe();
        
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
        [OperationContract(IsInitiating = false)]
        [FaultContract(typeof(XSynqFaultCode))]
        void PendigsNotify(int Count);

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
        [OperationContract(IsInitiating = false)]
        [FaultContract(typeof(XSynqFaultCode))]
        void LockRequest(string BranchXPath);
        
        /// <summary>
        /// Solicita el desbloqueo de una rama del arbol.
        /// </summary>
        /// <param name="BranchXPath">Camino XPath a bloquearse</param>
        /// <remarks>
        /// El desbloqueo completa de forma asincronica cuando el publisher llama
        /// por callBack al suscriptor y confirma el desbloqueo.
        /// </remarks>
        [OperationContract(IsInitiating = false)]
        [FaultContract(typeof(XSynqFaultCode))]
        void UnlockRequest(string BranchXPath);

    }
}