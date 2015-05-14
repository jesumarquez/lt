using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;

namespace Logictracker.Description.Common
{
    [FrameworkElement(XName = "FileMonitor", IsContainer = false)]
    public class FileMonitor : FrameworkElement
    {
        [ElementAttribute(XName = "File", IsSmartProperty = true, IsRequired = true)]
        public string File
        {
            get { return (string)GetValue("File"); }
            set { SetValue("File", value); }
        }

        [ElementAttribute(XName = "StableAge", IsSmartProperty = true, IsRequired = false, DefaultValue = 30)]
        public int StableAge
        {
            get { return (int)GetValue("StableAge"); }
            set { SetValue("StableAge", value); }
        }

        [ElementAttribute(XName = "OnChangeCommandTarget", IsSmartProperty = true, IsRequired = true)]
        public FrameworkElement OnChangeCommandTarget
        {
            get { return (FrameworkElement)GetValue("OnChangeCommandTarget"); }
            set { SetValue("OnChangeCommandTarget", value); }
        }

        [ElementAttribute(XName = "OnChangeCommandName", IsSmartProperty = true, IsRequired = true)]
        public string OnChangeCommandName
        {
            get { return (string)GetValue("OnChangeCommandName"); }
            set { SetValue("OnChangeCommandName", value); }
        }
    }
}
