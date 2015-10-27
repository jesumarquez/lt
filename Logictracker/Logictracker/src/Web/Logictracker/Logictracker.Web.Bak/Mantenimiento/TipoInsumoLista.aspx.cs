using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class TipoInsumoLista : SecuredListPage<TipoInsumoVo>
    {
        protected override string RedirectUrl { get { return "TipoInsumoAlta.aspx"; } }
        protected override string VariableName { get { return "MAN_TIPO_INSUMOS"; } }
        protected override string GetRefference() { return "MAN_TIPO_INSUMOS"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TipoInsumoVo> GetListData()
        {
            var list = DAOFactory.TipoInsumoDAO.GetList(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected});
            
            return list.Select(v => new TipoInsumoVo(v)).ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            return data;
        }

        #endregion
    }
}
