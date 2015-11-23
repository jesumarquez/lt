using System.Linq;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ModeloAlta : SecuredAbmPage<Modelo>
    {
        protected override string RedirectUrl { get { return "ModeloLista.aspx"; } }
        protected override string VariableName { get { return "PAR_MODELOS"; } }
        protected override string GetRefference() { return "PAR_MODELOS"; }

        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbMarca.SetSelectedValue(EditObject.Marca != null ? EditObject.Marca.Id : 0);
            cbInsumo.SetSelectedValue(EditObject.Insumo != null ? EditObject.Insumo.Id : cbInsumo.NoneValue);
            txtModelo.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
            txtRendimiento.Text = EditObject.Rendimiento.ToString("#0.00");
            txtCapacidad.Text = EditObject.Capacidad.ToString("#0.00");
            txtCostoModelo.Text = EditObject.Costo.ToString("#0");
            txtVidaUtil.Text = EditObject.VidaUtil.ToString("#0");
            txtRendimientoRalenti.Text = EditObject.RendimientoRalenti.ToString("#0.00");
        }

        protected override void OnDelete()
        {
            DAOFactory.ModeloDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            double rendimiento, rendimientoRalenti, capacidad;
            int costoModelo, vidaUtil;
            
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Marca = DAOFactory.MarcaDAO.FindById(cbMarca.Selected);
            EditObject.Insumo = cbInsumo.Selected > 0 ? DAOFactory.InsumoDAO.FindById(cbInsumo.Selected) : null;
            EditObject.Descripcion = txtModelo.Text;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Rendimiento = double.TryParse(txtRendimiento.Text.Trim(), out rendimiento) ? rendimiento : 0.0;
            EditObject.Capacidad = double.TryParse(txtCapacidad.Text.Trim(), out capacidad) ? capacidad : 0.0;
            EditObject.Costo = int.TryParse(txtCostoModelo.Text.Trim(), out costoModelo) ? costoModelo : 0;
            EditObject.VidaUtil = int.TryParse(txtVidaUtil.Text.Trim(), out vidaUtil) ? vidaUtil : 0;
            EditObject.RendimientoRalenti = double.TryParse(txtRendimientoRalenti.Text.Trim(), out rendimientoRalenti) ? rendimientoRalenti : 0;

            DAOFactory.ModeloDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {   
            ValidateEmpty(txtModelo.Text, "Entities", "PARENTI61");
            var code = ValidateEmpty(txtCodigo.Text, "CODE");

            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbMarca.Selected, "PARENTI06");

            if (!DAOFactory.ModeloDAO.IsCodeUnique(cbEmpresa.Selected, cbLinea.Selected, cbMarca.Selected, EditObject.Id, code))
                ThrowDuplicated("CODE");

            if (EditMode)
            {
                var coches = DAOFactory.CocheDAO.FindByModelo(EditObject.Id);
                var asignado = coches.Any(x => !IsValidEmpresaLinea(x, cbEmpresa.Selected, cbLinea.Selected));
                if (asignado) ThrowError("La modificaciones no se pueden realizar porque hay vehículos asignados a este modelo.");
            }
        }
    }
}
