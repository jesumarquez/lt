#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using InfoSoftGlobal;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.Layers.MessageQueue;
using Logictracker.Mailing;
using Logictracker.Reports.Messaging;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.CustomWebControls.Helpers;
using Logictracker.Web.CustomWebControls.HiddenFields;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;
using Logictracker.Web.Helpers.ExportHelpers;
using Logictracker.Web.Helpers.FussionChartHelpers;

#endregion

namespace Logictracker.Web.BaseClasses.BasePages
{
    #region Public Enums

    /// <summary>
    /// Enum that defines available chart types.
    /// </summary>
    public enum GraphTypes { Barrs, Lines, MultiLine, MultiColumn, StackedColumn }

    #endregion

    #region Public Classes

    /// <summary>
    /// Class that implements all common logic to all reports that contains a fusioncharts graph.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SecuredGraphReportPage<T> : SecuredBaseReportPage<T>
    {
        #region Protected Properties

        /// <summary>
        /// Custom actions toolbar.
        /// </summary>
        protected override ToolBar ToolBar { get { return MasterReportGraphPage.ToolBar; } }

        /// <summary>
        /// Div for containing the associated graph.
        /// </summary>
        protected virtual HtmlGenericControl Graph { get { return MasterReportGraphPage.DivGraph; } }

        /// <summary>
        /// Div for containing the printed version of the graph.
        /// </summary>
        protected virtual HtmlGenericControl GraphPrint { get { return MasterReportGraphPage.DivGraphPrint; } }

        /// <summary>
        /// Not found message label.
        /// </summary>
        protected override InfoLabel NotFound { get { return MasterReportGraphPage.NotFound; } }

        /// <summary>
        /// Info messages label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return MasterReportGraphPage.LblInfo; } }

        /// <summary>
        /// Update panle for generating the report.
        /// </summary>
        protected virtual UpdatePanel UpdatePanelGraph { get { return MasterReportGraphPage.UpdatePanelReport; } }

        /// <summary>
        /// Update panel for generating the printed version of the report.
        /// </summary>
        protected virtual UpdatePanel UpdatePanelGraphPrint { get { return MasterReportGraphPage.UpdatePanelPrint; } }

        /// <summary>
        /// Associated search button.
        /// </summary>
        protected override ResourceButton BtnSearch { get { return MasterReportGraphPage.btnSearch; } }

        /// <summary>
        /// Label of the Graph X Axis.
        /// </summary>
        protected abstract string XAxisLabel { get; }

        /// <summary>
        /// Label of the Graph Y Axis.
        /// </summary>
        protected abstract string YAxisLabel { get; }

        protected BaseReportGraphMasterPage MasterPage { get { return Master as BaseReportGraphMasterPage; } }
        protected override Boolean ScheduleButton { get { return false; } }
        protected DropDownList CbSchedulePeriodicidad { get { return MasterPage.cbSchedulePeriodicidad; } }
        protected TextBox TxtScheduleMail { get { return MasterPage.txtScheduleMail; } }
        protected ResourceButton BtScheduleGuardar { get { return MasterPage.btScheduleGuardar; } }
        protected ModalPopupExtender ModalSchedule { get { return MasterPage.modalSchedule; } }

        protected TextBox SendReportTextBoxEmail { get { return MasterPage.SendReportTextBoxEmail; } }
        protected TextBox SendReportTextBoxReportName { get { return MasterPage.SendReportTextBoxReportName; } }
        protected ModalPopupExtender SendReportModalPopupExtender { get { return MasterPage.SendReportModalPopupExtender; } }
        protected ResourceButton SendReportOkButton { get { return MasterPage.SendReportOkButton; } }

        protected virtual RadioButton RadioButtonExcel { get { return MasterPage.RadioButtonExcel; } }
        protected virtual RadioButton RadioButtonHtml { get { return MasterPage.RadioButtonHtml; } }

        /// <summary>
        /// List of FusionChartsItem used by the graph.
        /// </summary>
        protected virtual List<FusionChartsDataset> GraphDataSet
        {
            get { return ViewState["Items"] != null ? (List<FusionChartsDataset>)ViewState["Items"] : new List<FusionChartsDataset>(); }
            set { ViewState["Items"] = value; }
        }

        /// <summary>
        /// List of FusionChartsItem used by the graph.
        /// </summary>
        protected virtual List<string> GraphCategories
        {
            get { return ViewState["Categories"] != null ? (List<string>)ViewState["Categories"] : new List<string>(); }
            set { ViewState["Categories"] = value; }
        }

        /// <summary>
        /// Graph Type.
        /// </summary>
        protected abstract GraphTypes GraphType { get; }

        /// <summary>
        /// Report graph default width.
        /// </summary>
        protected virtual int? DefaultWidth { get { return null; } }

        /// <summary>
        /// Report graph default height.
        /// </summary>
        protected virtual int? DefaultHeight { get { return null; } }

        /// <summary>
        /// Report graph margin width.
        /// </summary>
        protected virtual int MarginWidth { get { return 275; } }

        /// <summary>
        /// Report graph margin heigth.
        /// </summary>
        protected virtual int MarginHeigth { get { return 125; } }

        /// <summary>
        /// Page size handling control.
        /// </summary>
        protected SizeHiddenField Size { get; private set; }

        #endregion

        #region Private Properties

        /// <summary>
        /// Char definition file path.
        /// </summary>
        private string ChartXmlDefinition
        {
            get
            {
                var template = String.Empty;

                switch (GraphType)
                {
                    case GraphTypes.Barrs: template = "FCF_Column3D.swf"; break;
                    case GraphTypes.Lines: template = "FCF_Line.swf"; break;
                    case GraphTypes.MultiLine: template = "FCF_MSLine.swf"; break;
                    case GraphTypes.MultiColumn: template = "FCF_MSColumn2D.swf"; break;
                    case GraphTypes.StackedColumn: template = "FCF_StackedColumn3D.swf"; break;
                }

                return String.Concat(FusionChartDir, template);
            }
        }

        /// <summary>
        /// Associated master page.
        /// </summary>
        private BaseReportGraphMasterPage MasterReportGraphPage { get { return Master as BaseReportGraphMasterPage; } }

        #endregion
     
        #region Protected Methods

        /// <summary>
        /// Defines a control for handling page size.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddSizeHandlingControl();
            if (ScheduleButton) BtScheduleGuardar.Click += BtScheduleGuardarClick;
            if (SendReportButton) SendReportOkButton.Click += ButtonOkSendReportClick;
        }

