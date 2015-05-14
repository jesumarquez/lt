#region Usings

using System;

#endregion

namespace Logictracker.Description.Attributes
{
    /// <summary>
    /// Describe y habilita un comando administrativo sobre el contenido del
    /// FrameworkElement al que se asocia el atributo.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ItemManagmentCommandAttribute : Attribute
    {
        /// <summary>
        /// Describe y habilita un comando administrativo sobre el contenido del
        /// FrameworkElement al que se asocia el atributo.
        /// </summary>
        /// <param name="commandName">
        /// Nombre del Comando Administrativo. Los comandos predefinidos son
        /// "Add" y "Delete" 
        /// </param>
        /// <param name="type">Tipo sobre el que se habilita el comando administrativo.</param>
        public ItemManagmentCommandAttribute(string commandName, Type type)
        {
            Type = type;
            CommandName = commandName;
        }

        public Type Type { get; private set; }
        
        public string CommandName { get; private set; }

        public string ConditionProperty;

        public string ConditionHint;
    }
}