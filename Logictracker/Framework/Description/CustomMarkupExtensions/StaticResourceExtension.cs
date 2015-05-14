using Logictracker.Description.Attributes;
using Logictracker.Description.Metadata;
using Logictracker.Description.Runtime;

namespace Logictracker.Description.CustomMarkupExtensions
{
    [CustomMarkupExtension("StaticResource")]
    public class StaticResourceExtension : ICustomMarkupExtension
    {
        public object ProvideValue(FrameworkElement frameworkElement, ElementAttributeMetadata attributeMetadata, string keyword, string content)
        {
            return frameworkElement.Application.GetFrameworkElement(content);
        }
    }
}