        /// <summary>
        /// Set ups event handlers if a master page is defined.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (MasterReportGraphPage != null)
            {
                MasterReportGraphPage.ChartCreation += CreateChart;
                MasterReportGraphPage.ChartCreationPrint += CreateChartPrint;
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Performs the search using the specified parameters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            try { DoSearch(); }
            catch (Exception ex) { ShowError(ex); }
        }

        /// <summary>
        /// Gets report data and generates the chart.
        /// </summary>
        protected void DoSearch()
        {
            ReportObjectsList = GetResults();

            Graph.Visible = ReportObjectsList.Count > 0;

            if (NotFound != null) NotFound.Text = !Graph.Visible ? CultureManager.GetSystemMessage("NO_RESULT_FOR_CURRENT_FILTERS") : null;
        }

        /// <summary>
        /// Gets the graph XML file.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetGraphXml();

        protected abstract Dictionary<string, string> GetExcelItemList();

        protected virtual List<string> GetExcelExtraItemList() { return new List<string>(); }

        /// <summary>
        /// Prints the current report.
        /// </summary>
        protected override void Print()
        {
            try
            {
                base.Print();

                GraphPrint.Visible = true;

                UpdatePanelGraphPrint.Update();

                var sh = new ScriptHelper(this);

                sh.RegisterStartupScript("print", "PrintReport();");
            }
            catch (Exception ex) { ShowError(ex); }
        }
       
