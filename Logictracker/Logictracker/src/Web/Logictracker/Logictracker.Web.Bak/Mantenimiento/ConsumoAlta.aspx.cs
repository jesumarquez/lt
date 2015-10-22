using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.DAL.NHibernate;
using Logictracker.Messages.Saver;
using Logictracker.Messaging;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.DropDownLists;

namespace Logictracker.Mantenimiento
{
    public partial class ConsumoAlta : SecuredAbmPage<ConsumoCabecera>
    {
        public class ConsumoDetalleAux
        {
            public ConsumoDetalle ConsumoDetalle { get; set; }
            public double ConsumoAnterior { get; set; }
        }

        protected override string GetRefference() { return "MAN_CONSUMOS"; }
        protected override string VariableName { get { return "MAN_CONSUMOS"; } }
        protected override string RedirectUrl { get { return "ConsumosLista.aspx"; } }

        protected List<ConsumoDetalle> NuevosConsumos
        {
            get { return (List<ConsumoDetalle>)(ViewState["NuevosConsumos"] ?? new List<ConsumoDetalle>()); }
            set { ViewState["NuevosConsumos"] = value; }
        }

        protected List<int> ParaBorrar
        {
            get { return (List<int>)(ViewState["ParaBorrar"] ?? new List<int>()); }
            set { ViewState["ParaBorrar"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && !EditMode)
                dtFecha.SetDate();
        }

        protected override void Bind()
        {
            var idEmpresa = cbEmpresa.AllValue;
            var idLinea = cbLinea.AllValue;

            if (EditObject.Vehiculo != null && EditObject.Vehiculo.Empresa != null)
            {
                idEmpresa = EditObject.Vehiculo.Empresa.Id;
                idLinea = EditObject.Vehiculo.Linea != null ? EditObject.Vehiculo.Linea.Id : cbLinea.AllValue;
            }
            else
            {
                if (EditObject.Deposito != null && EditObject.Deposito.Empresa != null)
                {
                    idEmpresa = EditObject.Deposito.Empresa.Id;
                    idLinea = EditObject.Deposito.Linea != null ? EditObject.Deposito.Linea.Id : cbLinea.AllValue;
                }
                else
                {
                    if (EditObject.DepositoDestino != null && EditObject.DepositoDestino.Empresa != null)
                    {
                        idEmpresa = EditObject.DepositoDestino.Empresa.Id;
                        idLinea = EditObject.DepositoDestino.Linea != null ? EditObject.DepositoDestino.Linea.Id : cbLinea.AllValue;
                    }
                    else
                    {
                        if (EditObject.Proveedor != null && EditObject.Proveedor.Empresa != null)
                        {
                            idEmpresa = EditObject.Proveedor.Empresa.Id;
                            idLinea = EditObject.Proveedor.Linea != null ? EditObject.Proveedor.Linea.Id : cbLinea.AllValue;
                        }
                    }
                }
            }

            cbEmpresa.SetSelectedValue(idEmpresa);
            cbLinea.SetSelectedValue(idLinea);
            cbTransportista.SetSelectedValue(EditObject.Vehiculo != null && EditObject.Vehiculo.Transportista != null
                                                 ? EditObject.Vehiculo.Transportista.Id
                                                 : cbTransportista.AllValue);
            cbVehiculo.SetSelectedValue(EditObject.Vehiculo != null ? EditObject.Vehiculo.Id : cbVehiculo.NoneValue);
            cbEmpleado.SetSelectedValue(EditObject.Empleado != null ? EditObject.Empleado.Id : cbEmpleado.NoneValue);
            dtFecha.SelectedDate = EditObject.Fecha.ToDisplayDateTime();
            txtKilometros.Text = EditObject.KmDeclarados.ToString("#0");

            cbTipoMovimiento.SetSelectedValue(EditObject.TipoMovimiento);
            cbTipoMovimiento.Enabled = false;
            cbTipoProveedor.SetSelectedValue(EditObject.Proveedor != null && EditObject.Proveedor.TipoProveedor != null ? EditObject.Proveedor.TipoProveedor.Id : cbTipoProveedor.NoneValue);
            cbTipoProveedor.Enabled = false;
            cbProveedor.SetSelectedValue(EditObject.Proveedor != null ? EditObject.Proveedor.Id : cbProveedor.NoneValue);
            cbProveedor.Enabled = false;
            cbDeposito.SetSelectedValue(EditObject.Deposito != null ? EditObject.Deposito.Id : cbDeposito.NoneValue);
            cbDeposito.Enabled = false;
            cbDepositoDestino.SetSelectedValue(EditObject.DepositoDestino != null ? EditObject.DepositoDestino.Id : cbDepositoDestino.NoneValue);
            cbDepositoDestino.Enabled = false;
            
            txtFactura.Text = EditObject.NumeroFactura;
            txtImporte.Text = EditObject.ImporteTotal.ToString("#0.00");

            ReBindParametros(EditObject.Detalles.Cast<ConsumoDetalle>());

            for (var i = 0; i < gridConsumos.Rows.Count; i++)
            {
                var cbTipoInsumo = gridConsumos.Rows[i].FindControl("cbTipoInsumo") as TipoInsumoDropDownList;
                var cbInsumo = gridConsumos.Rows[i].FindControl("cbInsumo") as InsumoDropDownList;

                if (cbTipoInsumo != null) cbTipoInsumo.Enabled = false;
                if (cbInsumo != null) cbInsumo.Enabled = false;
            }
        }

        protected override void OnDelete()
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {

                EditObject.Estado = ConsumoCabecera.Estados.Eliminado;
                DAOFactory.ConsumoCabeceraDAO.SaveOrUpdate(EditObject);

                ////// ACTUALIZAR STOCK //////

                if (EditObject.Deposito != null ||
                    EditObject.DepositoDestino != null)
                {
                    var detalles = EditObject.Detalles.Cast<ConsumoDetalle>();
                    foreach (var consumoDetalle in detalles)
                    {
                        if (EditObject.Deposito != null)
                        {
                            var stock = DAOFactory.StockDAO.GetByDepositoAndInsumo(EditObject.Deposito.Id, consumoDetalle.Insumo.Id);
                            stock.Cantidad += consumoDetalle.Cantidad;
                            DAOFactory.StockDAO.SaveOrUpdate(stock);
                        }

                        if (EditObject.DepositoDestino != null)
                        {
                            var stock = DAOFactory.StockDAO.GetByDepositoAndInsumo(EditObject.DepositoDestino.Id, consumoDetalle.Insumo.Id);
                            stock.Cantidad -= consumoDetalle.Cantidad;
                            DAOFactory.StockDAO.SaveOrUpdate(stock);
                        }
                    }
                }
                //////////////////////////////

                transaction.Commit();
            }
        }

