using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObjects.CicloLogistico.Distribucion;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Helpers;
using Logictracker.Web.Helpers.ExportHelpers;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class ControlarDistribucion : SecuredBaseReportPage<VehiculoSinControlar>
    {
        public static class Views
        {
            public const int Vehiculos = 0;
            public const int Control = 1;
            public const int Resumen = 2;
        }
        public static class Navigate
        {
            public const string Fwd = "Fwd";
            public const string Bck = "Bck";
            public const string Go = "Go";
        }
        protected override string VariableName { get { return "CLOG_CONTROL_DISTRIBUCION"; } }
        protected override string GetRefference() { return "CLOG_CONTROL_DISTRIBUCION"; }
        protected override bool HideSearch { get { return true; } }
        protected override bool ExcelButton { get { return true; } }

        protected VsProperty<int> Empresa { get { return this.CreateVsProperty<int>("Empresa"); } }
        protected VsProperty<int> Periodo { get { return this.CreateVsProperty<int>("Periodo"); } }
        protected VsProperty<int> IndexControl { get { return this.CreateVsProperty<int>("IndexControl"); } }
        protected VsProperty<List<int>> ListControl { get { return this.CreateVsProperty<List<int>>("ListControl"); } }
        protected VsProperty<int> CountControl { get { return this.CreateVsProperty<int>("CountControl"); } }
        protected VsProperty<int> ControlandoId { get { return this.CreateVsProperty<int>("ControlandoId"); } }
        
        protected override void ExportToCsv()
        {
            var builder = new GridToCSVBuilder(Usuario.CsvSeparator);

            var g = gridResumen;
            var allowPaging = gridResumen.AllowPaging;

            var distribuciones = GetDistribuciones(new[] { -1 });
            var controladas = distribuciones.Where(d => d.Controlado).Select(d => new ViajeDistribucionVo(d)).ToList();
            
            g.AllowPaging = false;
            g.DataSource = controladas;
            g.DataBind();

            builder.GenerateHeader(CultureManager.GetMenu(VariableName), GetFilterValues());
            builder.GenerateColumns(g);
            builder.GenerateFields(g);

            Session["CSV_EXPORT"] = builder.Build();
            Session["CSV_FILE_NAME"] = CultureManager.GetMenu(Module.Name);

            OpenWin(String.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));

            g.AllowPaging = allowPaging;
        }
        protected override void ExportToExcel()
        {
            var path = HttpContext.Current.Request.Url.AbsolutePath;
            path = Path.GetFileNameWithoutExtension(path) + ".xlsx";

            var builder = new GridToExcelBuilder(path, Usuario.ExcelFolder);
            var distribuciones = GetDistribuciones(new[] { -1 });
            var controladas = distribuciones.Where(d => d.Controlado).Select(d => new ViajeDistribucionVo(d)).ToList();
            var list = controladas.Select(c => new ControlDistribucionVo(c)).ToList();

            builder.GenerateHeader(CultureManager.GetMenu(VariableName), GetFilterValues());
            builder.GenerateColumns(list);
            builder.GenerateFields(list);
         
            Session["TMP_FILE_NAME"] = builder.CloseAndSave();
            Session["CSV_FILE_NAME"] = CultureManager.GetMenu(Module.Name);

            OpenWin(String.Concat(ApplicationPath, "Common/exportExcel.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }

        protected override List<VehiculoSinControlar> GetResults() { return null; }
        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            try
            {
                ValidateEntity(cbPeriodo.Selected, "PERIODO");
                Empresa.Set(cbEmpresa.Selected);
                Periodo.Set(cbPeriodo.Selected);
                SetView(Views.Vehiculos);
            }
            catch(Exception ex)
            {
                ShowError(ex);
            }
        }
        protected void TabActiveTabChanged(object sender, EventArgs e)
        {
            SetView(tab.ActiveTabIndex);
        }
        protected void ControlCommand(object sender, CommandEventArgs e)
        {
            Move(e.CommandName);
        }
        protected void GridResumenRowCommand(object sender, C1GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Modificar")
            {
                var id = Convert.ToInt32(gridResumen.DataKeys[e.Row.RowIndex].Value);
                var viaje = DAOFactory.ViajeDistribucionDAO.FindById(id);
                Controlar(viaje, false);
                SetView(Views.Resumen);
            }
        }
        private void Move(string command)
        {
            var index = IndexControl.Get();
            var count = CountControl.Get();
            switch (command)
            {
                case Navigate.Bck: if (index > 0) IndexControl.Set(--index); break;
                case Navigate.Fwd: if (index < count - 1) IndexControl.Set(++index); break;
                case Navigate.Go:
                    int newIndex;
                    if(!int.TryParse(txtIndexControl.Text, out newIndex)) newIndex = index+1;
                    if (newIndex > 0 && newIndex <= count) IndexControl.Set(newIndex-1); 
                    break;
            }
            SetView(Views.Control);
        }
        protected void SaveCommand(object sender, CommandEventArgs e)
        {
            try
            {
                var viaje = DAOFactory.ViajeDistribucionDAO.FindById(ControlandoId.Get());
                if (e.CommandName == "Cancel")
                {
                    Controlar(viaje, false);
                    SetView(Views.Control);
                }
                else
                {
                    WebControl txt = null;
                    foreach (C1GridViewRow row in gridEntregas.Rows)
                    {
                        var txtKmControlado = row.FindControl("txtKmControlado") as TextBox;
                        txtKmControlado.CssClass = "LogicTextbox_Invalid";
                        var km = ValidateDouble(txtKmControlado.Text, "KM");
                        txtKmControlado.CssClass = "LogicTextbox";
                        var id = Convert.ToInt32(gridEntregas.DataKeys[row.RowIndex].Value);
                        var entrega = viaje.Detalles.First(d => d.Id == id);
                        entrega.KmControlado = km;
                        DAOFactory.EntregaDistribucionDAO.SaveOrUpdate(entrega);
                        txt = txtKmControlado;
                    }
                    ShowInfo("Valores guardados correctamente");
                    if (e.CommandName == "Control")
                    {
                        Controlar(viaje, true);
                        Move(Navigate.Fwd);
                    }
                    else
                    {
                        // recalcular total km controlado por javascript
                        if (txt != null)
                        {
                            var sh = new ScriptHelper(this);
                            sh.RegisterStartupScript("calcularTotal", string.Format("calcularTotal($get('{0}'));", txt.ClientID));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
        protected void MapCommand(object sender, CommandEventArgs e)
        {
            var viaje = DAOFactory.ViajeDistribucionDAO.FindById(ControlandoId.Get());
            ViewMap(viaje);
        }
        protected override void Print()
        {
            base.Print();

            var distribuciones = GetDistribuciones(new[] { -1 });
            var controladas = distribuciones.Where(d => d.Controlado).Select(d => new ViajeDistribucionVo(d)).ToList();
            
            gridPrint.GroupedColumns.Clear();
            gridPrint.GroupedColumns.Add(gridResumen.Columns[0] as C1Field);
            gridPrint.DataSource = controladas.Select(c => new ControlDistribucionVo(c));

            try
            {
                gridPrint.DataBind();
                upPrint.Update();

                var sh = new ScriptHelper(this);

                sh.RegisterStartupScript("print", "PrintReport();");
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void SetView(int view)
        {
            switch(view)
            {
                case Views.Vehiculos:
                    ListControl.Set(null);
                    gridVehiculos.DataSource = VehiculosPendientes;
                    gridVehiculos.DataBind();
                    IndexControl.Set(0);
                    break;
                case Views.Control:
                    if (!VehiculosSeleccionados.Any())
                    {
                        ShowError(string.Format(CultureManager.GetError("MUST_SELECT_VALUE"), CultureManager.GetEntity("PARENTI03")));
                        SetView(Views.Vehiculos);
                        return;
                    }
                    if (ListControl.Get() == null)
                    {
                        var viajes = GetDistribuciones(VehiculosSeleccionados).Where(x => !x.Controlado).Select(v => v.Id).ToList();
                        ListControl.Set(viajes);
                        CountControl.Set(viajes.Count);
                    }
                    var index = IndexControl.Get();
                    var count = CountControl.Get();

                    var viaje = GetDistribucionAControlar(index);
                    BindViaje(viaje);

                    txtIndexControl.Text = (index + 1).ToString("#0");
                    lblCountControl.Text = count.ToString("#0");
                    btSiguiente.Visible = index < count - 1;
                    btAnterior.Visible = index > 0;
                    
                    break;
                case Views.Resumen:
                    var distribuciones = GetDistribuciones(new[]{-1});
                    var controladas = distribuciones.Where(d => d.Controlado).Select(d=>new ViajeDistribucionVo(d)).ToList();
                    //var noControladas = distribuciones.Where(d => !d.Controlado).ToList();

                    gridResumen.GroupedColumns.Clear();
                    gridResumen.GroupedColumns.Add(gridResumen.Columns[0] as C1Field);
                    gridResumen.DataSource = controladas;
                    gridResumen.DataBind();
                    break;
            }

            tab.ActiveTabIndex = view;
            updCompleto.Visible = true;
            updCompleto.Update();
        }

        protected void BindViaje(ViajeDistribucion viaje)
        {
            ControlandoId.Set(viaje.Id);

            btAplicar.Enabled = !viaje.Controlado;
            btControlar.Visible = !viaje.Controlado;
            btModificar.Visible = viaje.Controlado;

            lblTipo.Text = CultureManager.GetLabel(
                viaje.Tipo == 2 ? "TIPODISTRI_RECORRIDO_FIJO"
                : viaje.Tipo == 1 ? "TIPODISTRI_DESORDENADA"
                : "TIPODISTRI_NORMAL");
            lblVehiculo.Text = viaje.Vehiculo.Interno;
            lblCodigo.Text = viaje.Codigo;
            lblFecha.Text = viaje.Inicio.ToString("dd-MM-yyyy");
            lblEmpleado.Text = viaje.Empleado != null ? viaje.Empleado.Entidad.Descripcion : string.Empty;
            lblBase.Text = viaje.Linea.Descripcion;
            CalcularKm(viaje);

            var entregas = GetEntregasAControlar(viaje);
            gridEntregas.DataSource = entregas;
            gridEntregas.DataBind();

            gridEntregas.FooterRow.Cells[5].Text = entregas.Where(e => e.KmGps.HasValue).Sum(e => Math.Round(e.KmGps.Value * 10.0) / 10).ToString("0.0 km");
            gridEntregas.FooterRow.Cells[6].Text = entregas.Where(e=>e.KmCalculado.HasValue).Sum(e => Math.Round(e.KmCalculado.Value*10.0)/10).ToString("0.0 km");
            gridEntregas.FooterRow.Cells[7].Text = "<div id='kmTotal'>" + entregas.Where(e => e.KmControlado.HasValue).Sum(e => Math.Round(e.KmControlado.Value * 10.0) / 10).ToString("0.0 km") + "</div>";
        }

        protected void GridEntregasRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            var entrega = e.Row.DataItem as EntregaAControlar;
            var txtKmControlado = e.Row.FindControl("txtKmControlado") as TextBox;
            txtKmControlado.Attributes.Add("onchange", "calcularTotal(this);");
            var gpsCell = e.Row.Cells[5];
            var isapiCell = e.Row.Cells[6];

            var visitado = entrega.Base || entrega.Estado == EntregaDistribucion.Estados.Completado ||
                           entrega.Estado == EntregaDistribucion.Estados.NoCompletado;
            if (visitado)
            {
                txtKmControlado.Text = e.Row.RowIndex > 0 
                    ? entrega.KmControlado.HasValue ? entrega.KmControlado.Value.ToString() : isapiCell.Text
                    : "0";
                txtKmControlado.Enabled = e.Row.RowIndex > 0 && btAplicar.Enabled;    

                if (btAplicar.Enabled && e.Row.RowIndex > 0)
                {
                    gpsCell.Text = string.Format("<div onclick='$get(\"{1}\").value = \"{0}\"; calcularTotal($get(\"{1}\"))'>{0} km</div>", gpsCell.Text, txtKmControlado.ClientID);
                    isapiCell.Text = string.Format("<div onclick='$get(\"{1}\").value = \"{0}\"; calcularTotal($get(\"{1}\"))'>{0} km</div>", isapiCell.Text, txtKmControlado.ClientID);
                }
                else
                {
                    if (e.Row.RowIndex == 0)
                    {
                        gpsCell.Text = string.Empty;
                        isapiCell.Text = string.Empty;
                    }
                    else
                    {
                        gpsCell.Text = string.Format("{0} km", gpsCell.Text);
                        isapiCell.Text = string.Format("{0} km", isapiCell.Text);
                    }
                }
            }
            else
            {
                gpsCell.Text = "0 km";
                isapiCell.Text = "0 km";
                txtKmControlado.Text = "0";
                txtKmControlado.Enabled = false;
            }
        }

        protected void ViewMap(ViajeDistribucion viaje)
        {
            if (viaje.Detalles.Count == 0)
            {
                ShowInfo("No hay datos para mostrar");
                return;
            }

            OpenWin(ResolveUrl(UrlMaker.MonitorLogistico.GetUrlDistribucion(viaje.Id, true)), "_blank");
        }

        private void Controlar(ViajeDistribucion viaje, bool controlar)
        {
            viaje.Controlado = controlar;
            viaje.UsuarioControl = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            viaje.FechaControl = DateTime.UtcNow;
            DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
        }

        #region Calculos

        private void CalcularKm(ViajeDistribucion viaje)
        {
            EntregaDistribucion anterior = null;
            var detalles = viaje.GetEntregasOrdenadas();
            foreach (var entrega in detalles)
            {
                if(entrega.PuntoEntrega != null && entrega.Estado != EntregaDistribucion.Estados.Completado &&
                   entrega.Estado != EntregaDistribucion.Estados.NoCompletado)
                {
                    continue;
                }
                
                if (anterior != null)
                {
                    var changed = false;
                    if (!entrega.KmGps.HasValue || entrega.KmGps.Value == 0)
                    {
                        entrega.KmGps = DAOFactory.CocheDAO.GetDistance(viaje.Vehiculo.Id, anterior.ManualOSalida, entrega.ManualOSalida);
                        changed = true;
                    }
                    if (!entrega.KmCalculado.HasValue || entrega.KmCalculado.Value == 0)
                    {
                        entrega.KmCalculado = GeocoderHelper.CalcularDistacia(anterior.ReferenciaGeografica.Latitude,
                                                                               anterior.ReferenciaGeografica.Longitude,
                                                                               entrega.ReferenciaGeografica.Latitude,
                                                                               entrega.ReferenciaGeografica.Longitude);
                        changed = true;
                    }
                    if (changed)
                    {
                        DAOFactory.EntregaDistribucionDAO.SaveOrUpdate(entrega);
                    }
                }
                anterior = entrega;
            }
        } 

        #endregion

        #region Datos

        private List<VehiculoSinControlar> _vehiculosPendientes;

        protected List<VehiculoSinControlar> VehiculosPendientes
        {
            get
            {
                if (_vehiculosPendientes == null)
                {
                    var viajes = GetDistribuciones(new[] { -1 });
                    var byVehiculo = viajes.Where(v => !v.Controlado).GroupBy(v => v.Vehiculo, v => v);

                    _vehiculosPendientes = byVehiculo.Select(v => new VehiculoSinControlar { Interno = v.Key != null ? v.Key.Interno : string.Empty, Viajes = v.Count(), Id = v.Key != null ? v.Key.Id : -1 }).ToList();
                }
                return _vehiculosPendientes;
            }
        } 
        protected List<ViajeDistribucion> GetDistribuciones(IEnumerable<int> vehiculos)
        {
            var periodo = DAOFactory.PeriodoDAO.FindById(Periodo.Get());
            var empresas = new[] { Empresa.Get() };
            var desde = periodo.FechaDesde;
            var hasta = periodo.FechaHoraHasta;
            var estados = new int[] {
                                        ViajeDistribucion.Estados.Cerrado,
                                        ViajeDistribucion.Estados.EnCurso,
                                        ViajeDistribucion.Estados.Pendiente
                                    };
            return DAOFactory.ViajeDistribucionDAO.GetList(empresas, 
                                                           new[] { -1 }, // LINEAS
                                                           new[] { -1 }, // TRANSPORTISTAS
                                                           new[] { -1 }, // DEPTOS
                                                           new[] { -1 }, // CENTROS DE COSTO
                                                           new[] { -1 }, // SUB CENTROS DE COSTO
                                                           vehiculos, 
                                                           estados, 
                                                           desde, 
                                                           hasta)
                                                  .ToList();
        }

        protected ViajeDistribucion GetDistribucionAControlar(int index)
        {
            var list = ListControl.Get();
            return DAOFactory.ViajeDistribucionDAO.FindById(list[index]);
        }

        protected IEnumerable<int> VehiculosSeleccionados
        {
            get 
            {
                return from C1GridViewRow row in gridVehiculos.Rows let chkSelect = row.FindControl("chkSelect") as CheckBox where chkSelect.Checked select Convert.ToInt32(gridVehiculos.DataKeys[row.RowIndex].Value);
            }
        }
        protected List<EntregaAControlar> GetEntregasAControlar(ViajeDistribucion viaje)
        {
            return viaje.GetEntregasOrdenadas().Select(e => new EntregaAControlar
                                                  {
                                                      Id=e.Id,
                                                      Entrega = e.Descripcion,
                                                      Tipo = e.TipoServicio != null ? e.TipoServicio.Descripcion : e.Linea != null ? CultureManager.GetEntity("PARENTI02") : string.Empty,
                                                      Programado = e.Programado,
                                                      Entrada = e.Entrada,
                                                      Manual = e.Manual,
                                                      Salida = e.Salida,
                                                      KmGps = e.KmGps,
                                                      KmCalculado = e.KmCalculado,
                                                      KmControlado = e.KmControlado,
                                                      Estado = e.Estado,
                                                      Base = e.Linea != null
                                                  }).ToList();
        }

        #endregion

        protected void CbEmpresaSelectedIndexChanged(object sender, EventArgs e)
        {
            var now = DateTime.UtcNow;
            var periodoActual = (from ListItem li in cbPeriodo.Items
                    let periodo = DAOFactory.PeriodoDAO.FindById(Convert.ToInt32(li.Value))
                    where now >= periodo.FechaDesde && now < periodo.FechaHasta
                    select li).FirstOrDefault();
            if (periodoActual != null) { periodoActual.Selected = true; }

        }
    }

    public class VehiculoSinControlar
    {
        public int Id { get; set; }
        public string Interno { get; set; }
        public int Viajes { get; set; }
    }
    public class EntregaAControlar
    {
        public int Id { get; set; }
        public string Entrega { get; set; }
        public string Tipo { get; set; }
        public DateTime Programado { get; set; }
        public DateTime? Entrada { get; set; }
        public DateTime? Manual { get; set; }
        public DateTime? Salida { get; set; }
        public double? KmGps { get; set; }
        public double? KmCalculado { get; set; }
        public double? KmControlado { get; set; }
        public short Estado { get; set; }
        public bool Base { get; set; }
    }
}