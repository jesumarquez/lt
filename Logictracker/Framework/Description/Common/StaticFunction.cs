using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;

namespace Logictracker.Description.Common
{
    [FrameworkElement(XName = "StaticFunction", IsContainer = false)]
    public class StaticFunctionElement : FrameworkElement
    {
        [ElementAttribute(XName = "Name", IsSmartProperty = true, IsRequired = true)]
        public string Name
        {
            get { return (string)GetValue("Name"); }
            set { SetValue("Name", value); }
        }

        [ElementAttribute(XName = "Class", IsSmartProperty = true, IsRequired = true)]
        public string Class
        {
            get { return (string)GetValue("Class"); }
            set { SetValue("Class", value); }
        }

        [ElementAttribute(XName = "Method", IsSmartProperty = true, IsRequired = true)]
        public string Method
        {
            get { return (string)GetValue("Method"); }
            set { SetValue("Method", value); }
        }

        [ElementAttribute(XName = "ReturnType", IsSmartProperty = true, IsRequired = true)]
        public string ReturnType
        {
            get { return (string)GetValue("ReturnType"); }
            set { SetValue("ReturnType", value); }
        }

        public virtual object FunctionExecute(params object[] args)
        {
            return true;
        }
    }
}
