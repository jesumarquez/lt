#region Usings

using System;
using Logictracker.Messages.Sender;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionParametrosDispositivoAlta : SecuredAbmPage<TipoParametroDispositivo>
    {
        #region Protected Properties

        /// <summary>
        /// Associated list page.
        /// </summary>
        protected override string RedirectUrl { get { return "ParametrosDispositivoLista.aspx"; } }

        /// <summary>
        /// Report Name.
        /// </summary>
        protected override string VariableName { get { return "PAR_PARAM_DISPOSITIVOS"; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initial data binding.
        /// </summary>
        protected override void Bind()
        {
            txtConsumidor.Text = EditObject.Consumidor;
            txtDescripcion.Text = EditObject.Descripcion;
            txtNombre.Text = EditObject.Nombre;
            txtTipoDato.Text = EditObject.TipoDato;
            txtValorInicial.Text = EditObject.ValorInicial;
            chbEditable.Checked = EditObject.Editable;
            chkReset.Checked = EditObject.RequiereReset;
        }

        /// <summary>
        /// Device T initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlTipoDispositivo_PreBind(object sender, EventArgs e)
        {
            if (EditMode && EditObject.DispositivoTipo != null) ddlTipoDispositivo.EditValue = EditObject.DispositivoTipo.Id;
        }

        /// <summary>
        /// This ABM does not support delete.
        /// </summary>
        protected override void OnDelete() { }

        /// <summary>
        /// Saves or updates the parameter T with the givenn values.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Consumidor = txtConsumidor.Text;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.DispositivoTipo = DAOFactory.TipoDispositivoDAO.FindById(ddlTipoDispositivo.Selected);
            EditObject.Editable = chbEditable.Checked;
            EditObject.Nombre = txtNombre.Text;
            EditObject.TipoDato = txtTipoDato.Text;
            EditObject.ValorInicial = txtValorInicial.Text;
            EditObject.RequiereReset = chkReset.Checked;

            var reset = !EditMode && EditObject.RequiereReset;
            DAOFactory.TipoParametroDispositivoDAO.SaveOrUpdate(EditObject);
            if (reset)
            {
                var devices = DAOFactory.DispositivoDAO.GetByTipo(EditObject.DispositivoTipo);
                foreach (var dispositivo in devices)
                {
                    MessageSender.CreateReboot(dispositivo, null).Send();    
                }
            }
        }

        /// <summary>
        /// Validates data before saving.
        /// </summary>
        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(txtNombre.Text)) ThrowMustEnter("NAME");
            if (string.IsNullOrEmpty(txtDescripcion.Text)) ThrowMustEnter("DESCRIPCION");
            if (string.IsNullOrEmpty(txtTipoDato.Text)) ThrowMustEnter("TIPO_DATO");
            if (string.IsNullOrEmpty(txtConsumidor.Text)) ThrowMustEnter("CONSUMIDOR");
            if (string.IsNullOrEmpty(txtValorInicial.Text)) ThrowMustEnter("VALOR_INICIAL");
        }

        /// <summary>
        /// Adds actions icon to the toolbar. This ABM does not support delete so that action is ignored.
        /// </summary>
        protected override void AddToolBarIcons()
        {
            var module = Module;

            if (module.Add)
            {
                ToolBar.AddNewToolbarButton();

                ToolBar.AddDuplicateToolbarButton();

                if (!module.Edit) ToolBar.AddSaveToolbarButton();
            }

            if (module.Edit) ToolBar.AddSaveToolbarButton();

            ToolBar.AddListToolbarButton();
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "PARAMETROS_DISPOSITIVO"; }

        #endregion
    }
}
