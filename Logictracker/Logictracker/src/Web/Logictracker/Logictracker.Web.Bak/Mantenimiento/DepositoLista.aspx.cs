using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class DepositoLista : SecuredListPage<DepositoVo>
    {
        protected override string RedirectUrl { get { return "DepositoAlta.aspx"; } }
        protected override string VariableName { get { return "MAN_DEPOSITOS"; } }
        protected override string GetRefference() { return "MAN_DEPOSITOS"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<DepositoVo> GetListData()
        {
            var list = DAOFactory.DepositoDAO.GetList(new[] { cbEmpresa.Selected }, 
                                                      new[] { cbLinea.Selected });

            return list.Select(v => new DepositoVo(v)).ToList();
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
