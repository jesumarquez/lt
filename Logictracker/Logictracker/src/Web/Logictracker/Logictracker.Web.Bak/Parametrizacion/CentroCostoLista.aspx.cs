using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionCentroCostoLista : SecuredListPage<CentroDeCostosVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "CENTROS_COSTOS"; } }
        protected override string RedirectUrl { get { return "CentroCostoAlta.aspx"; } }
        protected override string GetRefference() { return "CENTRO_COSTOS"; }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override List<CentroDeCostosVo> GetListData()
        {
            return DAOFactory.CentroDeCostosDAO.GetList(new[] {cbEmpresa.Selected},
                                                        new[] {cbLinea.Selected},
                                                        new[] {cbDepartamento.Selected})
                                               .Select(cc => new CentroDeCostosVo(cc))
                                               .ToList();
        }

        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticDepartamento, cbDepartamento);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticDepartamento, cbDepartamento.Selected);
            return data;
        }
    }
}
