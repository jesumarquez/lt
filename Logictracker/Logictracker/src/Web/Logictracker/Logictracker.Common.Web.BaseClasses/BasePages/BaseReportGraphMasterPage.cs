#region Usings

using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Logictracker.Web.CustomWebControls.Buttons;

#endregion

namespace Logictracker.Web.BaseClasses.BasePages
{
    #region Public Classes

    /// <summary>
    /// Custom event args for chart creation event handling.
    /// </summary>
    public class ChartCreationEventArgs : EventArgs { public String ChartXml { get; set; } }

    /// <summary>
    /// Common functionality for all the graph report pages layout.
    /// </summary>
    public abstract class BaseReportGraphMasterPage : BaseReportMasterPage
    {
        #region Public Properties

        /// <summary>
        /// Div for containing the graph associated to the report.
        /// </summary>
        public abstract HtmlGenericControl DivGraph { get; }

        /// <summary>
        /// Div for containing the printed version of the report.
        /// </summary>
        public abstract HtmlGenericControl DivGraphPrint { get; }

        /// <summary>
        /// Auxiliar update panel for printing the report.
        /// </summary>
        public abstract UpdatePanel UpdatePanelPrint { get; }

        public abstract DropDownList cbSchedulePeriodicidad { get; }
        public abstract TextBox txtScheduleMail { get; }
        public abstract ResourceButton btScheduleGuardar { get; }
        public abstract ModalPopupExtender modalSchedule { get; }

        #endregion

        #region Public Events

        /// <summary>
        /// Event for handling the chart creation event.
        /// </summary>
        public event EventHandler<ChartCreationEventArgs> ChartCreation;

        /// <summary>
        /// Event for handling the printed version chart creation.
        /// </summary>
        public event EventHandler<ChartCreationEventArgs> ChartCreationPrint;

        #endregion

        #region Private Methods

        /// <summary>
        /// Delegates to the report page the creation of the graph.
        /// </summary>
        /// <param name="e"></param>
        private void OnChartCreation(ChartCreationEventArgs e)
        {
            var handler = ChartCreation;

            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Delegates to the report page the creation of the printed version graph.
        /// </summary>
        /// <param name="e"></param>
        private void OnChartCreationPrint(ChartCreationEventArgs e)
        {
            var handler = ChartCreationPrint;

            if (handler != null) handler(this, e);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the graph.
        /// </summary>
        /// <returns></returns>
        protected string CreateChart()
        {
            var eventArgs = new ChartCreationEventArgs();

            OnChartCreation(eventArgs);

            return eventArgs.ChartXml;
        }

        /// <summary>
        /// Creates the printed version of the graph.
        /// </summary>
        /// <returns></returns>
        protected string CreateChartPrint()
        {
            var eventArgs = new ChartCreationEventArgs();

            OnChartCreationPrint(eventArgs);

            return eventArgs.ChartXml;
        }

        #endregion
    }

    #endregion
}
