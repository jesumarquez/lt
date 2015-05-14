#region Usings

using System;

#endregion

namespace Logictracker.Description.Attributes
{
    /// <summary>
    /// Aplica una restricción sobre los tipos de items que puede contener un elemento.
    /// Cuando un FrameworkElement no incluye ninguna restricción, entonces cualquier
    /// contenido es valido. Si se aplica almenos una restricción, entonces se deben
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ItemConstraintAttribute :Attribute
    {
        /// <summary>
        /// Tipo al que refiere la restricción, debe ser una clase o una interface, en
        /// ambos casos el tipo concreto debe heredar de FrameworkElement.
        /// </summary>
        public Type ItemType { get; private set; }

        /// <summary>
        /// Numero minimos de Items que debe tener el FrameworkElement del tipo 
        /// especificado.
        /// </summary>
        public int MinItems;

        /// <summary>
        /// Numero maximo de Items que puede tener el FrameworkElement del tipo 
        /// especificado.
        /// </summary>
        public int MaxItems;

        /// <summary>
        /// Define la restricción de tipos.
        /// </summary>
        /// <param name="itemType">Tipo sobre el que se aplica la restricción.</param>
        public ItemConstraintAttribute(Type itemType) 
        {
            ItemType = itemType;
        }
    }
}
