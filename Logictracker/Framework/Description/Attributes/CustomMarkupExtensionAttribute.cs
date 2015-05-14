#region Usings

using System;

#endregion

namespace Logictracker.Description.Attributes
{
    /// <summary>
    /// Decora clases que extienden la sintaxis del XML 
    /// sin necesisdad de tocar el core.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, 
                    AllowMultiple = false)]
    public class CustomMarkupExtensionAttribute : Attribute
    {
        /// <summary>
        /// Keyword que identifica la extension.
        /// </summary>
        public string Keyword;

        /// <summary>
        /// Crea el Attribute y estabece los valores por defecto.
        /// </summary>
        public CustomMarkupExtensionAttribute(string keyWord)
        {
            Keyword = keyWord;
        }
    }
}