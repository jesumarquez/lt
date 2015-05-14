#region Usings

using System;
using Logictracker.Description.Attributes;

#endregion

namespace Logictracker.Description.Metadata
{
    public class ItemConstraintMetadata
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

        
        public ItemConstraintMetadata(ItemConstraintAttribute attribute) 
        {
            ItemType = attribute.ItemType;
            MinItems = attribute.MinItems;
            MaxItems = attribute.MinItems;
        }
    }
}
