using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Geocoder.Core.VO;
using LinqToExcel;
using Logictracker.Configuration;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.CustomWebControls.Labels;
using C1.Web.UI.Controls.C1GridView;
using System.Text;
using System.Web.UI;

namespace Logictracker.Web.BaseClasses.BasePages
{
    /// <summary>
    /// Secured Import Base Page.
    /// </summary>
    public abstract class SecuredImportPage : ApplicationSecuredPage
    {
        #region MasterPage Properties

        protected BaseImportMasterPage MasterPage { get { return Master as BaseImportMasterPage; } }

        protected override InfoLabel LblInfo { get { return MasterPage.LblInfo; } }

        protected FileUpload FileUpload { get { return MasterPage.FileUpload; } }

        protected Button UploadButton { get { return MasterPage.UploadButton; } }

        protected Button ImportButton { get { return MasterPage.ImportButton; } }

        protected IButtonControl ClearButton { get { return MasterPage.ClearButton; } }

        protected Panel PanelMapping { get { return MasterPage.PanelMapping; } }

        protected Panel PanelImport { get { return MasterPage.PanelImport; } }

        protected Panel PanelUpload { get { return MasterPage.PanelUpload; } }

        protected DropDownList CbWorksheets { get { return MasterPage.CbWorksheets; } }

        protected C1GridView Grid { get { return MasterPage.Grid; } }

        protected CheckBox CheckHasHeader { get { return MasterPage.CheckHasHeader; } }

        protected DropDownList CbImportMode { get { return MasterPage.CbImportMode; } }

        protected UpdatePanel UpdatePanelGrid { get { return MasterPage.UpdatePanelGrid; } }
        #endregion

        #region Function Specific Properties

        /// <summary>
        /// The abm page associated to the list.
        /// </summary>
        protected abstract string RedirectUrl { get; }

        protected virtual string ResourceName { get { return "Menu"; } }

        protected abstract string VariableName { get; }

        protected virtual bool Redirect { get { return true; } }

        #endregion

        #region Toolbar Buttons
        
        protected virtual bool ListButton { get { return true; } } 

        #endregion

        #region Import Properties
        
        protected string FileName
        {
            get { return (string)(ViewState["FileName"] ?? (ViewState["FileName"] = string.Empty)); }
            set { ViewState["FileName"] = value; }
        }

        protected string CurrentWorkSheet
        {
            get
            {
                return CbWorksheets.SelectedIndex < 0 ? string.Empty : CbWorksheets.SelectedValue;
            }
        }

        protected FileTypes FileType
        {
            get { return (FileTypes)(ViewState["FileType"] ?? (ViewState["FileType"] = FileTypes.Excel97)); }
            set { ViewState["FileType"] = value; }
        }

        protected virtual string[] ImportModes
        {
            get { return new[]{"Default"}; }
        }

        protected string CurrentImportMode
        {
            get
            {
                return CbImportMode.SelectedIndex < 0 ? string.Empty : CbImportMode.SelectedValue;
            }
        }
        
        #endregion

        #region Page Events

        protected override void OnPreLoad(EventArgs e)
        {
            UploadButton.Click += UploadButtonClick;
            ImportButton.Click += ImportButtonClick;
            ClearButton.Click += ClearButtonOnClick;
            CbWorksheets.SelectedIndexChanged += CbWorksheetsSelectedIndexChanged;
            CbImportMode.SelectedIndexChanged += CbImportModeSelectedIndexChanged;

            if (MasterPage.ToolBar != null)
            {
                AddToolBarIcons();

                MasterPage.ToolBar.ItemCommand += ToolbarItemCommand;
                MasterPage.ToolBar.ResourceName = ResourceName;
                MasterPage.ToolBar.VariableName = VariableName;
            }

            base.OnPreLoad(e);
        }

