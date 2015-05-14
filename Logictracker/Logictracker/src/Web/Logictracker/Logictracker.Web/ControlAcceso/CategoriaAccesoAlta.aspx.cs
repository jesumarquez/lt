using System;
using Logictracker.Types.BusinessObjects.ControlAcceso;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.ControlAcceso
{
    public partial class CategoriaAccesoAlta : SecuredAbmPage<CategoriaAcceso>
    {
        protected override string RedirectUrl { get { return "CategoriaAccesoLista.aspx"; } }
        protected override string VariableName { get { return "AC_CATEGORIA"; } }
        protected override string GetRefference() { return "AC_CATEGORIA"; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            txtNombre.Text = EditObject.Nombre;
            txtDescripcion.Text = EditObject.Descripcion;
        }
        protected override void OnSave()
        {
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Nombre = txtNombre.Text;
            EditObject.Descripcion = txtDescripcion.Text;

            DAOFactory.CategoriaAccesoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            var nombre = ValidateEmpty(txtNombre.Text, "NAME");

            var byName = DAOFactory.CategoriaAccesoDAO.FindByName(cbEmpresa.SelectedValues, new[]{-1}, nombre);
            ValidateDuplicated(byName, "NAME");
        }
        protected override void OnDelete()
        {
            DAOFactory.CategoriaAccesoDAO.Delete(EditObject);
        }
    }
}