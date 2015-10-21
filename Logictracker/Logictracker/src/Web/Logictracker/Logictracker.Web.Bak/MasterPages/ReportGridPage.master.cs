using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.MasterPages
{
    public partial class ReportGridPage : BaseReportGridMasterPage
    {
        private ReportPage master { get { return (Master as ReportPage); } }
        public override ToolBar ToolBar { get { return master.ToolBar; } }

        public override InfoLabel NotFound { get { return master.NotFound; } }
        public override InfoLabel LblInfo { get { return master.LblInfo; } }

        public override UpdatePanel UpdatePanelReport { get { return master.UpdatePanelReport; } }
        public override C1GridView Grid { get { return grid; } }

        public override ResourceButton btnSearch { get { return master.btnSearch; } }
        public override Control PanelSearch { get { return master.PanelSearch; } }
        public override TextBox TextBoxSearch { get { return master.TextBoxSearch; } }

        public override Repeater PrintFilters { get { return master.PrintFilters; } }
        public override C1GridView GridPrint { get { return gridPrint; } }
        public override UpdatePanel UpdatePanelPrint { get { return upPrint; } }

        public override DropDownList cbSchedulePeriodicidad { get { return cbPeriodicidad; } }
        public override TextBox txtScheduleMail { get { return txtMail; } }
        public override ResourceButton btScheduleGuardar { get { return btnGuardar; } }
        public override ModalPopupExtender modalSchedule { get { return mpePanel; } }

        public override TextBox SendReportTextBoxEmail { get { return TextBoxEmailSendReport; } }

        public override TextBox SendReportTextBoxReportName { get { return textBoxReportName; } }

        public override ModalPopupExtender SendReportModalPopupExtender { get { return popUpSendReport; } }
        public override ResourceButton SendReportOkButton { get { return ButtonOkSendReport; } }

        public override RadioButton RadioButtonExcel { get { return rbutExcel; } }
        public override RadioButton RadioButtonHtml { get { return rbutHtml; } }
    }
}