        private void ClearButtonOnClick(object sender, EventArgs eventArgs)
        {
            foreach(C1GridViewRow row in Grid.Rows)
            {
                var combo = row.FindControl("cbMapping") as DropDownList;
                if (combo == null) continue;
                combo.SelectedIndex = 0;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            ScriptManager.GetCurrent(Page).AsyncPostBackTimeout = (int)TimeSpan.FromMinutes(30).TotalSeconds;

            if(!IsPostBack)
            {
                BindImportModes();
            }
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            SetPanels();
            base.OnPreRender(e);
        }

        #endregion

        #region Toolbar

        /// <summary>
        /// Adds tooblbar icons according to user privileges.
        /// </summary>
        protected virtual void AddToolBarIcons()
        {
            MasterPage.ToolBar.Controls.Clear();

            if (ListButton) MasterPage.ToolBar.AddListToolbarButton();

        }

        protected virtual void UpdateToolbar()
        {
            AddToolBarIcons();

            if (MasterPage.ToolBar.UpdatePanel != null) MasterPage.ToolBar.UpdatePanel.Update();
        }

        /// <summary>
        /// Handles toolbar actions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ToolbarItemCommand(object sender, CommandEventArgs e)
        {
            try
            {
                switch (e.CommandName)
                {
                    case "Open": Open(); break;
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        /// <summary>
        /// Show the list of all existing objects of the same T.
        /// </summary>
        private void Open() { Response.Redirect(RedirectUrl, false); }

        #endregion

        #region Upload

        protected void UploadButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (!FileUpload.HasFile) ThrowError("FILE_EXPECTED");
                if (FileUpload.PostedFile.ContentType != "application/vnd.ms-excel") ThrowError("FILE_BAD_EXTENSION");

                var path = Server.MapPath(Config.Directory.TmpDir);
                var filename = string.Format("{0}_{1}", DateTime.Now.ToString("yyyyMMdd_HHmmss"), FileUpload.FileName);

                FileType = Path.GetExtension(filename).ToUpper() == ".CSV" ? FileTypes.Csv : FileTypes.Excel97;
                FileName = Path.Combine(path, filename);

                //if (FileType == FileTypes.Csv) ThrowError("FILE_BAD_EXTENSION");

                FileUpload.SaveAs(FileName);

                LoadWorkSheets();
            }
            catch (Exception ex)
            {
                FileName = string.Empty;
                ShowError(ex);
            }
        } 

        #endregion

        #region Import

        protected void ImportButtonClick(object sender, EventArgs e)
        {
            try
            {
                ValidateMapping();

                var excel = new ExcelQueryFactory(FileName);

                var rows = CreateRows(excel);

                Import(rows);

                AfterImport();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        protected virtual void AfterImport()
        {
            try
            {
                if (Redirect)
                    Response.Redirect(RedirectUrl);
            } 
            catch (Exception)
            {
                // do nothing (como el redireccionamiento esta prohibido y tira excepcion, para que funcione igual y no tire error haciendolo lo catcheo).
            }
        }

        protected virtual List<ImportRow> CreateRows(ExcelQueryFactory excel)
        {
            IEnumerable<List<Cell>> sheet = CheckHasHeader.Checked
                                                ? excel.Worksheet(CurrentWorkSheet).Select(t=>t as List<Cell>)
                                                : excel.WorksheetNoHeader(CurrentWorkSheet).Select(t => t as List<Cell>);
            List<string> columns = null;
            switch (FileType)
            {
                case FileTypes.Excel97:
                    var xls = new ExcelQueryFactory(FileName);
                    columns = CheckHasHeader.Checked
                                  ? xls.GetColumnNames(CurrentWorkSheet).ToList()
                                  : xls.GetColumnNames(CurrentWorkSheet).Select((c, i) => "Column" + 1).ToList();
                    break;
                case FileTypes.Csv:
                    var csv = File.ReadAllLines(FileName);
                    if (csv[0].Contains('|'))
                    {
                        columns = GetDefaultHeader(1);
                        var sh = new List<List<Cell>>();
                        for (var i = 0; i < csv.Count(); i++)
                        {
                            var l = new List<Cell>();
                            var cell = new Cell(csv[i]);
                            l.Add(cell);
                            sh.Add(l);
                        }
                        sheet = sh;
                    }
                    else
                    {
                        columns = CheckHasHeader.Checked ? csv[0].Split(';').ToList() : GetDefaultHeader(csv[0].Split(';').Count());
                        sheet = BuildCsvSheet(sheet);
                    }
                    break;
            }

            var list = new List<ImportRow>(sheet.Count());

            var emptyCount = 0;

            foreach (var row in sheet)
            {
                var importRow = new ImportRow();
                for(var i = 0; i < columns.Count(); i++)
                {
                    var column = columns[i];
                    importRow[column] = row[i];
                }

                var empty = row.All(v => string.IsNullOrEmpty(v.Value.ToString().Trim()));
                if (empty) emptyCount++;
                else emptyCount = 0;

                if (emptyCount > 1) break;

                if (!empty && ValidateRow(importRow)) list.Add(importRow);
            }
            return list;
        }

        private static List<string> GetDefaultHeader(int cant)
        {
            var ret = new List<string>();
            for (var i = 0; i < cant; i++)
            {
                ret.Add("Column" + i);
            }
            return ret;
        }

        private static IEnumerable<List<Cell>> BuildCsvSheet(IEnumerable<List<Cell>> sheet)
        {
            var ret = new List<List<Cell>>();

            foreach (var list in sheet)
            {
                var sb = new StringBuilder();
                foreach (var c in list)
                {
                    if (sb.Length > 0) 
                        sb.Append(",");
                    
                    sb.Append(c.ToString());
                }
                
                var str = sb.ToString().Split(';');
                var l = new List<Cell>();
                for (var j = 0; j < str.Count(); j++)
                {
                    l.Add(new Cell(str[j]));   
                }
                ret.Add(l);
            }

            return ret;
        }

        protected virtual void ValidateMapping()
        {
            var fields = GetMappingFields();
            foreach(var field in fields)
            {
                var isMapped = IsMapped(field.Value);
                if(field.RequiredMapping && !isMapped)
                {
                    throw new ApplicationException("Falta mapear " + field.Name);
                }
                var hasDefault = HasDefault(field.Value);
                if(field.RequiredValue && (!isMapped && !hasDefault))
                {
                    throw new ApplicationException("Falta mapeo o valor default para " + field.Name);
                }
            }
            ValidateFilters();
        }
        protected virtual void ValidateFilters()
        {
            
        }

        protected abstract void Import(List<ImportRow> rows);

        protected virtual bool ValidateRow(ImportRow row) { return true; }

        #endregion

        #region Worksheet & Column Mapping
        
        protected void CbWorksheetsSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadColumns();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        void CbImportModeSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadColumns();
                OnImportModeChange();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        protected virtual void OnImportModeChange()
        {
        }

        protected virtual void LoadWorkSheets()
        {
            var excel = new ExcelQueryFactory(FileName);
            var sheets = new List<string>();
            switch (FileType)
            {
                case FileTypes.Excel97:
                    sheets = excel.GetWorksheetNames().ToList();
                    break;
                case FileTypes.Csv:
                    sheets.Add(excel.GetWorksheetNames().LastOrDefault());
                    break;
                default:
                    break;
            }
            
            CbWorksheets.DataSource = sheets;
            CbWorksheets.DataBind();

            LoadColumns();
        }
        protected virtual void LoadColumns()
        {
            var fields = GetMappingFields();
            Grid.RowDataBound += GridRowDataBound;
            Grid.DataSource = fields;
            Grid.DataBind();
        } 

        private void GridRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if(e.Row.RowType != C1GridViewRowType.DataRow) return;
            var combo = e.Row.FindControl("cbMapping") as DropDownList;
            var hidden = e.Row.FindControl("hidValue") as HiddenField;
            var textbox = e.Row.FindControl("txtDefault") as TextBox;
            
            var column = (FieldValue) e.Row.DataItem;

            e.Row.Cells[0].Text = (e.Row.RowIndex + 1).ToString("#0");
            e.Row.Cells[1].Text = column.Name;
            if(column.RequiredValue || column.RequiredMapping)
            {
                e.Row.Cells[0].Style.Add("font-weight", "bold");
                e.Row.Cells[1].Style.Add("font-weight", "bold");
            }
            hidden.Value = column.Value;
            textbox.Visible = !column.DisableDefault;
            BindFields(combo, e.Row.RowIndex);
        }

        protected virtual void BindFields(DropDownList combo, int index)
        {
            combo.Items.Clear();
            if (string.IsNullOrEmpty(FileName)) return;

            IEnumerable<string> columns = null; 
            switch (FileType)
            {
                case FileTypes.Excel97:
                    var excel = new ExcelQueryFactory(FileName);
                    columns = excel.GetColumnNames(CurrentWorkSheet).ToList();
                    break;
                case FileTypes.Csv:
                    var csv = File.ReadAllLines(FileName);
                    columns = csv[0].Split(';');
                    break;
            }
            combo.Items.Add(string.Empty);
            foreach (var fieldValue in columns)
            {
                combo.Items.Add(fieldValue);
            }

            if(combo.Items.Count > index + 1) combo.SelectedIndex = index + 1;
        }

        protected abstract List<FieldValue> GetMappingFields();

        #endregion

        protected string GetValue(ImportRow row, string value)
        {
            return HasDefault(value) ? GetDefaultByValue(value) : row[GetColumnByValue(value)];
        }

        protected string GetColumnByValue(string value)
        {
            foreach (C1GridViewRow row in Grid.Rows)
            {
                var hidValue = row.FindControl("hidValue") as HiddenField;
                if (hidValue == null) continue;
                if (hidValue.Value == value)
                {
                    return (row.FindControl("cbMapping") as DropDownList).SelectedValue;
                }
            }
            return string.Empty;
        }
        protected string GetDefaultByValue(string value)
        {
            foreach (C1GridViewRow row in Grid.Rows)
            {
                var hidValue = row.FindControl("hidValue") as HiddenField;
                if (hidValue == null) continue;
                if (hidValue.Value == value)
                {
                    return (row.FindControl("txtDefault") as TextBox).Text;
                }
            }
            return string.Empty;
        }
        
        protected Data GetLogiclinkData(Entities entity, ImportRow row)
        {
            var data = new Data {Entity = (int)entity};
            foreach (var field in GetMappingFields())
            {
                if (field.LogiclinkEntity != entity) continue;
                var value = row[GetColumnByValue(field.Value)];
                if (value == string.Empty)
                {
                    value = GetDefaultByValue(field.Value);
                    if (value == string.Empty)
                    {
                        continue;
                    }
                }
                if(field.LogiclinkProperty == 0)
                {
                    value = string.Format("{0}=\"{1}\"", field.Name, value);
                }
                data.Add(field.LogiclinkProperty, value);
            }
            return data;
        }
        protected bool IsMapped(string value)
        {
            return !string.IsNullOrEmpty(GetColumnByValue(value));
        }
        protected bool HasDefault(string value)
        {
            return !string.IsNullOrEmpty(GetDefaultByValue(value));
        }
        private void SetPanels()
        {
            PanelImport.Visible = !string.IsNullOrEmpty(FileName);
        }

        protected void BindImportModes()
        {
            CbImportMode.Items.Clear();
            foreach (var mode in ImportModes)
            {
                CbImportMode.Items.Add(new ListItem(mode, mode));
            }
            OnImportModeChange();
            CbImportMode.Enabled = ImportModes.Length > 1;
        }

        protected void UpdateGrid()
        {
            LoadColumns();
            UpdatePanelGrid.Update();
        }

        #region Referencias Geograficas
        public ReferenciaGeografica GetReferenciaGeografica(TipoReferenciaGeografica tipoReferencia, ReferenciaGeografica geoRef, Empresa empresa, Linea linea, string codigo, string descripcion, DateTime? vigenciaDesde, DateTime? vigenciaHasta, double latitud, double longitud)
        {
            geoRef = GetReferenciaGeografica(tipoReferencia, geoRef, empresa, linea, codigo, descripcion, vigenciaDesde, vigenciaHasta);

            if (geoRef.Direccion == null)
            {
                var nomencladas = NomenclarByLatLon(latitud, longitud);
                SetDireccion(geoRef, nomencladas);
            }
            return geoRef;
        }
        public ReferenciaGeografica GetReferenciaGeografica(TipoReferenciaGeografica tipoReferencia, ReferenciaGeografica geoRef, Empresa empresa, Linea linea, string codigo, string descripcion, DateTime? vigenciaDesde, DateTime? vigenciaHasta, string direccion)
        {
            geoRef = GetReferenciaGeografica(tipoReferencia, geoRef, empresa, linea, codigo, descripcion, vigenciaDesde, vigenciaHasta);
            if (geoRef.Direccion == null)
            {
                var nomencladas = NomenclarByDireccion(direccion);
                SetDireccion(geoRef, nomencladas);
            }
            return geoRef;
        }
        public ReferenciaGeografica GetReferenciaGeografica(TipoReferenciaGeografica tipoReferencia, ReferenciaGeografica geoRef, Empresa empresa, Linea linea, string codigo, string descripcion, DateTime? vigenciaDesde, DateTime? vigenciaHasta, string calle, int altura, string esquina, string partido, string provincia)
        {
            geoRef = GetReferenciaGeografica(tipoReferencia, geoRef, empresa, linea, codigo, descripcion, vigenciaDesde, vigenciaHasta);
            if (geoRef.Direccion == null)
            {
                var nomencladas = NomenclarByCalle(calle, altura, esquina, partido, provincia);
                SetDireccion(geoRef, nomencladas);
            }
            return geoRef;
        }

        public ReferenciaGeografica GetReferenciaGeografica(TipoReferenciaGeografica tipoReferencia, ReferenciaGeografica geoRef, Empresa empresa, Linea linea, string codigo, string descripcion, DateTime? vigenciaDesde, DateTime? vigenciaHasta)
        {
            if (geoRef == null)
            {
                geoRef = new ReferenciaGeografica
                             {
                                 Codigo = codigo,
                                 Empresa = empresa,
                                 Linea = linea,
                                 EsFin = tipoReferencia.EsFin,
                                 EsInicio = tipoReferencia.EsInicio,
                                 EsIntermedio = tipoReferencia.EsIntermedio,
                                 Icono = tipoReferencia.Icono,
                                 InhibeAlarma = tipoReferencia.InhibeAlarma,
                                 TipoReferenciaGeografica = tipoReferencia,
                                 Color = tipoReferencia.Color
                             };
                foreach (TipoReferenciaVelocidad maxima in tipoReferencia.VelocidadesMaximas)
                    geoRef.VelocidadesMaximas.Add(new ReferenciaVelocidad
                                                      {
                                                          ReferenciaGeografica = geoRef,
                                                          TipoVehiculo = maxima.TipoVehiculo,
                                                          VelocidadMaxima = maxima.VelocidadMaxima
                                                      });
            }

            geoRef.Baja = false;
            geoRef.Descripcion = descripcion;
            geoRef.Observaciones = string.Empty;

            UpdateVigencia(geoRef, vigenciaDesde, vigenciaHasta);

            return geoRef;
        }
        private void SetDireccion(ReferenciaGeografica geoRef, IList<DireccionVO> nomencladas)
        {
            if (nomencladas.Count == 1)
            {
                var direccionNomenclada = nomencladas[0];

                var dir = new Direccion
                              {
                                  Altura = direccionNomenclada.Altura,
                                  Calle = direccionNomenclada.Calle,
                                  Descripcion = direccionNomenclada.Direccion.Truncate(128),
                                  IdCalle = direccionNomenclada.IdPoligonal,
                                  IdEntrecalle = -1,
                                  IdEsquina = direccionNomenclada.IdEsquina,
                                  IdMapa = (short)direccionNomenclada.IdMapaUrbano,
                                  Latitud = direccionNomenclada.Latitud,
                                  Longitud = direccionNomenclada.Longitud,
                                  Pais = "Argentina",
                                  Partido = direccionNomenclada.Partido,
                                  Provincia = direccionNomenclada.Provincia,
                                  Vigencia = new Vigencia { Inicio = DateTime.Now }
                              };

                var pol = new Poligono
                              {
                                  Radio = 100,
                                  Vigencia = new Vigencia { Inicio = DateTime.Now }
                              };
                pol.AddPoints(new[]
                                  {
                                      new PointF((float) direccionNomenclada.Longitud,
                                                 (float) direccionNomenclada.Latitud)
                                  });


                geoRef.AddHistoria(dir, pol, DateTime.UtcNow);
            }
        }

        private void UpdateVigencia(ReferenciaGeografica referencia, DateTime? vigenciaDesde, DateTime? vigenciaHasta)
        {
            if (vigenciaDesde.HasValue || vigenciaHasta.HasValue)
            {
                if (referencia.Vigencia == null || !referencia.Vigencia.Vigente(DateTime.UtcNow))
                {
                    referencia.Vigencia = new Vigencia { Inicio = vigenciaDesde, Fin = vigenciaHasta };
                }
                else
                {
                    var desde = vigenciaDesde.HasValue ? vigenciaDesde.Value : DateTime.MaxValue;
                    if (referencia.Vigencia.Inicio.HasValue && referencia.Vigencia.Inicio.Value < desde) desde = referencia.Vigencia.Inicio.Value;
                    if (desde == DateTime.MaxValue) desde = DateTime.UtcNow;

                    var hasta = vigenciaHasta.HasValue ? vigenciaHasta.Value : DateTime.MinValue;
                    if (referencia.Vigencia.Fin.HasValue && referencia.Vigencia.Fin.Value > hasta) hasta = referencia.Vigencia.Fin.Value;
                    if (hasta == DateTime.MinValue) hasta = DateTime.UtcNow;
                    referencia.Vigencia = new Vigencia { Inicio = desde, Fin = hasta };
                }
                DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(referencia);
            }
        }
        protected static IList<DireccionVO> NomenclarByLatLon(double latitud, double longitud)
        {
            return new List<DireccionVO> { GeocoderHelper.Cleaning.GetDireccionMasCercana(latitud, longitud) };
        }
        protected static IList<DireccionVO> NomenclarByCalle(string calle, int altura, string esquina, string partido, string provincia)
        {
            return GeocoderHelper.Cleaning.NomenclarDireccion(calle, altura, esquina ?? string.Empty, partido ?? string.Empty, provincia ?? string.Empty);
        }
        protected static IList<DireccionVO> NomenclarByDireccion(string direccion)
        {
            return GeocoderHelper.Cleaning.GetSmartSearch(direccion);
        }
        #endregion

        #region SubClasses

        protected enum FileTypes
        {
            Excel97,
            Csv
        }

        protected class FieldValue
        {
            public string Name { get; set;}
            public string Value { get; set;}
            public int LogiclinkProperty { get; set; }
            public Entities LogiclinkEntity { get; set; }
            public bool RequiredValue { get; set; } // Default o mapping
            public bool RequiredMapping { get; set; }
            public bool DisableDefault { get; set; }

            public FieldValue(string name)
            {
                Name = name;
                Value = name.Replace(' ', '_');
            }
            public FieldValue(string name, int logiclinkProperty, Entities entity)
                :this(name)
            {
                LogiclinkProperty = logiclinkProperty;
                LogiclinkEntity = entity;
            }
        }

        protected class ImportRow
        {
            private readonly Dictionary<string, string> _fields = new Dictionary<string, string>();

            public IEnumerable<string> Fields { get { return _fields.Keys; } }
            public IEnumerable<string> Values { get { return _fields.Values; } }

            public string this[string field]
            {
                get
                {
                    return _fields.ContainsKey(field) ? _fields[field] : string.Empty;
                }
                set
                {
                    if (_fields.ContainsKey(field)) _fields[field] = value;
                    else _fields.Add(field, value);
                }
            }

            public short? GetInt16(string field)
            {
                short cellValue;
                return short.TryParse(this[field], out cellValue) ? (short?) cellValue : null;
            }

            public int? GetInt32(string field)
            {
                int cellValue;
                return int.TryParse(this[field], out cellValue) ? (int?)cellValue : null;
            }
            public long? GetInt64(string field)
            {
                long cellValue;
                return long.TryParse(this[field], out cellValue) ? (long?)cellValue : null;
            }
            public double? GetDouble(string field)
            {
                double cellValue;
                return double.TryParse(this[field], out cellValue) ? (double?)cellValue : null;
            }
            public string GetString(string field)
            {
                return this[field];
            }
            public DateTime? GetDateTime(string field)
            {
                DateTime cellValue;
                return DateTime.TryParse(this[field], out cellValue) ? (DateTime?)cellValue : null;
            }
        } 

        #endregion
    }

    public static class StringExtensions
    {
        public static string Truncate(this string text, int length)
        {
            if (text == null) return string.Empty;
            return text.Length > length ? text.Substring(0, length) : text;
        }
    }
}