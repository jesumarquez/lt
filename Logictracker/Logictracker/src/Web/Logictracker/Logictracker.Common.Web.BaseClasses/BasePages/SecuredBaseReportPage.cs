using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;
using Logictracker.Web.Helpers.ExportHelpers;

namespace Logictracker.Web.BaseClasses.BasePages
{
    public abstract class SecuredBaseReportPage<T> : ApplicationSecuredPage
    {
        private List<T> _reportObjectsList;

        #region Private Properties

        /// <summary>
        /// Base report master page.
        /// </summary>
        private BaseReportMasterPage BaseReportMasterPage { get { return Master as BaseReportMasterPage; } }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Report data proxy.
        /// </summary>
        protected List<T> ReportObjectsList
        {
            get { return _reportObjectsList ?? new List<T>(); }
            set { _reportObjectsList = value; }
        }

        /// <summary>
        /// Not found message label.
        /// </summary>
        protected virtual InfoLabel NotFound { get { return BaseReportMasterPage.NotFound; } }

        protected override InfoLabel LblInfo { get { return BaseReportMasterPage.LblInfo; } }
        /// <summary>
        /// C1 web tool bar.
        /// </summary>
        protected virtual ToolBar ToolBar { get { return BaseReportMasterPage.ToolBar; } }

        /// <summary>
        /// Associated search button.
        /// </summary>
        protected virtual ResourceButton BtnSearch { get { return BaseReportMasterPage.btnSearch; } }

        /// <summary>
        /// Resource group nam for searching variables.
        /// </summary>
        protected virtual String ResourceName { get { return "Menu"; } }

        /// <summary>
        /// The variable name associated to the report title.
        /// </summary>
        protected abstract String VariableName { get; }

        /// <summary>
        /// Content that holds all the data to print as filter values.
        /// </summary>
        protected virtual Repeater PrintFilters { get { return BaseReportMasterPage.PrintFilters; } }

        /// <summary>
        /// Determines wither to add or not the csv export button.
        /// </summary>
        protected virtual Boolean CsvButton { get { return true; } }
        protected virtual Boolean ExcelButton { get { return false; } }

        protected virtual Boolean ScheduleButton { get { return false; } }
        protected virtual Boolean SendReportButton { get { return false; } }

        /// <summary>
        /// Determines wither to add or not the print button.
        /// </summary>
        protected virtual Boolean PrintButton { get { return true; } }
        protected virtual Control PanelSearch { get { return BaseReportMasterPage != null ? BaseReportMasterPage.PanelSearch : null; } }
        protected virtual TextBox TextBoxSearch { get { return BaseReportMasterPage != null ? BaseReportMasterPage.TextBoxSearch : null; } }
        protected virtual bool HideSearch { get { return false; } }
        protected virtual bool NoContentUpdate { get { return false; } }


        #endregion

        #region Protected Methods

        /// <summary>
        /// Defines how to generate the csv exported report header.
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void GenerateCsvHeader(GraphToCsvBuilder builder) { }

        /// <summary>
        /// Exports the current report into a csv file.
        /// </summary>
        protected abstract void ExportToCsv();

        protected abstract void ExportToExcel();

        protected virtual void Schedule() { }

        protected virtual void SendReportToMail() { }

        protected virtual Empresa GetEmpresa() { return null; }
        protected virtual Linea GetLinea() { return null; }


        /// <summary>
        /// Gets the associated filter vaues.
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<String, String> GetFilterValues(){return new Dictionary<String, String>();}

        protected virtual Dictionary<String, String> GetFilterValuesProgramados() { return new Dictionary<String, String>(); }

