using C1.Web.UI.Controls.C1GridView;
using C1.Web.UI.Controls.C1Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.DAL.NHibernate;
using Logictracker.Security;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.ValueObject.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class CocheAlta : SecuredAbmPage<Coche>
    {
        protected override string VariableName { get { return "PAR_VEHICULOS"; } }
        protected override string RedirectUrl { get { return "CocheLista.aspx"; } }
        protected override string GetRefference() { return "COCHE"; }
        protected override bool DeleteButton { get { return false; } }

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cbDispositivo.Enabled = WebSecurity.IsSecuredAllowed(Securables.AssingDevices);
            lblCosto.Visible = txtCosto.Visible = abmTabConsumos.Visible = abmTabCostos.Visible = WebSecurity.IsSecuredAllowed(Securables.ViewCost);

            if (!EditMode)
            {
                cbEstados.SetSelectedValue(Coche.Estados.Activo);
                abmTabConsumos.Visible = abmTabCostos.Visible = abmTabDocumentos.Visible = abmTabTurnos.Visible = false;
                dtInicioActividad.SelectedDate = DateTime.Today;
                dtInicioActividad.Enabled = false;
            }

            if (!IsPostBack)
            {
                dtFechaDesde.SelectedDate = DateTime.Today.AddMonths(-1);
                dtFechaHasta.SelectedDate = DateTime.Today.AddDays(1).AddMinutes(-1);
            }
        } 

        protected override void Bind()
        {
            ddlEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : ddlEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            ddlTransportista.SetSelectedValue(EditObject.Transportista != null ? EditObject.Transportista.Id : ddlTransportista.NoneValue);
            cbTipoEmpleado.SetSelectedValue(EditObject.Chofer != null ? EditObject.Chofer.TipoEmpleado != null ? EditObject.Chofer.TipoEmpleado.Id : cbTipoEmpleado.NullValue : cbTipoEmpleado.NullValue);
            cbEmpleado.SetSelectedValue(EditObject.Chofer != null ? EditObject.Chofer.Id : -2);
            cbDepartamento.SetSelectedValue(EditObject.Departamento != null ? EditObject.Departamento.Id : cbDepartamento.AllValue);
            ddlCentro.SetSelectedValue(EditObject.CentroDeCostos != null ? EditObject.CentroDeCostos.Id : ddlCentro.AllValue);
            ddlSubCentro.SetSelectedValue(EditObject.SubCentroDeCostos != null ? EditObject.SubCentroDeCostos.Id : ddlSubCentro.AllValue);
            cbTipoCoche.SetSelectedValue(EditObject.TipoCoche != null ? EditObject.TipoCoche.Id : cbTipoCoche.NullValue);
            cbDispositivo.SetSelectedValue(EditObject.Dispositivo != null ? EditObject.Dispositivo.Id : cbDispositivo.NullValue);
            cbEstados.SetSelectedValue(EditObject.Estado);
            cbMarca.SetSelectedValue(EditObject.Modelo != null && EditObject.Modelo.Marca != null ? EditObject.Modelo.Marca.Id : cbMarca.AllValue);
            cbModelo.SetSelectedValue(EditObject.Modelo != null ? EditObject.Modelo.Id : cbModelo.NoneValue);

            txtInterno.Text = EditObject.Interno;            
            txtPatente.Text = EditObject.Patente;
            txtPatente.Enabled = WebSecurity.IsSecuredAllowed(Securables.EditarPatente);

            if (EditObject.AnioPatente != 0) npAnio.Value = EditObject.AnioPatente;
            txtNumeroChasis.Text = EditObject.NroChasis;
            txtNumeroMotor.Text = EditObject.NroMotor;
            txtPoliza.Text = EditObject.Poliza;
            dpVencimiento.SelectedDate = EditObject.FechaVto.HasValue ? EditObject.FechaVto.Value.ToDisplayDateTime() : (DateTime?)null;
            txtRefference.Text = string.IsNullOrEmpty(EditObject.Referencia) ? string.Empty : EditObject.Referencia;

            chkControlaKm.Checked = EditObject.ControlaKm;
            chkControlaHs.Checked = EditObject.ControlaHs;
            chkIdentificaChoferes.Checked = EditObject.IdentificaChoferes;

            chkReportaAssistCargo.Checked = EditObject.ReportaAssistCargo;
            chkEsPuerta.Checked = EditObject.EsPuerta;

            if (EditObject.ControlaKm || EditObject.ControlaHs)
            {
                chkControlaTurnos.Visible = true;
                chkControlaTurnos.Checked = EditObject.ControlaTurnos;
                chkControlaServicios.Visible = true;
                chkControlaServicios.Checked = EditObject.ControlaServicios;
                lblPorcentaje.Visible = true;
                npPorcentaje.Visible = true;
                npPorcentaje.Value = EditObject.PorcentajeProductividad;
            }

            npKilometros.Value = Convert.ToInt32(EditObject.KilometrosDiarios);
            npVelocidadPromedio.Value = EditObject.VelocidadPromedio;
            txtCapacidad.Value = EditObject.Capacidad;
            npCapacidadCarga.Value = EditObject.CapacidadCarga;

            npOdometroInicial.Value = Convert.ToInt32(EditObject.InitialOdometer);
            cbCliente.SelectedValues = EditObject.Clientes.OfType<Cliente>().Where(c => !c.Baja).Select(c => c.Id).ToList();

            BindOdometros();
            BindShifts();

            if (!IsPostBack)
            {
                DocumentList1.LoadDocumentos(-1, -1, EditObject.Id, -1, -1);

                dtFechaDesde.SelectedDate = DateTime.Today.AddMonths(-6);
                dtFechaHasta.SelectedDate = DateTime.Today.AddDays(1).AddMinutes(-1);
                var desde = dtFechaDesde.SelectedDate.Value.ToDataBaseDateTime();
                var hasta = dtFechaHasta.SelectedDate.Value.ToDataBaseDateTime();

                TicketList.LoadTickets(EditObject.Id, desde, hasta);
                TicketsMantenimientoList.LoadTickets(EditObject.Id, desde, hasta);
                listConsumos.LoadConsumos(EditObject.Id, desde, hasta);
            }

            var entidad = EditObject.Dispositivo != null 
                            ? DAOFactory.EntidadDAO.FindByDispositivo(new[] { EditObject.Empresa != null ? EditObject.Empresa.Id : -1 },
                                                                      new[] { EditObject.Linea != null ? EditObject.Linea.Id : -1 },
                                                                      new[] { EditObject.Dispositivo != null ? EditObject.Dispositivo.Id : -1 })
                            : null;

            chkTelemetria.Checked = entidad != null && entidad.Id != 0;
            chkTelemetria.Enabled = !chkTelemetria.Checked;

            txtRendimiento.Text = EditObject.CocheOperacion != null && EditObject.CocheOperacion.Rendimiento > 0.0
                                      ? EditObject.CocheOperacion.Rendimiento.ToString("#0.00")
                                      : EditObject.Modelo != null
                                            ? EditObject.Modelo.Rendimiento.ToString("#0.00")
                                            : "0,00";

            txtCosto.Text = EditObject.CocheOperacion != null ? EditObject.CocheOperacion.CostoKmUltimoMes.ToString("#0.00") : "0.00";
            dtInicioActividad.SelectedDate = EditObject.CocheOperacion != null
                                                 ? EditObject.CocheOperacion.FechaInicio.ToDisplayDateTime()
                                                 : DateTime.Today;
        }

        protected override void OnDelete() { DAOFactory.CocheDAO.Delete(EditObject); } 

        protected override void OnSave()
        {
            var cambioDeDistrito = EditObject.Empresa != null && EditObject.Empresa.Id != ddlEmpresa.Selected;

            using (var transaction = SmartTransaction.BeginTransaction())
            {

                try
                {
                    var entidad = EditMode && EditObject.Dispositivo != null
                        ? DAOFactory.EntidadDAO.FindByDispositivo(new[] {EditObject.Empresa != null ? EditObject.Empresa.Id : -1},
                            new[] {EditObject.Linea != null ? EditObject.Linea.Id : -1}, new[] {EditObject.Dispositivo != null ? EditObject.Dispositivo.Id : -1})
                        : null;

                    EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
                    EditObject.Empresa = ddlEmpresa.Selected > 0
                        ? DAOFactory.EmpresaDAO.FindById(ddlEmpresa.Selected)
                        : EditObject.Linea != null ? EditObject.Linea.Empresa : null;
                    EditObject.Interno = txtInterno.Text.Trim();
                    //EditObject.Marca = AltaMarca1.MarcaId >= 0 ? DAOFactory.MarcaDAO.FindById(AltaMarca1.MarcaId) : null;
                    EditObject.Modelo = cbModelo.Selected > 0 ? DAOFactory.ModeloDAO.FindById(cbModelo.Selected) : null;
                    EditObject.ModeloDescripcion = EditObject.Modelo != null 
                        ? EditObject.Modelo.Descripcion.Length > 32
                            ? EditObject.Modelo.Descripcion.Substring(0,32) 
                            : EditObject.Modelo.Descripcion
                        : string.Empty;
                    EditObject.Patente = txtPatente.Text;
                    EditObject.AnioPatente = Convert.ToInt16(npAnio.Value);
                    EditObject.NroChasis = txtNumeroChasis.Text;
                    EditObject.NroMotor = txtNumeroMotor.Text;
                    EditObject.Poliza = txtPoliza.Text;
                    EditObject.Chofer = cbEmpleado.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(cbEmpleado.Selected) : null;
                    EditObject.TipoCoche = DAOFactory.TipoCocheDAO.FindById(cbTipoCoche.Selected);
                    EditObject.Referencia = txtRefference.Text;

                    EditObject.Dispositivo = cbDispositivo.Selected > 0 ? DAOFactory.DispositivoDAO.FindById(cbDispositivo.Selected) : null;
                    EditObject.Transportista = ddlTransportista.Selected > 0 ? DAOFactory.TransportistaDAO.FindById(ddlTransportista.Selected) : null;
                    EditObject.FechaVto = dpVencimiento.SelectedDate.HasValue && dpVencimiento.SelectedDate.Value != DateTime.MinValue
                        ? dpVencimiento.SelectedDate.Value.ToDataBaseDateTime()
                        : (DateTime?) null;
                    EditObject.Departamento = cbDepartamento.Selected > 0 ? DAOFactory.DepartamentoDAO.FindById(cbDepartamento.Selected) : null;
                    EditObject.CentroDeCostos = ddlCentro.Selected > 0 ? DAOFactory.CentroDeCostosDAO.FindById(ddlCentro.Selected) : null;
                    EditObject.SubCentroDeCostos = ddlSubCentro.Selected > 0 ? DAOFactory.SubCentroDeCostosDAO.FindById(ddlSubCentro.Selected) : null;
                    EditObject.ControlaHs = chkControlaHs.Checked;
                    EditObject.ControlaKm = chkControlaKm.Checked;
                    EditObject.PorcentajeProductividad = Convert.ToInt32(npPorcentaje.Value > 0 ? npPorcentaje.Value : 0);

                    var controla = chkControlaKm.Checked || chkControlaHs.Checked;

                    EditObject.ControlaServicios = controla && chkControlaServicios.Checked;
                    EditObject.ControlaTurnos = controla && chkControlaTurnos.Checked;

                    EditObject.KilometrosDiarios = npKilometros.Value;
                    EditObject.VelocidadPromedio = Convert.ToInt32(npVelocidadPromedio.Value);
                    EditObject.Capacidad = Convert.ToInt32(txtCapacidad.Value);
                    EditObject.CapacidadCarga = Convert.ToInt32(npCapacidadCarga.Value);
                    EditObject.IdentificaChoferes = chkIdentificaChoferes.Checked;
                    if (chkReportaAssistCargo.Visible) EditObject.ReportaAssistCargo = chkReportaAssistCargo.Checked;
                    if (chkEsPuerta.Visible) EditObject.EsPuerta = chkEsPuerta.Checked;

                    EditObject.InitialOdometer = npOdometroInicial.Value;

                    #region Add Clientes

                    EditObject.Clientes.Clear();
                    var clientes = cbCliente.SelectedValues.Where(c => c > 0);
                    foreach (var id in clientes) EditObject.Clientes.Add(DAOFactory.ClienteDAO.FindById(id));

                    #endregion

                    var estado = (short) cbEstados.Selected;

                    if (estado != EditObject.Estado)
                        EditObject.DtCambioEstado = DateTime.UtcNow;

                    EditObject.Estado = estado;

                    #region Coche Operación

                    if (EditObject.Id == 0)
                    {
                        var cocheOperacion = new CocheOperacion
                                             {
                                                 FechaInicio = DateTime.Today.ToDataBaseDateTime(),
                                                 CostoDiaHistorico = 0.0,
                                                 CostoKmHistorico = 0.0,
                                                 CostoKmUltimoAnio = 0.0,
                                                 CostoKmUltimoMes = 0.0,
                                                 CostoMesHistorico = 0.0,
                                                 CostoUltimoMes = 0.0,
                                                 Rendimiento = 0.0
                                             };
                        DAOFactory.CocheOperacionDAO.Save(cocheOperacion);

                        EditObject.CocheOperacion = cocheOperacion;
                    }
                    else
                    {
                        if (EditObject.CocheOperacion != null)
                        {
                            EditObject.CocheOperacion.FechaInicio = dtInicioActividad.SelectedDate.Value.ToDataBaseDateTime();
                        }
                        else
                        {
                            var cocheOperacion = new CocheOperacion
                                                 {
                                                     FechaInicio = dtInicioActividad.SelectedDate.Value.ToDataBaseDateTime(),
                                                     CostoDiaHistorico = 0.0,
                                                     CostoKmHistorico = 0.0,
                                                     CostoKmUltimoAnio = 0.0,
                                                     CostoKmUltimoMes = 0.0,
                                                     CostoMesHistorico = 0.0,
                                                     CostoUltimoMes = 0.0,
                                                     Rendimiento = 0.0
                                                 };
                            DAOFactory.CocheOperacionDAO.Save(cocheOperacion);

                            EditObject.CocheOperacion = cocheOperacion;
                        }
                    }

                    #endregion

                    if (EditMode)
                    {
                        var puerta = DAOFactory.PuertaAccesoDAO.FindByVehiculo(EditObject.Id);
                        if (puerta != null &&
                            (!Equals(puerta.Empresa, EditObject.Empresa) || !Equals(puerta.Linea, EditObject.Linea)))
                        {
                            puerta.Empresa = EditObject.Empresa;
                            puerta.Linea = EditObject.Linea;
                            DAOFactory.PuertaAccesoDAO.SaveOrUpdate(puerta);
                        }
                    }

                    DAOFactory.CocheDAO.SaveOrUpdate(EditObject);

                    #region Agregar Coche al Usuario

                    var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

                    if (!EditMode &&
                        user.PorCoche)
                    {
                        user.AddCoche(EditObject);
                        DAOFactory.UsuarioDAO.SaveOrUpdate(user, true);
                    }

                    #endregion

                    if (chkTelemetria.Checked)
                    {
                        if (entidad != null &&
                            entidad.Id != 0)
                        {
                            if (EditObject.Dispositivo != null)
                            {
                                entidad.Dispositivo = EditObject.Dispositivo;
                            }
                        }
                        else
                        {
                            // pruebo si hay una con el dispo nuevo.
                            var idDispositivo = EditObject.Dispositivo != null ? EditObject.Dispositivo.Id : -1;
                            ValidateEntity(idDispositivo, "PARENTI08");
                            entidad = DAOFactory.EntidadDAO.FindByDispositivo(new[] {EditObject.Empresa != null ? EditObject.Empresa.Id : -1},
                                new[] {EditObject.Linea != null ? EditObject.Linea.Id : -1}, new[] {idDispositivo});

                            if (entidad == null)
                            {
                                var tipoEntidad = DAOFactory.TipoEntidadDAO.FindByCode(new[] {EditObject.Empresa != null ? EditObject.Empresa.Id : -1},
                                    new[] {EditObject.Linea != null ? EditObject.Linea.Id : -1}, "VEH");

                                entidad = new EntidadPadre
                                          {
                                              Codigo = EditObject.Patente,
                                              Descripcion = EditObject.Interno,
                                              Dispositivo = EditObject.Dispositivo,
                                              Empresa = EditObject.Empresa,
                                              Linea = EditObject.Linea,
                                              ReferenciaGeografica = EditObject.Linea.ReferenciaGeografica,
                                              TipoEntidad = tipoEntidad,
                                              Url = "Caudalimetro definitiva.jpg"
                                          };
                            }
                        }

                        if (cambioDeDistrito) DAOFactory.EntidadDAO.SaveAndUpdateEmpresa(entidad, EditObject);
                        else DAOFactory.EntidadDAO.SaveOrUpdate(entidad);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        protected override void OnDuplicate()
        {
            base.OnDuplicate();
            DocumentList1.ClearDocumentos();

            // Reconfig y Rebind de Dispositivos para que no aparezca el del vehiculo original
            cbDispositivo.Coche = 0;
            cbDispositivo.DataBind();
            upDispositivo.Update();
            chkTelemetria.Enabled = true;
            upTelemetria.Update();
        }

        protected void BtnActualizar_OnClick(object sender, EventArgs e)
        {
            if (dtFechaDesde.SelectedDate != null && dtFechaHasta.SelectedDate != null)
                listConsumos.LoadConsumos(EditObject.Id, dtFechaDesde.SelectedDate.Value.ToDataBaseDateTime(), dtFechaHasta.SelectedDate.Value.ToDataBaseDateTime());
        }

        #region Validations

        protected override void ValidateSave()
        {
            ValidateEntity(ddlEmpresa.Selected, "PARENTI01");

            var interno = ValidateEmpty(txtInterno.Text, "INTERNO");
            var patente = ValidateEmpty(txtPatente.Text, "PATENTE");

            var tipo = DAOFactory.TipoCocheDAO.FindById(cbTipoCoche.Selected);
            //if(!tipo.NoEsVehiculo) ValidateEntity(cbModelo.Selected, "PARENTI61");

            if (chkTelemetria.Checked)
            {
                ValidateEntity(cbLinea.Selected, "PARENTI02");
                var tipoEntidad = DAOFactory.TipoEntidadDAO.FindByCode(new[] {ddlEmpresa.Selected},
                                                                       new[] {cbLinea.Selected},
                                                                       "VEH");
                var id = tipoEntidad != null ? tipoEntidad.Id : 0;
                if (id <= 0) ThrowError("NO_EXISTE_VEH");
            }
            
            if (tipo.Id > 0 && tipo.SeguimientoPersona)
                ValidateEntity(cbEmpleado.Selected, "Labels", "RESPONSABLE");

            if (npOdometroInicial.Value < 0) ThrowMustEnter("ODOMETRO_VALOR_INICIAL");

            if (cbEstados.Selected == Coche.Estados.Inactivo && (cbDispositivo.SelectedIndex >= 0 && cbDispositivo.Selected != cbDispositivo.AllValue)) ThrowError("VEHICULO_ASIGNADO");

            var byInterno = DAOFactory.CocheDAO.FindByInterno(ddlEmpresa.SelectedValues, cbLinea.SelectedValues, interno);
            ValidateDuplicated(byInterno, "INTERNO");

            var byPatente = DAOFactory.CocheDAO.FindByPatente(ddlEmpresa.Selected, patente);
            ValidateDuplicated(byPatente, "PATENTE");
        }

        protected override void ValidateDelete() { if (cbDispositivo.Selected > 0) ThrowError("VEHICULO_CON_DISPOSITIVO"); }

        #endregion

        #region Controls Events

        protected void ChkChanged(object sender, EventArgs eventArgs)
        {
            var controla = chkControlaHs.Checked || chkControlaKm.Checked;
            chkControlaServicios.Visible = chkControlaTurnos.Visible = npPorcentaje.Visible = lblPorcentaje.Visible = controla;
            BindShifts();
        }

        protected void DdlTransportistaSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!EditMode)
            {
                var transportista = ddlTransportista.Selected > 0 ? DAOFactory.TransportistaDAO.FindById(ddlTransportista.Selected) : null;
                
                if (transportista != null)
                {
                    chkIdentificaChoferes.Checked = transportista.IdentificaChoferes;
                }
                else
                {
                    var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
                    
                    if (linea != null)
                    {
                        chkIdentificaChoferes.Checked = linea.IdentificaChoferes;
                    }
                    else
                    {
                        var empresa = ddlEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlEmpresa.Selected) : null;
                        chkIdentificaChoferes.Checked = empresa != null && empresa.IdentificaChoferes;
                    }
                }
            }
        }

        protected void CbTipoCocheSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTipoCoche.SelectedIndex < 0) { return; }
            BindShifts();
            if (!EditMode && cbTipoCoche.Selected > 0)
            {
                var tipoCoche = DAOFactory.TipoCocheDAO.FindById(cbTipoCoche.Selected);
                npKilometros.Value = Convert.ToInt32(tipoCoche.KilometrosDiarios);
                txtCapacidad.Value = tipoCoche.Capacidad;
                npCapacidadCarga.Value = tipoCoche.CapacidadCarga;
            }

            var tipo = DAOFactory.TipoCocheDAO.FindById(cbTipoCoche.Selected);
            panelDatosVehiculo.Visible = !tipo.NoEsVehiculo;
            abmTabOdometros.Show = !tipo.NoEsVehiculo;
            abmTabCostos.Show = !tipo.NoEsVehiculo;
            panelValoresReferencia.Visible = !tipo.NoEsVehiculo;
            lblPatente.VariableName = tipo.NoEsVehiculo ? "CODE" : "PATENTE";
        }

        protected void DdlDepartamentoSelectedIndexChanged(object sender, EventArgs e) { BindShifts(); }

        protected void DdlCentroSelectedIndexChanged(object sender, EventArgs e) { BindShifts(); }

        protected void CbDispositivoPreBind(object sender, EventArgs e)
        {
            if (EditMode && EditObject.Dispositivo != null) 
                cbDispositivo.Coche = EditObject.Id;
        }

        protected void BtnResetClick(object sender, EventArgs e)
        {
            if (EditMode)
            {
                EditObject.ResetPartialOdometer();
                DAOFactory.CocheDAO.SaveOrUpdate(EditObject);
                Bind();
            }
        }

        #region Grid Odometros

        protected void GridOdometrosItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var odometro = e.Row.DataItem as MovOdometroVehiculoVO;
                if (odometro != null)
                {
                    ((C1NumericInput)e.Row.Cells[2].FindControl("npAjusteKm")).Value = Convert.ToDouble(odometro.AjusteKilometros);
                    ((C1NumericInput)e.Row.Cells[2].FindControl("npAjusteKm")).Enabled = odometro.Kilometros != "-";

                    ((C1NumericInput)e.Row.Cells[4].FindControl("npAjusteDias")).Value = Convert.ToDouble(odometro.AjusteDias);
                    ((C1NumericInput)e.Row.Cells[4].FindControl("npAjusteDias")).Enabled = odometro.Dias != "-";

                    ((C1NumericInput)e.Row.Cells[6].FindControl("npAjusteHoras")).Value = Convert.ToDouble(odometro.AjusteHoras);
                    ((C1NumericInput)e.Row.Cells[6].FindControl("npAjusteHoras")).Enabled = odometro.Horas != "-";

                    e.Row.Cells[7].Text = odometro.UltimoUpdate.Equals(DateTime.MinValue)
                                                                ? "-"
                                                                : string.Format("{0} {1}",
                                                                                odometro.UltimoUpdate.ToDisplayDateTime().ToShortDateString(),
                                                                                odometro.UltimoUpdate.ToDisplayDateTime().ToShortTimeString());

                    e.Row.Cells[8].Text = odometro.UltimoDisparo.Equals(DateTime.MinValue)
                                                                ? "-"
                                                                : string.Format("{0} {1}",
                                                                                odometro.UltimoDisparo.ToDisplayDateTime().ToShortDateString(),
                                                                                odometro.UltimoDisparo.ToDisplayDateTime().ToShortTimeString());
                                        
                    var reset = e.Row.FindControl("lnkReset") as LinkButton;

                    if (reset != null)
                        reset.Visible = odometro.EsReseteable;

                    if (odometro.Color != Color.Empty)
                        e.Row.BackColor = odometro.Color;
                }
            }
        }

        protected void GridOdometrosItemCommand(object sender, C1GridViewCommandEventArgs e)
        {
            if (EditMode && e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var odometro = GetOdometrosList()[e.Row.RowIndex];
                switch (e.CommandName)
                {
                    case "Reset":
                        DAOFactory.MovOdometroVehiculoDAO.ResetOdometer(odometro.IdMovOdometro);
                        Bind();
                        break;
                    case "Actualizar":
                        var ajusteKm = ((C1NumericInput)e.Row.FindControl("npAjusteKm")).Value;
                        var ajusteDias = ((C1NumericInput)e.Row.FindControl("npAjusteDias")).Value;
                        var ajusteHoras = ((C1NumericInput)e.Row.FindControl("npAjusteHoras")).Value;
                        DAOFactory.MovOdometroVehiculoDAO.UpdateOdometer(odometro.IdMovOdometro, ajusteKm, (int)ajusteDias, ajusteHoras);
                        Response.Redirect("~/Parametrizacion/CocheLista.aspx");
                        break;
                }
            }
        }

        #endregion

        #region Grid Costos

        protected void GridCostosItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var concepto = e.Row.DataItem as string[];
                if (concepto != null)
                {
                    var lbl = e.Row.FindControl("lblDescripcion") as Label;
                    if (lbl != null) lbl.Text = concepto[0];

                    lbl = e.Row.FindControl("lblCorriente") as Label;
                    if (lbl != null) lbl.Text = concepto[1];

                    lbl = e.Row.FindControl("lblAcumuladoAnual") as Label;
                    if (lbl != null) lbl.Text = concepto[2];

                    lbl = e.Row.FindControl("lblAcumuladoTotal") as Label;
                    if (lbl != null) lbl.Text = concepto[3];
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region Private Methods

        #region Turnos

        private void BindShifts()
        {
            var shifts = DAOFactory.ShiftDAO.GetVehicleShifts(EditObject);
            panelTurnos.Visible = !shifts.Count.Equals(0) && (chkControlaHs.Checked || chkControlaKm.Checked);

            if (panelTurnos.Visible)
            {
                gridTurnos.DataSource = (from Shift s in shifts select new ShiftVO(s)).ToList();
                gridTurnos.DataBind();
            }
        } 

        #endregion

        #region Odometros

        private void BindOdometros()
        {
            npOdometroInicial.Value = Convert.ToInt32(EditObject.InitialOdometer);
            lblDailyOdometer.Text = string.Concat(EditObject.DailyOdometer.ToString("0.00"), "km");
            lblApplicationOdometer.Text = string.Concat(EditObject.ApplicationOdometer.ToString("0.00"), "km");
            lblPartialOdometer.Text = string.Concat(EditObject.PartialOdometer.ToString("0.00"), "km");
            lblTotalOdometer.Text = string.Concat((EditObject.InitialOdometer + EditObject.ApplicationOdometer).ToString("0.00"), "km");

            AddLastUpdateDate();
            AddLastResetDate();

            lblOdometroAplicacion.Visible = lblOdometroDiario.Visible = lblOdometroTotal.Visible = lblOdometroParcial.Visible = btnReset.Visible = true;

            BindVehicleOdometers();
        }

        private void AddLastUpdateDate()
        {
            if (EditObject.LastOdometerUpdate.HasValue)
            {
                lblUltimaActualizacion.Visible = true;
                var lastUpdate = EditObject.LastOdometerUpdate.Value.ToDisplayDateTime();
                lblLastUpdate.Text = string.Format("{0} {1}", lastUpdate.ToShortDateString(), lastUpdate.ToShortTimeString());
            }
        }

        private void AddLastResetDate()
        {
            if (EditObject.LastOdometerReset.HasValue)
            {
                var reset = EditObject.LastOdometerReset.Value.ToDisplayDateTime();
                lblPartialOdometer.Text = string.Concat(lblPartialOdometer.Text, "(", reset.ToShortDateString(), " ", reset.ToShortTimeString(), ")");
            }
        }

        private void BindVehicleOdometers()
        {
            gridOdometros.Visible = EditObject.Odometros.Count > 0;

            if (gridOdometros.Visible)
            {
                gridOdometros.DataSource = GetOdometrosList();
                gridOdometros.DataBind();
            }
        }

        private List<MovOdometroVehiculoVO> GetOdometrosList()
        {
            return EditObject.Odometros.OfType<MovOdometroVehiculo>()
                .Select(o => new MovOdometroVehiculoVO(o))
                .OrderBy(o => o.Descripcion)
                .ToList();
        }

        #endregion

        #region Costos

        private void BindCostos()
        {
            var desdeCorriente = DateTime.Today.AddDays(-DateTime.Today.Day + 1).ToDataBaseDateTime();
            var desdeAnual = DateTime.Today.AddMonths(-DateTime.Today.Month + 1).AddDays(-DateTime.Today.Day + 1).ToDataBaseDateTime();
            var desdeTotal = EditObject.CocheOperacion != null ? EditObject.CocheOperacion.FechaInicio : DateTime.MinValue;
            var hasta = DateTime.UtcNow;

            // KM RECORRIDOS, HS MOVIMIENTO Y HS RALENTI
            var routesCorriente = MergeResults(ReportFactory.MobileRoutesDAO.GetMobileRoutes(EditObject.Id, desdeCorriente, hasta));
            var routesAnual = MergeResults(ReportFactory.MobileRoutesDAO.GetMobileRoutes(EditObject.Id, desdeAnual, hasta));
            var routesTotal = desdeTotal != DateTime.MinValue ? MergeResults(ReportFactory.MobileRoutesDAO.GetMobileRoutes(EditObject.Id, desdeTotal, hasta)) : null;
            
            var kmCorriente = 0.0;
            var kmAnual = 0.0;
            var kmTotal = 0.0;
            
            var hsMovimientoCorriente = 0.0;
            var hsMovimientoAnual = 0.0;
            var hsMovimientoTotal = 0.0;
            
            var hsRalentiCorriente = 0.0;
            var hsRalentiAnual = 0.0;
            var hsRalentiTotal = 0.0;
            
            foreach (var route in routesCorriente)
            {
                kmCorriente += route.Kilometers;

                if (route.VehicleStatus == "En Movimiento")
                    hsMovimientoCorriente += route.Duration;

                if (route.EngineStatus == "Encendido" && route.VehicleStatus == "Detenido")
                    hsRalentiCorriente += route.Duration;
            }

            foreach (var route in routesAnual)
            {
                kmAnual += route.Kilometers;

                if (route.VehicleStatus == "En Movimiento")
                    hsMovimientoAnual += route.Duration;

                if (route.EngineStatus == "Encendido" && route.VehicleStatus == "Detenido")
                    hsRalentiAnual += route.Duration;
            }

            if (routesTotal != null)
            {
                foreach (var route in routesTotal)
                {
                    kmTotal += route.Kilometers;

                    if (route.VehicleStatus == "En Movimiento")
                        hsMovimientoTotal += route.Duration;

                    if (route.EngineStatus == "Encendido" && route.VehicleStatus == "Detenido")
                        hsRalentiTotal += route.Duration;
                }
            }

            //CONSUMO (NO COMBUSTIBLE)
            var consumosCorriente = DAOFactory.ConsumoDetalleDAO.GetList(new[] {EditObject.Empresa != null ? EditObject.Empresa.Id : -1}, 
                                                                         new[] {EditObject.Linea != null ? EditObject.Linea.Id : -1}, 
                                                                         new[] {EditObject.Transportista != null ? EditObject.Transportista.Id : -1}, 
                                                                         new[] {-1}, // DEPARTAMENTO
                                                                         new[] {EditObject.CentroDeCostos != null ? EditObject.CentroDeCostos.Id : -1},
                                                                         new[] {EditObject.TipoCoche != null ? EditObject.TipoCoche.Id : -1},
                                                                         new[] {EditObject.Id}, 
                                                                         new[] {-1}, // TIPO EMPLEADO
                                                                         new[] {-1}, // EMPLEADO
                                                                         new[] {-1}, // TIPO PROVEEDOR
                                                                         new[] {-1}, // PROVEEDOR
                                                                         new[] { -1 }, // DEPOSITO ORIGEN
                                                                         new[] { -1 }, // DEPOSITO DESTINO
                                                                         desdeCorriente, 
                                                                         hasta, 
                                                                         new[] {-1})
                                                                .Where(c => !c.Insumo.TipoInsumo.DeCombustible);
            var totalConsumosCorriente = consumosCorriente.Sum(consumo => consumo.ImporteTotal);

            var consumosAnual = DAOFactory.ConsumoDetalleDAO.GetList(new[] { EditObject.Empresa != null ? EditObject.Empresa.Id : -1 },
                                                                     new[] { EditObject.Linea != null ? EditObject.Linea.Id : -1 },
                                                                     new[] { EditObject.Transportista != null ? EditObject.Transportista.Id : -1 },
                                                                     new[] { -1 }, // DEPARTAMENTO
                                                                     new[] { EditObject.CentroDeCostos != null ? EditObject.CentroDeCostos.Id : -1 },
                                                                     new[] { EditObject.TipoCoche != null ? EditObject.TipoCoche.Id : -1 },
                                                                     new[] { EditObject.Id },
                                                                     new[] { -1 }, // TIPO EMPLEADO
                                                                     new[] { -1 }, // EMPLEADO
                                                                     new[] { -1 }, // TIPO PROVEEDOR
                                                                     new[] { -1 }, // PROVEEDOR
                                                                     new[] { -1 }, // DEPOSITO ORIGEN
                                                                     new[] { -1 }, // DEPOSITO DESTINO
                                                                     desdeAnual, 
                                                                     hasta, 
                                                                     new[] { -1 })
                                                            .Where(c => !c.Insumo.TipoInsumo.DeCombustible);
            var totalConsumosAnual = consumosAnual.Sum(consumo => consumo.ImporteTotal);

            var consumosTotal = desdeTotal != DateTime.MinValue
                                    ? DAOFactory.ConsumoDetalleDAO.GetList(new[] {EditObject.Empresa != null ? EditObject.Empresa.Id : -1},
                                                                           new[] {EditObject.Linea != null ? EditObject.Linea.Id : -1},
                                                                           new[] {EditObject.Transportista != null ? EditObject.Transportista.Id : -1},
                                                                           new[] {-1}, // DEPARTAMENTO
                                                                           new[] {EditObject.CentroDeCostos != null ? EditObject.CentroDeCostos.Id : -1},
                                                                           new[] {EditObject.TipoCoche != null ? EditObject.TipoCoche.Id : -1},
                                                                           new[] {EditObject.Id},
                                                                           new[] {-1}, // TIPO EMPLEADO
                                                                           new[] {-1}, // EMPLEADO
                                                                           new[] {-1}, // TIPO PROVEEDOR
                                                                           new[] {-1}, // PROVEEDOR
                                                                           new[] { -1 }, // DEPOSITO ORIGEN
                                                                           new[] { -1 }, // DEPOSITO DESTINO
                                                                           desdeTotal,
                                                                           hasta,
                                                                           new[] {-1})
                                                                  .Where(c => !c.Insumo.TipoInsumo.DeCombustible)
                                    : null;
            var totalConsumosTotal = consumosTotal != null ? consumosTotal.Sum(consumo => consumo.ImporteTotal) : 0.0;

            //COSTO X KM
            var costoXKmCorriente = kmCorriente != 0.0 ? totalConsumosCorriente/kmCorriente : 0.0;
            var costoXKmAnual = kmAnual != 0.0 ? totalConsumosAnual / kmAnual : 0.0;
            var costoXKmTotal = kmTotal != 0.0 ? totalConsumosTotal / kmTotal : 0.0;

            // CONSUMO COMBUSTIBLE COSTO PROMEDIO X LITRO Y LITROS CARGADOS
            var consumosCombustibleCorriente = DAOFactory.ConsumoDetalleDAO.GetList(new[] { EditObject.Empresa != null ? EditObject.Empresa.Id : -1 },
                                                                                    new[] { EditObject.Linea != null ? EditObject.Linea.Id : -1 },
                                                                                    new[] { EditObject.Transportista != null ? EditObject.Transportista.Id : -1 },
                                                                                    new[] { -1 }, // DEPARTAMENTO
                                                                                    new[] { EditObject.CentroDeCostos != null ? EditObject.CentroDeCostos.Id : -1 },
                                                                                    new[] { EditObject.TipoCoche != null ? EditObject.TipoCoche.Id : -1 },
                                                                                    new[] { EditObject.Id },
                                                                                    new[] { -1 }, // TIPO EMPLEADO
                                                                                    new[] { -1 }, // EMPLEADO
                                                                                    new[] { -1 }, // TIPO PROVEEDOR
                                                                                    new[] { -1 }, // PROVEEDOR
                                                                                    new[] { -1 }, // DEPOSITO ORIGEN
                                                                                    new[] { -1 }, // DEPOSITO DESTINO
                                                                                    desdeCorriente, 
                                                                                    hasta, 
                                                                                    new[] { -1 })
                                                                           .Where(c => c.Insumo.TipoInsumo.DeCombustible);
            var totalConsumosCombustibleCorriente = consumosCombustibleCorriente.Sum(consumo => consumo.ImporteTotal);
            var cantidadCombustibleCorriente = consumosCombustibleCorriente.Sum(consumo => consumo.Cantidad);
            var promedioXLitroCorriente = cantidadCombustibleCorriente != 0.0
                                              ? totalConsumosCombustibleCorriente/cantidadCombustibleCorriente
                                              : 0.0;

            var consumosCombustibleAnual = DAOFactory.ConsumoDetalleDAO.GetList(new[] { EditObject.Empresa != null ? EditObject.Empresa.Id : -1 },
                                                                                new[] { EditObject.Linea != null ? EditObject.Linea.Id : -1 },
                                                                                new[] { EditObject.Transportista != null ? EditObject.Transportista.Id : -1 },
                                                                                new[] { -1 }, // DEPARTAMENTO
                                                                                new[] { EditObject.CentroDeCostos != null ? EditObject.CentroDeCostos.Id : -1 },
                                                                                new[] { EditObject.TipoCoche != null ? EditObject.TipoCoche.Id : -1 },
                                                                                new[] { EditObject.Id },
                                                                                new[] { -1 }, // TIPO EMPLEADO
                                                                                new[] { -1 }, // EMPLEADO
                                                                                new[] { -1 }, // TIPO PROVEEDOR
                                                                                new[] { -1 }, // PROVEEDOR
                                                                                new[] { -1 }, // DEPOSITO ORIGEN
                                                                                new[] { -1 }, // DEPOSITO DESTINO
                                                                                desdeAnual, 
                                                                                hasta, 
                                                                                new[] { -1 })
                                                                       .Where(c => c.Insumo.TipoInsumo.DeCombustible);
            var totalConsumosCombustibleAnual = consumosCombustibleAnual.Sum(consumo => consumo.ImporteTotal);
            var cantidadCombustibleAnual = consumosCombustibleAnual.Sum(consumo => consumo.Cantidad);
            var promedioXLitroAnual = cantidadCombustibleAnual != 0.0
                                          ? totalConsumosCombustibleAnual/cantidadCombustibleAnual
                                          : 0.0;

            var consumosCombustibleTotal = desdeTotal != DateTime.MinValue
                                               ? DAOFactory.ConsumoDetalleDAO.GetList(new[] {EditObject.Empresa != null ? EditObject.Empresa.Id : -1},
                                                                                      new[] {EditObject.Linea != null ? EditObject.Linea.Id : -1},
                                                                                      new[] {EditObject.Transportista != null ? EditObject.Transportista.Id : -1},
                                                                                      new[] {-1}, // DEPARTAMENTO
                                                                                      new[] {EditObject.CentroDeCostos != null ? EditObject.CentroDeCostos.Id : -1},
                                                                                      new[] {EditObject.TipoCoche != null ? EditObject.TipoCoche.Id : -1},
                                                                                      new[] {EditObject.Id},
                                                                                      new[] {-1}, // TIPO EMPLEADO
                                                                                      new[] {-1}, // EMPLEADO
                                                                                      new[] {-1}, // TIPO PROVEEDOR
                                                                                      new[] {-1}, // PROVEEDOR
                                                                                      new[] { -1 }, // DEPOSITO ORIGEN
                                                                                      new[] { -1 }, // DEPOSITO DESTINO
                                                                                      desdeTotal,
                                                                                      hasta,
                                                                                      new[] {-1})
                                                                             .Where(c => c.Insumo.TipoInsumo.DeCombustible)
                                               : null;
            var totalConsumosCombustibleTotal = consumosCombustibleTotal != null ? consumosCombustibleTotal.Sum(consumo => consumo.ImporteTotal) : 0.0;
            var cantidadCombustibleTotal = consumosCombustibleTotal != null ? consumosCombustibleTotal.Sum(consumo => consumo.Cantidad) : 0.0;
            var promedioXLitroTotal = cantidadCombustibleTotal != 0.0
                                          ? totalConsumosCombustibleTotal / cantidadCombustibleTotal
                                          : 0.0;

            var costoCombustibleXKmCorriente = kmCorriente != 0.0 ? totalConsumosCombustibleCorriente/kmCorriente : 0.0;
            var costoCombustibleXKmAnual = kmAnual != 0.0 ? totalConsumosCombustibleAnual/ kmAnual : 0.0;
            var costoCombustibleXKmTotal = kmTotal != 0.0 ? totalConsumosCombustibleTotal / kmTotal : 0.0;

            //KM PRODUCTIVO TICKET
            //var kmTicketCorriente = 0.0;
            //var kmTicketAnual = 0.0;
            //var kmTicketTotal = 0.0;

            //COSTO PRODUCTIVO
            //var costoProductivoCorriente = kmTicketCorriente != 0.0 ? (totalConsumosCorriente+totalConsumosCombustibleCorriente) / kmTicketCorriente : 0.0;
            //var costoProductivoAnual = kmTicketAnual != 0.0 ? (totalConsumosAnual+totalConsumosCombustibleAnual) / kmTicketAnual : 0.0;
            //var costoProductivoTotal = kmTicketTotal != 0.0 ? (totalConsumosTotal+totalConsumosCombustibleTotal) / kmTicketTotal : 0.0;

            //LITROS CONSUMIDOS
            var rendimiento = EditObject.CocheOperacion != null && EditObject.CocheOperacion.Rendimiento > 0
                                  ? EditObject.CocheOperacion.Rendimiento
                                  : EditObject.Modelo != null && EditObject.Modelo.Rendimiento > 0
                                        ? EditObject.Modelo.Rendimiento
                                        : 0;
            var litrosConsumidosCorriente = (kmCorriente / 100.0) * rendimiento;
            var litrosConsumidosAnual = (kmAnual / 100.0) * rendimiento;
            var litrosConsumidosTotal = (kmTotal / 100.0) * rendimiento;

            //AMORTIZACION
            var amortizacionCorriente = EditObject.Modelo != null && EditObject.Modelo.VidaUtil != 0
                                            ? (double)EditObject.Modelo.Costo/(EditObject.Modelo.VidaUtil*12)
                                            : 0.0;
            var amortizacionAnual = EditObject.Modelo != null && EditObject.Modelo.VidaUtil != 0
                                            ? amortizacionCorriente * hasta.Month
                                            : 0.0;
            var amortizacionTotal = EditObject.Modelo != null && EditObject.Modelo.VidaUtil != 0 && desdeTotal != DateTime.MinValue
                                        ? hasta.Year == desdeTotal.Year
                                              ? (hasta.Month - desdeTotal.Month) * amortizacionCorriente
                                              : hasta.Year > desdeTotal.Year
                                                    ? (((hasta.Year - desdeTotal.Year - 1) * 12)
                                                      + hasta.Month == desdeTotal.Month
                                                          ? 12
                                                          : hasta.Month > desdeTotal.Month
                                                                ? hasta.Month - desdeTotal.Month
                                                                : 12 - desdeTotal.Month + hasta.Month) * amortizacionCorriente
                                                    : 0.0
                                        : 0.0;

            var costos = new List<string[]>
                             {
                                 new[] {CultureManager.GetLabel("KM_RECORRIDOS"), kmCorriente.ToString("#0.00") + " Km.", kmAnual.ToString("#0.00") + " Km.", kmTotal.ToString("#0.00") + " Km."},
                                 //new[] {CultureManager.GetLabel("KM_PRODUCTIVOS"), kmTicketCorriente.ToString("#0.00"), kmTicketAnual.ToString("#0.00"), kmTicketTotal.ToString("#0.00")},
                                 
                                 new[] {CultureManager.GetLabel("HS_MOVIMIENTO"), hsMovimientoCorriente.ToString("#0.00") + " Hs.", hsMovimientoAnual.ToString("#0.00") + " Hs.", hsMovimientoTotal.ToString("#0.00") + " Hs."},
                                 new[] {CultureManager.GetLabel("HS_RALENTI"), hsRalentiCorriente.ToString("#0.00") + " Hs.", hsRalentiAnual.ToString("#0.00") + " Hs.", hsRalentiTotal.ToString("#0.00") + " Hs."},
                                 
                                 new[] {CultureManager.GetLabel("CONSUMO_$_NO_COMBUSTIBLE"), "$" + totalConsumosCorriente.ToString("#0.00"), "$" + totalConsumosAnual.ToString("#0.00"), "$" + totalConsumosTotal.ToString("#0.00")},
                                 new[] {CultureManager.GetLabel("CONSUMO_$_COMBUSTIBLE"), "$" + totalConsumosCombustibleCorriente.ToString("#0.00"), "$" + totalConsumosCombustibleAnual.ToString("#0.00"), "$" + totalConsumosCombustibleTotal.ToString("#0.00")},
                                 new[] {CultureManager.GetLabel("CONSUMO_$_TOTAL"), "$" + (totalConsumosCorriente+totalConsumosCombustibleCorriente).ToString("#0.00"), "$" + (totalConsumosAnual+totalConsumosCombustibleAnual).ToString("#0.00"), "$" + (totalConsumosTotal+totalConsumosCombustibleTotal).ToString("#0.00")},
                                 
                                 new[] {CultureManager.GetLabel("COSTO_NO_COMBUSTIBLE_X_KM"), "$" + costoXKmCorriente.ToString("#0.00"), "$" + costoXKmAnual.ToString("#0.00"), "$" + costoXKmTotal.ToString("#0.00")},
                                 new[] {CultureManager.GetLabel("COSTO_COMBUSTIBLE_X_KM"), "$" + costoCombustibleXKmCorriente.ToString("#0.00"), "$" + costoCombustibleXKmAnual.ToString("#0.00"), "$" + costoCombustibleXKmTotal.ToString("#0.00")},
                                 
                                 new[] {CultureManager.GetLabel("COSTO_TOTAL_X_KM"), "$" + (costoXKmCorriente+costoCombustibleXKmCorriente).ToString("#0.00"), "$" + (costoXKmAnual+costoCombustibleXKmAnual).ToString("#0.00"), "$" + (costoXKmTotal+costoCombustibleXKmTotal).ToString("#0.00")},
                                 //new[] {CultureManager.GetLabel("COSTO_TOTAL_X_KM_PRODUCTIVO"), costoProductivoCorriente.ToString("#0.00"), costoProductivoAnual.ToString("#0.00"), costoProductivoTotal.ToString("#0.00")},
                                 
                                 new[] {CultureManager.GetLabel("COSTO_PROMEDIO_LITRO"), "$" + promedioXLitroCorriente.ToString("#0.00"), "$" + promedioXLitroAnual.ToString("#0.00"), "$" + promedioXLitroTotal.ToString("#0.00")},
                                 new[] {CultureManager.GetLabel("LITROS_CARGADOS_COMBUSTIBLE"), cantidadCombustibleCorriente.ToString("#0.00") + " Lts.", cantidadCombustibleAnual.ToString("#0.00") + " Lts.", cantidadCombustibleTotal.ToString("#0.00") + " Lts."},
                                 new[] {CultureManager.GetLabel("LITROS_CONSUMIDOS"), litrosConsumidosCorriente.ToString("#0.00") + " Lts.", litrosConsumidosAnual.ToString("#0.00") + " Lts.", litrosConsumidosTotal.ToString("#0.00") + " Lts."},

                                 new[] {CultureManager.GetLabel("AMORTIZACION"), "$" + amortizacionCorriente.ToString("#0.00"), "$" + amortizacionAnual.ToString("#0.00"), "$" + amortizacionTotal.ToString("#0.00")}
                             };
            
            gridCostos.DataSource = costos;
            gridCostos.DataBind();
        }

        #endregion

        #endregion

        protected void AbmTabCostosSelected(object sender, EventArgs e)
        {
            if (gridCostos.Rows.Count > 0) return;
            BindCostos();
            upCostos.Update();
        }
        protected void AbmTabTurnosSelected(object sender, EventArgs e)
        {
            BindShifts();
            UpdatePanel3.Update();
        }
        
        private static IEnumerable<MobileRoutes> MergeResults(List<MobileRoutes> routes)
        {
            if (routes == null || routes.Count.Equals(0))
                routes = new List<MobileRoutes>();

            for (var i = 1; i < routes.Count; i++)
            {
                if (routes[i - 1].EqualState(routes[i]))
                {
                    MergeRouteFragments(routes[i - 1], routes[i]);
                    routes.RemoveAt(i);
                    i--;
                }
            }

            return routes;
        }

        private static void MergeRouteFragments(MobileRoutes pastFragment, MobileRoutes currentFragment)
        {
            pastFragment.AverageSpeed = pastFragment.AverageSpeed >= currentFragment.AverageSpeed ? pastFragment.AverageSpeed : currentFragment.AverageSpeed;
            pastFragment.Duration += currentFragment.Duration;
            pastFragment.FinalTime = currentFragment.FinalTime;
            pastFragment.InfractionsDuration += currentFragment.InfractionsDuration;
            pastFragment.Infractions += currentFragment.Infractions;
            pastFragment.Kilometers += currentFragment.Kilometers;
            pastFragment.MaxSpeed = pastFragment.MaxSpeed >= currentFragment.MaxSpeed ? pastFragment.MaxSpeed : currentFragment.MaxSpeed;
            pastFragment.MinSpeed = pastFragment.MinSpeed <= currentFragment.MinSpeed ? pastFragment.MinSpeed : currentFragment.MinSpeed;
        }
    }
}