using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;

namespace Logictracker.Description.Common
{
    [FrameworkElement(XName = "String", IsContainer = false)]
    public class StringElement : FrameworkElement
    {
        [ElementAttribute(XName = "Headers", IsSmartProperty = true, IsRequired = false)]
        public string Headers
        {
            get { return (string)GetValue("Headers"); }
            set { SetValue("Headers", value); }
        }
    }
}
