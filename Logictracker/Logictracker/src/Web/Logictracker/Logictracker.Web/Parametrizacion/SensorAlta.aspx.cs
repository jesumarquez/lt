using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class SensorAlta : SecuredAbmPage<Sensor>
    {
        protected override string RedirectUrl { get { return "SensorLista.aspx"; } }
        protected override string VariableName { get { return "PAR_SENSOR"; } }
        protected override string GetRefference() { return "PAR_SENSOR"; }

        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Dispositivo != null && EditObject.Dispositivo.Empresa != null 
                                        ? EditObject.Dispositivo.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Dispositivo != null && EditObject.Dispositivo.Linea != null 
                                        ? EditObject.Dispositivo.Linea.Id : cbLinea.AllValue);

            if (EditObject.Dispositivo != null)
                cbDispositivo.Items.Insert(0, new ListItem(EditObject.Dispositivo.Codigo, EditObject.Dispositivo.Id.ToString()));

            cbDispositivo.SetSelectedValue(EditObject.Dispositivo != null ? EditObject.Dispositivo.Id : cbDispositivo.NullValue);
            cbTipoMedicion.SetSelectedValue(EditObject.TipoMedicion != null ? EditObject.TipoMedicion.Id : cbTipoMedicion.NullValue);
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
        }

        protected override void OnDelete()
        {
            DAOFactory.SensorDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Dispositivo = cbDispositivo.Selected > 0 ? DAOFactory.DispositivoDAO.FindById(cbDispositivo.Selected) : null;
            EditObject.TipoMedicion = cbTipoMedicion.Selected > 0 ? DAOFactory.TipoMedicionDAO.FindById(cbTipoMedicion.Selected) : null;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Codigo = txtCodigo.Text;

            DAOFactory.SensorDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {   
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            ValidateEntity(cbDispositivo.Selected, "PARENTI08");
            ValidateEntity(cbTipoMedicion.Selected, "PARENTI77");

            var dispositivo = DAOFactory.DispositivoDAO.FindById(cbDispositivo.Selected);

            var sensor = DAOFactory.SensorDAO.FindByCode(dispositivo.Id, code);
            ValidateDuplicated(sensor, "CODE");
        }
    }
}
