using System;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class ViajeProgramadoAlta : SecuredAbmPage<ViajeProgramado>
    {
        protected override String RedirectUrl { get { return "ViajeProgramadoLista.aspx"; } }
        protected override String VariableName { get { return "PAR_VIAJE_PROGRAMADO"; } }
        protected override String GetRefference() { return "PAR_VIAJE_PROGRAMADO"; }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa.Id);
            cbTransportista.SetSelectedValue(EditObject.Transportista != null ? EditObject.Transportista.Id : cbTransportista.AllValue);
            cbTipoVehiculo.SetSelectedValue(EditObject.TipoCoche != null ? EditObject.TipoCoche.Id : cbTipoVehiculo.AllValue);
            txtCodigo.Text = EditObject.Codigo;
        }

        protected override void OnDelete()
        {
            DAOFactory.ViajeProgramadoDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Transportista = cbTransportista.Selected > 0 ? DAOFactory.TransportistaDAO.FindById(cbTransportista.Selected) : null;
            EditObject.TipoCoche = cbTipoVehiculo.Selected > 0 ? DAOFactory.TipoCocheDAO.FindById(cbTipoVehiculo.Selected) : null;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Horas = 0.0;
            EditObject.Km = 0.0;
            
            DAOFactory.ViajeProgramadoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            var descripcion = ValidateEmpty(txtCodigo.Text, "CODIGO");

            var byCodigo = DAOFactory.ViajeProgramadoDAO.FindByCodigo(cbEmpresa.Selected, txtCodigo.Text);
            ValidateDuplicated(byCodigo, "CODIGO");
        }
    }
}