        protected override void OnSave()
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {

                EditObject.Vehiculo = cbVehiculo.Selected > 0 ? DAOFactory.CocheDAO.FindById(cbVehiculo.Selected) : null;
                EditObject.Proveedor = cbProveedor.Selected > 0 ? DAOFactory.ProveedorDAO.FindById(cbProveedor.Selected) : null;
                EditObject.Deposito = cbDeposito.Selected > 0 ? DAOFactory.DepositoDAO.FindById(cbDeposito.Selected) : null;
                EditObject.DepositoDestino = cbDepositoDestino.Selected > 0 ? DAOFactory.DepositoDAO.FindById(cbDepositoDestino.Selected) : null;
                EditObject.Empleado = cbEmpleado.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(cbEmpleado.Selected) : null;
                EditObject.TipoMovimiento = (short) cbTipoMovimiento.Selected;

                EditObject.Estado = ConsumoCabecera.Estados.Pagado;
                EditObject.Fecha = dtFecha.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.Value) : new DateTime();
                int km;
                EditObject.KmDeclarados = int.TryParse(txtKilometros.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out km) ? km : 0;
                EditObject.NumeroFactura = txtFactura.Text;
                EditObject.ImporteTotal = Convert.ToDouble((string) txtImporte.Text.Replace(".", ","));

                foreach (var cons in ParaBorrar)
                {
                    var consumoDetalle = DAOFactory.ConsumoDetalleDAO.FindById(cons);

                    if (EditObject.Deposito != null)
                    {
                        var stock = DAOFactory.StockDAO.GetByDepositoAndInsumo(EditObject.Deposito.Id, consumoDetalle.Insumo.Id);
                        stock.Cantidad += consumoDetalle.Cantidad;
                        DAOFactory.StockDAO.SaveOrUpdate(stock);
                    }

                    if (EditObject.DepositoDestino != null)
                    {
                        var stock = DAOFactory.StockDAO.GetByDepositoAndInsumo(EditObject.DepositoDestino.Id, consumoDetalle.Insumo.Id);
                        stock.Cantidad -= consumoDetalle.Cantidad;
                        DAOFactory.StockDAO.SaveOrUpdate(stock);
                    }

                    DAOFactory.ConsumoDetalleDAO.Delete(consumoDetalle);
                }

