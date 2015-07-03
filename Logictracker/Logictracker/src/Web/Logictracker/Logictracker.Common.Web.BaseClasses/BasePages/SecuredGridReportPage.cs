using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.Layers.MessageQueue;
using Logictracker.Mailing;
using Logictracker.Reports.Messaging;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.Util;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.CustomWebControls.Helpers;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;
using Logictracker.Web.Helpers;
using Logictracker.Web.Helpers.ExportHelpers;
using Logictracker.Messaging;

namespace Logictracker.Web.BaseClasses.BasePages
{
    public abstract class SecuredGridReportPage<T> : SecuredBaseReportPage<T>, IGridded<T>
    {
        public GridUtils<T> GridUtils { get; set; }

        protected BaseReportGridMasterPage MasterPage { get { return Master as BaseReportGridMasterPage; } }

        protected virtual UpdatePanel UpdatePanelGrid { get { return MasterPage.UpdatePanelReport; } }
        protected virtual UpdatePanel UpdatePanelPrint { get { return MasterPage.UpdatePanelPrint; } }
        protected virtual C1GridView GridPrint { get { return MasterPage != null ? MasterPage.GridPrint : null; } }
        protected override InfoLabel LblInfo { get { return MasterPage.LblInfo; } }
        protected override InfoLabel NotFound { get { return MasterPage.LblInfo; } }
        protected override ToolBar ToolBar { get { return MasterPage.ToolBar; } }
        protected override ResourceButton BtnSearch { get { return MasterPage.btnSearch; } }
        
        protected DropDownList CbSchedulePeriodicidad { get { return MasterPage.cbSchedulePeriodicidad; } }
        protected TextBox TxtScheduleMail { get { return MasterPage.txtScheduleMail; } }
        protected ResourceButton BtScheduleGuardar { get { return MasterPage.btScheduleGuardar; } }
        protected ModalPopupExtender ModalSchedule { get { return MasterPage.modalSchedule; } }

        protected TextBox SendReportTextBoxEmail { get { return MasterPage.SendReportTextBoxEmail; } }
        protected TextBox SendReportTextBoxReportName{ get { return MasterPage.SendReportTextBoxReportName; } }
        protected ModalPopupExtender SendReportModalPopupExtender { get { return MasterPage.SendReportModalPopupExtender; } }
        protected ResourceButton SendReportOkButton { get { return MasterPage.SendReportOkButton; } }
        
        #region IGridded

        /// <summary>
        /// Data Grid
        /// </summary>
        public virtual C1GridView Grid { get { return MasterPage.Grid; } }

        /// <summary>
        /// Search string filter
        /// </summary>
        public virtual string SearchString { get { return TextBoxSearch != null ? TextBoxSearch.Text: string.Empty; } set { if(TextBoxSearch != null) TextBoxSearch.Text = value; } }


        /// <summary>
        /// ViewState
        /// </summary>
        public StateBag StateBag { get { return ViewState; } }

        /// <summary>
        /// Data list
        /// </summary>
        public List<T> Data
        {
            get
            {
                if (ReportObjectsList == null || ReportObjectsList.Count == 0)
                    ReportObjectsList = GetResults();

                return ReportObjectsList;
            } 
            set { ReportObjectsList = value;}
        }

        /// <summary>
        /// Number of rows on each Grid Page.
        /// </summary>
        public virtual int PageSize { get { return 50; } }

        /// <summary>
        /// Click on Rows Generates Event
        /// </summary>
        public virtual bool SelectableRows { get { return true; } }

        /// <summary>
        /// Mouse over grid rows changes row style
        /// </summary>
        public virtual bool MouseOverRowEffect { get { return false; } }

        public virtual OutlineMode GridOutlineMode { get { return OutlineMode.None; } }

        public virtual bool HasTotalRow { get { return false; } }
        //protected abstract bool SendReportButton { get; }

        #endregion

