using Logictracker.Description.Metadata;
using Logictracker.Description.Runtime;

namespace Logictracker.Description.CustomMarkupExtensions
{
    public interface ICustomMarkupExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <param name="attributeMetadata"></param>
        /// <param name="keyword"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        object ProvideValue(FrameworkElement frameworkElement, ElementAttributeMetadata attributeMetadata, string keyword, string content);
    }
}