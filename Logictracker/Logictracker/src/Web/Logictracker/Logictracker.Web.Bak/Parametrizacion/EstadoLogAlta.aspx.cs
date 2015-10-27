#region Usings

using System;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionEstadoLogAlta : SecuredAbmPage<Estado>
    {
        #region Protected Properties

        /// <summary>
        /// Associated list page.
        /// </summary>
        protected override String RedirectUrl { get { return "EstadoLogLista.aspx"; } }

        /// <summary>
        /// Report Name.
        /// </summary>
        protected override String VariableName { get { return "PAR_ESTADOLOG"; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Binds the current edited object.
        /// </summary>
        protected override void Bind()
        {
            npCodigo.Value = EditObject.Codigo;
            npDeltaTime.Value = EditObject.Deltatime;
            txtDescripcion.Text = EditObject.Descripcion;
            npOrden.Value = EditObject.Orden;
            SelectIcon1.Selected = EditObject.Icono.Id;
            chkInformar.Checked = EditObject.Informar;
            radAutomatico.Checked = EditObject.Modo;
            radManual.Checked = !EditObject.Modo;
        }

        /// <summary>
        /// Deletes the current edited object.
        /// </summary>
        protected override void OnDelete()
        {
            try { DAOFactory.EstadoDAO.Delete(EditObject); }
            catch (Exception e)
            {
            	ThrowError(e, "STATE_IS_USED_IN_CURRENT_TICKETS");
            }
        }

        /// <summary>
        /// Saves the current edited object.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Codigo = Convert.ToInt32(npCodigo.Value);
            EditObject.Deltatime = (short)npDeltaTime.Value;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.EsPuntoDeControl = (short)cbTipo.Selected;
            EditObject.Icono = DAOFactory.IconoDAO.FindById(SelectIcon1.Selected);
            EditObject.Modo = radAutomatico.Checked;
            EditObject.Orden = (short)npOrden.Value;
            EditObject.Informar = chkInformar.Checked;
            EditObject.Linea = DAOFactory.LineaDAO.FindById(cbLinea.Selected);
            EditObject.Mensaje = DAOFactory.MensajeDAO.FindById(cbMensaje.Selected);

            DAOFactory.EstadoDAO.SaveOrUpdate(EditObject);
        }

        /// <summary>
        /// Validates the current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            var descripcion = ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");

            if (SelectIcon1.Selected <= 0) ThrowMustEnter("ICON");

            if (cbMensaje.Selected.Equals(0)) ThrowMustEnter("MENSAJE");

            var byCodigo = DAOFactory.EstadoDAO.FindByCodigo(cbLinea.Selected, Convert.ToInt32(npCodigo.Value));
            ValidateDuplicated(byCodigo, "CODE");
        }

        /// <summary>
        /// Location initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbEmpresa_PreBind(Object sender, EventArgs e) { if (EditMode) cbEmpresa.EditValue = EditObject.Linea != null ? EditObject.Linea.Empresa.Id : cbEmpresa.NullValue; }

        /// <summary>
        /// Company initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbLinea_PreBind(Object sender, EventArgs e) { if (EditMode) cbLinea.EditValue = EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.NullValue; }

        /// <summary>
        /// Message T initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbMensaje_PreBind(Object sender, EventArgs e) { if (EditMode) cbMensaje.EditValue = EditObject.Mensaje != null ? EditObject.Mensaje.Id : cbMensaje.NullValue; }

        /// <summary>
        /// Type initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbTipo_PreBind(Object sender, EventArgs e) { if (EditMode) cbTipo.EditValue = EditObject.EsPuntoDeControl; }

        /// <summary>
        /// Gets the security refference.
        /// </summary>
        /// <returns></returns>
        protected override String GetRefference() { return "ESTADOLOG"; }

        #endregion
    }
}
