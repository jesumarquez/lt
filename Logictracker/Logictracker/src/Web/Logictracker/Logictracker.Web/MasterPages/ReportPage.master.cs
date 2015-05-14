#region Usings

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

#endregion

namespace Logictracker.MasterPages
{
    public partial class ReportPage : BaseReportMasterPage
    {
        public override ToolBar ToolBar { get { return ToolBar1; } }
        public override InfoLabel NotFound { get { return infoLabel1; } }
        public override InfoLabel LblInfo { get { return infoLabel1; } }
        public override UpdatePanel UpdatePanelReport { get { return updReport; } }
        public override ResourceButton btnSearch { get { return buttonSearch; } }
        public override Repeater PrintFilters { get { return FiltrosPrint; } }

        public override Control PanelSearch { get { return panelBuscar; } }
        public override TextBox TextBoxSearch { get { return txtBuscar; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AdvancedFiltersCheck();
        }

        private void AdvancedFiltersCheck()
        {
            if (PanelSearch.Visible) return;
            if (ContentFiltrosAvanzados.Controls.Count == 0) btExpandFilters.Visible = false;
            else 
            {
                if (!HasContent(ContentFiltrosAvanzados)) btExpandFilters.Visible = false;
            }
        }
        private bool HasContent(ContentPlaceHolder contentPlaceHolder)
        {
            foreach (var control in contentPlaceHolder.Controls)
            {
                var lit = control as LiteralControl;
                if (lit != null && lit.Text.Trim() == string.Empty) continue;
                var cph = control as ContentPlaceHolder;
                if (cph != null && !HasContent(cph)) continue;
                return true;
            }
            return false;
        }
    }
}
