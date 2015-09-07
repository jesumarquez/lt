using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.Empleados;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoEmpleadoLista : SecuredListPage<TipoEmpleadoVo>
    {
        protected override string RedirectUrl { get { return "TipoEmpleadoAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPO_EMPLEADO"; } }
        protected override string GetRefference() { return "TIPOEMPLEADO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TipoEmpleadoVo> GetListData()
        {
            var linea = ddlBase.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
            var empresa = ddlDistrito.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : linea != null ? linea.Empresa : null;
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            return DAOFactory.TipoEmpleadoDAO.FindByEmpresaAndLinea(empresa, linea, user)
                .OfType<TipoEmpleado>().Select(t => new TipoEmpleadoVo(t)).ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            if (empresa != null) ddlDistrito.SetSelectedValue((int)empresa);
            if (linea != null) ddlBase.SetSelectedValue((int)linea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlDistrito.Selected);
            data.AddStatic(FilterData.StaticBase, ddlBase.Selected);
            return data;
        }
    }
}