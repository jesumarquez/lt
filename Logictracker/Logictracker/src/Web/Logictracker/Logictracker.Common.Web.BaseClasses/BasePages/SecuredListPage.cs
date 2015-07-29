using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Web.BaseClasses.Util;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;
using Logictracker.Web.Helpers.ExportHelpers;

namespace Logictracker.Web.BaseClasses.BasePages
{
    /// <summary>
    /// Secured Listing Base Page.
    /// </summary>
    public abstract class SecuredListPage<T> : ApplicationSecuredPage, IGridded<T>
    {
        public GridUtils<T> GridUtils { get; set; }

        protected BaseListMasterPage MasterPage { get { return Master as BaseListMasterPage; } }

        protected virtual UpdatePanel UpdatePanelGrid { get { return MasterPage.UpdatePanelGrid; } }

        protected override InfoLabel LblInfo { get { return MasterPage.LblInfo; } }

        private List<T> _listObjects;

        private bool _needsBind;

        protected abstract List<T> GetListData();

        #region Function Specific Properties

        /// <summary>
        /// The abm page associated to the list.
        /// </summary>
        protected abstract string RedirectUrl { get; }

        protected virtual string ImportUrl { get { return string.Empty; } }

        protected virtual string ResourceName { get { return "Menu"; } }

        protected abstract string VariableName { get; }

        protected virtual bool HideSearch { get { return false; } }

        protected virtual bool OpenInNewWindow { get { return false; } }

        #endregion

        #region Toolbar Properties

        protected virtual bool AddButton { get { return true; } }
        protected virtual bool DeleteButton { get { return false; } }
        protected virtual bool DuplicateButton { get { return false; } }
        protected virtual bool CsvButton { get { return true; } }
        protected virtual bool PrintButton { get { return false; } }
        protected virtual bool ImportButton { get { return false; } }
        protected virtual bool StartAllButton { get { return false; } }
        protected virtual bool ExcelButton { get { return false; } }
        protected virtual bool EditButton { get { return false; } }
        protected virtual bool ListButton { get { return false; } }

        #endregion

        #region IGridded

        /// <summary>
        /// Data Grid
        /// </summary>
        public virtual C1GridView Grid { get { return MasterPage.ListGrid; } }

        /// <summary>
        /// Search string filter
        /// </summary>
        public virtual string SearchString { get { return MasterPage.TextBoxSearch.Text; } set { MasterPage.TextBoxSearch.Text = value; } }

        /// <summary>
        /// ViewState
        /// </summary>
        public StateBag StateBag { get { return ViewState; } }

        /// <summary>
        /// Data list
        /// </summary>
        public List<T> Data { get { return _listObjects ?? (_listObjects = GetListData()); } set { _listObjects = value; } }

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
        public virtual bool MouseOverRowEffect { get { return true; } }

        public virtual OutlineMode GridOutlineMode { get { return OutlineMode.None; } }

        public virtual bool HasTotalRow { get { return false; } }

        #endregion

        #region Page Events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            GridUtils = new GridUtils<T>(Grid, this);
            GridUtils.GenerateCustomColumns += GridUtilsGenerateCustomColumns;
            GridUtils.CreateRowTemplate += GridUtilsCreateRowTemplate;
            GridUtils.RowDataBound += GridUtilsRowDataBound;
            GridUtils.SelectedIndexChanged += GridUtilsSelectedIndexChanged;
            GridUtils.Binding += GridUtilsBinding;
            GridUtils.RowCommand += GridUtilsRowCommand;
        }
        
