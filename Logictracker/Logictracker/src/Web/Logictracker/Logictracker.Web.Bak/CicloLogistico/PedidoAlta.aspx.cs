using System;
using System.Globalization;
using Logictracker.DAL.NHibernate;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class PedidoAlta : SecuredAbmPage<Pedido>
    {
        protected override string RedirectUrl { get { return "PedidoLista.aspx"; } }
        protected override string VariableName { get { return "CLOG_PEDIDO"; } }
        protected override string GetRefference() { return "PEDIDO"; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if(!EditMode)
            {
                txtCodigo.Text = @"Auto";
            }
            if (!IsPostBack && EditObject.Id == 0)
            {
                dtFechaEnObra.SelectedDate = DateTime.Today.AddDays(1);
                dtHoraCarga.SelectedDate = DateTime.Today.AddDays(1);
            }
        }

        protected void cbPuntoEntrega_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditMode) return;
            if (cbPuntoEntrega.Selected <= 0)
            {
                txtContacto.Text = txtObservacion.Text = string.Empty;
            }
            else
            {
                var punto = DAOFactory.PuntoEntregaDAO.FindById(cbPuntoEntrega.Selected);
                txtContacto.Text = punto.Telefono;
                txtObservacion.Text = punto.Comentario1 + Environment.NewLine + punto.Comentario2 + Environment.NewLine +
                                      punto.Comentario3;
            }
        }

        protected void dtFechaEnObra_DateChanged(object sender, EventArgs e)
        {
            if (!dtFechaEnObra.SelectedDate.HasValue) return;
            dtHoraCarga.SelectedDate = dtFechaEnObra.SelectedDate.Value.Subtract(TimeSpan.FromHours(1));
        }

        protected override void OnDuplicate()
        {
            txtCodigo.Text = @"Auto";
            upCodigo.Update();
            base.OnDuplicate();            
        }

        protected override void Bind()
        {
            if (EditObject.Empresa != null) cbEmpresa.SetSelectedValue(EditObject.Empresa.Id);
            if (EditObject.Linea != null) cbLinea.SetSelectedValue(EditObject.Linea.Id);
            if (EditObject.BocaDeCarga != null) cbBocaDeCarga.SetSelectedValue(EditObject.BocaDeCarga.Id);
            if (EditObject.Cliente != null) cbCliente.SetSelectedValue(EditObject.Cliente.Id);
            if (EditObject.PuntoEntrega != null) cbPuntoEntrega.SetSelectedValue(EditObject.PuntoEntrega.Id);
            if (EditObject.Producto != null) cbProducto.SetSelectedValue(EditObject.Producto.Id);

            txtCodigo.Text = EditObject.Codigo;
            dtFechaEnObra.SelectedDate = EditObject.FechaEnObra.ToDisplayDateTime();
            txtCantidad.Text = EditObject.Cantidad.ToString(CultureInfo.InvariantCulture);
            txtAjuste.Text = EditObject.CantidadAjuste.ToString(CultureInfo.InvariantCulture);
            txtTiempoCiclo.Text = EditObject.TiempoCiclo.ToString();
            txtFrecuencia.Text = EditObject.Frecuencia.ToString();
            txtCargaViaje.Text = EditObject.CargaViaje.ToString("#0.00");
            chkCargaViaje.Checked = EditObject.CargaViaje == 10.00;
            txtBomba.Text = EditObject.NumeroBomba;
            chkEsMinimixer.Checked = EditObject.EsMinimixer;
            if (EditObject.HoraCarga != DateTime.MinValue)
                dtHoraCarga.SelectedDate = EditObject.HoraCarga.ToDisplayDateTime();
            txtContacto.Text = EditObject.Contacto;
            txtObservacion.Text = EditObject.Observacion;

        }

        protected override void OnDelete() { DAOFactory.PedidoDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            using (var tran = SmartTransaction.BeginTransaction())
            {
                try
                {
                    EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
                    EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
                    EditObject.BocaDeCarga = cbBocaDeCarga.Selected > 0 ? DAOFactory.BocaDeCargaDAO.FindById(cbBocaDeCarga.Selected) : null;
                    EditObject.Cliente = cbCliente.Selected > 0 ? DAOFactory.ClienteDAO.FindById(cbCliente.Selected) : null;
                    EditObject.PuntoEntrega = cbPuntoEntrega.Selected > 0 ? DAOFactory.PuntoEntregaDAO.FindById(cbPuntoEntrega.Selected) : null;
                    EditObject.Producto = cbProducto.Selected > 0 ? DAOFactory.ProductoDAO.FindById(cbProducto.Selected) : null;

                    if (!EditMode) EditObject.Codigo = DAOFactory.PedidoDAO.FindNextId().ToString();
                    EditObject.FechaEnObra = SecurityExtensions.ToDataBaseDateTime(dtFechaEnObra.SelectedDate.Value);
                    EditObject.Cantidad = ValidateDouble(txtCantidad.Text, "CANTIDAD_PEDIDO");
                    EditObject.CantidadAjuste = ValidateDouble(txtAjuste.Text, "CANTIDAD_AJUSTES");
                    EditObject.TiempoCiclo = Convert.ToInt32((string) txtTiempoCiclo.Text.Trim());
                    EditObject.Frecuencia = Convert.ToInt32((string) txtFrecuencia.Text.Trim());
                    EditObject.CargaViaje = chkCargaViaje.Checked ? 10.00 : Convert.ToDouble((string) txtCargaViaje.Text);
                    EditObject.NumeroBomba = txtBomba.Text.Trim();
                    EditObject.EsMinimixer = chkEsMinimixer.Checked;
                    EditObject.HoraCarga = SecurityExtensions.ToDataBaseDateTime(dtHoraCarga.SelectedDate.Value);
                    EditObject.Contacto = StringExtensions.Truncate(txtContacto.Text, 255);
                    EditObject.Observacion = StringExtensions.Truncate(txtObservacion.Text, 255);

                    if (!EditMode) EditObject.Estado = Pedido.Estados.Pendiente;

                    DAOFactory.PedidoDAO.SaveOrUpdate(EditObject);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbLinea.Selected, "PARENTI02");
            ValidateEntity(cbBocaDeCarga.Selected, "BOCADECARGA");
            ValidateEntity(cbCliente.Selected, "CLIENT");
            ValidateEntity(cbPuntoEntrega.Selected, "PARENTI44");
            ValidateEntity(cbProducto.Selected, "PARENTI63");

            var codigo = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            var byCode = DAOFactory.PedidoDAO.FindByCode(cbEmpresa.Selected, codigo);
            ValidateDuplicated(byCode, "CODE");

            var enObra = ValidateEmpty((DateTime?) dtFechaEnObra.SelectedDate, (string) "FECHA_EN_OBRA");
            var enCarga = ValidateEmpty((DateTime?) dtHoraCarga.SelectedDate, (string) "HORA_CARGA");

            if (enObra < enCarga)
                ThrowInvalidValue("HORA_CARGA");

            ValidateDouble(txtCantidad.Text, "CANTIDAD_PEDIDO");
            ValidateDouble(txtAjuste.Text, "CANTIDAD_AJUSTES");

            ValidateInt32(txtTiempoCiclo.Text, "PEDIDO_TIEMPO_CICLO");
            ValidateInt32(txtFrecuencia.Text, "PEDIDO_FRECUENCIA_ENTREGA");
            if (!chkCargaViaje.Checked)
            {
                var cargaViaje = ValidateEmpty((string) txtCargaViaje.Text, (string) "PEDIDO_CARGA_POR_VIAJE");
                var carga = ValidateDouble(cargaViaje, "PEDIDO_CARGA_POR_VIAJE");
                if (carga <= 0) 
                    ThrowInvalidValue("PEDIDO_CARGA_POR_VIAJE");
            }
        }

        protected void ChkCargaViaje_CheckedChanged(object sender, EventArgs e)
        {
            txtCargaViaje.Enabled = !chkCargaViaje.Checked;
        }
    }
}
