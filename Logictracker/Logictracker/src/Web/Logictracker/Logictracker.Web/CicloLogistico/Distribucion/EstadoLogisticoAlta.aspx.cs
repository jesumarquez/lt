using System;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class EstadoLogisticoAlta : SecuredAbmPage<EstadoLogistico>
    {
        protected override String RedirectUrl { get { return "EstadoLogisticoLista.aspx"; } }
        protected override String VariableName { get { return "PAR_ESTADO_LOGISTICO"; } }
        protected override String GetRefference() { return "PAR_ESTADO_LOGISTICO"; }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa.Id);
            npDemora.Value = EditObject.Demora;
            txtDescripcion.Text = EditObject.Descripcion;
            cbMensajeInicio.SetSelectedValue(EditObject.MensajeInicio.Id);
            cbMensajeFin.SetSelectedValue(EditObject.MensajeFin != null ? EditObject.MensajeFin.Id : cbMensajeFin.NoneValue);
            cbTipoGeocercaInicio.SetSelectedValue(EditObject.TipoGeocercaInicio != null ? EditObject.TipoGeocercaInicio.Id : cbTipoGeocercaInicio.NoneValue);
            cbTipoGeocercaFin.SetSelectedValue(EditObject.TipoGeocercaFin != null ? EditObject.TipoGeocercaFin.Id : cbTipoGeocercaFin.NoneValue);
            cbIcono.Selected = EditObject.Icono != null ? EditObject.Icono.Id : 0;
            chkProductivo.Checked = EditObject.Productivo;
            chkIterativo.Checked = EditObject.Iterativo;
            chkControlInverso.Checked = EditObject.ControlInverso;
        }

        protected override void OnDelete()
        {
            DAOFactory.EstadoLogisticoDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Demora = (short)npDemora.Value;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Icono = cbIcono.Selected > 0 ? DAOFactory.IconoDAO.FindById(cbIcono.Selected) : null;
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.MensajeInicio = DAOFactory.MensajeDAO.FindById(cbMensajeInicio.Selected);
            EditObject.MensajeFin = cbMensajeFin.Selected > 0 ? DAOFactory.MensajeDAO.FindById(cbMensajeFin.Selected) : null;
            EditObject.TipoGeocercaInicio = cbTipoGeocercaInicio.Selected > 0 ? DAOFactory.TipoReferenciaGeograficaDAO.FindById(cbTipoGeocercaInicio.Selected) : null;
            EditObject.TipoGeocercaFin = cbTipoGeocercaFin.Selected > 0 ? DAOFactory.TipoReferenciaGeograficaDAO.FindById(cbTipoGeocercaFin.Selected) : null;
            EditObject.Productivo = chkProductivo.Checked;
            EditObject.Iterativo = chkIterativo.Checked;
            EditObject.ControlInverso = chkControlInverso.Checked;

            DAOFactory.EstadoLogisticoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            var descripcion = ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");

            if (cbIcono.Selected <= 0) ThrowMustEnter("ICON");
            ValidateEntity(cbMensajeInicio.Selected, "INICIO");

            var byDescripcion = DAOFactory.EstadoLogisticoDAO.FindByDescripcion(cbEmpresa.Selected, txtDescripcion.Text);
            ValidateDuplicated(byDescripcion, "DESCRIPCION");
        }

        protected void cbMensajeInicioOnSelectedIndexChanged(object sender, EventArgs e)
        { 
            if (cbMensajeInicio.Selected > 0)
            {
                cbTipoGeocercaInicio.Enabled = true;
            }
            else
            {
                cbTipoGeocercaInicio.SetSelectedValue(cbTipoGeocercaInicio.NoneValue);
                cbTipoGeocercaInicio.Enabled = false;
            }
        }

        protected void cbMensajeFinOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMensajeFin.Selected > 0)
            {
                cbTipoGeocercaFin.Enabled = true;
            }
            else
            {
                cbTipoGeocercaFin.SetSelectedValue(cbTipoGeocercaInicio.NoneValue);
                cbTipoGeocercaFin.Enabled = false;
            }
        }

        protected void cbEmpresa_PreBind(Object sender, EventArgs e) { if (EditMode) cbEmpresa.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.NullValue; }
        protected void cbMensajeInicio_PreBind(Object sender, EventArgs e) { if (EditMode) cbMensajeInicio.EditValue = EditObject.MensajeInicio != null ? EditObject.MensajeInicio.Id : cbMensajeInicio.NullValue; }
        protected void cbMensajeFin_PreBind(Object sender, EventArgs e) { if (EditMode) cbMensajeFin.EditValue = EditObject.MensajeFin != null ? EditObject.MensajeFin.Id : cbMensajeFin.NullValue; }
        protected void cbTipoGeocercaInicio_PreBind(Object sender, EventArgs e) { if (EditMode) cbTipoGeocercaInicio.EditValue = EditObject.TipoGeocercaInicio != null ? EditObject.TipoGeocercaInicio.Id : cbTipoGeocercaInicio.NullValue; }
        protected void cbTipoGeocercaFin_PreBind(Object sender, EventArgs e) { if (EditMode) cbTipoGeocercaFin.EditValue = EditObject.TipoGeocercaFin != null ? EditObject.TipoGeocercaFin.Id : cbTipoGeocercaFin.NullValue; }
    }
}
