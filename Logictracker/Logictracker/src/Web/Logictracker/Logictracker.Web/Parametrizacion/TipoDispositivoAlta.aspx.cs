using System;
using System.Linq;
using Logictracker.DAL.NHibernate;
using Logictracker.Messages.Sender;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using System.Collections.Generic;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionTipoDispositivoAlta : SecuredAbmPage<TipoDispositivo>
    {
        protected override string RedirectUrl { get { return "TipoDispositivoLista.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPO_DISPOSITIVO"; } }
        protected override string GetRefference() { return "TIPO_DISPOSITIVO"; }

        protected VsProperty<List<int>> DeletedIds { get { return this.CreateVsProperty("DeletedIds", new List<int>()); } }

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            abmTabDetalles.Visible = EditMode;
        }

        /// <summary>
        /// Initial data binding.
        /// </summary>
        protected override void Bind()
        {
            txtModelo.Text = EditObject.Modelo;
            txtFabricante.Text = EditObject.Fabricante;
            txtColaComando.Text = EditObject.ColaDeComandos;

            if (EditObject.Configuraciones != null)
            {
                ddlConfiguracion.SelectedValues =
                    EditObject.Configuraciones.OfType<ConfiguracionDispositivo>().Select(config => config.Id).ToList();
            }

            ddlPadre.SetSelectedValue(EditObject.TipoDispositivoPadre != null ? EditObject.TipoDispositivoPadre.Id : ddlPadre.NoneValue);

            var it = ddlPadre.Items.FindByValue(EditObject.Id.ToString());
            if(it != null) ddlPadre.Items.Remove(it);

            BindParametros(false);
        }

        /// <summary>
        /// This ABM does not support delete method.
        /// </summary>
        protected override void OnDelete() { DAOFactory.TipoDispositivoDAO.Delete(EditObject); }

        /// <summary>
        /// Adds the type being duplicated to the available parent types.
        /// </summary>
        protected override void OnDuplicate()
        {
            ddlPadre.DataBind();
            upPadre.Update();
            btNewParam.Visible = false;
            abmTabDetalles.Enabled = false;
        }

        /// <summary>
        /// Saves or updates the device T with the givenn values.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Modelo = txtModelo.Text;
            EditObject.Fabricante = txtFabricante.Text;
            EditObject.ColaDeComandos = txtColaComando.Text;

            EditObject.Firmware = ddlFirmware.Selected > 0 ? DAOFactory.FirmwareDAO.FindById(ddlFirmware.Selected) : null;
            EditObject.TipoDispositivoPadre = ddlPadre.Selected > 0 ? DAOFactory.TipoDispositivoDAO.FindById(ddlPadre.Selected) : null;

            AddConfigurations();

            var reset = SetParametros();

            DAOFactory.TipoDispositivoDAO.SaveOrUpdate(EditObject);


            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var parametros = EditObject.TiposParametro.Cast<TipoParametroDispositivo>().ToList();

                    var devices = DAOFactory.DispositivoDAO.GetByTipo(EditObject);
                    foreach (var dispositivo in devices)
                    {
                        if (EditMode && reset)
                        {
                            MessageSender.CreateReboot(dispositivo, null).Send();
                        }
                        var save = false;
                        var detalles = dispositivo.DetallesDispositivo.Cast<DetalleDispositivo>().ToList();
                        foreach (var parametro in parametros)
                        {
                            var detalle = detalles.FirstOrDefault(d => d.TipoParametro.Id == parametro.Id);
                            if (detalle != null) continue;

                            detalle = new DetalleDispositivo
                                      {
                                          Dispositivo = dispositivo,
                                          Revision = 1,
                                          TipoParametro = parametro,
                                          Valor = parametro.ValorInicial
                                      };

                            dispositivo.DetallesDispositivo.Add(detalle);
                            save = true;
                        }
                        if (save) DAOFactory.DispositivoDAO.Update(dispositivo);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Validates data before saving.
        /// </summary>
        protected override void ValidateSave()
        {
            if (String.IsNullOrEmpty(txtModelo.Text)) ThrowMustEnter("MODELO");
            if (String.IsNullOrEmpty(txtFabricante.Text)) ThrowMustEnter("FABRICANTE");

            // Parametros
            foreach (var par in from C1GridViewRow row in grid.Rows select new Parameter(row))
            {
                if (string.IsNullOrEmpty(par.TxtNombre.Text)) ThrowMustEnter("NAME");
                if (string.IsNullOrEmpty(par.TxtDescripcion.Text)) ThrowMustEnter("DESCRIPCION");
                if (string.IsNullOrEmpty(par.TxtTipoDato.Text)) ThrowMustEnter("TIPO_DATO");
                if (string.IsNullOrEmpty(par.TxtConsumidor.Text)) ThrowMustEnter("CONSUMIDOR");
            }
        }

        /// <summary>
        /// Validate data before dalete.
        /// </summary>
        protected override void ValidateDelete()
        {
            var devices = DAOFactory.DispositivoDAO.GetByTipo(EditObject);

            if (devices != null && devices.Count > 0) throw new Exception(CultureManager.GetError("ASSIGNED_DEVICE_TYPE"));
        }

        /// <summary>
        /// Device type firmware initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DdlFirmwareInitialBinding(object sender, EventArgs e)
        {
            if (EditMode) ddlFirmware.EditValue = EditObject.Firmware != null ? EditObject.Firmware.Id : ddlFirmware.NullValue;
        }

        /// <summary>
        /// Device parent initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DdlPadreInitialBinding(object sender, EventArgs e)
        {
            if (EditMode) ddlPadre.EditValue = EditObject.TipoDispositivoPadre != null ? EditObject.TipoDispositivoPadre.Id : ddlPadre.NullValue;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the selected configurations to the current device.
        /// </summary>
        private void AddConfigurations()
        {
            var currentIds = EditObject.Configuraciones.OfType<ConfiguracionDispositivo>().Select(config => config.Id).ToList();
            var ids = ddlConfiguracion.SelectedValues.Where(selected => !selected.Equals(0)).ToList();

            foreach (var id in ids.Where(id => !currentIds.Contains(id))) EditObject.AddConfiguration(DAOFactory.ConfiguracionDispositivoDAO.FindById(id));

            foreach (var id in currentIds.Where(id => !ids.Contains(id)).ToList()) EditObject.DeleteConfiguration(DAOFactory.ConfiguracionDispositivoDAO.FindById(id));
        }

        private bool SetParametros()
        {
            var parametros = EditObject.TiposParametro.Cast<TipoParametroDispositivo>().ToList();
            EditObject.TiposParametro.Clear();
            var reset = false;
            var di = DeletedIds.Get();
            foreach (C1GridViewRow row in grid.Rows)
            {
                var id = Convert.ToInt32(grid.DataKeys[row.RowIndex].Value);
                var parametro = id > 0 ? parametros.FirstOrDefault(p => p.Id == id) : null;
                if (parametro == null) parametro = new TipoParametroDispositivo { DispositivoTipo = EditObject };

                var par = new Parameter(row);
                reset |= par.ChkReboot.Checked && (id == 0 || parametro.Nombre != par.TxtNombre.Text);

                par.Fill(parametro);

                EditObject.TiposParametro.Add(parametro);
            }
            return reset;
        }

        private int _deletedIndex = -1;
        private void BindParametros(bool add)
        {
            var parametros = EditObject.TiposParametro
                .Cast<TipoParametroDispositivo>()
                .OrderBy(d => d.Descripcion)
                .ToList();

            foreach (C1GridViewRow row in grid.Rows)
            {
                var id = Convert.ToInt32(grid.DataKeys[row.RowIndex].Value);

                if (id > 0) continue;
                if(row.RowIndex == _deletedIndex) continue;
                var parametro = new TipoParametroDispositivo { DispositivoTipo = EditObject };

                var par = new Parameter(row);
                par.Fill(parametro);
                parametros.Add(parametro);
            }
            var del = DeletedIds.Get();
            parametros = parametros.Where(p => !del.Contains(p.Id)).ToList();
            if (add)
            {
                parametros.Add(new TipoParametroDispositivo { DispositivoTipo = EditObject, Nombre = string.Empty });
            }

            grid.DataSource = parametros;
            grid.DataBind();
        }

        protected void grid_RowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;

            var parametro = e.Row.DataItem as TipoParametroDispositivo;
            var row = new Parameter(e.Row);
            row.Bind(parametro);

        }

        protected void grid_RowCommand(object sender, C1GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var id = Convert.ToInt32(grid.DataKeys[e.Row.RowIndex].Value);

                if (id > 0)
                {
                    var di = DeletedIds.Get();
                    di.Add(id);
                    DeletedIds.Set(di);
                }
                else _deletedIndex = e.Row.RowIndex;
                grid.RowDeleting += grid_RowDeleting;
                grid.DeleteRow(e.Row.RowIndex);
                BindParametros(false);

            }
        }

        protected void grid_RowDeleting(object sender, C1GridViewDeleteEventArgs e)
        {

        }


        protected void btNewParam_Click(object sender, EventArgs e)
        {
            BindParametros(true);
        }

        private class Parameter
        {
            private C1GridViewRow Row { get; set; }
            public TextBox TxtNombre { get; private set; }
            public TextBox TxtDescripcion { get; private set; }
            public TextBox TxtTipoDato { get; private set; }
            public TextBox TxtConsumidor { get; private set; }
            public TextBox TxtValorInicial { get; private set; }
            public CheckBox ChkEditable { get; private set; }
            public CheckBox ChkReboot { get; private set; }

            public Parameter(C1GridViewRow row)
            {
                Row = row;
                TxtNombre = GetControl<TextBox>("txtNombre");
                TxtDescripcion = GetControl<TextBox>("txtDescripcion");
                TxtTipoDato = GetControl<TextBox>("txtTipoDato");
                TxtConsumidor = GetControl<TextBox>("txtConsumidor");
                TxtValorInicial = GetControl<TextBox>("txtValorInicial");
                ChkEditable = GetControl<CheckBox>("chkEditable");
                ChkReboot = GetControl<CheckBox>("chkReboot");
            }

            public void Bind(TipoParametroDispositivo parametro)
            {
                TxtNombre.Text = parametro.Nombre;
                TxtDescripcion.Text = parametro.Descripcion;
                TxtTipoDato.Text = parametro.TipoDato;
                TxtConsumidor.Text = parametro.Consumidor;
                TxtValorInicial.Text = parametro.ValorInicial;
                ChkEditable.Checked = parametro.Editable;
                ChkReboot.Checked = parametro.RequiereReset;
            }
            public TipoParametroDispositivo Fill(TipoParametroDispositivo parametro)
            {
                parametro.Consumidor = TxtConsumidor.Text;
                parametro.Descripcion = TxtDescripcion.Text;
                parametro.Editable = ChkEditable.Checked;
                parametro.Nombre = TxtNombre.Text;
                parametro.TipoDato = TxtTipoDato.Text;
                parametro.ValorInicial = TxtValorInicial.Text;
                parametro.RequiereReset = ChkReboot.Checked;
                return parametro;
            }
            private T GetControl<T>(string name) where T : class
            {
                return Row.FindControl(name) as T;
            }
        }
        #endregion
    }
}
