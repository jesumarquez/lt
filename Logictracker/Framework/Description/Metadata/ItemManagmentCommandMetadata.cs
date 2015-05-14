#region Usings

using System;
using Logictracker.Description.Attributes;

#endregion

namespace Logictracker.Description.Metadata
{
    /// <summary>
    /// Describe y habilita un comando administrativo sobre el contenido del
    /// FrameworkElement al que se asocia el atributo.
    /// </summary>
    public class ItemManagmentCommandMetadata
    {

        public ItemManagmentCommandMetadata(ItemManagmentCommandAttribute attribute)
        {
            Type = attribute.Type;
            CommandName = attribute.CommandName;
            ConditionProperty = attribute.ConditionProperty;
            ConditionHint = attribute.ConditionHint;
        }

        public Type Type { get; private set; }

        public string CommandName { get; private set; }

        public string ConditionProperty { get; private set; }

        public string ConditionHint { get; private set; }
    }
}