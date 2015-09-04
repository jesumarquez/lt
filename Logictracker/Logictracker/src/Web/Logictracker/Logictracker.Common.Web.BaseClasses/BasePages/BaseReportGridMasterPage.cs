using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using AjaxControlToolkit;
using Logictracker.Web.CustomWebControls.Buttons;

namespace Logictracker.Web.BaseClasses.BasePages
{
    public abstract class BaseReportGridMasterPage: BaseReportMasterPage
    {
        public abstract C1GridView Grid { get; }
        public abstract C1GridView GridPrint { get; }
        public abstract UpdatePanel UpdatePanelPrint { get; }
        public abstract DropDownList cbSchedulePeriodicidad { get;}
        
        public abstract TextBox txtScheduleMail { get;}
        public abstract ResourceButton btScheduleGuardar { get; }
        public abstract ModalPopupExtender modalSchedule { get; }

        public abstract TextBox SendReportTextBoxEmail { get; }
        public abstract TextBox SendReportTextBoxReportName { get; }
        public abstract ModalPopupExtender SendReportModalPopupExtender { get; }
        public abstract ResourceButton SendReportOkButton { get; }

        public abstract RadioButton RadioButtonExcel { get; }
        public abstract RadioButton RadioButtonHtml { get; }
        public abstract RadioButton RadioBtnExcelSendReport { get; }
        public abstract RadioButton RadioBtnHtmlSendReport { get; }
    }
}
