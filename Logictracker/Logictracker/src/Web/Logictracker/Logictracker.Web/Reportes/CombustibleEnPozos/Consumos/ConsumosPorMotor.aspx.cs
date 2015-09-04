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

namespace Logictracker.Reportes.CombustibleEnPozos.Consumos
{
    public partial class Reportes_ControlDeCombustible_Pozos_Consumos_ConsumosPorMotor : SecuredGridReportPage<ConsumoVo>
    {
        #region Protected Properties

        protected override InfoLabel NotFound { get { return null; } }

        protected override ToolBar ToolBar { get { return null; } }

        protected override string VariableName { get { return ""; } }

        protected override InfoLabel LblInfo { get { return null; } }

        public override C1GridView Grid { get { return grid; } }

        protected override ResourceButton BtnSearch { get { return null; } }

        protected override UpdatePanel UpdatePanelGrid { get { return upGrid; } }

        protected override C1GridView GridPrint { get { return null; } }

        protected override UpdatePanel UpdatePanelPrint { get { return null; } }


        /// <summary>
        /// Gets from session or viewstate the data to show.
        /// </summary>
        protected List<ConsumoVo> ReportData
        {
            get
            {
                if (Session["ConsumosDetail"] != null)
                {
                    var reportData = (List<ConsumoVo>)Session["ConsumosDetail"];
                    ViewState.Add("ConsumosDetail", reportData);

                    if (Session["KeepInSes"] == null) {Session.Remove("ConsumosDetail");}

                    Session["KeepInSes"] = null;

                    return ViewState["ConsumosDetail"] != null ? (List<ConsumoVo>)ViewState["ConsumosDetail"] : new List<ConsumoVo>();
                }

                return ViewState["ConsumosDetail"] != null ? (List<ConsumoVo>)ViewState["ConsumosDetail"] : new List<ConsumoVo>();
            }
        }

        #endregion

        #region Protected Methods

        protected override List<ConsumoVo> GetResults() { return ReportData; }

        public override int PageSize { get { return 15; } }

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

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "CONSUMOS"; }

        #endregion
    }
}