                var list = GetConsumosFromView();
                EditObject.Detalles.Clear();

                foreach (var consumoDetalleAux in list)
                {
                    EditObject.Detalles.Add(consumoDetalleAux.ConsumoDetalle);

                    ////// ACTUALIZAR STOCK //////

                    if (consumoDetalleAux.ConsumoDetalle.Cantidad != consumoDetalleAux.ConsumoAnterior)
                    {
                        if (EditObject.Deposito != null)
                        {
                            var stock = DAOFactory.StockDAO.GetByDepositoAndInsumo(EditObject.Deposito.Id, consumoDetalleAux.ConsumoDetalle.Insumo.Id);

                            stock.Cantidad += consumoDetalleAux.ConsumoAnterior - consumoDetalleAux.ConsumoDetalle.Cantidad;

                            DAOFactory.StockDAO.SaveOrUpdate(stock);

                            if (stock.AlarmaActiva)
                            {
                                var msgSaver = new MessageSaver(DAOFactory);

                                if (stock.Cantidad < stock.StockCritico)
                                {
                                    msgSaver.Save(MessageIdentifier.StockCritic.ToString("d"), EditObject.Vehiculo, EditObject.Fecha, null,
                                        "Alarma Stock Crítico");
                                }
                                else if (stock.Cantidad < stock.PuntoReposicion)
                                {
                                    msgSaver.Save(MessageIdentifier.StockReposition.ToString("d"), EditObject.Vehiculo, EditObject.Fecha, null,
                                        "Alarma Reposición de Stock");
                                }
                            }
                        }

                        if (EditObject.DepositoDestino != null)
                        {
                            var stock = DAOFactory.StockDAO.GetByDepositoAndInsumo(EditObject.DepositoDestino.Id, consumoDetalleAux.ConsumoDetalle.Insumo.Id) ??
                                        new Stock {Deposito = EditObject.DepositoDestino, Insumo = consumoDetalleAux.ConsumoDetalle.Insumo};

                            stock.Cantidad += consumoDetalleAux.ConsumoDetalle.Cantidad - consumoDetalleAux.ConsumoAnterior;

                            DAOFactory.StockDAO.SaveOrUpdate(stock);
                        }
                    }

                    ////// ACTUALIZAR PRECIO //////

                    var insumo = consumoDetalleAux.ConsumoDetalle.Insumo;
                    insumo.ValorReferencia = consumoDetalleAux.ConsumoDetalle.ImporteUnitario;
                    DAOFactory.InsumoDAO.SaveOrUpdate(insumo);
                }

                DAOFactory.ConsumoCabeceraDAO.SaveOrUpdate(EditObject);

                if (EditObject.Vehiculo != null) VerificarOdometros(EditObject.Vehiculo);

