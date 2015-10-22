#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

#endregion

namespace Logictracker.Admin
{
    /// <summary>
    /// Checks current queues status.
    /// </summary>
    public partial class AdminQueue : SecuredGridReportPage<QueueVo>
    {
        #region Protected Properties

        /// <summary>
        /// The grid to display report results.
        /// </summary>
        public override C1GridView Grid { get { return gridResults; } }

        /// <summary>
        /// The update panel associated to the grid.
        /// </summary>
        protected override UpdatePanel UpdatePanelGrid { get { return updGrid; } }

        /// <summary>
        /// Report title.
        /// </summary>
        protected override string VariableName { get { return String.Empty; } }

        /// <summary>
        /// The search button.
        /// </summary>
        protected override ResourceButton BtnSearch { get { return btnActualizar; } }

        /// <summary>
        /// Not found label message.
        /// </summary>
        protected override InfoLabel NotFound { get { return infoLabel1; } }

        /// <summary>
        /// C1 web tool bar.
        /// </summary>
        protected override ToolBar ToolBar { get { return null; } }

        /// <summary>
        /// Info messages label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        /// <summary>
        /// Auxiliar print grid.
        /// </summary>
        protected override C1GridView GridPrint { get { return null; } }

        /// <summary>
        /// Auxiliar print update panel.
        /// </summary>
        protected override UpdatePanel UpdatePanelPrint { get { return null; } }

        /// <summary>
        /// Auxiliar print filters.
        /// </summary>
        protected override Repeater PrintFilters { get { return null; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initial queues state binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            Bind();
        }

        /// <summary>
        /// Gets reports objects.
        /// </summary>
        /// <returns></returns>
		protected override List<QueueVo> GetResults() { return QueueStatus.QueueStatus.GetEnqueuedMessagesPerQueue().Select(queue => new QueueVo { Name = queue.Key, Messages = queue.Value }).ToList(); }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "QUEUES_ADMIN"; }

        #endregion
    }
}