        protected override void OnPreLoad(EventArgs e)
        {
            if (MasterPage.ToolBar != null)
            {
                AddToolBarIcons();
                MasterPage.ToolBar.ItemCommand += ToolbarItemCommand;
                MasterPage.ToolBar.ResourceName = ResourceName;
                MasterPage.ToolBar.VariableName = VariableName;
            }

            base.OnPreLoad(e);

            if (!IsPostBack)
            {
                GridUtils.GenerateColumns();
                if (!GridUtils.AnyIncludedInSearch || HideSearch) MasterPage.PanelSearch.Visible = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            MasterPage.TextBoxSearch.TextChanged += FilterChangedHandler;

            base.OnLoad(e);

            if (!IsPostBack)
            {
                var filters = LoadFilters();
                if (filters != null)
                {
                    var searchFilter = filters["__search_string__"];
                    if (searchFilter != null) SearchString = (string)searchFilter;

                    if (SearchString != string.Empty)
                    {
                        var script = "$get('trFiltrosAvanzados').style.display = '';";
                        Page.ClientScript.RegisterStartupScript(typeof(string), ClientID + "_Create", script, true);
                    }
                    OnLoadFilters(filters);
                }

                Bind();
            }

            CreateHeaderTemplate(Grid.HeaderRow);
            foreach (C1GridViewRow row in Grid.Rows) CreateRowTemplate(row);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsPostBack && _needsBind) Bind();

            CreateHeaderTemplate(Grid.HeaderRow);

            var filters = LoadFilters();

            SaveFilters(GetFilters(filters));
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

        #endregion

        #region Grid Events

        void GridUtilsSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedIndexChanged();
            Session.Add("id", Grid.DataKeys[Grid.SelectedIndex].Value);
            if (OpenInNewWindow)
            {
                OpenWin(RedirectUrl, "_blank");
            }
            else
            {
                Response.Redirect(RedirectUrl);
            }
        }
        void GridUtilsRowDataBound(object sender, RowEventArgs<T> e) { OnRowDataBound(e.Grid, e.Event, e.DataItem); }
        void GridUtilsCreateRowTemplate(object sender, C1GridViewRowEventArgs e) { CreateRowTemplate(e.Row); }
        void GridUtilsGenerateCustomColumns(object sender, EventArgs e) { GenerateCustomColumns(); }
        void GridUtilsBinding(object sender, EventArgs e) { OnBinding(); }
        void GridUtilsRowCommand(object sender, C1GridViewCommandEventArgs e) { OnRowCommand(Grid, e); }

        #endregion

        #region Grid Actions

