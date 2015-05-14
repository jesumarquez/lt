using System;

namespace Urbetrack.Managment
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, 
                    AllowMultiple = false)]
    public class FrameworkElementAttribute : Attribute
    {
        /// <summary>
        /// Nombre del tag XML que describe el Elemento.
        /// </summary>
        public string XName;

        /// <summary>
        /// Espacio de Nombres del tag XML que describe el Elemento.
        /// </summary>
        public string XNamespace;

        /// <summary>
        /// Si se establece verdadero, el miembro es contenedor de otros
        /// FrameworkElement. Por defecto es verdadero.
        /// </summary>
        public bool IsContainer;

        public FrameworkElementAttribute()
        {
            IsContainer = true;
            XNamespace = "http://walks.ath.cx/framework";
        }
        
    }
}