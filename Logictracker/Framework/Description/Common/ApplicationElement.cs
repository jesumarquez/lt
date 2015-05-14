using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;

namespace Logictracker.Description.Common
{
    /// <summary>
    /// Elemento String
    /// </summary>
    [FrameworkElement(XName = "Application", IsContainer = true)]
    public class ApplicationElement : FrameworkElement
    {
		#region Attributes

		[ElementAttribute(XName = "ApplicationMode", IsSmartProperty = true, IsRequired = false, DefaultValue = ApplicationMode.Manual)]
		public ApplicationMode ApplicationMode
		{
			get { return (ApplicationMode)GetValue("ApplicationMode"); }
			set { SetValue("ApplicationMode", value); }
		}

		[ElementAttribute(XName = "Resources", IsSmartProperty = true, IsRequired = false, DefaultValue = RunMode.Console)]
		public ResourcesElement Resources
		{
			get { return (ResourcesElement)GetValue("Resources"); }
			set { SetValue("Resources", value); }
		}

		[ElementAttribute(XName = "RunMode", IsSmartProperty = true, IsRequired = false, DefaultValue = RunMode.Console)]
		public RunMode RunMode
		{
			get { return (RunMode)GetValue("RunMode"); }
			set { SetValue("RunMode", value); }
		}
		
		#endregion
    }
}
