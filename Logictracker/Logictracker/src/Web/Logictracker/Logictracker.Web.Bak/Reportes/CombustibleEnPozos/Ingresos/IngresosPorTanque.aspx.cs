using System;
using System.Collections.Generic;
using System.Web.UI;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Reportes.CombustibleEnPozos.Ingresos
{
    public partial class Reportes_CombustibleEnPozos_Ingresos_IngresosPorTanque : SecuredGridReportPage<IngresoVo>
    {
        /// <summary>
        /// Gets from session or viewstate the data to show.
        /// </summary>
        private List<IngresoVo> ReportData
        {
            get
            {
                if(Session["IngresosDetail"] != null)
                {
                    var reportData = (List<IngresoVo>)Session["IngresosDetail"];
                    ViewState.Add("IngresosDetail", reportData);

                    if (Session["KeepInSes"] == null) { Session.Remove("IngresosDetail"); }

                    Session["KeepInSes"] = null;

                    return reportData;
                }

                return ViewState["IngresosDetail"] != null ? (List<IngresoVo>)ViewState["IngresosDetail"] : new List<IngresoVo>();
            }
        }

        #region Protected Properties

        protected override InfoLabel NotFound { get { return null; } }

        protected override ToolBar ToolBar { get { return null; } }

        protected override string VariableName { get { return String.Empty; } }

        protected override InfoLabel LblInfo { get { return null; } }

        public override C1GridView Grid { get { return grid; } }

        protected override UpdatePanel UpdatePanelGrid { get { return upGrid; } }

        protected override ResourceButton BtnSearch { get { return null; } }

        protected override string GetRefference() { return "INGRESOS_POZO"; }

        protected override C1GridView GridPrint { get { return null; } }

        protected override UpdatePanel UpdatePanelPrint { get { return null; } }

        #endregion

        #region Protected Methods

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, IngresoVo dataItem)
        {
            GridUtils.GetCell(e.Row, IngresoVo.IndexTanque).Text = dataItem.Tanque ?? "Sin Identificar";
        }

        protected override List<IngresoVo> GetResults() { return ReportData; }

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

