using System;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects.CicloLogistico;

namespace Logictracker.Parametrizacion.CicloLogistico
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
            cbMensaje.SetSelectedValue(EditObject.Mensaje.Id);
            cbIcono.Selected = EditObject.Icono != null ? EditObject.Icono.Id : 0;
        }

        protected override void OnDelete()
        {
            DAOFactory.EstadoLogisticoDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Demora = (short)npDemora.Value;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Icono = DAOFactory.IconoDAO.FindById(cbIcono.Selected);
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Mensaje = DAOFactory.MensajeDAO.FindById(cbMensaje.Selected);

            DAOFactory.EstadoLogisticoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            var descripcion = ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");

            if (cbIcono.Selected <= 0) ThrowMustEnter("ICON");

            if (cbMensaje.Selected.Equals(0)) ThrowMustEnter("MENSAJE");

            var byDescripcion = DAOFactory.EstadoLogisticoDAO.FindByDescripcion(cbEmpresa.Selected, txtDescripcion.Text);
            ValidateDuplicated(byDescripcion, "DESCRIPCION");
        }

        protected void cbEmpresa_PreBind(Object sender, EventArgs e) { if (EditMode) cbEmpresa.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.NullValue; }

        protected void cbMensaje_PreBind(Object sender, EventArgs e) { if (EditMode) cbMensaje.EditValue = EditObject.Mensaje != null ? EditObject.Mensaje.Id : cbMensaje.NullValue; }
    }
}
