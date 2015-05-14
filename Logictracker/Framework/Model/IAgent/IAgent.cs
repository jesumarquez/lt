#region Usings

using System;

#endregion

namespace Urbetrack.Model
{
    /// <summary>
    /// Objeto que 
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Procesa la solicitud dada en <paramref name="request"/>. (vea Observaciones)
        /// </summary>
        /// <param name="request">Url con el XPath de localizacion.</param>
        /// <returns>String que contiene un Documento XML con la respuesta final.</returns>
        string AgentRequest(Uri request);


        /// <summary>
        /// Procesa una solcitud acompañada de un post.
        /// </summary>
        /// <param name="request">Url solicitada.</param>
        /// <param name="post">Documento XML en forma de string con la informacion posteada.</param>
        /// <returns>Documento XML en forma de string con la respuesta final.</returns>
        /// <remarks>
        /// El Agente procesa la solicitud dada en <paramref name="request"/>. Si 
        /// corresponde por el XPath (obtenido del componente Path del Uri), se traslada 
        /// la instancia y luego el dominio, y finalmente se localiza el objeto 
        /// concreto direccionado, sobre el cual se procesa el post (si esta presente)
        /// y se obtiene el resultado enviando luego al cliente.
        /// Nota: se usa string dado que XLinq no implementa serializacion.
        /// </remarks>
        string AgentRequest(Uri request, string post);
    }
}
