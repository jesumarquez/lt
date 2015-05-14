using System.Collections.Generic;
using Urbetrack.Managment;

namespace Urbetrack.SessionBorder
{
    /// <summary>
    /// Elemento String
    /// </summary>
    [FrameworkElement(XName = "MessageHook", IsContainer = false)]
    public class MessageHook : FrameworkElement
    {
        /// <summary>
        /// Metodo abstracto que debe implementar el elemento concreto
        /// en el cual tiene la oportunidad de contruir las estructuras
        /// que necesita para su operacion.
        /// </summary>
        /// <param name="Report"></param>
        /// <returns></returns>
        public override bool SetupComplete(List<string> Report)
        {
            return true;   
        }
    }
}