        /// <summary>
        /// Exports the Grid to CSV.
        /// Cuidado con esto porque rebindea la grilla sacandole el paging.
        /// </summary>
        protected override void ExportToCsv()
        {
            var builder = new GraphToCsvBuilder(Usuario.CsvSeparator);

            builder.GenerateHeader(CultureManager.GetMenu(VariableName), GetFilterValues());

            GetGraphCategoriesAndDatasets();

            builder.ExportGraph(XAxisLabel, YAxisLabel, GraphCategories, GraphDataSet);

            Session["CSV_EXPORT"] = builder.Build();
            Session["CSV_FILE_NAME"] = "report";

            OpenWin(String.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }

        protected override void ExportToExcel()
        {
            var path = HttpContext.Current.Request.Url.AbsolutePath;
            path = Path.GetFileNameWithoutExtension(path) + ".xlsx";

            var builder = new GridToExcelBuilder(path, Usuario.ExcelFolder);
            
            var list = GetExcelItemList();
            if (list.Count > 5000)
            {
                ShowInfo(CultureManager.GetLabel("EXCEL_DEMASIADOS_MENSAJES"));
                return;
            }
            var extraItems = GetExcelExtraItemList();

            builder.GenerateHeader(CultureManager.GetMenu(VariableName), GetFilterValues());
            builder.AddExcelItemList(list);
            builder.AddExcelExtraItemList(extraItems);

            SetExcelSessionVars(builder.CloseAndSave());

            OpenWin(String.Concat(ApplicationPath, "Common/exportExcel.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }

        protected void SetExcelSessionVars(string filename)
        {
            Session["TMP_FILE_NAME"] = filename;
            Session["CSV_FILE_NAME"] = CultureManager.GetMenu(Module.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Schedule()
        {
            CbSchedulePeriodicidad.Items.Clear();
            CbSchedulePeriodicidad.Items.Insert(0, new ListItem(CultureManager.GetLabel("DIARIO"), "D"));
            CbSchedulePeriodicidad.Items.Insert(1, new ListItem(CultureManager.GetLabel("SEMANAL"), "S"));
            CbSchedulePeriodicidad.Items.Insert(2, new ListItem(CultureManager.GetLabel("MENSUAL"), "M"));

            ModalSchedule.Show();
        }

        void BtScheduleGuardarClick(object sender, EventArgs e)
        {
            var reporte = GetReportType();           
            var empresa = GetEmpresa();
            var linea = GetLinea();

            var prog = new ProgramacionReporte
            {
                ReportName = SendReportTextBoxReportName.Text,
                Report = reporte,
                Periodicity = CbSchedulePeriodicidad.SelectedValue[0],
                Mail = TxtScheduleMail.Text,
                Empresa = empresa ?? linea.Empresa,
                Created = DateTime.Now,
                Description = GetDescription(reporte + " " + CbSchedulePeriodicidad.SelectedValue),
                Active = false,
                Format = RadioButtonHtml.Checked
                            ? ProgramacionReporte.FormatoReporte.Html
                            : ProgramacionReporte.FormatoReporte.Excel
            };

            prog.Vehicles = GetSelectedVehicles();
            prog.Drivers = GetSelectedDrivers();
            prog.MessageTypes = GetSelectedMessageTypes();
            prog.InCicle = GetCicleCheck();

            DAOFactory.ProgramacionReporteDAO.Save(prog);

            ModalSchedule.Hide();

            SendConfirmationMail(reporte,prog.Description);
        }
      
        private void SendConfirmationMail(string reportType, string description)
        {
            var configFile = Config.Mailing.ReportConfirmation;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);
            sender.Config.Subject = string.Format("Se ha creado un nuevo reporte programado ({0})", reportType);
            sender.Config.Body = description;

            sender.SendMail();
        }

        protected override void SendReportToMail()
        {
            SendReportModalPopupExtender.Show();
        }

        private void ButtonOkSendReportClick(object sender, EventArgs e)
        {
            var reportCommand = GenerateReportCommand(GetReportType());

            var queue = GetMailReportMsmq();

            if (queue == null) { throw new ApplicationException("No se pudo acceder a la cola"); }
            if (reportCommand != null) queue.Send(reportCommand);

            ModalSchedule.Hide();
        }

        private IReportCommand GenerateReportCommand(string reportType)
        {
            var reportId = DAOFactory.ProgramacionReporteDAO.GetReportIdByReportName("Manual");

            switch (reportType)
            {
                case "AccumulatedKilometersReport":
                    return new AccumulatedKilometersReportCommand
                    {
                        ReportId = reportId, //Id de reporte manual inactivo
                        CustomerId = GetCompanyId(),
                        Email = SendReportTextBoxEmail.Text,
                        FinalDate = GetToDateTime(),
                        InitialDate = GetSinceDateTime(),
                        InCicle = GetCicleCheck(),
                        VehiclesId = GetSelectedListByField("vehicles")
                    };
                case "MobilesTimeReport":
                    return new MobilesTimeReportCommand
                    {
                        ReportId = reportId,
                        CustomerId = GetCompanyId(),
                        Email = SendReportTextBoxEmail.Text,
                        FinalDate = GetToDateTime(),
                        InitialDate = GetSinceDateTime(),
                        VehiclesId = GetSelectedListByField("vehicles"),
                    };
                default:
                    return null;
            }
        }

        private string GetReportType()
        {
            switch (Page.ToString())
            {
                case "ASP.reportes_estadistica_mobileskilometers_aspx":
                    return "AccumulatedKilometersReport";
                case "ASP.reportes_estadistica_mobilestime_aspx":
                    return "MobilesTimeReport";
                default:
                    return Page.ToString();
            }
        }

        private IMessageQueue GetMailReportMsmq()
        {
            var queueName = Config.ReportMsmq.QueueName;
            var queueType = Config.ReportMsmq.QueueType;
            if (String.IsNullOrEmpty(queueName)) return null;

            var umq = new IMessageQueue(queueName);
            if (queueType.ToLower() == "xml") umq.Formatter = "XmlMessageFormatter";

            return !umq.LoadResources() ? null : umq;
        }

        /// <summary>
        /// Method to fill the categories and datasets properties needed by the CSV Export.
        /// </summary>
        protected abstract void GetGraphCategoriesAndDatasets();

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates chart based on search results.
        /// </summary>
        /// <returns></returns>
        private void CreateChart(object sender, ChartCreationEventArgs e)
        {
            try
            {
                e.ChartXml = FusionCharts.RenderChartHTML(ChartXmlDefinition, "", GetGraphXml(), "Report", Size.Width.ToString("#0"), Size.Heigth.ToString("#0"), false);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        /// <summary>
        /// Creates the charts as needed for its printed version.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateChartPrint(object sender, ChartCreationEventArgs e)
        {
            try
            {
                if (ReportObjectsList.Count <= 0) return;

                var graphXml = GetGraphXml();

                graphXml = graphXml.Replace("<graph", "<graph animation='0'");

                e.ChartXml = FusionCharts.RenderChartHTML(ChartXmlDefinition, "", graphXml, "ReportPrint", "800", "350", false);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        /// <summary>
        /// Adds a control to the page for size handling.
        /// </summary>
        private void AddSizeHandlingControl()
        {
            Size = new SizeHiddenField { ID = "size", DefaultWidth = DefaultWidth, DefaultHeight = DefaultHeight, MarginWidth = MarginWidth, MarginHeigth = MarginHeigth };

            Form.Controls.Add(Size);
        }
        
        #endregion
    }

    #endregion

    public abstract class SecuredMixedReportPage<T,TO>: SecuredGraphReportPage<T>
    {
        protected virtual List<TO> GetMixedReportResults() { return null; }

        protected override void ExportToExcel()
        {
            var path = HttpContext.Current.Request.Url.AbsolutePath;
            path = Path.GetFileNameWithoutExtension(path) + ".xlsx";

            var builder = new GridToExcelBuilder(path, Usuario.ExcelFolder);

            var list = GetExcelItemList();
            if (list.Count > 5000)
            {
                ShowInfo(CultureManager.GetLabel("EXCEL_DEMASIADOS_MENSAJES"));
                return;
            }

            builder.GenerateHeader(CultureManager.GetMenu(VariableName), GetFilterValues());
            builder.AddExcelItemList(list);

            var results = GetMixedReportResults();
            builder.GenerateColumns(results);
            builder.GenerateFields(results);

            SetExcelSessionVars(builder.CloseAndSave());

            OpenWin(String.Concat(ApplicationPath, "Common/exportExcel.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }
    }
}