        #region Page Event

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            GridUtils = new GridUtils<T>(Grid, this);
            GridUtils.GenerateCustomColumns += GridUtils_GenerateCustomColumns;
            GridUtils.CreateRowTemplate += GridUtils_CreateRowTemplate;
            GridUtils.RowDataBound += GridUtils_RowDataBound;
            GridUtils.SelectedIndexChanged += GridUtils_SelectedIndexChanged;
            GridUtils.Binding += GridUtils_Binding;
            GridUtils.AggregateCustomFormat += GridUtils_AggregateCustomFormat;
            if (ScheduleButton) BtScheduleGuardar.Click += BtScheduleGuardarClick;
            if (SendReportButton) SendReportOkButton.Click += ButtonOkSendReportClick;
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            if (!IsPostBack)
            {
                GridUtils.GenerateColumns();
                if ((!GridUtils.AnyIncludedInSearch || HideSearch) && PanelSearch != null) PanelSearch.Visible = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (GridPrint != null) GridPrint.RowDataBound += GridUtils.Grid_RowDataBound;

            base.OnLoad(e);
        }

        #endregion

        #region Bind

        /// <summary>
        /// Binds object to be listed.
        /// </summary>
        protected virtual void Bind()
        {
            try
            {
                GridUtils.Reset();
                GridUtils.Bind();
                UpdatePanelGrid.Update();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            ReportObjectsList = new List<T>();
            Grid.DataSource = ReportObjectsList;
            Grid.DataBind();
            try
            {
                ReportObjectsList = GetResults();
                Bind();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        #endregion

        #region Grid Events

        void GridUtils_SelectedIndexChanged(object sender, EventArgs e) { SelectedIndexChanged(); }
        void GridUtils_RowDataBound(object sender, RowEventArgs<T> e) { OnRowDataBound(e.Grid, e.Event, e.DataItem); }
        void GridUtils_CreateRowTemplate(object sender, C1GridViewRowEventArgs e) { CreateRowTemplate(e.Row); }
        void GridUtils_GenerateCustomColumns(object sender, EventArgs e) { GenerateCustomColumns(); }
        void GridUtils_Binding(object sender, EventArgs e) { OnBinding(); }
        void GridUtils_AggregateCustomFormat(object sender, CustomFormatEventArgs e)
        {
            e.FormattedString = OnCustomFormatString(e.AggregateHandler, e.Type, e.Value);
        }

        #endregion

        #region Grid Actions

        protected virtual void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, T dataItem) { }
        protected virtual void CreateHeaderTemplate(C1GridViewRow row) { }
        protected virtual void CreateRowTemplate(C1GridViewRow row) { }
        protected virtual void GenerateCustomColumns() { }
        protected virtual void SelectedIndexChanged() { }
        protected virtual void OnBinding() { if (ReportObjectsList == null || ReportObjectsList.Count == 0) ReportObjectsList = GetResults(); }
        protected virtual string OnCustomFormatString(AggregateHandler aggregate, Type type, double value) { return value.ToString(); }

        #endregion
              
        #region Printing & Exporting

        protected override void Print()
        {
            base.Print();

            var gridPrintUtils = new GridUtils<T>(MasterPage.GridPrint, this);
            gridPrintUtils.GenerateColumns();
            gridPrintUtils.Reset();

            gridPrintUtils.CopyColumns(Grid);

            try
            {
                gridPrintUtils.Bind();

                UpdatePanelPrint.Update();

                var sh = new ScriptHelper(this);

                sh.RegisterStartupScript("print", "PrintReport();");
            }
            catch (Exception ex) { ShowError(ex); }
        }

        protected override void ExportToExcel()
        {
            Logger.Debug("ExportToExcel start");
            var path = HttpContext.Current.Request.Url.AbsolutePath;
            path = Path.GetFileNameWithoutExtension(path) + ".xlsx";

            var builder = new GridToExcelBuilder(path, Usuario.ExcelFolder);

            var list = GridUtils.Search(Data, SearchString);

            Logger.Debug("ExportToExcel builder.GenerateHeader");
            builder.GenerateHeader(CultureManager.GetMenu(VariableName), GetFilterValues());
            Logger.Debug("ExportToExcel builder.GenerateColumns");
            builder.GenerateColumns(list);
            Logger.Debug("ExportToExcel builder.GenerateFields");
            //  Hay que mejorar este GenerateFields
            builder.GenerateFields(list, CustomExportFormat);

            Logger.Debug("ExportToExcel SetExcelSessionVars");
            SetExcelSessionVars(builder.CloseAndSave());

            OpenWin(String.Concat(ApplicationPath, "Common/exportExcel.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
            Logger.Debug("ExportToExcel end");
        }

        /// <summary>
        /// Exports the Grid to CSV.
        /// Cuidado con esto porque rebindea la grilla sacandole el paging.
        /// </summary>
        protected override void ExportToCsv()
        {
            var builder = new GridToCSVBuilder(Usuario.CsvSeparator);

            var g = Grid;
            var allowPaging = Grid.AllowPaging;

            var list = GridUtils.Search(Data, SearchString);

            g.AllowPaging = false;
            g.DataSource = list;
            g.DataBind();

            builder.GenerateHeader(CultureManager.GetMenu(VariableName), GetFilterValues());

            builder.GenerateColumns(g);
            builder.GenerateFields(g);

            SetCsvSessionVars(builder.Build());

            OpenWin(String.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));

            g.AllowPaging = allowPaging;
        }

        protected virtual string CustomExportFormat(Type type, object value)
        {
            return value != null ? value.ToString() : string.Empty;
        }
        
        protected void SetCsvSessionVars(string csv)
        {
            Session["CSV_EXPORT"] = csv;
            Session["CSV_FILE_NAME"] = CultureManager.GetMenu(Module.Name);
        }

        protected void SetExcelSessionVars(string filename)
        {
            Session["TMP_FILE_NAME"] = filename;
            Session["CSV_FILE_NAME"] = CultureManager.GetMenu(Module.Name);
        }
        
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
            string reporte;

            switch (Page.ToString())
            {
                case "ASP.reportes_accidentologia_mensajesvehiculo_aspx":
                    reporte = "Mensajes Vehículo";
                    break;
                case "ASP.reportes_accidentologia_infractionsdetails_aspx":
                    reporte = "Detalle de Infracciones";
                    break;
                case "ASP.reportes_accidentologia_operatorranking_aspx":
                    reporte = "Ranking de Choferes";
                    break;
                case "ASP.reportes_combustibleenpozos_consistenciastockpozo_aspx":
                    reporte = "Consistencia de Pozos";
                    break;
                case "ASP.reportes_combustibleenpozos_eventoscombustible_aspx":
                    reporte = "Eventos Combustible";
                    break;
                case "ASP.reportes_estadistica_mobilepoishistoric_aspx":
                    reporte = "Cercanía a Puntos de Interés";
                    break;
                case "ASP.reportes_datosoperativos_eventos_aspx":
                    reporte = "EventsReport";
                    break;
                case "ASP.reportes_datosoperativos_geocercasevents_aspx":
                    reporte = "Reporte de Geocercas";
                    break;
                case "ASP.reportes_estadistica_reporteodometros_aspx":
                    reporte = "Reporte de Odometros";
                    break;
                case "ASP.reportes_estadistica_actividadvehicular_aspx":
                    reporte = "Reporte de Actividad Vehicular";
                    break;
                case "ASP.reporte_estadistica_accidentologiaresumenvehicular_aspx":
                    reporte = "Resumen Vehicular";
                    break;
                case "ASP.reportes_ciclologistico_duracionestadosticket_aspx":
                    reporte = "Control de Ciclo";
                    break;
                default:
                    reporte = Page.ToString();
                    break;
            }

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
                               Description = SendReportTextBoxReportName.Text,
                               //Description = BuildDescription(),
                               Active = false
                           };

            prog.Vehicles = GetSelectedVehicles();
            prog.Drivers = GetSelectedDrivers();
            prog.MessageTypes = GetSelectedMessageTypes();



            //var parametros = new StringBuilder();

            //// PARAMETROS PARA CREAR REPORTE
            //var filtros = GetFilterValuesProgramados();

            //foreach (var key in filtros.Keys)
            //{
            //    if (!parametros.ToString().Equals(""))
            //        parametros.Append("&");

            //    parametros.Append(key + "=" + filtros[key] + "");
            //}

            //prog.Drivers = parametros.ToString();

            //// PARAMETROS PARA CREAR ENCABEZADO
            //parametros = new StringBuilder();
            //filtros = GetFilterValues();
            //filtros.Remove("Desde");
            //filtros.Remove("Hasta");

            //foreach (var key in filtros.Keys)
            //{
            //    if (!parametros.ToString().Equals(""))
            //        parametros.Append("&");

            //    parametros.Append(key + "=" + filtros[key] + "");
            //}

            //prog.MessageTypes = parametros.ToString();

            DAOFactory.ProgramacionReporteDAO.Save(prog);

            ModalSchedule.Hide();

            SendConfirmationMail(prog);
        }

        private string BuildDescription()
        {
            var desc = new StringBuilder();
            desc.AppendFormat("Empresa : {0} - Vehiculos: ", GetEmpresa().RazonSocial);

            foreach (var vehicleName in GetSelectedVehicleNames())
            {
                desc.Append(vehicleName + ",");
            }
            
            desc.Append(" Mensajes: ");
            
            foreach (var messageName in GetSelectedMessageNames())
            {
                desc.Append(messageName + ",");
            }

            desc.Append(" Conductores: ");

            foreach (var driverName in GetSelectedDriverNames())
            {
                desc.Append(driverName + ",");
            }

            return desc.ToString();
        }

        private void SendConfirmationMail(ProgramacionReporte prog)
        {
            var configFile = Config.Mailing.ReportConfirmation;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);

            sender.Config.Subject = "Se ha creado un nuevo reporte programado";
            var body = new StringBuilder("Datos del nuevo reporte programado:\n\n");
            body.AppendFormat("Conductores: {0} \n",prog.Drivers);
            body.AppendFormat("Correo detino: {0} \n",prog.Mail);
            body.AppendFormat("Tipos de mensajes: {0} \n",prog.MessageTypes);
            body.AppendFormat("Reporte: {0} \n", prog.Report);
            body.AppendFormat("Vehiculos: {0} \n", prog.Vehicles);
            body.AppendFormat("Creado: {0} \n", prog.Created.ToString());
            body.AppendFormat("Empresa: {0} \n", prog.Empresa.RazonSocial);

            sender.Config.Body = body.ToString();
            
            sender.SendMail();
        }

        protected override void SendReportToMail()
        {
            SendReportModalPopupExtender.Show();
        }

        private void ButtonOkSendReportClick(object sender, EventArgs e)
        {
            var eventReportCmd = new EventReportCommand()
            {
                BaseId = 0,
                CustomerId = GetCompanyId(),
                Email = SendReportTextBoxEmail.Text,
                DriversId = GetSelectedListByField("drivers"),
                FinalDate = GetToDateTime(),
                InitialDate = GetSinceDateTime(),
                MessagesId = GetSelectedListByField("messages"),
                VehiclesId = GetSelectedListByField("vehicles")
            };

            var queue = GetMailReportMsmq();
            if (queue == null)
            {
                throw new ApplicationException("No se pudo crear la cola");
            }
            queue.Send(eventReportCmd);

            ModalSchedule.Hide();

            //SendConfirmationMail(eventReportCmd);
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

        #endregion

    }
}