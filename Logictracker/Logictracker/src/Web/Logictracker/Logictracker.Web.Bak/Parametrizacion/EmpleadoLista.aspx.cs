using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Empleados;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class EmpleadoLista : SecuredListPage<EmpleadoVo>
    {
        protected override string RedirectUrl { get { return "EmpleadoAlta.aspx"; } }
        protected override string ImportUrl { get { return "EmpleadoImport.aspx"; } }
        protected override string VariableName { get { return "PAR_EMPLEADOS"; } }
        protected override string GetRefference() { return "EMPLEADO"; }
        protected override bool ExcelButton { get { return true; } }
        protected override bool ImportButton { get { return true; } }

        protected override List<EmpleadoVo> GetListData()
        {
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : linea != null ? linea.Empresa : null;
            
            var emp = new List<int> { empresa == null ? -1 : empresa.Id };
            var lin = new List<int> { linea == null ? -1 : linea.Id };
            var tipoEmp = new List<int> { cbTipoEmpleado.Selected };
            var transp = new List<int> { ddlTransportista.Selected };
            var cat = new List<int> { cbCategoriaAcceso.Selected };

            return DAOFactory.EmpleadoDAO.GetList(emp, lin, tipoEmp, transp, cat)
                                         .Select(c => new EmpleadoVo(c, DAOFactory))
                                         .ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            var transportista = data[FilterData.StaticTransportista];
            var tipoEmpleado = data[FilterData.StaticTipoEmpleado];
            var catAcceso = data[FilterData.StaticCategoriaAcceso];

            if (empresa != null) cbEmpresa.SetSelectedValue((int)empresa);
            if (linea != null) cbLinea.SetSelectedValue((int)linea);
            if (transportista != null) ddlTransportista.SetSelectedValue((int)transportista);
            if (tipoEmpleado != null) cbTipoEmpleado.SetSelectedValue((int)tipoEmpleado);
            if (catAcceso != null) cbCategoriaAcceso.SetSelectedValue((int)catAcceso);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTransportista, ddlTransportista.Selected);
            data.AddStatic(FilterData.StaticTipoEmpleado, cbTipoEmpleado.Selected);
            data.AddStatic(FilterData.StaticCategoriaAcceso, cbCategoriaAcceso.Selected);
            return data;
        }
    }
}
