using System;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionTipoMensajeAlta : SecuredAbmPage<TipoMensaje>
    {
        protected override string RedirectUrl { get { return "TipoMensajeLista.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPO_MENSAJE"; } }
        protected override string GetRefference() { return "TIPOMENSAJE"; }
        
        protected override void Bind()
        {
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;

            cbGenerico.Checked = EditObject.EsGenerico;
            chkUsuario.Checked = EditObject.DeUsuario;
            chkMantenimiento.Checked = EditObject.DeMantenimiento;
            chkEstadoLogistico.Checked = EditObject.DeEstadoLogistico;
            chkCombustible.Checked = EditObject.DeCombustible;
            chkConfirmacion.Checked = EditObject.DeConfirmacion;
            chkRechazo.Checked = EditObject.DeRechazo;
            chkAtencion.Checked = EditObject.DeAtencion;
            
            if (EditObject.Icono != null) ucIcon.Selected = EditObject.Icono.Id;

            var list = EditObject.Mensajes.OfType<Mensaje>().Select(p => p.Id).ToList();
            cbMensaje.SetSelectedIndexes(list);
        }

        protected override void OnDelete() { DAOFactory.TipoMensajeDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Icono = DAOFactory.IconoDAO.FindById(ucIcon.Selected);

            EditObject.EsGenerico = cbGenerico.Checked;
            EditObject.DeEstadoLogistico = chkEstadoLogistico.Checked;
            EditObject.DeMantenimiento = chkMantenimiento.Checked;
            EditObject.DeUsuario = chkUsuario.Checked;
            EditObject.DeCombustible = chkCombustible.Checked;
            EditObject.DeConfirmacion = chkConfirmacion.Checked;
            EditObject.DeRechazo = chkRechazo.Checked;
            EditObject.DeAtencion = chkAtencion.Checked;

            EditObject.Linea = ddlBase.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
            EditObject.Empresa = ddlDistrito.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : EditObject.Linea != null ? EditObject.Linea.Empresa : null;

            AddMensajes();

            DAOFactory.TipoMensajeDAO.SaveOrUpdate(EditObject);

            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            if (EditMode || !user.PorTipoMensaje) { return; }

            user.AddTipoMensaje(DAOFactory.TipoMensajeDAO.FindById(EditObject.Id));

            DAOFactory.UsuarioDAO.SaveOrUpdate(user, false);
        }

        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(txtCodigo.Text)) ThrowMustEnter("CODE");
            if (string.IsNullOrEmpty(txtDescripcion.Text)) ThrowMustEnter("DESCRIPCION");
            if (ddlBase.Selected.Equals(0)) ThrowMustEnter("LINEA");
        }

        private void AddMensajes()
        {
            var list = cbMensaje.SelectedValues;
            EditObject.ClearMensajes();

            if (QueryExtensions.IncludesAll(list)) return;

            foreach (var id in list) EditObject.AddMensaje(DAOFactory.MensajeDAO.FindById(id));
        }

        protected void DdlDistritoInitialBinding(object sender, EventArgs e)
        {
            if (!EditMode) return;

            ddlDistrito.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : EditObject.Linea != null ? EditObject.Linea.Empresa.Id : ddlDistrito.AllValue;

            if (EditObject.EsGenerico) ddlDistrito.Enabled = false;
        }

        protected void DdlBaseInitialBinding(object sender, EventArgs e)
        {
            if (!EditMode) return;

            ddlBase.EditValue = EditObject.Linea != null ? EditObject.Linea.Id : ddlBase.AllValue;

            if (EditObject.EsGenerico) ddlBase.Enabled = false;
        }

        protected void ChkGenericoChanged(object sender, EventArgs e)
        {
            if (cbGenerico.Checked)
            {
                ddlBase.SelectedIndex = -1;
                ddlBase.Enabled = false;
                ddlDistrito.SelectedIndex = -1;
                ddlDistrito.Enabled = false;
            }
            else
            {
                ddlBase.Enabled = true;
                ddlDistrito.Enabled = true;
            }
        }

        protected void ChkConfirmacionCheckedChanged(object sender, EventArgs e)
        {
            if (chkConfirmacion.Checked) chkRechazo.Checked = false;
        }

        protected void ChkRechazoCheckedChanged(object sender, EventArgs e)
        {
            if (chkRechazo.Checked) chkConfirmacion.Checked = false;
        }
    }
}