#region Usings

using System;
using System.Collections.Generic;
using System.Web.UI;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

#endregion

namespace Logictracker.Reportes.ControlDeCombustible.Despachos
{
    public partial class ControlDeCombustible_Despachos_DespachosPorMovil : SecuredGridReportPage<DespachoVo>
    {

        #region Protected Properties

        protected override InfoLabel NotFound { get { return null; } }

        protected override ToolBar ToolBar { get { return null; } }

        protected override string VariableName { get { return ""; } }

        protected override InfoLabel LblInfo { get { return null; } }

        public override C1GridView Grid { get { return grid; } }

        protected override ResourceButton BtnSearch { get { return null; } }

        protected override UpdatePanel UpdatePanelGrid { get { return upGrid; } }

        protected override string GetRefference() { return "DESPACHOS"; }

        protected override C1GridView GridPrint { get { return null; } }

        protected override UpdatePanel UpdatePanelPrint { get { return null; } }

        /// <summary>
        /// Gets from session or viewstate the data to show.
        /// </summary>
        private List<DespachoVo> ReportData
        {
            get
            {
                if (Session["DespachosDetail"] != null)
                {
                    var reportData = (List<DespachoVo>)Session["DespachosDetail"];
                    ViewState.Add("DespachosDetail", reportData);

                    if (Session["KeepInSes"] == null) { Session.Remove("DespachosDetail"); }

                    Session["KeepInSes"] = null;

                    return reportData;
                }

                return ViewState["DespachosDetail"] != null ? (List<DespachoVo>)ViewState["DespachosDetail"] : new List<DespachoVo>();
            }
        }


        #endregion

        #region Protected Methods

        /// <summary>
        /// Returns the data from the viewstate
        /// </summary>
        /// <returns></returns>
        protected override List<DespachoVo> GetResults() { return ReportData; }

        public override int PageSize { get { return 12; } }

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
