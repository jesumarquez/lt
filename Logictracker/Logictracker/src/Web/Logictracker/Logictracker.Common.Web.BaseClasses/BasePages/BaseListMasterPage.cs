#region Usings

using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

#endregion

namespace Logictracker.Web.BaseClasses.BasePages
{
    public abstract class BaseListMasterPage: BaseMasterPage
    {
        /// <summary>
        /// C1 Tool Bar.
        /// </summary>
        public abstract ToolBar ToolBar { get; }

        /// <summary>
        /// List grid.
        /// </summary>
        public abstract C1GridView ListGrid { get; }

        /// <summary>
        /// Not found label message.
        /// </summary>
        public abstract InfoLabel NotFound { get; }

        public abstract InfoLabel LblInfo { get; }

        public abstract UpdatePanel UpdatePanelGrid { get; }

        public abstract Control PanelSearch { get; }

        public abstract TextBox TextBoxSearch { get; }

        public abstract UpdatePanel UpdatePanelFilters { get; }
    }
}
