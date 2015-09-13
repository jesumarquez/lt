using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionEmpleadoAlta : SecuredAbmPage<Empleado>
    {
        protected override string VariableName { get { return "PAR_EMPLEADOS"; } }
        protected override string RedirectUrl { get { return "EmpleadoLista.aspx"; } }
        protected override string GetRefference() { return "EMPLEADO"; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ddlReporta1.EditValue = ddlReporta1.Selected;
            ddlReporta2.EditValue = ddlReporta2.Selected;
            ddlReporta3.EditValue = ddlReporta3.Selected;

            ddlReporta1.DataBind();
            ddlReporta2.DataBind();
            ddlReporta3.DataBind();

            if (!ddlReporta1.Selected.Equals(-2))
            {
                ddlReporta2.RemoveItem(ddlReporta1.Selected);
                ddlReporta3.RemoveItem(ddlReporta1.Selected);
            }

            if (!ddlReporta2.Selected.Equals(-2))
            {
                ddlReporta1.RemoveItem(ddlReporta2.Selected);
                ddlReporta3.RemoveItem(ddlReporta2.Selected);
            }

            if (ddlReporta3.Selected.Equals(-2)) return;

            ddlReporta1.RemoveItem(ddlReporta3.Selected);
            ddlReporta2.RemoveItem(ddlReporta3.Selected);
        }

        protected override void OnDuplicate()
        {
            base.OnDuplicate();
            cbDispositivo.Empleado = 0;
            DocumentList1.ClearDocumentos();
        }

        protected override void OnSave()
        {
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;

            EditObject.Departamento = cbDepartamento.Selected > 0 ? DAOFactory.DepartamentoDAO.FindById(cbDepartamento.Selected) : null;
            EditObject.CentroDeCostos = cbCentroDeCosto.Selected > 0 ? DAOFactory.CentroDeCostosDAO.FindById(cbCentroDeCosto.Selected) : null;

            EditObject.Legajo = txtLegajo.Text;
            EditObject.Antiguedad = (int) npAntiguedad.Value;
            EditObject.Art = txtART.Text;
            EditObject.Licencia = txtLicencia.Text.PadLeft(8, '0');
            EditObject.Telefono = txtTelefono.Text;
            EditObject.Mail = txtMail.Text;
            EditObject.EsResponsable = chbResponsable.Checked;
            EditObject.Tarjeta = ddlTarjeta.Selected > 0 ? DAOFactory.TarjetaDAO.FindById(ddlTarjeta.Selected) : null;
            EditObject.Transportista = ddlTransportista.Selected > 0 ? DAOFactory.TransportistaDAO.FindById(ddlTransportista.Selected) : null;
            EditObject.TipoEmpleado = ddlTipoEmpleado.Selected > 0 ? DAOFactory.TipoEmpleadoDAO.FindById(ddlTipoEmpleado.Selected): null;
            EditObject.Dispositivo = cbDispositivo.Selected > 0 ? DAOFactory.DispositivoDAO.FindById(cbDispositivo.Selected) : null;
            EditObject.Entidad = AltaEntidad.GetEntidad(EditObject.Id == 0);

            EditObject.Reporta1 = ddlReporta1.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(ddlReporta1.Selected) : null;
            EditObject.Reporta2 = ddlReporta2.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(ddlReporta2.Selected) : null;
            EditObject.Reporta3 = ddlReporta3.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(ddlReporta3.Selected) : null;

            EditObject.Categoria = cbCategoria.Selected > 0
                                       ? DAOFactory.CategoriaAccesoDAO.FindById(cbCategoria.Selected)
                                       : null;

            var tarjeta = EditObject.Tarjeta;
            if (tarjeta != null && (tarjeta.Empresa != EditObject.Empresa || tarjeta.Linea != EditObject.Linea))
            {
                tarjeta.Empresa = EditObject.Empresa;
                tarjeta.Linea = EditObject.Linea;
                DAOFactory.TarjetaDAO.SaveOrUpdate(tarjeta);
            }

            DAOFactory.EmpleadoDAO.SaveOrUpdate(EditObject);
        }

        protected override void OnDelete()
        {
            EditObject.Tarjeta = null;

            DAOFactory.EmpleadoDAO.Delete(EditObject);
        }

        protected void CbDispositivoPreBind(object sender, EventArgs e)
        {
            if (EditMode && EditObject.Dispositivo != null)
                cbDispositivo.Empleado = EditObject.Id;
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbDepartamento.SetSelectedValue(EditObject.Departamento != null ? EditObject.Departamento.Id : cbDepartamento.NoneValue);
            ddlTransportista.SetSelectedValue(EditObject.Transportista != null ? EditObject.Transportista.Id : ddlTransportista.AllValue);
            cbCentroDeCosto.SetSelectedValue(EditObject.CentroDeCostos != null ? EditObject.CentroDeCostos.Id : cbCentroDeCosto.NoneValue);
            ddlTipoEmpleado.SetSelectedValue(EditObject.TipoEmpleado != null ? EditObject.TipoEmpleado.Id : ddlTipoEmpleado.AllValue);
            ddlReporta1.SetSelectedValue(EditObject.Reporta1 != null ? EditObject.Reporta1.Id : ddlReporta1.AllValue);
            ddlReporta2.SetSelectedValue(EditObject.Reporta2 != null ? EditObject.Reporta2.Id : ddlReporta2.AllValue);
            ddlReporta3.SetSelectedValue(EditObject.Reporta3 != null ? EditObject.Reporta3.Id : ddlReporta3.AllValue);
            cbCategoria.SetSelectedValue(EditObject.Categoria != null ? EditObject.Categoria.Id : cbCategoria.NoneValue);
            cbDispositivo.SetSelectedValue(EditObject.Dispositivo != null ? EditObject.Dispositivo.Id : cbDispositivo.NoneValue);

            if (EditObject.Tarjeta != null)
            {
                ddlTarjeta.Chofer = EditObject.Id;
                ddlTarjeta.EditValue = EditObject.Tarjeta.Id;
                ddlTarjeta.DataBind();
            }
            else ddlTarjeta.EditValue = ddlTarjeta.NullValue;

            txtLegajo.Text = EditObject.Legajo;
            npAntiguedad.Value = EditObject.Antiguedad;
            txtART.Text = EditObject.Art;
            txtLicencia.Text = EditObject.Licencia;
            txtTelefono.Text = EditObject.Telefono;
            txtMail.Text = EditObject.Mail;
            chbResponsable.Checked = EditObject.EsResponsable;

            AltaEntidad.SetEntidad(EditObject.Entidad);

            DocumentList1.LoadDocumentos(-1, -1, -1, EditObject.Id, -1);
        }

        protected override void ValidateSave()
        {
            var legajo = ValidateEmpty((string) txtLegajo.Text, (string) "LEGAJO");

            var byLegajo = DAOFactory.EmpleadoDAO.FindByLegajo(cbEmpresa.Selected, cbLinea.Selected, legajo);
            ValidateDuplicated(byLegajo, "LEGAJO");
        }
    }
}