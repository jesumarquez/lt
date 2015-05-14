#region Usings

using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

#endregion

namespace Logictracker.Web.BaseClasses.BasePages
{
    public abstract class BaseReportMasterPage : BaseMasterPage
    {
        /// <summary>
        /// C1 Tool Bar.
        /// </summary>
        public abstract ToolBar ToolBar { get; }

        /// <summary>
        /// Not found label message.
        /// </summary>
        public abstract InfoLabel NotFound { get; }

        public abstract InfoLabel LblInfo { get; }

        public abstract UpdatePanel UpdatePanelReport { get; }

        public abstract ResourceButton btnSearch { get; }

        public abstract Repeater PrintFilters { get; }
        public abstract Control PanelSearch { get; }
        public abstract TextBox TextBoxSearch { get; }
    }
}
