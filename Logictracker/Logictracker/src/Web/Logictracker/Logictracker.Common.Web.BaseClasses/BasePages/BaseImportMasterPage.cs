using System.Web.UI;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;
using System.Web.UI.WebControls;

namespace Logictracker.Web.BaseClasses.BasePages
{
    public abstract class BaseImportMasterPage : BaseMasterPage
    {
        public abstract InfoLabel LblInfo { get; }

        public abstract FileUpload FileUpload { get; }

        public abstract Button UploadButton { get; }

        public abstract Button ImportButton { get; }
        public abstract IButtonControl ClearButton { get; }

        public abstract ToolBar ToolBar { get; }

        public abstract Panel PanelMapping { get; }

        public abstract Panel PanelImport { get; }

        public abstract Panel PanelUpload { get; }

        public abstract DropDownList CbWorksheets { get; }

        public abstract C1GridView Grid { get; }

        public abstract CheckBox CheckHasHeader { get; }

        public abstract DropDownList CbImportMode { get; }
        public abstract UpdatePanel UpdatePanelGrid { get; }
    }
}
