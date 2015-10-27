using System;
using System.Collections.Generic;
using System.Web.UI;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Reportes.ControlDeCombustible.Remitos
{
    public partial class ControlDeCombustible_Remitos_RemitosPorTanque : SecuredGridReportPage<RemitoVo>
    {
        #region Protected Properties
       
        protected override InfoLabel NotFound { get { return null; } }

        protected override ToolBar ToolBar { get { return null; } }

        protected override string VariableName { get { return ""; } }

        protected override InfoLabel LblInfo { get { return null; } }

        public override C1GridView Grid { get { return grid; } }

        protected override ResourceButton BtnSearch { get { return null; } }

        protected override UpdatePanel UpdatePanelGrid { get { return upGrid; } }

        protected override string GetRefference() { return "REMITOS"; }

        protected override C1GridView GridPrint { get { return null; } }

        protected override UpdatePanel UpdatePanelPrint { get { return null; } }

        /// <summary>
        /// Gets from session or viewstate the data to show.
        /// </summary>
        private List<RemitoVo> ReportData
        {
            get
            {
                if (Session["RemitosDetail"] != null)
                {
                    var reportData = (List<RemitoVo>)Session["RemitosDetail"];
                    ViewState.Add("RemitosDetail", reportData);

                    if (Session["KeepInSes"] == null) { Session.Remove("RemitosDetail"); }

                    Session["KeepInSes"] = null;

                    return reportData;
                }

                return ViewState["RemitosDetail"] != null ? (List<RemitoVo>)ViewState["RemitosDetail"] : new List<RemitoVo>();
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, RemitoVo dataItem)
        {
            GridUtils.GetCell(e.Row, RemitoVo.IndexTanque).Text = dataItem.Tanque ?? "Sin Identificar";
        }

        /// <summary>
        /// Returns the data from the viewstate
        /// </summary>
        /// <returns></returns>
        protected override List<RemitoVo> GetResults() { return ReportData; }

        /// <summary>
        /// Displays report results.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            Bind();
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            var isPrinting = !String.IsNullOrEmpty(Request.QueryString["IsPrinting"]) ? Convert.ToBoolean(Request.QueryString["IsPrinting"]) : false;

            if (isPrinting) grid.SkinID = "PrintGrid";
        }

        #endregion
    }
}
