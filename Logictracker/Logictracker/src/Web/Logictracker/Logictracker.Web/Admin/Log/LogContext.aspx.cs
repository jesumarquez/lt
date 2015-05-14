using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Admin.Log
{
    public partial class AdminLogContext : SecuredGridReportPage<LogContextVo>
    {
        #region Overriden Properties

        public override C1GridView Grid { get { return grid; } }

        protected override UpdatePanel UpdatePanelGrid { get { return updGrid; } }

        protected override InfoLabel NotFound { get { return infoLabel1; } }

        protected override ToolBar ToolBar { get { return ToolBar1; } }

        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        protected override string VariableName { get { return "LOG_CONTEXT_ADMIN"; } }

        protected override ResourceButton BtnSearch { get { return null; } }

        protected override bool PrintButton { get { return false; } }

        protected override string GetRefference() { return "LOGS_ADMIN"; }

        protected override UpdatePanel UpdatePanelPrint { get { return null; } }

        protected override C1GridView GridPrint { get { return null; } }

        #endregion

        /// <summary>
        /// Property for getting the data associated to the report.
        /// </summary>
        private int IdLogEntry
        {
            get
            {
                if (Session["IdLogEntry"] != null)
                {
                    ViewState["IdLogEntry"] = Session["IdLogEntry"];

                    Session["IdLogEntry"] = null;
                }

                return (int)ViewState["IdLogEntry"];
            }
        }

        /// <summary>
        /// Gets the report data.
        /// </summary>
        /// <returns></returns>
        protected override List<LogContextVo> GetResults()
        {
            var reader = new Reader();

            return reader.GetContext(IdLogEntry).Select(con => new LogContextVo(con)).ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) Bind();
        }
    }
}