#region Usings

using System;

#endregion

namespace Logictracker.Description.Attributes
{
    /// <summary>
    /// Especifica que el FrameworkElement siempre y cuendo IsContainer sea verdadero
    /// combina 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ItemPropertyAttribute : Attribute
    {
        /// <summary>
        /// Nombre de la propiedad que 
        /// </summary>
        public string Property;
    }
}