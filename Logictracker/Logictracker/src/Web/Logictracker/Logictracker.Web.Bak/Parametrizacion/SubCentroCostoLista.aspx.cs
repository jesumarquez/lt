using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class SubCentroCostoLista : SecuredListPage<SubCentroDeCostosVo>
    {
        protected override string VariableName { get { return "SUBCENTROS_COSTOS"; } }
        protected override string RedirectUrl { get { return "SubCentroCostoAlta.aspx"; } }
        protected override string GetRefference() { return "SUBCENTRO_COSTOS"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<SubCentroDeCostosVo> GetListData()
        {
            return DAOFactory.SubCentroDeCostosDAO.GetList(new[] {cbEmpresa.Selected},
                                                           new[] {cbLinea.Selected},
                                                           new[] {cbDepartamento.Selected},
                                                           new[] {cbCentroDeCostos.Selected})
                                                   .Select(scc => new SubCentroDeCostosVo(scc))
                                                   .ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticDepartamento, cbDepartamento);
            data.LoadStaticFilter(FilterData.StaticCentroCostos, cbCentroDeCostos);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticDepartamento, cbDepartamento.Selected);
            data.AddStatic(FilterData.StaticCentroCostos, cbCentroDeCostos.Selected);
            return data;
        }
    }
}