        //programacion de reportes
        protected virtual string GetSelectedVehicles() { return string.Empty; }
        protected virtual string GetSelectedDrivers() { return string.Empty; }
        protected virtual string GetSelectedGeofences() { return string.Empty; }
        protected virtual string GetSelectedMessageTypes() { return string.Empty; }
        protected virtual string GetSelectedDocuments () { return string.Empty; }
        protected virtual string GetDescription(string s) { return s; }
        protected virtual bool GetCicleCheck() { return false; }
        protected virtual int GetOvercomeKilometers() { return 0; }
        protected virtual bool GetShowCornersCheck() { return false; }
        protected virtual bool GetCalculateKilometers() { return false; }
        protected virtual string GetOdometerType() { return string.Empty; }
        protected virtual double GetInGeofenceTime() { return 0; }
        protected virtual TimeSpan GetHigherDetentionFor() { return new TimeSpan(0,0,1,0); }
        protected virtual double GetHigherDistanceOf() { return 100; }
        protected virtual TimeSpan GetUnreportedTime() { return new TimeSpan(0, 0, 1, 0); }

        //envio manual de reportes
        protected virtual List<int> GetSelectedListByField(string field) { return new List<int>(); }
        protected virtual DateTime GetSinceDateTime() { return DateTime.UtcNow.AddDays(-1); }
        protected virtual DateTime GetToDateTime() { return DateTime.UtcNow; }
        protected virtual int GetCompanyId() { return 0; }

        /// <summary>
        /// Send the data displayed at the grid to report print page.
        /// </summary>
        protected virtual void Print()
        {
            OnPrePrint();

            PrintFilters.DataSource = GetFilterValues();

            PrintFilters.DataBind();
        }

        /// <summary>
        /// You should do here everything you want before printing.
        /// </summary>
        protected virtual void OnPrePrint() { }

        /// <summary>
        /// Inverts the selection for the listBox Items.
        /// </summary>
        /// <param name="listBox"></param>
        protected virtual void ToogleItems(ListBoxBase listBox) { if (listBox.GetSelectedIndices().Length == 0) listBox.ToogleItems(); }

        /// <summary>
        /// Performs user actions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ToolbarItemCommand(Object sender, CommandEventArgs e)
        {
            try
            {
                ReportObjectsList = GetResults();
                switch (e.CommandName)
                {
                    case ToolBar.ButtonCommandNamePrint: Print(); break;
                    case ToolBar.ButtonCommandNameCsv: ExportToCsv(); break;
                    case ToolBar.ButtonCommandNameSchedule: Schedule(); break;
                    case ToolBar.ButtonCommandNameSendReport : SendReportToMail(); break;
                    case ToolBar.ButtonCommandNameExcel: ExportToExcel();break;
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        /// <summary>
        /// Initial page behaivour and layout set up.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (BtnSearch!= null) BtnSearch.Click += BtnSearchClick;

            if(NoContentUpdate)
            {
                BaseReportMasterPage.UpdatePanelReport.Triggers.Clear();
                BaseReportMasterPage.UpdatePanelReport.ChildrenAsTriggers = false;
            }

            if (ToolBar == null) return;

            ToolBar.ResourceName = ResourceName;
            ToolBar.VariableName = VariableName;
        }

        /// <summary>
        /// Adds user actions to the toolbar depending on user privileges.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreLoad(EventArgs e)
        {
            if (ToolBar != null)
            {
                AddToolBarIcons();

                ToolBar.ItemCommand += ToolbarItemCommand;
            }

            base.OnPreLoad(e);

            if (!IsPostBack)
            {
                if ((HideSearch) && PanelSearch != null) PanelSearch.Visible = false;
            }
        }

        /// <summary>
        /// Searches ranking filtering by the givenn values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void BtnSearchClick(Object sender, EventArgs e);

        /// <summary>
        /// Adds the CSV button.
        /// </summary>
        protected virtual void AddToolBarIcons()
        {
            if (CsvButton) ToolBar.AddCsvToolbarButton();

            if (ExcelButton) ToolBar.AddExcelToolbarButton();

            if (PrintButton) ToolBar.AddPrintToolbarButton();

            if (ScheduleButton) ToolBar.AddScheduleButton();

            if (SendReportButton) ToolBar.AddSendReportButton();
        }

        /// <summary>
        /// Gets the report data.
        /// </summary>
        /// <returns></returns>
        protected abstract List<T> GetResults();

        /// <summary>
        /// Sorts report data.
        /// </summary>
        protected virtual void SortResults() { }

        #endregion
    }
}