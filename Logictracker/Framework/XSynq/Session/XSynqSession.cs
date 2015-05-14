#region Usings

using System.Xml.Linq;

#endregion

namespace Logictracker.XSynq.Session
{
    /// <summary>
    /// Representa y define una suscripcion.
    /// </summary>
    public class XSynqSession
    {
        /// <summary>
        /// Identificador univoco del Suscriptor.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Nombre del Documento que suscribe.
        /// </summary>
        public string DocumentName { get; set; }
        
        /// <summary>
        /// Nombre del Cliente que Suscribe.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Indicacion que el suscriptor es el propietario de la rama suscripta.
        /// </summary>
        public bool BranchAuthority { get; set; }

        /// <summary>
        /// Camino XPath que delimita la suscripcion a una rama especifica
        /// del arbol. 
        /// </summary>
        public string PublisherBranch { get; set; }

        /// <summary>
        /// Camino XPath de la rama suscripta en la copia local del suscriptor.
        /// </summary>
        /// <remarks>
        /// Esta propiedad permite que una aplicacion utilizar un unico XDocument
        /// suscripto a multiples publishers ya que definir la ubicacion local
        /// de la rama suscripta. 
        /// </remarks>
        public string SubscriberBranch { get; set; }

        /// <summary>
        /// Elemento auxiliar para salvar el XElement del documento 
        /// del suscriptor que funciona como raiz en la suscripcion.
        /// </summary>
        public XElement SourceElement { get; set; }
    }
}