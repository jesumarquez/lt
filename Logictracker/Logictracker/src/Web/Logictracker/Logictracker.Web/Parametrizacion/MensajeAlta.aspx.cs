using System;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionMensajeAlta : SecuredAbmPage<Mensaje>
    {
        #region Private Const Properties

        /// <summary>
        /// Code variable name.
        /// </summary>
        private const string CODE = "CODE";

        /// <summary>
        /// Invalid code message variable name.
        /// </summary>
        private const string CODIGO_INVALIDO = "CODIGO_INVALIDO";

        /// <summary>
        /// Description variable name.
        /// </summary>
        private const string DESCRIPCION = "DESCRIPCION";

        /// <summary>
        /// State messages variable name.
        /// </summary>
        private const string STATE_MESSAGE = "STATE_MESSAGE";

        /// <summary>
        /// System messages variable name.
        /// </summary>
        private const string SYSTEM_MESSAGE = "SYSTEM_MESSAGE";

        /// <summary>
        /// User messages variable name.
        /// </summary>
        private const string USER_MESSAGE = "USER_MESSAGE";

        #endregion

        #region Protected Properties

        protected override string VariableName { get { return "PAR_MENSAJES"; } }
        protected override string RedirectUrl { get { return "MensajeLista.aspx"; } }
        protected override string GetRefference() { return "MENSAJE"; }

        #endregion

        #region Protected Properties

        protected bool Duplicated
        {
            get { return (bool)(ViewState["Duplicated"] ?? false); }
            set { ViewState["Duplicated"] = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Saves the current edited object.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.TipoMensaje = DAOFactory.TipoMensajeDAO.FindById(cbTipoMensaje.Selected);
            EditObject.Acceso = (short)cbAcceso.Selected;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Origen = (byte)cbOrigen.Selected;
            EditObject.Texto = txtMensaje.Text;
            EditObject.Destino = (short)cbDestino.Selected;
            EditObject.Ttl = (short) npExpiracion.Value;
            EditObject.EsAlarma = chkAlarma.Checked;
            EditObject.Icono = DAOFactory.IconoDAO.FindById(selectIcon1.Selected) ?? EditObject.TipoMensaje.Icono;

            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;

            DAOFactory.MensajeDAO.SaveOrUpdate(EditObject);
        }

        /// <summary>
        /// Determines page set up according to action to be performed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            SetPageLayout();
        }

        //protected void dcTipoMensaje_selectedIndexChanged(object sender, EventArgs e) {SetPageLayout(); }

        /// <summary>
        /// Duplicates the message being currently edited.
        /// </summary>
        protected override void OnDuplicate()
        {
            base.OnDuplicate();

            Duplicated = true;
        }

        /// <summary>
        /// Deletes the current edited object.
        /// </summary>
        protected override void OnDelete() { DAOFactory.MensajeDAO.Delete(EditObject); }

        /// <summary>
        /// Binds the current edited object.
        /// </summary>
        protected override void Bind()
        {
            txtCodigo.Text = EditObject.Codigo;
            npExpiracion.Value = EditObject.Ttl;
            txtMensaje.Text = EditObject.Texto;
            txtDescripcion.Text = EditObject.Descripcion;
            chkAlarma.Checked = EditObject.EsAlarma;

            if (EditObject.Icono != null) selectIcon1.Selected = EditObject.Icono.Id;
        }

        /// <summary>
        /// Location initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbEmpresa_PreBind(object sender, EventArgs e)
        {
            if (EditMode) cbEmpresa.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : EditObject.Linea != null ? EditObject.Linea.Empresa.Id : cbEmpresa.AllValue;
        }

        /// <summary>
        /// Company initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbLinea_PreBind(object sender, EventArgs e) { if (EditMode) cbLinea.EditValue = EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue; }

        /// <summary>
        /// Message T initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbTipoMensaje_PreBind(object sender, EventArgs e)
        {
            if (EditMode) cbTipoMensaje.EditValue = EditObject.TipoMensaje != null ? EditObject.TipoMensaje.Id : cbTipoMensaje.AllValue;
        }

        /// <summary>
        /// Type initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbDestino_PreBind(object sender, EventArgs e) { if (EditMode) cbDestino.EditValue = EditObject.Destino; }

        /// <summary>
        /// Destination initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbOrigen_PreBind(object sender, EventArgs e) { if (EditMode) cbOrigen.EditValue = EditObject.Origen; }

        /// <summary>
        /// Access initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbAcceso_PreBind(object sender, EventArgs e) { if (EditMode) cbAcceso.EditValue = EditObject.Acceso; }

        /// <summary>
        /// Validates the current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(txtDescripcion.Text)) ThrowMustEnter(DESCRIPCION);

            if (string.IsNullOrEmpty(txtCodigo.Text))ThrowMustEnter(CODE);

            ValidateMessageCode();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates the message code.
        /// </summary>
        private void ValidateMessageCode()
        {
            int cod;

            if (!int.TryParse(txtCodigo.Text, out cod)) ThrowError(CODIGO_INVALIDO);

            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : linea != null ? linea.Empresa : null;

            if (EditObject.Id == 0 && DAOFactory.MensajeDAO.Exists(txtCodigo.Text, empresa, linea)) 
                ThrowDuplicated("CODE");

            var tipoMensaje = DAOFactory.TipoMensajeDAO.FindById(cbTipoMensaje.Selected);
            
            if (tipoMensaje.DeUsuario && (cod <= 20 || cod > 88)) ThrowError(USER_MESSAGE);

            if (tipoMensaje.DeEstadoLogistico && (cod <= 0 || cod > 20)) ThrowError(STATE_MESSAGE);

            if (tipoMensaje.EsDeSistema() && cod < 89) ThrowError(SYSTEM_MESSAGE);
        }

        /// <summary>
        /// Sets the layout of the page according to the context of the message beeing edited.
        /// </summary>
        private void SetPageLayout()
        {
            var tipoMensaje = cbTipoMensaje.Selected > 0 ? DAOFactory.TipoMensajeDAO.FindById(cbTipoMensaje.Selected) : null;

            var esGenerico = tipoMensaje != null && tipoMensaje.EsGenerico;

            var isParent = EditObject.Empresa == null || EditObject.Linea == null;

            if (IsPostBack)
            { 
                if (esGenerico && isParent)
                {
                    if (cbEmpresa.Items.FindByValue(cbEmpresa.AllValue.ToString()) == null) cbEmpresa.Items.Insert(0, new ListItem(cbEmpresa.AllItemsName, cbEmpresa.AllValue.ToString()));

                    cbEmpresa.SetSelectedValue(-1);
                    cbLinea.SetSelectedValue(-1);
                }

                if ((esGenerico && (!isParent || Duplicated)) || !esGenerico)
                {
                    var item = cbEmpresa.Items.FindByValue(cbEmpresa.AllValue.ToString());
                    if (item != null) cbEmpresa.Items.Remove(item);

                    if (item != null && !EditMode)
                    {
                        if (tipoMensaje != null)
                        {
                            if (tipoMensaje.Empresa != null)
                            {
                                cbEmpresa.SetSelectedValue(tipoMensaje.Empresa.Id);
                            }
                            else
                            { 
                                if (tipoMensaje.Linea != null)
                                {
                                    cbEmpresa.SetSelectedValue(tipoMensaje.Linea.Empresa.Id);
                                    cbLinea.SetSelectedValue(tipoMensaje.Linea.Id);
                                }
                                else
                                {
                                    //cbEmpresa.SetSelectedValue(-1);
                                }
                            }
                        }
                        else
                        {
                            //cbEmpresa.SetSelectedValue(-1);
                        }
                    }
                }
            }
            else if (esGenerico && (!isParent || Duplicated)) cbTipoMensaje.Enabled = cbDestino.Enabled = cbOrigen.Enabled = chkAlarma.Enabled = txtCodigo.Enabled = false;
            
            cbEmpresa.Enabled = cbLinea.Enabled = !esGenerico || !isParent || Duplicated;

            //cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.NullValue);
            //cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.NullValue);
        }

        #endregion
    }
}