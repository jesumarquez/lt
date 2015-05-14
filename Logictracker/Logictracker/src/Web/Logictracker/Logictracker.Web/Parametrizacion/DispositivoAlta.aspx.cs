using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Messages.Sender;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using C1.Web.UI.Controls.C1GridView;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionDispositivoAlta : SecuredAbmPage<Dispositivo>
    {
        protected override string VariableName { get { return "PAR_DISPOSITIVOS"; } }
        protected override string RedirectUrl { get { return "DispositivoLista.aspx"; } }
        protected override string GetRefference() { return "DISPOSITIVO"; }
        protected override bool DeleteButton { get { return false; } }

        protected VsProperty<int> OldDeviceType { get { return this.CreateVsProperty("OldDeviceType", -1); } }
        protected VsProperty<short> DeviceState { get { return this.CreateVsProperty("DeviceState", Dispositivo.Estados.Activo); } }
       
        #region Protected Methods

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
            abmTabDetalles.Visible = EditMode;
            if (!IsPostBack)
            {
                if (!EditMode)
                {
                    ddlEstado.EditValue = Dispositivo.Estados.Activo;
                    ddlEstado.Enabled = false;
                }
            }
		}

        protected override void OnSave()
        {
            EditObject.Clave = txtClave.Text;
            EditObject.Codigo = txtCodigo.Text.Trim();
            EditObject.Imei = txtIMEI.Text.Trim().TrimStart('0');
            EditObject.Port = Convert.ToInt32(txtPuerto.Text);
            EditObject.Estado = (short)ddlEstado.Selected;
            EditObject.Tablas = npTablas.Value.ToString("#0");
            EditObject.Telefono = txtTelefono.Text;
            EditObject.LineaTelefonica = cbLineaTelefonica.Selected > 0 ? DAOFactory.LineaTelefonicaDAO.FindById(cbLineaTelefonica.Selected) : null;
            EditObject.IdNum = txtId.Text != string.Empty ? Convert.ToInt32(txtId.Text) : EditObject.Id;

            EditObject.Linea = ddlPlanta.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
            EditObject.Empresa = ddlLocacion.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected): EditObject.Linea != null ? EditObject.Linea.Empresa : null;
            EditObject.FlashedFirmware = ddlFirmware.Selected > 0 ? DAOFactory.FirmwareDAO.FindById(ddlFirmware.Selected) : null;
            EditObject.TipoDispositivo = DAOFactory.TipoDispositivoDAO.FindById(ddlTipoDispositivo.Selected);
            EditObject.Precinto = cbPrecinto.Selected > 0 ? DAOFactory.PrecintoDAO.FindById(cbPrecinto.Selected) : null;
            AddConfigurations();

            if (DeviceState.Get() != EditObject.Estado) EditObject.DtCambioEstado = DateTime.UtcNow;

            SetDeviceConfiguration(EditObject);
            if (!EditMode) DAOFactory.DispositivoDAO.Save(EditObject);
            else DAOFactory.DispositivoDAO.Update(EditObject);

            if (EditObject.DetallesDispositivo.Cast<DetalleDispositivo>().Any(detail => detail.TipoParametro.RequiereReset))
                MessageSender.CreateReboot(EditObject, null);
        }

        protected override void AfterSave()
        {
            base.AfterSave();

            if (EditObject.IdNum == 0) EditObject.IdNum = EditObject.Id;

            DAOFactory.DispositivoDAO.SaveOrUpdate(EditObject);
        }

        protected override void OnDelete() { DAOFactory.DispositivoDAO.Delete(EditObject); }

        protected override void ValidateDelete()
        {
            if (DAOFactory.CocheDAO.FindMobileByDevice(EditObject.Id) != null) ThrowError("DEVICE_ASSIGNED");
        }

        protected override void Bind()
        {
            ddlLocacion.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : ddlLocacion.AllValue);
            ddlPlanta.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : ddlPlanta.AllValue);

            ddlTipoDispositivo.SetSelectedValue(EditObject.TipoDispositivo != null ? EditObject.TipoDispositivo.Id : ddlTipoDispositivo.AllValue);
            if (EditObject.TipoDispositivo != null) OldDeviceType.Set(EditObject.TipoDispositivo.Id);

            ddlFirmware.SetSelectedValue(EditObject.FlashedFirmware != null ? EditObject.FlashedFirmware.Id : ddlFirmware.AllValue);
            
            ddlEstado.SetSelectedValue(EditObject.Estado);
            
            txtCodigo.Text = EditObject.Codigo;
            txtPuerto.Text = EditObject.Port.ToString("#0");
            txtIMEI.Text = EditObject.Imei;
            txtClave.Text = EditObject.Clave;
            txtTelefono.Text = EditObject.Telefono;
            txtId.Text = EditObject.IdNum.ToString("#");
            txtId.Enabled = !EditObject.Verificado;

            if (EditObject.LineaTelefonica != null)
                cbLineaTelefonica.Items.Insert(0, new ListItem(EditObject.LineaTelefonica.NumeroLinea, EditObject.LineaTelefonica.Id.ToString("#0")));

            cbLineaTelefonica.SetSelectedValue(EditObject.LineaTelefonica != null ? EditObject.LineaTelefonica.Id : cbLineaTelefonica.NoneValue);

            if (EditObject.Precinto != null)
                cbPrecinto.Items.Insert(0, new ListItem(EditObject.Precinto.Codigo, EditObject.Precinto.Id.ToString("#0")));
            
            cbPrecinto.SetSelectedValue(EditObject.Precinto != null ? EditObject.Precinto.Id : cbPrecinto.NoneValue);

            var coche = DAOFactory.CocheDAO.FindMobileByDevice(EditObject.Id);

            tbMovil.Text = coche != null ? coche.Interno : "Sin Asignar";

            BindConfigurations();

            DeviceState.Set(EditObject.Estado);

            BindDetalles();
        }

        protected override void ValidateSave()
        {           
            var code = ValidateEmpty(txtCodigo.Text, "CODE");
            ValidateEmpty(txtClave.Text, "CLAVE");
            var imei = ValidateEmpty(txtIMEI.Text, "IMEI");
            //ValidateEmpty(txtTelefono.Text, "MDN");

            //ValidateEntity(cbLineaTelefonica.Selected, "PARENTI74");

            // Inactivo con vehiculo asignado
            if (ddlEstado.Selected == Dispositivo.Estados.Inactivo && DAOFactory.CocheDAO.FindMobileByDevice(EditObject.Id) != null) 
                ThrowError("DEVICE_ASSIGNED");

            // Imei Duplicado
            var dispo = DAOFactory.DispositivoDAO.GetByIMEI(imei);
            ValidateDuplicated(dispo, "IMEI");

            // Code Duplicado
            var byCode = DAOFactory.DispositivoDAO.GetByCode(code);
            ValidateDuplicated(byCode, "CODE");

            // DeviceId Duplicado
            if (txtId.Text != string.Empty)
            {
                var id = ValidateInt32(txtId.Text, "ID");
                if (!DAOFactory.DispositivoDAO.IsUniqueIdNum(id, EditObject.Id))
                    ThrowDuplicated("ID");
            }
        }

        protected void DdlTipoDispositivoSelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditMode) RebindDetalles();
        }

    	#endregion

        #region Private Methods

        private void BindConfigurations()
        {
            if (EditObject.Configuraciones != null) ddlConfiguracion.SelectedValues = EditObject.Configuraciones.OfType<ConfiguracionDispositivo>().Select(config => config.Id).ToList();
        }

        private void AddConfigurations()
        {
            var currentIds = EditObject.Configuraciones.OfType<ConfiguracionDispositivo>().Select(config => config.Id).ToList();
            var ids = ddlConfiguracion.SelectedValues.Where(selected => !selected.Equals(0)).ToList();

            foreach (var id in ids.Where(id => !currentIds.Contains(id))) EditObject.AddConfiguration(DAOFactory.ConfiguracionDispositivoDAO.FindById(id));

            foreach (var id in currentIds.Where(id => !ids.Contains(id)).ToList()) EditObject.DeleteConfiguration(DAOFactory.ConfiguracionDispositivoDAO.FindById(id));
        }

        private void SetDeviceConfiguration(Dispositivo dispositivo)
        {
            if (!EditMode)
            {
                var tipo = dispositivo.TipoDispositivo;
                if (tipo != null)
                {
                    foreach (TipoParametroDispositivo par in tipo.TiposParametro)
                    {
                        var detalle = new DetalleDispositivo
                                          {
                                              Dispositivo = dispositivo,
                                              Revision = 0,
                                              TipoParametro = par,
                                              Valor = par.ValorInicial
                                          };
                        EditObject.DetallesDispositivo.Add(detalle);
                    }
                }
            }
            else
            {
                var maxRevision = dispositivo.MaxRevision + 1;
                var detallesById = dispositivo.DetallesDispositivo.Cast<DetalleDispositivo>().ToDictionary(d => d.Id, d => d);

                EditObject.DetallesDispositivo.Clear();

                foreach (C1GridViewRow newParameter in grid.Rows)
                {
                    var idDetail = Convert.ToInt32(grid.DataKeys[newParameter.RowIndex].Value);
                    var idParametro = Convert.ToInt32(grid.DataKeys[newParameter.RowIndex].Values[1]);

                    var txtValor = newParameter.FindControl("txtValor") as TextBox;

                    if(idDetail > 0)
                    {
                        var oldParameter = detallesById[idDetail];
                        if (txtValor != null && oldParameter.Valor != txtValor.Text)
                        {
                            oldParameter.Valor = txtValor.Text;
                            oldParameter.Revision = maxRevision;
                        }

                        EditObject.DetallesDispositivo.Add(oldParameter);
                    }
                    else
                    {
                        var parametro = DAOFactory.TipoParametroDispositivoDAO.FindById(idParametro);
                        var oldParameter = new DetalleDispositivo
                            {
                                Dispositivo = EditObject,
                                Revision = maxRevision,
                                TipoParametro = parametro,
                                Valor = txtValor != null ? txtValor.Text : parametro.ValorInicial
                            };
                        EditObject.DetallesDispositivo.Add(oldParameter);
                    }
                }
            }

        }
        private void RebindDetalles()
        {
            try
            {
                var detallesActuales = new Dictionary<string, string>();

                foreach (C1GridViewRow newParameter in grid.Rows)
                {
                    var idParametro = Convert.ToInt32(grid.DataKeys[newParameter.RowIndex].Values[1]);
                    var txtValor = newParameter.FindControl("txtValor") as TextBox;
                    if (txtValor == null) continue;
                    var parametro = DAOFactory.TipoParametroDispositivoDAO.FindById(idParametro);
                    if (detallesActuales.ContainsKey(parametro.Nombre))
                        detallesActuales[parametro.Nombre] = txtValor.Text;
                    else detallesActuales.Add(parametro.Nombre, txtValor.Text);
                }

                var tipoDispositivo = DAOFactory.TipoDispositivoDAO.FindById(ddlTipoDispositivo.Selected);
                var tiposNuevos = tipoDispositivo.TiposParametro.Cast<TipoParametroDispositivo>().ToList();

                var detalles = tiposNuevos
                    .Select(
                        d =>
                        new DetalleDispo(d, detallesActuales.ContainsKey(d.Nombre) ? detallesActuales[d.Nombre] : null))
                    .ToList();

                grid.DataSource = detalles;
                grid.DataBind();
            }
            catch(Exception ex)
            {
                ShowError(ex);
            }
        }
        private void BindDetalles()
        {
            var detallesActuales = EditObject.DetallesDispositivo.Cast<DetalleDispositivo>().Where(dd=> dd.TipoParametro.DispositivoTipo == dd.Dispositivo.TipoDispositivo);
            var detalles = detallesActuales.Select(d => new DetalleDispo(d)).ToList();

            var tipos = detallesActuales.Select(d=>d.TipoParametro.Id).ToList();

            var tiposFaltantes = EditObject.TipoDispositivo.TiposParametro.Cast<TipoParametroDispositivo>()
                                                                          .Where(p => !tipos.Contains(p.Id))
                                                                          .ToList();

            detalles = detalles.Union(tiposFaltantes.Select(t => new DetalleDispo(t)))
                               .OrderBy(d => d.Descripcion)
                               .ToList();

            grid.DataSource = detalles;
            grid.DataBind();
        }

        protected void GridRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;
            var txtValor = e.Row.FindControl("txtValor") as TextBox;
            var detalle = e.Row.DataItem as DetalleDispo;
            if (txtValor != null && detalle != null)
            {
                txtValor.Text = detalle.Valor;
                txtValor.Enabled = detalle.Editable;
            }
        }

        private class DetalleDispo
        {
            public int Id { get; set; }
            public int IdParametro { get; set; }
            public string Parametro { get; set; }
            public string Descripcion { get; set; }
            public string Tipo { get; set; }
            public string Valor { get; set; }
            public string Default { get; set; }            
            public bool Editable { get; set; }

            public DetalleDispo(DetalleDispositivo detalle)
            {
                Id = detalle.Id;
                Parametro = detalle.TipoParametro.Nombre;
                Descripcion = detalle.TipoParametro.Descripcion;
                Tipo = detalle.TipoParametro.TipoDato;
                Valor = detalle.Valor;
                Default = detalle.TipoParametro.ValorInicial;
                Editable = detalle.TipoParametro.Editable;
                IdParametro = detalle.TipoParametro.Id;
            }
            public DetalleDispo(TipoParametroDispositivo parametro)
            {
                Id = 0;
                Parametro = parametro.Nombre;
                Descripcion = parametro.Descripcion;
                Tipo = parametro.TipoDato;
                Valor = parametro.ValorInicial;
                Default = parametro.ValorInicial;
                Editable = parametro.Editable;
                IdParametro = parametro.Id;
            }
            public DetalleDispo(TipoParametroDispositivo parametro, string valor) : this(parametro)
            {
                if(valor != null) Valor = valor;
            }
        }

        #endregion
    }
}
