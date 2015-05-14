#region Usings

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

#endregion

namespace Logictracker.MasterPages
{
    public partial class ReportGraphPage : BaseReportGraphMasterPage
    {
        #region Private Properties

        /// <summary>
        /// The associated master page.
        /// </summary>
        private ReportPage MasterReportPage { get { return (Master as ReportPage); } }

        #endregion

        #region Public Properties

        /// <summary>
        /// Custom actions toolbar.
        /// </summary>
        public override ToolBar ToolBar { get { return MasterReportPage.ToolBar; } }

        /// <summary>
        /// Not found message label.
        /// </summary>
        public override InfoLabel NotFound { get { return MasterReportPage.LblInfo; } }

        /// <summary>
        /// Info messages label.
        /// </summary>
        public override InfoLabel LblInfo { get { return MasterReportPage.LblInfo; } }

        /// <summary>
        /// Auxiliar update panel for refreshing the report.
        /// </summary>
        public override UpdatePanel UpdatePanelReport { get { return MasterReportPage.UpdatePanelReport; } }

        /// <summary>
        /// Associated search button.
        /// </summary>
        public override ResourceButton btnSearch { get { return MasterReportPage.btnSearch; } }

        /// <summary>
        /// Region thar contains the data to be printed as filter values.
        /// </summary>
        public override Repeater PrintFilters { get { return MasterReportPage.PrintFilters; } }

        /// <summary>
        /// Div for containing the report graph.
        /// </summary>
        public override HtmlGenericControl DivGraph { get { return divChart; } }

        /// <summary>
        /// Div for containing the printed version of the graph.
        /// </summary>
        public override HtmlGenericControl DivGraphPrint {get { return divPrint; } }

        /// <summary>
        /// Auxiliar update panel for generating the printed version of the report.
        /// </summary>
        public override UpdatePanel UpdatePanelPrint { get { return upPrint; } }

        public override DropDownList cbSchedulePeriodicidad { get { return cbPeriodicidad; } }

        public override TextBox txtScheduleMail { get { return txtMail; } }

        public override ResourceButton btScheduleGuardar { get { return btnGuardar; } }

        public override ModalPopupExtender modalSchedule { get { return mpePanel; } }

        public override Control PanelSearch { get { return null; } }
        public override TextBox TextBoxSearch { get { return null; } }

        #endregion
    }
}