        protected virtual void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, T dataItem) { }
        protected virtual void CreateHeaderTemplate(C1GridViewRow row) { }
        protected virtual void CreateRowTemplate(C1GridViewRow row) { }
        protected virtual void GenerateCustomColumns() { }
        protected virtual void SelectedIndexChanged() { }
        protected virtual void OnBinding() { }
        protected virtual void OnRowCommand(C1GridView grid, C1GridViewCommandEventArgs e) { }


        #endregion

        #region Toolbar

        /// <summary>
        /// Adds tool bar action icons.
        /// </summary>
        protected void AddToolBarIcons()
        {
            if (Module.Add && AddButton) MasterPage.ToolBar.AddNewToolbarButton();

            if (Module.Add && ImportButton) MasterPage.ToolBar.AddImportToolbarButton();

            if (CsvButton) MasterPage.ToolBar.AddCsvToolbarButton();

            if (ExcelButton) MasterPage.ToolBar.AddExcelToolbarButton();

            if (DuplicateButton) MasterPage.ToolBar.AddDuplicateToolbarButton();

            if (DeleteButton) MasterPage.ToolBar.AddDeleteToolbarButton();

            if (PrintButton) MasterPage.ToolBar.AddPrintToolbarButton();

            if (StartAllButton) MasterPage.ToolBar.AddStartAllToolbarButton();

            if (EditButton) MasterPage.ToolBar.AddEditToolbarButton();

            if (ListButton) MasterPage.ToolBar.AddListToolbarButton();
        }

        /// <summary>
        /// Attends user actions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ToolbarItemCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case ToolBar.ButtonCommandNameNew: New(); break;
                case ToolBar.ButtonCommandNameCsv: ExportToCsv(); break;
                case ToolBar.ButtonCommandNameExcel: ExportToExcel(); break;
                case ToolBar.ButtonCommandNameSendReport: SendReportToMail(); break;
                case ToolBar.ButtonCommandNameDuplicate: Duplicate(); break;
                case ToolBar.ButtonCommandNameDelete: Delete(); break;
                case ToolBar.ButtonCommandNameImport: Import(); break;
                case ToolBar.ButtonCommandNameStartAll: StartAll(); break;
                case ToolBar.ButtonCommandNameEdit: Edit(); break;
                case ToolBar.ButtonCommandNameOpen: Open(); break;
            }
        }

        /// <summary>
        /// Adds a new object of the T associated to the list.
        /// </summary>
        protected virtual void New()
        {
            Session["id"] = null;
            if (OpenInNewWindow)
            {
                OpenWin(RedirectUrl, "_blank");
            }
            else
            {
                Response.Redirect(RedirectUrl);
            }
        }

        protected virtual void Duplicate() { }

        protected virtual void Delete() { }

        protected virtual void StartAll() { }

        protected virtual void Edit() { }
        
        protected virtual void Import()
        {
            Response.Redirect(ImportUrl);
        }

        protected virtual void Open() { }

        #endregion

        #region Generic Control Events

        /// <summary>
        /// Rebinds data for the newly seleced filter values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void FilterChangedHandler(object sender, EventArgs e) { if (IsPostBack) _needsBind = true; }

        #endregion

        #region CSV Export

        /// <summary>
        /// Exports the Grid to CSV.
        /// Cuidado con esto porque rebindea la grilla sacandole el paging.
        /// </summary>
        protected virtual void ExportToCsv()
        {
            var builder = new GridToCSVBuilder(Usuario.CsvSeparator);

            var g = Grid;
            var allowPaging = Grid.AllowPaging;

            g.AllowPaging = false;
            g.DataSource = Data;
            g.DataBind();

            GenerateCsvHeader(builder);

            builder.GenerateColumns(/*null, */g);
            builder.GenerateFields(g);

            SetCsvSessionVars(builder.Build());

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));

            g.AllowPaging = allowPaging;
        }

        protected void SetCsvSessionVars(string csv)
        {
            Session["CSV_EXPORT"] = csv;
            Session["CSV_FILE_NAME"] = CultureManager.GetMenu(Module.Name);
        }

        protected virtual  void ExportToExcel()
        {
            var path = HttpContext.Current.Request.Url.AbsolutePath;
            path = Path.GetFileNameWithoutExtension(path) + ".xlsx";

            var builder = new GridToExcelBuilder(path, Usuario.ExcelFolder);

            var list = GridUtils.Search(Data, SearchString);
            if (list.Count > 5000)
            {
                ShowInfo(CultureManager.GetLabel("EXCEL_DEMASIADOS_MENSAJES"));
                return;
            }

            builder.GenerateHeader(CultureManager.GetMenu(VariableName),new Dictionary<string, string>());
            builder.GenerateColumns(list);
            builder.GenerateFields(list);
            
            SetExcelSessionVars(builder.CloseAndSave());

            OpenWin(String.Concat(ApplicationPath, "Common/exportExcel.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }

        protected virtual void SendReportToMail()
        {
            var path = HttpContext.Current.Request.Url.AbsolutePath;
            path = Path.GetFileNameWithoutExtension(path) + ".xlsx";

            var builder = new GridToExcelBuilder(path, Usuario.ExcelFolder);

            var list = GridUtils.Search(Data, SearchString);

            builder.GenerateHeader(CultureManager.GetMenu(VariableName), new Dictionary<string, string>());
            builder.GenerateColumns(list);
            builder.GenerateFields(list);

            SetExcelSessionVars(builder.CloseAndSave());

            OpenWin(String.Concat(ApplicationPath, "Common/exportExcel.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }

        protected void SetExcelSessionVars(string filename)
        {
            Session["TMP_FILE_NAME"] = filename;
            Session["CSV_FILE_NAME"] = CultureManager.GetMenu(Module.Name);
        }

        /// <summary>
        /// Generates the header for the builder
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void GenerateCsvHeader(GridToCSVBuilder builder) { }

        #endregion 

        #region Filter Memory

        protected virtual FilterData GetFilters(FilterData data)
        {
            return null;
        }
        
        protected virtual void OnLoadFilters(FilterData data)
        { }

        private FilterData LoadFilters()
        {
            return FilterData.Load(this);
        }

        private void SaveFilters(FilterData data)
        {
            if (data == null) data = new FilterData();
            data.Add("__search_string__", SearchString);
            data.Save(this);
        }

        #endregion
    }
}