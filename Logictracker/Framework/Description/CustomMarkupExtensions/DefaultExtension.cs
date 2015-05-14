using Logictracker.Description.Attributes;
using Logictracker.Description.Metadata;
using Logictracker.Description.Runtime;

namespace Logictracker.Description.CustomMarkupExtensions
{
    [CustomMarkupExtension("Default")]
    public class DefaultExtension : ICustomMarkupExtension
    {
        public object ProvideValue(FrameworkElement frameworkElement, ElementAttributeMetadata attributeMetadata, string keyword, string content)
        {
            return attributeMetadata.DefaultValue;
        }
    }
}
