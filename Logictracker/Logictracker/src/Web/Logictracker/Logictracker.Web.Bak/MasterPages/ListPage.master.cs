#region Usings

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

#endregion

namespace Logictracker.MasterPages
{
    public partial class ListPage : BaseListMasterPage
    {
        public override ToolBar ToolBar { get { return ToolBar1; } }
        public override C1GridView ListGrid { get { return grid; } }
        public override InfoLabel NotFound { get { return infoLabel1; } }
        public override InfoLabel LblInfo { get { return infoLabel1; } }
        public override UpdatePanel UpdatePanelGrid { get { return updGrid; } }
        public override Control PanelSearch { get { return panelBuscar; } }
        public override TextBox TextBoxSearch { get { return txtBuscar; } }
        public override UpdatePanel UpdatePanelFilters { get { return updFilters; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AdvancedFiltersCheck();
        }
        private void AdvancedFiltersCheck()
        {
            if (PanelSearch.Visible) return;
            if (ContentFiltrosAvanzados.Controls.Count == 0) btExpandFilters.Visible = false;
            else if (ContentFiltrosAvanzados.Controls.Count == 1)
            {
                var lit = ContentFiltrosAvanzados.Controls[0] as LiteralControl;
                if (lit != null && lit.Text.Trim() == string.Empty) btExpandFilters.Visible = false;
            }
        }
    }
}
