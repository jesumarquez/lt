using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class PuertaAccesoAlta : SecuredAbmPage<PuertaAcceso>
    {
        protected override string GetRefference() { return "PUERTA"; }
        protected override string VariableName { get { return "PAR_PUERTAS"; } }
        protected override string RedirectUrl { get { return "PuertaAccesoLista.aspx"; } }

        protected override void Bind()
        {
            cbVehiculo.Coche = EditObject.Vehiculo.Id;
            cbEmpresa.SetSelectedValue(EditObject.Empresa.Id);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.NullValue);
            cbVehiculo.SetSelectedValue(EditObject.Vehiculo.Id);
            txtDescripcion.Text = EditObject.Descripcion;
            npPuerta.Value = EditObject.Codigo;
            cbZonaAccesoEntrada.SetSelectedValue(EditObject.ZonaAccesoEntrada != null ? EditObject.ZonaAccesoEntrada.Id : cbZonaAccesoEntrada.NoneValue);
            cbZonaAccesoSalida.SetSelectedValue(EditObject.ZonaAccesoSalida != null ? EditObject.ZonaAccesoSalida.Id : cbZonaAccesoSalida.NoneValue);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");
            ValidateEntity(cbVehiculo.Selected, "PARENTI03");

            var puerta = DAOFactory.PuertaAccesoDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, Convert.ToInt16(npPuerta.Value));
            ValidateDuplicated(puerta, "CODE");
        }

        protected override void OnSave()
        {
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;

            EditObject.Codigo = Convert.ToInt16(npPuerta.Value);
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Vehiculo = DAOFactory.CocheDAO.FindById(cbVehiculo.Selected);
            EditObject.ZonaAccesoEntrada = cbZonaAccesoEntrada.Selected > 0 ? DAOFactory.ZonaAccesoDAO.FindById(cbZonaAccesoEntrada.Selected) : null;
            EditObject.ZonaAccesoSalida = cbZonaAccesoSalida.Selected > 0 ? DAOFactory.ZonaAccesoDAO.FindById(cbZonaAccesoSalida.Selected) : null;

            DAOFactory.PuertaAccesoDAO.SaveOrUpdate(EditObject);
        }
        
        protected override void OnDelete()
        {
            DAOFactory.PuertaAccesoDAO.Delete(EditObject);
        }
    }
}