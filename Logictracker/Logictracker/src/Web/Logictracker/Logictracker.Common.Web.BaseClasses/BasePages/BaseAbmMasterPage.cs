#region Usings

using System.Web.UI.HtmlControls;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

#endregion

namespace Logictracker.Web.BaseClasses.BasePages
{
    public abstract class BaseAbmMasterPage : BaseMasterPage
    {
        public abstract ToolBar ToolBar { get; }

        public abstract InfoLabel LblInfo { get; }

        public abstract HtmlGenericControl LblId { get; }

        public abstract void SetTabIndex(int index);
    }
}
