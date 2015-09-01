using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Geocoder.Core.VO;
using LinqToExcel;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Services.Helpers;
using System.Drawing;
using Logictracker.Configuration;
using Logictracker.Web.BaseClasses.BaseControls;

namespace Logictracker.App_Controls
{
    public partial class App_Controls_DireccionImport : BaseUserControl
    {
        private const string Reporte = "Procesado: {0} de {8}<br/> Nomencladas: {1} ({2}%)<br/>Ambiguas: {3} ({4}%)<br/>Sin Nomenclar: {5} ({6}%)<br/>Sin procesar: {9} ({10}%)<br/>Tiempo: {7}<br/>";
        private const string Result = "{0};{1};{2};{3};{4};{5};{6};{7}<br/>";
        private string FileName
        {
            get { return (string)(ViewState["FileName"] ??(ViewState["FileName"] = string.Empty)); }
            set { ViewState["FileName"] = value; }
        }

        private int Index
        {
            get { return (int)(ViewState["Index"] ?? 0); }
            set { ViewState["Index"] = value; }
        }
        private int TotalCount
        {
            get { return (int)(ViewState["TotalCount"] ?? 0); }
            set { ViewState["TotalCount"] = value; }
        }
        private int Ambiguas
        {
            get { return (int)(ViewState["Ambiguas"] ?? 0); }
            set { ViewState["Ambiguas"] = value; }
        }
        private int Nomencladas
        {
            get { return (int)(ViewState["Nomencladas"] ?? 0); }
            set { ViewState["Nomencladas"] = value; }
        }
        private int SinNomenclar
        {
            get { return (int)(ViewState["SinNomenclar"] ?? 0); }
            set { ViewState["SinNomenclar"] = value; }
        }
        private int SinProcesar
        {
            get { return (int)(ViewState["SinProcesar"] ?? 0); }
            set { ViewState["SinProcesar"] = value; }
        }
        private long Start
        {
            get { return (long)(ViewState["Start"] ?? Convert.ToInt64(0)); }
            set { ViewState["Start"] = value; }
        }

