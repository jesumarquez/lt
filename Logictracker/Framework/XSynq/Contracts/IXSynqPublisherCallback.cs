#region Usings

using System.ServiceModel;
using Logictracker.XSynq.Utilities;

#endregion

namespace Logictracker.XSynq.Contracts
{
    /// <summary>
    /// Contrato de servicio de un suscriptor en la arquitectura de replicacion 
    /// para XSynq (XML).
    /// </summary>
    /// <remarks>
    /// Encargado de preparar y enviar como conjuntos, todas las novedades 
    /// originadas localmente, y tambien de recibir y procesar 
    /// (aplicarlas en el documento XML) los conjuntos de novedades 
    /// provenientes del Publisher, consolidando el documento "esclavo" que
    /// se utiliza localmente.
    /// </remarks>
    public interface IXSynqPublisherCallback
    {

        /// <summary>
        /// Fuerza al suscriptor a descartar su copia local y las novedades pentientes
        /// que tenga. 
        /// </summary>
        /// <remarks>
        /// Este metodo es llamado por el publisher al iniciar la suscripcion para 
        /// establecer la copia local del suscriptor.
        /// </remarks>
        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(XSynqFaultCode))]
        void Reset(string xDocument);

        /// <summary>
        /// Obtiene el conjunto de novedades pendientes hacia arriba.
        /// </summary>
        /// <returns>un ChangeSet con todas las novedades.</returns>
        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(XSynqFaultCode))]
        string GetChangeSet();
 
        /// <summary>
        /// Consolidad el ChangeSet dado en el documento local.
        /// </summary>
        /// <param name="ChangeSet">ChangeSet a aplicarse sobre le documento local.</param>
        [OperationContract(IsOneWay = false)]
        [FaultContract(typeof(XSynqFaultCode))]
        void ApplyChangeSet(string ChangeSet);
    }
}