using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class PreasignacionViajeVehiculoAlta : SecuredAbmPage<PreasignacionViajeVehiculo>
    {
        protected override string RedirectUrl { get { return "PreasignacionViajeVehiculoLista.aspx"; } }
        protected override string VariableName { get { return "PREASIGNACION_VIAJE_VEHICULO"; } }
        protected override string GetRefference() { return "PREASIGNACION_VIAJE_VEHICULO"; }
        
        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbVehiculo.SetSelectedValue(EditObject.Vehiculo != null ? EditObject.Vehiculo.Id : cbVehiculo.AllValue);
            cbTransportista.SetSelectedValue(EditObject.Transportista != null ? EditObject.Transportista.Id : cbTransportista.AllValue);

            txtCodigo.Text = EditObject.Codigo.ToUpperInvariant();
        }

        protected override void OnSave()
        {
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Transportista = cbTransportista.Selected > 0 ? DAOFactory.TransportistaDAO.FindById(cbTransportista.Selected) : null;
            EditObject.Vehiculo = cbVehiculo.Selected > 0 ? DAOFactory.CocheDAO.FindById(cbVehiculo.Selected) : null;

            EditObject.Codigo = txtCodigo.Text.ToUpperInvariant();

            DAOFactory.PreasignacionViajeVehiculoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbVehiculo.Selected, "PARENTI03");
            var codigo = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");
            
            var byCode = DAOFactory.PreasignacionViajeVehiculoDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, cbTransportista.Selected, codigo);
            ValidateDuplicated(byCode, "CODE");
        }

        protected override void OnDelete()
        {
            DAOFactory.PreasignacionViajeVehiculoDAO.Delete(EditObject);
        }
    }
}
