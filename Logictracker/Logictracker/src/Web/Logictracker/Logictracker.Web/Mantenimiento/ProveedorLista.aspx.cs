using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class ProveedorLista : SecuredListPage<ProveedorVo>
    {
        protected override string RedirectUrl { get { return "ProveedorAlta.aspx"; } }
        protected override string VariableName { get { return "MAN_PROVEEDOR"; } }
        protected override string GetRefference() { return "MAN_PROVEEDOR"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<ProveedorVo> GetListData()
        {
            var list = DAOFactory.ProveedorDAO.GetList(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, new[] {cbTipoProveedor.Selected});

            return list.Select(v => new ProveedorVo(v)).ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticTipoProveedor, cbTipoProveedor);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTipoProveedor, cbTipoProveedor.Selected);
            return data;
        }

        #endregion
    }
}
