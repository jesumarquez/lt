using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class InsumoLista : SecuredListPage<InsumoVo>
    {
        protected override string RedirectUrl { get { return "InsumoAlta.aspx"; } }
        protected override string VariableName { get { return "MAN_INSUMOS"; } }
        protected override string GetRefference() { return "MAN_INSUMOS"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<InsumoVo> GetListData()
        {
            var list = DAOFactory.InsumoDAO.GetList(new[] { cbEmpresa.Selected }, new[] { cbLinea.Selected }, new[]{cbTipoInsumo.Selected});

            return list.Select(v => new InsumoVo(v)).ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticTipoInsumo, cbTipoInsumo);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTipoInsumo, cbTipoInsumo.Selected);
            return data;
        }

        #endregion
    }
}