                transaction.Commit();
            }
        }

        private void VerificarOdometros(Coche vehiculo)
        {
            foreach (var consumo in NuevosConsumos)
            {
                DAOFactory.MovOdometroVehiculoDAO.ResetByVehicleAndInsumo(vehiculo, consumo.Insumo);
            }
        }

        protected override void ValidateSave()
        {
            switch (cbTipoMovimiento.Selected)
            {
                case ConsumoCabecera.TiposMovimiento.DepositoAVehiculo:
                    ValidateEntity(cbDeposito.Selected, "PARENTI87");
                    ValidateEntity(cbVehiculo.Selected, "PARENTI03");
                    ValidateInt32(txtKilometros.Text, "KM_DECLARADOS");
                    break;
                case ConsumoCabecera.TiposMovimiento.ProveedorAVehiculo:
                    ValidateEntity(cbProveedor.Selected, "PARENTI59");
                    ValidateEntity(cbVehiculo.Selected, "PARENTI03");
                    ValidateInt32(txtKilometros.Text, "KM_DECLARADOS");
                    break;
                case ConsumoCabecera.TiposMovimiento.ProveedorADeposito:
                    ValidateEntity(cbProveedor.Selected, "PARENTI59");
                    ValidateEntity(cbDepositoDestino.Selected, "Labels", "DEPOSITO_DESTINO");
                    break;
                case ConsumoCabecera.TiposMovimiento.DepositoADeposito:
                    ValidateEntity(cbDeposito.Selected, "PARENTI87");
                    ValidateEntity(cbDepositoDestino.Selected, "Labels", "DEPOSITO_DESTINO");
                    break;
            }

            ValidateEmpty((DateTime?) dtFecha.SelectedDate, (string) "FECHA");
            //ValidateEmpty(txtFactura.Text, "NRO_FACTURA");
            ValidateDouble(txtImporte.Text, "IMPORTE");

            if (txtFactura.Text.Trim() != string.Empty && !DAOFactory.ConsumoCabeceraDAO.IsFacturaUnique(txtFactura.Text.Trim(), cbProveedor.Selected, cbDeposito.Selected, EditObject.Id))
                ThrowDuplicated("NRO_FACTURA");

            if (cbTipoMovimiento.Selected == ConsumoCabecera.TiposMovimiento.DepositoAVehiculo
             || cbTipoMovimiento.Selected == ConsumoCabecera.TiposMovimiento.DepositoADeposito)
            {
                for (var i = 0; i < gridConsumos.Rows.Count; i++)
                {
                    var cbInsumo = gridConsumos.Rows[i].FindControl("cbInsumo") as InsumoDropDownList;
                    var txtCantidad = gridConsumos.Rows[i].FindControl("txtCantidad") as TextBox;

                    var insumoId = cbInsumo != null ? cbInsumo.Selected : 0;
                    var cantidad = txtCantidad != null ? Convert.ToDouble(txtCantidad.Text) : 0.0;

                    var stock = DAOFactory.StockDAO.GetByDepositoAndInsumo(cbDeposito.Selected, insumoId);

                    if (stock == null)
                        ThrowError("STOCK_INEXISTENTE");

                    if (stock != null && stock.Cantidad < cantidad)
                        ThrowError("STOCK_INSUFICIENTE");
                }
            }
        }

        protected void CbVehiculoOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var vehiculo = DAOFactory.CocheDAO.FindById(cbVehiculo.Selected);
            if (cbVehiculo.Selected <= 0 || vehiculo == null) return;

            lblCentroCostos.Text = vehiculo.CentroDeCostos != null ? vehiculo.CentroDeCostos.Descripcion : CultureManager.GetLabel("NINGUNO");

            cbEmpleado.SetSelectedValue(vehiculo.Chofer != null ? vehiculo.Chofer.Id : -1);
        }

        protected void TxtCantidad_OnTextChanged(object sender, EventArgs e)
        {
            var list = GetConsumosFromView();
            ReBindParametros(list.Select(c => c.ConsumoDetalle));
        }

        protected void TxtImporteUnitario_OnTextChanged(object sender, EventArgs e)
        {
            var list = GetConsumosFromView();
            ReBindParametros(list.Select(c => c.ConsumoDetalle));
        }

        protected void CbTipoInsumo_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var cbTipoInsumo = sender as TipoInsumoDropDownList;
            if (cbTipoInsumo != null)
            {
                var index = int.Parse(cbTipoInsumo.Attributes["index"]);

                if (gridConsumos.Rows.Count > 0 && gridConsumos.Rows.Count > index)
                {
                    var row = gridConsumos.Rows[index];
                    var cbInsumo = row.FindControl("cbInsumo") as InsumoDropDownList;
                    if (cbInsumo != null)
                    {
                        cbInsumo.ClearItems();
                        var insumos = DAOFactory.InsumoDAO.GetList(cbEmpresa.SelectedValues, cbLinea.SelectedValues, cbTipoInsumo.SelectedValues).ToList();

                        foreach (var insumo in insumos)
                        {
                            cbInsumo.AddItem(insumo.Codigo + " - " + insumo.Descripcion, insumo.Id);
                        }
                        cbInsumo.SetSelectedValue(cbInsumo.Selected);
                    }
                }
            }
        }

        protected void CbInsumo_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var cbInsumo = sender as InsumoDropDownList;
            if (cbInsumo != null)
            {
                var index = int.Parse(cbInsumo.Attributes["index"]);

                if (gridConsumos.Rows.Count > 0 && gridConsumos.Rows.Count > index)
                {
                    var row = gridConsumos.Rows[index];
                    var lblUnidadMedida = row.FindControl("lblUnidadMedida") as Label;
                    if (lblUnidadMedida != null && cbInsumo.Selected > 0)
                    {
                        var insumo = DAOFactory.InsumoDAO.FindById(cbInsumo.Selected);

                        lblUnidadMedida.Text = insumo != null && insumo.UnidadMedida != null ? insumo.UnidadMedida.Descripcion : string.Empty;
                    }
                }
            }
        }

        protected void CbTipoMovimiento_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbTipoMovimiento.Selected)
            {
                case ConsumoCabecera.TiposMovimiento.DepositoADeposito:
                    cbTipoProveedor.Enabled = false;
                    cbProveedor.SetSelectedValue(cbProveedor.NoneValue);
                    cbProveedor.Enabled = false;
                    cbVehiculo.SetSelectedValue(cbVehiculo.NoneValue);
                    cbVehiculo.Enabled = false;
                    
                    cbDeposito.Enabled = true;
                    cbDepositoDestino.Enabled = true;
                    break;
                case ConsumoCabecera.TiposMovimiento.DepositoAVehiculo:
                    cbTipoProveedor.Enabled = false;
                    cbProveedor.SetSelectedValue(cbProveedor.NoneValue);
                    cbProveedor.Enabled = false;
                    cbDepositoDestino.SetSelectedValue(cbDepositoDestino.NoneValue);
                    cbDepositoDestino.Enabled = false;

                    cbDeposito.Enabled = true;
                    cbVehiculo.Enabled = true;
                    break;
                case ConsumoCabecera.TiposMovimiento.ProveedorADeposito:
                    cbVehiculo.SetSelectedValue(cbVehiculo.NoneValue);
                    cbVehiculo.Enabled = false;
                    cbDeposito.SetSelectedValue(cbDeposito.NoneValue);
                    cbDeposito.Enabled = false;
                    
                    cbTipoProveedor.Enabled = true;
                    cbProveedor.Enabled = true;
                    cbDepositoDestino.Enabled = true;
                    break;
                case ConsumoCabecera.TiposMovimiento.ProveedorAVehiculo:
                    cbDeposito.SetSelectedValue(cbDeposito.NoneValue);
                    cbDeposito.Enabled = false;
                    cbDepositoDestino.SetSelectedValue(cbDepositoDestino.NoneValue);
                    cbDepositoDestino.Enabled = false;

                    cbTipoProveedor.Enabled = true;
                    cbProveedor.Enabled = true;
                    cbVehiculo.Enabled = true;
                    break;
            }
        }

        protected void GridConsumosItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;

            var consumoDetalle = e.Row.DataItem as ConsumoDetalle;
            if (consumoDetalle == null) return;

            var lblUnidadMedida = e.Row.FindControl("lblUnidadMedida") as Label;
            var txtCantidad = e.Row.FindControl("txtCantidad") as TextBox;
            var txtImporteUnitario = e.Row.FindControl("txtImporteUnitario") as TextBox;
            var txtImporteTotal = e.Row.FindControl("txtImporteTotal") as TextBox;
            var cbTipoInsumo = e.Row.FindControl("cbTipoInsumo") as TipoInsumoDropDownList;
            var cbInsumo = e.Row.FindControl("cbInsumo") as InsumoDropDownList;

            if (cbInsumo != null && cbTipoInsumo != null) cbTipoInsumo.Attributes.Add("index", e.Row.RowIndex.ToString("#0"));
            if (cbInsumo != null && lblUnidadMedida != null) cbInsumo.Attributes.Add("index", e.Row.RowIndex.ToString("#0"));

            if (lblUnidadMedida != null) lblUnidadMedida.Text = consumoDetalle.Insumo != null && consumoDetalle.Insumo.UnidadMedida != null ? consumoDetalle.Insumo.UnidadMedida.Descripcion : string.Empty;
            if (txtCantidad != null) txtCantidad.Text = consumoDetalle.Cantidad.ToString("#0.00");
            if (txtImporteUnitario != null) txtImporteUnitario.Text = consumoDetalle.ImporteUnitario.ToString("#0.00");
            if (txtImporteTotal != null) txtImporteTotal.Text = (consumoDetalle.Cantidad * consumoDetalle.ImporteUnitario).ToString("#0.00");
            if (cbTipoInsumo != null)
            {
                var tipos = DAOFactory.TipoInsumoDAO.GetList(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected});
                cbTipoInsumo.ClearItems();
                foreach (var tipoInsumo in tipos) cbTipoInsumo.AddItem(tipoInsumo.Descripcion, tipoInsumo.Id);
                cbTipoInsumo.SetSelectedValue(consumoDetalle.Insumo != null && consumoDetalle.Insumo.TipoInsumo != null ? consumoDetalle.Insumo.TipoInsumo.Id : -1);
            }
            if (cbInsumo != null && cbTipoInsumo != null)
            {
                var insumos = DAOFactory.InsumoDAO.GetList(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, new[] {cbTipoInsumo.Selected});
                cbInsumo.ClearItems();
                foreach (var insumo in insumos) cbInsumo.AddItem(insumo.Codigo + " - " + insumo.Descripcion, insumo.Id);

                if (consumoDetalle.Insumo != null) cbInsumo.SetSelectedValue(consumoDetalle.Insumo.Id);
            }

            if (lblUnidadMedida != null && lblUnidadMedida.Text == string.Empty 
                && cbInsumo != null && cbInsumo.Selected > 0)
            {
                var insumo = DAOFactory.InsumoDAO.FindById(cbInsumo.Selected);
                if (insumo != null && insumo.UnidadMedida != null)
                    lblUnidadMedida.Text = insumo.UnidadMedida.Descripcion;
            }
        }

        protected void GridConsumosItemCommand(object sender, C1GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Eliminar":
                    DeleteConsumo(e.Row.RowIndex);
                    break;
            }
        }

        protected void BtNewParamClick(object sender, EventArgs e)
        {
            var parameter = new ConsumoDetalle { Cantidad = 0.0, ImporteUnitario = 0.0, ImporteTotal = 0.0 };
            var par = NuevosConsumos;
            par.Add(parameter);
            NuevosConsumos = par;
            var list = GetConsumosFromView().Select(c => c.ConsumoDetalle).ToList();
            list.Add(parameter);
            ReBindParametros(list);
        }

        protected List<ConsumoDetalleAux> GetConsumosFromView()
        {
            var paraBorrar = ParaBorrar;
            var nuevosConsumos = NuevosConsumos;
            var list = new List<ConsumoDetalleAux>();

            var newparcount = 0;
            foreach (C1GridViewRow row in gridConsumos.Rows)
            {
                var id = (int)gridConsumos.DataKeys[row.RowIndex].Value;
                if (paraBorrar.Contains(id)) continue;

                ConsumoDetalle consumoDetalle;
                var consumoAnterior = 0.0;

                if (id == 0)
                {
                    consumoDetalle = nuevosConsumos[newparcount];
                    consumoDetalle.ConsumoCabecera = EditObject;
                    newparcount++;
                }
                else
                {
                    consumoDetalle = (from ConsumoDetalle p in EditObject.Detalles where p.Id == id select p).First();
                    consumoAnterior = DAOFactory.ConsumoDetalleDAO.FindById(consumoDetalle.Id).Cantidad;
                }
                var txtCantidad = row.FindControl("txtCantidad") as TextBox;
                var txtImporteUnitario = row.FindControl("txtImporteUnitario") as TextBox;
                var cbInsumo = row.FindControl("cbInsumo") as InsumoDropDownList;
                
                consumoDetalle.Cantidad = txtCantidad != null ? Convert.ToDouble(txtCantidad.Text) : 0.0;
                consumoDetalle.ImporteUnitario = txtImporteUnitario != null ? Convert.ToDouble(txtImporteUnitario.Text.Replace('.',',')) : 0.0;
                consumoDetalle.ImporteTotal = consumoDetalle.Cantidad * consumoDetalle.ImporteUnitario;
                consumoDetalle.Insumo = cbInsumo != null ? DAOFactory.InsumoDAO.FindById(cbInsumo.Selected) : null;

                var consumoDetalleAux = new ConsumoDetalleAux { ConsumoDetalle = consumoDetalle, 
                                                                ConsumoAnterior = consumoAnterior };

                list.Add(consumoDetalleAux);
            }

            return list;
        }

        private void ReBindParametros(IEnumerable<ConsumoDetalle> detalles)
        {
            gridConsumos.DataSource = detalles;
            gridConsumos.DataBind();
            
            CalcularTotalFactura();
        }

        private void CalcularTotalFactura()
        {
            var total = 0.0;

            foreach (C1GridViewRow row in gridConsumos.Rows)
            {
                var txt = row.FindControl("txtImporteTotal") as TextBox;
                if (txt != null) total += Convert.ToDouble(txt.Text.Replace('.', ','));
            }

            txtImporte.Text = total.ToString("#0.00");
        }

        private void DeleteConsumo(int index)
        {
            var id = (int)gridConsumos.DataKeys[index].Value;
            if (id > 0)
            {
                var todel = ParaBorrar;
                todel.Add(id);
                ParaBorrar = todel;
            }
            var list = GetConsumosFromView();
            if (id == 0) list.RemoveAt(index);
            ReBindParametros(list.Select(c => c.ConsumoDetalle));
        }
    }
}