        private string CurrentWorkSheet{get { return cbWorksheets.SelectedIndex < 0 ? string.Empty :cbWorksheets.SelectedValue; }}
        private string ColumnaDescripcion{get { return cbDescripcion.SelectedIndex < 0 ? string.Empty : cbDescripcion.SelectedValue; }}
        private string ColumnaCodigo{get { return cbCodigo.SelectedIndex < 0 ? string.Empty : cbCodigo.SelectedValue; }}
        private string ColumnaCalle { get { return cbCalle.SelectedIndex < 0 ? string.Empty : cbCalle.SelectedValue; } }
        private string ColumnaAltura { get { return cbAltura.SelectedIndex < 0 ? string.Empty : cbAltura.SelectedValue; } }
        private string ColumnaEsquina { get { return cbEsquina.SelectedIndex < 0 ? string.Empty : cbEsquina.SelectedValue; } }
        private string ColumnaPartido { get { return cbPartido.SelectedIndex < 0 ? string.Empty : cbPartido.SelectedValue; } }
        private string ColumnaProvincia { get { return cbProvincia.SelectedIndex < 0 ? string.Empty : cbProvincia.SelectedValue; } }
        private string ColumnaLatitud { get { return cbLatitud.SelectedIndex < 0 ? string.Empty : cbLatitud.SelectedValue; } }
        private string ColumnaLongitud { get { return cbLongitud.SelectedIndex < 0 ? string.Empty : cbLongitud.SelectedValue; } }
        private string ColumnaDesde { get { return cbDesde.SelectedIndex < 0 ? string.Empty : cbDesde.SelectedValue; } }
        private string ColumnaHasta { get { return cbHasta.SelectedIndex < 0 ? string.Empty : cbHasta.SelectedValue; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(Page).AsyncPostBackTimeout = (int)TimeSpan.FromMinutes(30).TotalSeconds;
            Page.ClientScript.RegisterStartupScript(typeof(string), "processFunction",
                                                    "function process() { " +
                                                    Page.ClientScript.GetPostBackEventReference(btProcess, string.Empty) + "; }",
                                                    true);
        }

        protected void BtUploadClick(object sender, EventArgs e)
        {
            if (!filExcel.HasFile) return;
            if (filExcel.PostedFile.ContentType != "application/vnd.ms-excel") return;

            FileName = string.Concat(Server.MapPath(Config.Directory.TmpDir), 
                                     DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                     "_",
                                     filExcel.FileName,
                                     ".xls");

            filExcel.SaveAs(FileName);

            LoadWorkSheets();
            panelProcess.Visible = panelProgress.Visible = true;
            lblFileName.Text = filExcel.FileName;
            lblResult.Text = string.Empty;
        }

        protected void CbWorksheetsSelectedIndexChanged(object sender, EventArgs e)
        {
            LoadColumns();
        }

        protected void BtProcessClick(object sender, EventArgs e)
        {
            try
            {
                var excel = new ExcelQueryFactory(FileName);
                excel.AddMapping<ImportDireccion>(dir => dir.Descripcion, ColumnaDescripcion);
                excel.AddMapping<ImportDireccion>(dir => dir.Codigo, ColumnaCodigo);
                excel.AddMapping<ImportDireccion>(dir => dir.Calle, ColumnaCalle);
                excel.AddMapping<ImportDireccion>(dir => dir.Altura, ColumnaAltura);
                excel.AddMapping<ImportDireccion>(dir => dir.Esquina, ColumnaEsquina);
                excel.AddMapping<ImportDireccion>(dir => dir.Partido, ColumnaPartido);
                excel.AddMapping<ImportDireccion>(dir => dir.Provincia, ColumnaProvincia);
                excel.AddMapping<ImportDireccion>(dir => dir.Latitud, ColumnaLatitud);
                excel.AddMapping<ImportDireccion>(dir => dir.Longitud, ColumnaLongitud);

                if (!string.IsNullOrEmpty(ColumnaDesde)) 
                    excel.AddMapping<ImportDireccion>(dir => dir.VigenciaDesde, ColumnaDesde);
                if (!string.IsNullOrEmpty(ColumnaHasta))
                    excel.AddMapping<ImportDireccion>(dir => dir.VigenciaHasta, ColumnaHasta);
                var ws = excel.Worksheet<ImportDireccion>(CurrentWorkSheet).ToList();
                lblDirs.Text = string.Empty;
                Nomenclar(ws);
            }
            catch (Exception ex)
            {
                lblResult.Text += "<hr/>" + ex;
            }
        }
    
        private void Nomenclar(List<ImportDireccion> importData)
        {
            var step = 1;// Index < 10 || importData.Count < 20
            //   ? 1 : Index > importData.Count - 10 ? 1 : importData.Count <= 100 ? 5 : 10;
            var next = Index + step;
            if (Start == 0)
            {
                Start = DateTime.Now.Ticks;
                lblResult.Text = string.Empty;
            }
        
            for (var i = Index; i < importData.Count && i < next; i++)
            {
                var row = importData[i];
                Index = i + 1;
                TotalCount++;

                if(row.IsEmpty())
                {
                    Log("REGISTRO VACIO", row);
                    SinProcesar++;
                    continue;
                }

                var referencia = DAOFactory.ReferenciaGeograficaDAO.FindByCodigo(new [] { cbEmpresa.Selected },
                                                                                 new[] { cbLinea.Selected }, new[] { cbTipoGeoRef.Selected }, row.Codigo.Trim());

                if (!chkSobreescribir.Checked && (referencia != null && referencia.Baja == false))
                {
                    Log("CODIGO EXISTENTE", row);
                    UpdateVigencia(row, referencia);
                    SinProcesar++;
                    continue;
                }

                if (row.Calle != null)
                {
                    var dirs = GeocoderHelper.Cleaning.NomenclarDireccion(row.Calle, row.NumAltura,
                                                                          row.Esquina ?? string.Empty,
                                                                          row.Partido ?? string.Empty, row.Provincia);

                    if (dirs.Count == 0)
                    {
                        SinNomenclar++;
                        Log("SIN NOMENCLAR", row);
                        continue;
                    }
                    if (dirs.Count > 1)
                    {
                        Ambiguas++;
                        Log("AMBIGUA", row);
                        continue;
                    }
                    Nomencladas++;
                    CreateReferencia(row, dirs[0]);
                }
                else
                {
                    double lat;
                    double lon;
                    if (row.Latitud != null && row.Longitud != null 
                        && double.TryParse(row.Latitud, out lat)
                        && double.TryParse(row.Longitud, out lon))
                    {
                        if (lat != 0.0 && lon != 0.0)
                        {
                            Nomencladas++;
                            CreateReferencia(row, null);
                        }
                        else
                        {
                            SinNomenclar++;
                            Log("SIN NOMENCLAR", row);
                        }
                    }
                    else
                    {
                        SinNomenclar++;
                    }
                }
            }

            lblDirs.Text = string.Format(Reporte,
                                         TotalCount,
                                         Nomencladas,
                                         TotalCount > 0 ? (Nomencladas * 100.0 / TotalCount).ToString("0.00") : "0.00",
                                         Ambiguas,
                                         TotalCount > 0 ? (Ambiguas * 100.0 / TotalCount).ToString("0.00") : "0.00",
                                         SinNomenclar,
                                         TotalCount > 0 ? (SinNomenclar * 100.0 / TotalCount).ToString("0.00") : "0.00",
                                         TimeSpan.FromTicks(DateTime.Now.Ticks-Start),
                                         importData.Count,
                                         SinProcesar,
                                         TotalCount > 0 ? (SinProcesar * 100.0 / TotalCount).ToString("0.00") : "0.00");

            SetProgressBar(Index * 100 / importData.Count);
            if(Index < importData.Count)
            {
                ScriptManager.RegisterStartupScript(Page, typeof(string), "recall" + Index, " process(); ", true);
                if (panelProcess.Enabled)
                {
                    panelProcess.Enabled = false;
                    updProcess.Update();
                }
            }
            else
            {
                if (!panelProcess.Enabled)
                {
                    panelProcess.Enabled = true;
                    updProcess.Update();
                }
                Index = 0;
                Nomencladas = 0;
                SinNomenclar = 0;
                SinProcesar = 0;
                Ambiguas = 0;
                TotalCount = 0;
                Start = Convert.ToInt64(0);
            }
        }

        private void UpdateVigencia(ImportDireccion row, ReferenciaGeografica referencia)
        {
            if(row.VigenciaDesde2.HasValue || row.VigenciaHasta2.HasValue)
            {
                if(referencia.Vigencia == null)
                {
                    referencia.Vigencia = new Vigencia {Inicio = row.VigenciaDesde2, Fin = row.VigenciaHasta2};
                }
                else if(!referencia.Vigencia.Vigente(DateTime.UtcNow))
                {
                    referencia.Vigencia = new Vigencia {Inicio = row.VigenciaDesde2, Fin = row.VigenciaHasta2};
                }
                else
                {
                    var desde = row.VigenciaDesde2.HasValue ? row.VigenciaDesde2.Value : DateTime.MaxValue;
                    if(referencia.Vigencia.Inicio.HasValue && referencia.Vigencia.Inicio.Value < desde) desde = referencia.Vigencia.Inicio.Value;
                    if(desde == DateTime.MaxValue) desde = DateTime.UtcNow;

                    var hasta = row.VigenciaHasta2.HasValue ? row.VigenciaHasta2.Value : DateTime.MinValue;
                    if(referencia.Vigencia.Fin.HasValue && referencia.Vigencia.Fin.Value > hasta) hasta = referencia.Vigencia.Fin.Value;
                    if (hasta == DateTime.MinValue) hasta = DateTime.UtcNow;
                    referencia.Vigencia = new Vigencia { Inicio = desde, Fin = hasta};
                }
                DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(referencia);
                STrace.Trace("QtreeReset", "DireccionImport 1");
            }
        }

        private void Log(string status, ImportDireccion row)
        {
            lblResult.Text += string.Format(Result, status, row.Descripcion, row.Codigo, row.Calle, row.Altura, row.Esquina, row.Partido, row.Provincia);
        }

        private void SetProgressBar(int percent)
        {
            const int totalWidth = 200;
            litProgress.Text = string.Format(@"<div style='margin: auto; border: solid 1px #999999; background-color: #FFFFFF; width: {0}px; height: 10px;'>
                <div style='background-color: #0000AA; width: {1}px; height: 10px; font-size: 8px; color: #CCCCCC;'>{2}%</div>
                </div>", totalWidth, percent * totalWidth / 100, percent);
        }
        private void LoadWorkSheets()
        {
            var excel = new ExcelQueryFactory(FileName);
            cbWorksheets.DataSource = excel.GetWorksheetNames();
            cbWorksheets.DataBind();
        
            LoadColumns();
        }
        private void LoadColumns()
        {
            var excel = new ExcelQueryFactory(FileName);
            var columns = excel.GetColumnNames(CurrentWorkSheet).OrderBy(col => col);

            BindColumns(cbDescripcion, columns, true, "Descripcion");
            BindColumns(cbCodigo, columns, true, "Codigo");
            BindColumns(cbCalle, columns, true, "Calle");
            BindColumns(cbAltura, columns, true, "Altura");
            BindColumns(cbEsquina, columns, true, "Esquina");
            BindColumns(cbPartido, columns, true, "Partido");
            BindColumns(cbProvincia, columns, true, "Provincia");
            BindColumns(cbLatitud, columns, true, "Latitud");
            BindColumns(cbLongitud, columns, true, "Longitud");
            BindColumns(cbDesde, columns, true, "VigenciaDesde");
            BindColumns(cbHasta, columns, true, "VigenciaHasta");

            lblDirs.Text = string.Format(Reporte, 0, 0, "0.00", 0, "0.00", 0, "0.00", 0, excel.Worksheet(CurrentWorkSheet).Count(), 0, "0.00");
            SetProgressBar(0);
        }
        private static void BindColumns(ListControl cb, IEnumerable<string> values, bool addEmpty, string trySelect)
        {
            cb.Items.Clear();
            if (addEmpty) cb.Items.Add(string.Empty);
            foreach (var val in values) cb.Items.Add(val);
            TrySelect(cb, trySelect);
        }

        private static void TrySelect(ListControl cb, string value)
        {
            var it = cb.Items.FindByValue(value);
            if (it != null) it.Selected = true;
        }

        private void CreateReferencia(ImportDireccion datos, DireccionVO direccion)
        {
            var codigo = datos.Codigo.Trim();
            var referencia = DAOFactory.ReferenciaGeograficaDAO.FindByCodigo(new []{cbEmpresa.Selected},
                                                                             new[] { cbLinea.Selected }, new[] { cbTipoGeoRef.Selected }, codigo);

            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var empresa = linea != null ? linea.Empresa :
                                                            cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;

            int radio;
            if (!int.TryParse(txtRadio.Text, out radio)) radio = 100;

            var tipo = DAOFactory.TipoReferenciaGeograficaDAO.FindById(cbTipoGeoRef.Selected);
            if(referencia == null)
            {
                referencia = new ReferenciaGeografica
                                 {
                                     Codigo = codigo,
                                     Empresa = empresa, 
                                     Linea = linea,
                                     EsFin = tipo.EsFin,
                                     EsInicio = tipo.EsInicio,
                                     EsIntermedio = tipo.EsIntermedio,
                                     Icono = tipo.Icono,
                                     InhibeAlarma = tipo.InhibeAlarma,
                                     TipoReferenciaGeografica = tipo,
                                     Color = tipo.Color
                                 };
                foreach (TipoReferenciaVelocidad maxima in tipo.VelocidadesMaximas)
                    referencia.VelocidadesMaximas.Add(new ReferenciaVelocidad { ReferenciaGeografica = referencia, TipoVehiculo = maxima.TipoVehiculo, VelocidadMaxima = maxima.VelocidadMaxima });
            }
        
            referencia.Baja = false;
            referencia.Descripcion = datos.Descripcion.Trim();
            referencia.Observaciones = string.Empty;

            UpdateVigencia(datos, referencia);

            var dir = new Direccion
                          {
                              Altura = direccion != null ? direccion.Altura : -1,
                              Calle = direccion != null ? direccion.Calle : string.Empty,
                              Descripcion = direccion != null ? direccion.Direccion : string.Empty,
                              IdCalle = direccion != null ? direccion.IdPoligonal : -1,
                              IdEntrecalle = -1,
                              IdEsquina = direccion != null ? direccion.IdEsquina : -1,
                              IdMapa = direccion != null ? (short)direccion.IdMapaUrbano : (short)-1,
                              Latitud = direccion != null ? direccion.Latitud : Convert.ToDouble(datos.Latitud),
                              Longitud = direccion != null ? direccion.Longitud : Convert.ToDouble(datos.Longitud),
                              Pais = "Argentina",
                              Partido = direccion != null ? direccion.Partido : string.Empty,
                              Provincia = direccion != null ? direccion.Provincia : string.Empty,
                              Vigencia = new Vigencia {Inicio = DateTime.Now}
                          };

            var pol = new Poligono
                          {
                              Radio = radio,
                              Vigencia = new Vigencia {Inicio = DateTime.Now}
                          };
            pol.AddPoints(new[] { new PointF((float)dir.Longitud, (float)dir.Latitud) });

            referencia.AddHistoria(dir, pol, DateTime.UtcNow);

            DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(referencia);
            STrace.Trace("QtreeReset", "DireccionImport 2");
        }

        private class ImportDireccion
        {
            public string Descripcion { get; set; }
            public string Codigo { get; set; }
            public string Calle { get; set; }
            public string Altura { get; set; }
            public string Esquina { get; set; }
            public string Partido { get; set; }
            public string Provincia { get; set; }
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public string VigenciaDesde { get; set; }
            public string VigenciaHasta { get; set; }
            public int NumAltura { get { int alt; if (int.TryParse(Altura, out alt)) return alt; return -1; } }
            public DateTime? VigenciaDesde2
            {
                get
                {
                    DateTime d;
                    return DateTime.TryParse(VigenciaDesde, Usuario.Culture, DateTimeStyles.None, out d)
                               ? d.ToDataBaseDateTime()
                               : new DateTime?();
                }
            }
            public DateTime? VigenciaHasta2
            {
                get
                {
                    DateTime d;
                    return DateTime.TryParse(VigenciaHasta, Usuario.Culture, DateTimeStyles.None, out d)
                               ? d.ToDataBaseDateTime()
                               : new DateTime?();
                }
            }

            public bool IsEmpty()
            {
                return string.IsNullOrEmpty(Descripcion) || string.IsNullOrEmpty(Codigo);
            }
        }
    }
}
