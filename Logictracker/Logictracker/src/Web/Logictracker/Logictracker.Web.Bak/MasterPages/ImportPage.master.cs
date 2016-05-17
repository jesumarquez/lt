using System.Web.UI;
using Logictracker.Web.BaseClasses.BasePages;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.MasterPages
{
    public partial class ImportPage : BaseImportMasterPage
    {
        public override InfoLabel LblInfo { get { return infoLabel1; } }

        public override FileUpload FileUpload { get { return filExcel; } }

        public override Button UploadButton { get { return btUpload; } }

        public override Button ImportButton { get { return btImport; } }

        public override IButtonControl ClearButton { get { return btLimpiar; } }    

        public override ToolBar ToolBar { get { return ToolBar1; } }

        public override Panel PanelMapping { get { return panelMapping; } }

        public override Panel PanelImport { get { return panelImport; } }

        public override Panel PanelUpload { get { return panelUpload; } }

        public override DropDownList CbWorksheets { get { return cbWorksheets; } }

        public override C1GridView Grid { get { return grid; } }

        public override CheckBox CheckHasHeader { get { return chkHasHeader; } }

        public override DropDownList CbImportMode { get { return cbImportMode; } }

        public override UpdatePanel UpdatePanelGrid { get { return updUpload; } }

    }
}
